using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// オセロ盤の管理クラス
/// </summary>

public class GridManager : MonoBehaviour
{
    /// <summary>
    /// 石の管理用
    /// </summary>
    [SerializeField]
    private GameObject _StonePrefab;

    private readonly StoneColor[,] _StoneManager = new StoneColor[Cols, Rows];

    //8*8生成用
    public static int Cols { get; } = 8;
    public static int Rows { get; } = 8;

    [SerializeField]
    private GameObject _Canvas; //座標管理用

    /// <summary>
    /// オセロ盤のターン可能位置を表示用変数
    /// </summary>
    [SerializeField]
    private GameObject _FieldPrefab;

    private readonly Field[,] _FieldManager = new Field[Cols, Rows];

    /// <summary>
    /// 石のターン用
    /// </summary>
    [SerializeField]
    private TurnManager _TurnManager;

    //タップ座標格納用変数
    private int x;
    private int z;

    //探索時にターンできる石を格納する
    private readonly List<TurnStonePos> _TurnList = new List<TurnStonePos>();

    //ターン可能な石を格納する
    private readonly List<TurnStonePos> _TurnColorList = new List<TurnStonePos>();

    //CPU　AI用　石の状態を1手先戻すために必要な座標情報を格納する
    private readonly List<TurnStonePos> _UndoList = new List<TurnStonePos>();

    //ターン開始際の石のターンする際の演出のための非同期処理管理
    //コルーチンの唯一性を保つためのフラグ管理　※今後は、全ターン管理をコルーチン処理をしていきたい
    private bool CoroutineWaiting = false;
    private bool EnemyColutinWaiting = false;
    private bool SkipColutinWaiting = false;

    /// <summary>
    /// UI管理用
    /// </summary>
    [SerializeField]
    private UI_Managaer _UI_Managaer;

    //スキップチェック
    public bool all_stone_player_same;
    public bool all_stone_CPU_same;
    public bool all_stone_Draw_same;

    /// <summary>
    /// CPU AIの管理用
    /// </summary>
    [SerializeField]
    private Enemy_AI _Enemy_AI;

    /// <summary>
    /// 評価値管理用
    /// </summary>
    [SerializeField]
    private Evaluation_Score _Evaluation_Score_Count;

    private int Player_Stone_Count;
    private int CPU_Stone_Count;

    /// <summary>
    /// 石とターン可能位置の生成
    /// </summary>
    public void Grid_Prefab_Module_Make()
    {
        for (var i = 0; i < Cols; i++)
        {
            for (var k = 0; k < Rows; k++)
            {
                //石の生成
                var stone = Instantiate(_StonePrefab);
                stone.transform.parent = _Canvas.transform;

                //ボードの大きさと石の大きさをあわせるために微調整
                stone.transform.localPosition = new Vector3(k + (k * 0.3f), 0, i + (i * 0.2f));
                stone.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                stone.transform.Rotate(new Vector3(-90, 0, 0));

                stone.name = "Stone" + i + "  " + k;

                _StoneManager[i, k] = stone.GetComponent<StoneColor>();
                _StoneManager[i, k].StoneState = E_StoneState.EMPTY;

                //ターン可能位置の配置
                var field = Instantiate(_FieldPrefab, new Vector3(k, i, 20), Quaternion.Euler(0, 0, 0));

                field.transform.parent = _Canvas.transform;

                field.transform.localPosition = new Vector3(k + (k * 0.3f), 0, i + (i * 0.2f));
                field.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                field.transform.Rotate(new Vector3(-90, 0, 0));

                field.name = "field" + i + "  " + k;

                _FieldManager[i, k] = field.GetComponent<Field>();
                _FieldManager[i, k].GetFieldStone = E_firldState.NOTTURN;

            }

        }

        //初期配置
        _StoneManager[4, 3].StoneState = E_StoneState.WHITE;
        _StoneManager[4, 4].StoneState = E_StoneState.BLACK;
        _StoneManager[3, 3].StoneState = E_StoneState.BLACK;
        _StoneManager[3, 4].StoneState = E_StoneState.WHITE;
    }

    /// <summary>
    /// プレイヤーターン進行用
    /// </summary>
    public void Player_Now_Turn()
    {
        //コルーチンの処理の際は停止をさせる※不具合原因になりやすいフラグにつき改善していきたい
        if (!CoroutineWaiting)
        {
            //石を置ける状態を検索する
            bool cannot = false;
            _TurnManager.TurnColorListGet(GameScene_Controller.Instance.MyTurn, _TurnColorList, _FieldManager, _StoneManager, cannot);

            //TurnColorListに入れる
            bool listclear = false;
            _TurnManager.Stone_Select_Mode(_TurnColorList, _StoneManager, listclear);

            //TurnColorListはNULL
            if (!(_TurnColorList != null && _TurnColorList.Count > 0))
            {
                if (!all_stone_player_same && !all_stone_CPU_same) StartCoroutine(SkipCoroutin());
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var x_w = hit.collider.gameObject.transform.localPosition.x;
                var z_w = hit.collider.gameObject.transform.localPosition.z;

                //座標補正
                if (x_w <= 1) { x_w = 0; }
                else { x_w -= x_w * 0.2f; }

                z_w -= z_w * 0.1f;

                x = (int)x_w;
                z = (int)z_w;

            }

        }

        //タップした座標の石の状態が置ける状態であれば

        if (x >= 0 && x < Rows && z >= 0 && z < Cols)
        {
            if (_StoneManager[z, x].StoneState == E_StoneState.CANTURN)
            {
                //盤内であれば
                if (x >= 0 && x < Rows && z >= 0 && z < Cols)
                {
                    //タップ座標の石を置く
                    _StoneManager[z, x].StoneState = GameScene_Controller.Instance.MyTurn;

                    //石のターン処理を開始する
                    StartCoroutine(PlayerTurnCoroutin());
                }
                else
                {
                    //置ける石の場所がなければスキップする
                    if (!SkipColutinWaiting)
                    {
                        if (!all_stone_player_same && !all_stone_CPU_same && !all_stone_Draw_same)
                            StartCoroutine(SkipCoroutin());
                    }

                }

            }

        }
        //デバッグモード用
        if (Input.GetKey(KeyCode.A)) Debug_Draw();
        if (Input.GetKey(KeyCode.B)) Debug_All_Black();
        if (Input.GetKey(KeyCode.C)) Debug_All_WHITE();
    }

    /// <summary>
    /// プレイヤーのターン処理
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayerTurnCoroutin()
    {
        //非同期処理の間、唯一性を保つための処理
        if (CoroutineWaiting) yield break;

        CoroutineWaiting = true;

        _TurnManager.Reset_Color(_StoneManager, _FieldManager);

        SoundManager.Instance.SE_Stone_Tap_Play();

        yield return new WaitForSeconds(0.3f);

        //石のターンチェックをする
        bool cannot = false;
        _TurnManager.TurnCheak(_TurnList, _UndoList, GameScene_Controller.Instance.MyTurn, x, z, _StoneManager, cannot);

        SoundManager.Instance.SE_Stone_Turn();

        yield return new WaitForSeconds(0.3f);

        Play_End_Cheack();

        yield return new WaitForSeconds(0.3f);

        //CPUのターンへ
        GameScene_Controller.Instance.MyTurn =
            ((GameScene_Controller.Instance.MyTurn == E_StoneState.BLACK) ? E_StoneState.WHITE : E_StoneState.BLACK);

        _UI_Managaer.Select_UI_Now();

        CoroutineWaiting = false;

    }

    /// <summary>
    /// CPUtターン進行用
    /// </summary>
    public void Enemy_Now_Turn()
    {
        if (!EnemyColutinWaiting)
        {
            bool cannot = false;
            _TurnManager.TurnColorListGet(GameScene_Controller.Instance.MyTurn, _TurnList, _FieldManager, _StoneManager, cannot);

            bool listclear = false;
            _TurnManager.Stone_Select_Mode(_TurnColorList, _StoneManager, listclear);

            //NULLチェック            
            if (_TurnColorList != null && _TurnColorList.Count > 0)
            {
                //選択したCPUのレベルによって置く石の座標を変える
                var rerult = Enemy_AI_Level_Get(GameScene_Controller.Instance.MyEnemy_LEVEL);

                x = rerult.Get_Stone_Pos_x();
                z = rerult.Get_Stone_Pos_z();

                //石のターン処理を開始する
                StartCoroutine(EnemyTurnCoroutin());
            }
            else
            {
                if (!SkipColutinWaiting)
                {
                    if (!all_stone_player_same && !all_stone_CPU_same && !all_stone_Draw_same)
                        StartCoroutine(SkipCoroutin());
                }
            }
        }
    }

    /// <summary>
    /// CPUのターン処理
    /// </summary>
    /// <returns></returns>
    private IEnumerator EnemyTurnCoroutin()
    {
        if (EnemyColutinWaiting) yield break;

        EnemyColutinWaiting = true;

       //現在のフィールド・石の状態をリセットする
        _TurnManager.Reset_Color(_StoneManager, _FieldManager);
        yield return new WaitForSeconds(1.0f);

        SoundManager.Instance.SE_Stone_Tap_Play();
        _StoneManager[z, x].StoneState = GameScene_Controller.Instance.MyTurn;
        yield return new WaitForSeconds(0.2f);

        //石のターンチェックをする
        bool cannot = false;
        _TurnManager.TurnCheak(_TurnList, _UndoList, GameScene_Controller.Instance.MyTurn, x, z, _StoneManager, cannot);

        SoundManager.Instance.SE_Stone_Turn();
        yield return new WaitForSeconds(0.3f);

        Play_End_Cheack();
        yield return new WaitForSeconds(0.3f);

        //プレイヤーのターンへ
        GameScene_Controller.Instance.MyTurn =
            ((GameScene_Controller.Instance.MyTurn == E_StoneState.BLACK) ? E_StoneState.WHITE : E_StoneState.BLACK);

        _UI_Managaer.Select_UI_Now();

        EnemyColutinWaiting = false;
    }


    /// <summary>
    /// スキップ処理のコルーチン
    /// </summary>
    private IEnumerator SkipCoroutin()
    {
        if (SkipColutinWaiting) yield break;

        SkipColutinWaiting = true;
        _UI_Managaer.SkipON();

        yield return new WaitForSeconds(1.0f);

        GameScene_Controller.Instance.MyTurn =
           ((GameScene_Controller.Instance.MyTurn == E_StoneState.BLACK) ? E_StoneState.WHITE : E_StoneState.BLACK);
        _UI_Managaer.SkipOFF();

        SkipColutinWaiting = false;

    }

    /// <summary>
    /// 選択したCPUのAIレベルに応じた座標配置を行う
    /// </summary>
    /// <param name="level">選択したレベル</param>
    /// <returns></returns>
    public TurnStonePos Enemy_AI_Level_Get(E_LevelState level)
    {     
        switch (level)
        {
            case E_LevelState.LEVEL_1:

                var result = _Enemy_AI.LEVEL1_Return_Stone(_TurnColorList);
                Debug.Log("レベル1");
                return result;             

            case E_LevelState.LEVEL_2:

                result = _Enemy_AI.LEVEL2_Return_Stone(_TurnColorList);
                Debug.Log("レベル2");
                return result;

            case E_LevelState.LEVEL_3:

                result = _Enemy_AI.LEVEL3_Return_Stone2(_TurnColorList, _StoneManager, _FieldManager, GameScene_Controller.Instance.MyTurn);
                Debug.Log("レベル3");
                return result;

            default:
                result = _Enemy_AI.LEVEL1_Return_Stone(_TurnColorList);
                Debug.Log("レベルが未選択エラー");
                return result;
        }  
    }

    /// <summary>
    /// 試合終了フラグの確認・現在置かれている石のチェック（64個置かれていたら終了）
    /// 置かれてる石がすべて同じ色だったら終了チェック。
    /// </summary>
    public void Play_End_Cheack()
    {
        //自分が選んでいない石のターン
        E_StoneState notchoisestone = ((GameScene_Controller.Instance.Choiceng_Stone == E_StoneState.BLACK) ? E_StoneState.WHITE : E_StoneState.BLACK);

       //現在置かれている石をすべてカウント
        Player_Stone_Count = _Evaluation_Score_Count.StoneCount(_StoneManager, GameScene_Controller.Instance.Choiceng_Stone);

        CPU_Stone_Count = _Evaluation_Score_Count.StoneCount(_StoneManager, notchoisestone);

        //置かれているすべての石が同じ色かをチェックする
        all_stone_player_same = _Evaluation_Score_Count.All_Stone_Color_Count_Check(_StoneManager, GameScene_Controller.Instance.Choiceng_Stone);

        all_stone_CPU_same = _Evaluation_Score_Count.All_Stone_Color_Count_Check(_StoneManager, notchoisestone);

        var score = Player_Stone_Count + CPU_Stone_Count;
       
        if (score >= Cols * Rows)   //石のカウントが、64個以上になったら
        {
            Debug.Log("試合終了フラグ");
            Win_Lose_Rerult();
        }
        else
        {
            Debug.Log("PLAYER" + Player_Stone_Count + "ENEMY" + CPU_Stone_Count);
        }

        IsPutting_Stone_Check();
    }


    /// <summary>
    /// 64個以下だが、置ける場所がない場合ゲームの終了処理を行う
    /// </summary>
    public void IsPutting_Stone_Check()
    {
        //選んでない色の石のターン
        E_StoneState notchoisestone = ((GameScene_Controller.Instance.Choiceng_Stone == E_StoneState.BLACK) ? E_StoneState.WHITE : E_StoneState.BLACK);

        //PlayerとCPU両方で置くことができる石の状態を検索・検索したplayer_puttingcancheckをに入れる
        bool can = true;
        _TurnManager.TurnColorListGet(GameScene_Controller.Instance.Choiceng_Stone, _TurnColorList, _FieldManager, _StoneManager, can);

        _TurnManager.TurnColorListGet(notchoisestone, _TurnColorList, _FieldManager, _StoneManager, can);

        bool list = false;
        _TurnManager.Stone_Select_Mode(_TurnColorList, _StoneManager, list);

        //リストになにもはいっていなかったら勝敗判定を出す
        if (!(_TurnColorList != null && _TurnColorList.Count > 0)) { Win_Lose_Rerult(); }
        else { _TurnColorList.Clear(); }//リストに入っていたら、リストをクリアする 
    }


    /// <summary>
    /// 勝敗判定
    /// </summary>
    public void Win_Lose_Rerult()
    {
        Debug.Log("試合終了フラグ");

        if (Player_Stone_Count > CPU_Stone_Count)
        {
            Debug.Log("勝ち");
            all_stone_player_same = true;
            GameScene_Controller.Instance.Player_Win = true;
            GameScene_Controller.Instance.Get_PlayGameJudgment = E_Game_Judgment.WIN; 
            _UI_Managaer.Player_Win();

        }
        else if (Player_Stone_Count < CPU_Stone_Count)
        {
            Debug.Log("負け");
            all_stone_CPU_same = true;
            GameScene_Controller.Instance.Player_Lose = true;
            GameScene_Controller.Instance.Get_PlayGameJudgment = E_Game_Judgment.LOSE;
            _UI_Managaer.Player_Lose();

        }
        else
        {
            Debug.Log("ドロー");
            all_stone_Draw_same = true;
            GameScene_Controller.Instance.Player_Draw = true;
            GameScene_Controller.Instance.Get_PlayGameJudgment = E_Game_Judgment.LOSE;
            _UI_Managaer.Player_Draw();
        }
    }

    /// <summary>
    /// デバッグメニュー、引き分けにする
    /// </summary>
    public void Debug_Draw()
    {
        for (var i = 0; i < Cols; i++)
        {
            //石の配置
            for (var k = 0; k < Rows; k++)
            {
                if (k % 2 == 0) { _StoneManager[i, k].StoneState = E_StoneState.BLACK; }
                else { _StoneManager[i, k].StoneState = E_StoneState.WHITE; }               
            }

        }
        Play_End_Cheack();
    }

    /// <summary>
    /// デバッグメニュー、すべて黒にする
    /// </summary>
    public void Debug_All_Black()
    {
        for (var i = 0; i < Cols; i++)
        {
            //石の配置
            for (var k = 0; k < Rows; k++)
            {
                _StoneManager[i, k].StoneState = E_StoneState.BLACK;
            }
        }
        Play_End_Cheack();

    }

    /// <summary>
    /// デバッグメニュー、すべて白にする
    /// </summary>
    public void Debug_All_WHITE()
    {
        for (var i = 0; i < Cols; i++)
        {
            //石の配置
            for (var k = 0; k < Rows; k++)
            {
                _StoneManager[i, k].StoneState = E_StoneState.WHITE;
            }
        }
        Play_End_Cheack();

    }

}

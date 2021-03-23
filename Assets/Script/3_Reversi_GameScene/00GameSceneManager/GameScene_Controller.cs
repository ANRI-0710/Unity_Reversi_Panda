using UnityEngine;

/// <summary>
/// ゲームシーン制御クラス
/// プレイ開始前のオプション画面で下記情報をPlayerPrefで受け取り変数に代入する
/// ・先攻（白・黒）
/// ・ターン進行
/// ・CPUのAIレベル
/// </summary>
/// 

public enum E_LevelState//難易度1～3（playerprefで所得）
{ 
    LEVEL_1,
    LEVEL_2,
    LEVEL_3,
}

public enum E_Game_Judgment //勝負判定（1回のみ判定したいため）
{   
    WIN,
    LOSE,   
}

public class GameScene_Controller : MonoBehaviour
{
    /// <summary>
    /// ゲームコントロール用のインスタンス所得（リバーシ中のみのためシングルトンにしない）
    /// </summary>
    public static GameScene_Controller Instance { get; private set; }

    /// <summary>
    /// ゲーム勝敗判定用プロパティと変数
    /// </summary> 
    private int OneJudgmentCount;   //ワンフレームカウント用
    private E_Game_Judgment PlayJudgment;

    public E_Game_Judgment Get_PlayGameJudgment //プロパティ
    {
        get { return PlayJudgment; }
        set 
        {
            PlayJudgment = value;
            Game_Judgment();
            OneJudgmentCount++;
        } 
    }

    /// <summary>
    /// オセロ盤管理クラスへのアクセス
    /// </summary>
    [SerializeField]
    private GridManager _GridManeger;

    //ターン進行管理
    public E_StoneState MyTurn;

    //AIのレベル管理
    public E_LevelState MyEnemy_LEVEL;

    //白・黒どちらの色を選択したか
    public E_StoneState Choice_Stone_Color;

    //Choice_Stone_Colorで選択した色をMyTurnに代入するための変数
    public E_StoneState Choiceng_Stone;

    /// <summary>
    /// UI管理クラスへのアクセス
    /// </summary>
    [SerializeField]
    private UI_Managaer _UIManager;

    //クリアフラグ
    public bool Player_Win = false;
    public bool Player_Lose = false;
    public bool Player_Draw = false;

    //データ保存用変数
    private int _Select_Stone_Load;
    private int _CPU_Level_Load;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //AIレベル選定　playerPrefで所得
        _CPU_Level_Load = PlayerPrefs.GetInt(SaveData_Manager.KEY_CPU_LEVEL, 0);

        MyEnemy_LEVEL = (E_LevelState)_CPU_Level_Load;

        //先行か後攻用　playPrefで所得
        _Select_Stone_Load = PlayerPrefs.GetInt(SaveData_Manager.KEY_STONE_SELECT, 0);
        Choice_Stone_Color = (E_StoneState)_Select_Stone_Load;

        //先行か後攻かの判定
        if (Choice_Stone_Color == E_StoneState.BLACK) { MyTurn = E_StoneState.BLACK; }
        if (Choice_Stone_Color == E_StoneState.WHITE) { MyTurn = E_StoneState.WHITE; }

        Choiceng_Stone = MyTurn;

        //PlayerPrefs.DeleteAll();

        //プレイヤーが白を選んだら1ターンスキップする
        if (MyTurn == E_StoneState.WHITE)
        {
            MyTurn =
                ((MyTurn == E_StoneState.BLACK) ?
                E_StoneState.WHITE : E_StoneState.BLACK);
        }

        //リバーシ盤・石の生成
        _GridManeger.Grid_Prefab_Module_Make();
        _UIManager.Select_UI_Stone_Set();
        _UIManager.Select_UI_Now();

        //サウンド再生
        SoundManager.Instance.BGM_Game_Main_Play();

    }

    void Update()
    {
        //ターン進行管理
        if (Instance.Choiceng_Stone == MyTurn)
        {
            //自分が選択した色のターン
            _GridManeger.Player_Now_Turn();
        }
        else
        {
            //CPUターン
            _GridManeger.Enemy_Now_Turn();
        }

    }

    private void Game_Judgment()
    {
        switch (Get_PlayGameJudgment) 
        {
            case E_Game_Judgment.WIN:
                if (OneJudgmentCount == 1) { Game_Player_Win(); }             
                break;
            case E_Game_Judgment.LOSE:
                if (OneJudgmentCount == 1) { Game_Player_Lose(); }              
                break;
            default:
                break;
        }
    
    }



    /// <summary>
    /// 勝敗キーの書き込み・セーブ処理・勝った場合は餌3個
    /// </summary>
    private void Game_Player_Win()
    {
        //選択した色が黒だったら
        if (Choice_Stone_Color == E_StoneState.BLACK)
        {
            var blackwin = PlayerPrefs.GetInt("BLACKWIN", 0);
            blackwin++;
            PlayerPrefs.SetInt("BLACKWIN", blackwin);

        }
        else if (Choice_Stone_Color == E_StoneState.WHITE)
        {
            var whitewin = PlayerPrefs.GetInt("WHITEWIN", 0);
            whitewin++;
            PlayerPrefs.SetInt("WHITEWIN", whitewin);

        }
        var esaplus = PlayerPrefs.GetInt("ESA", 0);
        esaplus += 3;
        Debug.Log("ESA" + esaplus);
        PlayerPrefs.SetInt("ESA", esaplus);

    }

    /// <summary>
    ///  勝敗キーの書き込み・セーブ処理・負けた場合は、えさを１つ所得
    /// </summary>
    private void Game_Player_Lose()
    {
        var esaplus = PlayerPrefs.GetInt("ESA", 0);
        esaplus += 1;
        Debug.Log("ESA"+esaplus);
        PlayerPrefs.SetInt("ESA", esaplus);
    }
}

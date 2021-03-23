using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// CPUのAIを管理するクラス
/// </summary>

public class Enemy_AI : MonoBehaviour
{
    [SerializeField]
    private Evaluation_Score ScoreCount;

    [SerializeField]
    private TurnManager _TurnManager;

    [SerializeField]
    private Field _FieldManager;

    [SerializeField]
    private GameObject _CloneStone;

    //LEVEL3用（2手先読む）のターンリスト格納用
    private readonly List<TurnStonePos> LEV3_TurnList = new List<TurnStonePos>();
    private readonly List<TurnStonePos> LEV3_Undoist = new List<TurnStonePos>();
    private readonly List<TurnStonePos> LEV3_TurnColorList = new List<TurnStonePos>();


    /// <summary>
    /// LEVEL1:ターン可能リストからランダムで選択
    /// </summary>
    /// <param name="canputstone">検索用リスト</param>
    /// <returns></returns>
    public TurnStonePos LEVEL1_Return_Stone(List<TurnStonePos> canputstone)
    {

        var rerult = canputstone[Random.Range(0, canputstone.Count)];
        return rerult;
    }

    /// <summary>
    /// LEVEL2:ターン可能リストから最も強い評価値を選択
    /// </summary>
    /// <param name="canputstone">検索用リスト</param>
    /// <returns></returns>
    public TurnStonePos LEVEL2_Return_Stone(List<TurnStonePos> canputstone)
    {

        TurnStonePos rerult = canputstone[0];
        var score = ScoreCount.Eva_Score[rerult.Get_Stone_Pos_z(), rerult.Get_Stone_Pos_x()];

        foreach (var i in canputstone)
        {

            if (score <= ScoreCount.Eva_Score[i.Get_Stone_Pos_z(), i.Get_Stone_Pos_x()])
            {
                score = ScoreCount.Eva_Score[i.Get_Stone_Pos_z(), i.Get_Stone_Pos_x()];
                rerult = i;

            }

        }
        return rerult;

    }

   
    /// <summary>
    /// LEVEL3:ミニマックス法を使用し、2手先まで読む。
    /// </summary>
    /// <param name="canputstone">検索用リスト</param>
    /// <param name="stones"></param>
    /// <param name="nowturn">自分のターンか敵のターンか</param>
    /// <returns></returns>
    public TurnStonePos LEVEL3_Return_Stone2(List<TurnStonePos> canputstone, StoneColor[,] stones, Field[,] field, E_StoneState nowturn)
    {
        //反対の石
        E_StoneState enemyStone = ((nowturn == E_StoneState.BLACK) ? E_StoneState.WHITE : E_StoneState.BLACK);

        TurnStonePos rerult = canputstone[0];       
        int Limit = 2;  //再帰回数
        var resultscore = 0;
        foreach (var i in canputstone)
        {
         
            var a = Minimax(stones, enemyStone, Limit,i, field);
            //var testscore = a;

            Debug.Log(a);

            if (a >= resultscore)
            {
                resultscore = a;
                rerult = i;

                Debug.Log("resultscore" + resultscore);
            }
            

        }
        return rerult;
    }


    private int Minimax(StoneColor[,] stones, E_StoneState nowturn, int depth, TurnStonePos i, Field[,] field)
    {     
        /* 葉の場合、評価値を返す */
        if (depth == 0) return ScoreCount.Eva_Score[i.Get_Stone_Pos_z(),i.Get_Stone_Pos_x()];

        /* 現在の局面から1手進めた状態をa1,a2,a3・・akとする */
        //expand_node(node, 次の turn);
        StoneColor[,] stonecolors = new StoneColor[8, 8];

        for (var h = 0; h < 8; h++)
        {
            for (var j = 0; j < 8; j++)
            {
                var stone = Instantiate(_CloneStone);
                stonecolors[h, j] = stone.GetComponent<StoneColor>();
                stonecolors[h, j].StoneState = stones[h, j].StoneState;
                stonecolors[h, j].enabled = false;
            }

        }

        //反対の石
        E_StoneState enemyStone = ((nowturn == E_StoneState.BLACK) ? E_StoneState.WHITE : E_StoneState.BLACK);

        LEV3_TurnColorList.Clear();
        LEV3_TurnList.Clear();

        //ターンチェック
        bool can = false;
        _TurnManager.TurnCheak(LEV3_TurnList, LEV3_Undoist, nowturn, i.Get_Stone_Pos_x(), i.Get_Stone_Pos_z(), stonecolors, can);

        //ターンした後に、
        bool cannot = false;
        _TurnManager.TurnColorListGet(nowturn, LEV3_TurnColorList, field, stonecolors, cannot);

        //ターン可能リストに入れる
        bool listclear = false;
        _TurnManager.Stone_Select_Mode(LEV3_TurnColorList, stonecolors, listclear);

        var best = ScoreCount.Eva_Score[i.Get_Stone_Pos_z(), i.Get_Stone_Pos_x()];

        for (var w = 0; w < LEV3_TurnColorList.Count; w++) 
        {
            int val = Minimax(stonecolors, enemyStone, depth - 1, LEV3_TurnColorList[w], field);
            Debug.Log("vai" + val);

            if (nowturn != GameScene_Controller.Instance.Choice_Stone_Color && best < val) best = val;
            if (nowturn == GameScene_Controller.Instance.Choice_Stone_Color && best < -val) best = -val;

        }

        for (var h = 0; h < 8; h++)
        {
            for (var j = 0; j < 8; j++)
            {
                Destroy(stonecolors[h, j].gameObject);
            }
        }

        return best;
    }
   
    
    /// <summary>
    /// 石の状態をリセット
    /// </summary>
    /// <param name="stones"></param>
    public void Enemy_Reset_Color(StoneColor[,] stones)
    {
        for (var i = 0; i < GridManager.Cols; i++)
        {
            for (var k = 0; k < GridManager.Rows; k++) { stones[i, k].StoneColor_Reset(); }
        }
    }

    /// <summary>
    ///  オセロ盤のターン可能位置をリセット
    /// </summary>
    /// <param name="stones"></param>
    public void Enemy_Reset_Field(Field[,] field)
    {
        for (var i = 0; i < GridManager.Cols; i++)
        {
            for (var k = 0; k < GridManager.Rows; k++) { field[i, k].Field_Reset(); }
        }
    }
}










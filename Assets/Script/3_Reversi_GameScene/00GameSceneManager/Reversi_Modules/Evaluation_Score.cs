using UnityEngine;

/// <summary>
/// オセロ盤のスコアを管理するクラス
/// </summary>

public class Evaluation_Score : MonoBehaviour
{
    private const int cols = 8;
    private const int rows = 8;

    public int[,] Eva_Score = new int[cols, rows]
    {   //評価値
        
          {120, -20,  20,   5,   5,  20, -20, 120},
          {-20, -40,  -5,  -5,  -5,  -5, -40, -20},
          { 20,  -5,  15,   3,   3,  15,  -5,  20},
          {  5,  -5,   3,   3,   3,   3,  -5,   5},
          {  5,  -5,   3,   3,   3,   3,  -5,   5},
          { 20,  -5,  15,   3,   3,  15,  -5,  20},
          {-20, -40,  -5,  -5,  -5,  -5, -40, -20},
          {120, -20,  20,   5,   5,  20, -20, 120},

    };

    /// <summary>
    /// オセロ評価値の合計値を計算する
    /// </summary>
    /// <param name="stone"></param>
    /// <param name="turn"></param>
    /// <returns></returns>
    public int Return_Eve_Num(StoneColor[,] stone, E_StoneState turn)
    {
        var score = 0;
        for (var i = 0; i < cols; i++)
        {
            for (var k = 0; k < rows; k++)
            {
                if (stone[i, k].StoneState == turn) score += Eva_Score[i, k];
            }
        }
        return score;
    }

    /// <summary>
    /// ターンごとに置いている石の数を数える
    /// </summary>
    /// <param name="stonemanager"></param>
    /// <param name="Myturn"></param>
    /// <returns></returns>
    public int StoneCount(StoneColor[,] stonemanager, E_StoneState Myturn)
    {
        var resultscore = 0;

        for (var i = 0; i < cols; i++)
        {
            for (var k = 0; k < rows; k++)
            {
                if (stonemanager[i, k].StoneState == Myturn)
                {
                    resultscore += 1;
                }
            }
        }
        return resultscore;
    }

    /// <summary>
    /// 置かれている石がすべて自分の色であれば試合終了するフラグをtrueにする
    /// </summary>
    /// <param name="stonemanager"></param>
    /// <param name="Myturn"></param>
    /// <returns></returns>
    public bool All_Stone_Color_Count_Check(StoneColor[,] stonemanager, E_StoneState nowturn)
    {
        bool result = true;
        E_StoneState notturn =
        ((nowturn == E_StoneState.BLACK) ? E_StoneState.WHITE : E_StoneState.BLACK);

        for (var i = 0; i < cols; i++)
        {
            for (var k = 0; k < rows; k++) { if (stonemanager[i, k].StoneState == notturn) result = false; }
            
        }
        return result;
    }


}

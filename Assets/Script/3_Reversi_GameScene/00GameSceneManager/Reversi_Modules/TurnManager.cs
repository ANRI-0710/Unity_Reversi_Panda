using System.Collections.Generic;
using UnityEngine;
using static GridManager;

/// <summary>
/// 石のターンを管理するクラス
/// </summary>
/// 

//石の座標・EstoneState状態を保管するクラス（石のひっくり返しに必要）
public struct TurnStonePos
{
    private readonly int Stone_Pos_z;
    private readonly int Stone_Pos_x;

    public int Get_Stone_Pos_z() { return Stone_Pos_z; }
    public int Get_Stone_Pos_x() { return Stone_Pos_x; }

    //コンストラクタ 探索した方向に敵の色があったら、このリストに座標位置を入れる
    public TurnStonePos(int z, int x)
    {
        Stone_Pos_z = z;
        Stone_Pos_x = x;
    }
}

public class TurnManager : MonoBehaviour
{
    //タップした石からの8方向の確認をする
    private readonly int[] Turn_CHECK_X = new int[] { -1, -1, 0, 1, 1, 1, 0, -1 };
    private readonly int[] Turn_CHECK_Z = new int[] { 0, 1, 1, 1, 0, -1, -1, -1 };

    /// <summary>
    /// ターン可能の座標位置に石とFieldの状態をCANTURNにする, stonecheckは勝敗チェックの際に使用し
    /// 両社ともCANTURNがあるかどうかを調べる。
    /// </summary>
    /// <param name="nowturn">現在のターン情報</param>
    /// <param name="list">使用するリスト</param>
    /// <param name="fields">フィールド配列すべて</param>
    /// <param name="stones">石の配列すべて</param>
    /// <param name="stonecheck">trueの場合は置くことができる石の状態を検索しリストに入れて勝敗チェックを行う</param>
    public void TurnColorListGet(E_StoneState nowturn, List<TurnStonePos> list, Field[,] fields, StoneColor[,] stones, bool stonecheck)
    {
        E_StoneState enemyStone = ((nowturn == E_StoneState.BLACK) ? E_StoneState.WHITE : E_StoneState.BLACK);

        for (var i = 0; i < Turn_CHECK_Z.Length; i++)
        {
            for (var k = 0; k < Turn_CHECK_X.Length; k++)
            {

                //石がTurnの色と一致したら
                if (stones[i, k].StoneState == nowturn)
                {
                    for (var p = 0; p < Turn_CHECK_X.Length; p++)
                    {
                        var z1 = i;
                        var x1 = k;

                        //stonecheckがfalseだったらリストをクリアにする
                        if (!stonecheck) list.Clear();

                        //MyTurn位置を中心に8方向（画面端）に検索をかける
                        while (true)
                        {

                            z1 += Turn_CHECK_Z[p];
                            x1 += Turn_CHECK_X[p];

                            var z_plus = z1 + Turn_CHECK_Z[p];
                            var x_plus = x1 + Turn_CHECK_X[p];

                            //現在の座標位置を+1方向した位置が盤外だったらbreak
                            if (!(x1 >= 0 && x1 < Rows && z1 >= 0 && z1 < Cols)) break;

                            //現在の座標位置を+1方向した位置の石の状態がMyTurnと同じ色であればbreak
                            if (stones[z1, x1].StoneState == nowturn) break;

                            //現在の座標位置を+2方向した位置が盤外だったらbreak
                            if (!(x_plus >= 0 && x_plus < Rows && z_plus >= 0 && z_plus < Cols)) break;

                            //現在の座標位置を+2方向した位置の石の状態がEMPTYだったら
                            if (stones[z1, x1].StoneState == enemyStone && stones[z_plus, x_plus].StoneState == E_StoneState.EMPTY)
                            {
                                //EMPTYの位置をリストに保存
                                list.Add(new TurnStonePos(z_plus, x_plus));
                                break;

                            }//+2方向した位置がMyTurnと逆の色であれば検索をつづける
                            else if (stones[z1, x1].StoneState == enemyStone) { continue; }
                            else { break; }

                        }

                        if (!stonecheck)
                        {
                            foreach (var canturn in list)
                            {
                                //石の状態はCANTURNにする
                                stones[canturn.Get_Stone_Pos_z(), canturn.Get_Stone_Pos_x()].StoneState = E_StoneState.CANTURN;
                                fields[canturn.Get_Stone_Pos_z(), canturn.Get_Stone_Pos_x()].GetFieldStone = E_firldState.CANTURN;

                            }

                        }

                    }

                }

            }


        }

    }


    /// <summary>
    /// タップした座標から8方向チェックし、ターンできる石がある場合はリストに格納し、まとめてターンをする
    /// enemyundoがtrueの場合は、2手先に戻す処理をするため、ひっくり返したすべての石の情報と、石を置いた位置を保存する
    /// </summary>    
    public void TurnCheak(List<TurnStonePos> turnlist, List<TurnStonePos> undolist, E_StoneState nowturn, int x, int y, StoneColor[,] stones, bool enemyundo)
    {
        bool CanTurn_;

        //MyTurnではない色
        E_StoneState enemyStone = ((nowturn == E_StoneState.BLACK) ? E_StoneState.WHITE : E_StoneState.BLACK);

        for (var i = 0; i < Turn_CHECK_X.Length; i++)
        {
            //タップしたx・z情報をローカル変数に代入
            int stonex = x;
            int stonez = y;

            CanTurn_ = false;

            turnlist.Clear();

            //全8方向・1方向ずつ石の状態を確認する
            while (true)
            {
                stonez += Turn_CHECK_Z[i];
                stonex += Turn_CHECK_X[i];

                //座標がxが0以下で、8より大きかったらwhileからbreakする
                if (!(stonex >= 0 && stonex < Rows && stonez >= 0 && stonez < Cols)) break;

                //もし、enemystoneがあったらリストに格納する
                if (stones[stonez, stonex].StoneState == enemyStone)
                {
                    turnlist.Add(new TurnStonePos(stonez, stonex));

                }//MyTurnの色と一致する石であれば
                else if (stones[stonez, stonex].StoneState == nowturn)
                {
                    CanTurn_ = true;
                    break;

                }//もし、何も置かれていない石であればbreakする
                else if (stones[stonez, stonex].StoneState == E_StoneState.EMPTY) { break; }               

            }

            if (CanTurn_)
            {
                foreach (var canturn in turnlist)
                {
                    //リストに入った石をTurnの色に変更する
                    stones[canturn.Get_Stone_Pos_z(), canturn.Get_Stone_Pos_x()].StoneState = nowturn;

                    //敵AIリスト限定・元の手に戻せるようにひっくり返す座標を格納しておく
                    if (enemyundo) undolist.Add(new TurnStonePos(canturn.Get_Stone_Pos_z(), canturn.Get_Stone_Pos_x()));

                }

            }
        }

    }


    /// <summary>
    /// CPU専用　CANTURN状態の石を2手先読むためのリストに入れる
    /// </summary>
    public void Stone_Select_Mode(List<TurnStonePos> CanTurnList, StoneColor[,] stones, bool clear)
    {
        if (clear) { CanTurnList.Clear(); }
        for (var i = 0; i < GridManager.Cols; i++)
        {
            for (var k = 0; k < GridManager.Rows; k++)
            {

                if (stones[i, k].StoneState == E_StoneState.CANTURN) CanTurnList.Add(new TurnStonePos(i, k));
            }
        }

    }

    /// <summary>
    /// （MyTurn）ターン遷移時に石・フィールドの状態をリセットする
    /// </summary>
    public void Reset_Color(StoneColor[,] stones, Field[,] fields)
    {
        for (var i = 0; i < Cols; i++)
        {
            //石の配置
            for (var k = 0; k < Rows; k++)
            {
                stones[i, k].StoneColor_Reset();
                fields[i, k].GetFieldStone = E_firldState.NOTTURN;
            }
        }
    }
}
using UnityEngine;

/// <summary>
/// オセロ盤のターン可能位置を表示するクラス
/// </summary> 

/// <summary>
/// オセロ盤のターン可能位置を表示用の状態管理
/// </summary>
public enum E_firldState
{
    CANTURN,
    NOTTURN,
}

public class Field : MonoBehaviour
{

    [SerializeField]
    private MeshRenderer CanFirld;

    public E_firldState GetFieldStone
    {
        get => FirldState;
        set
        {
            FirldState = value;
            Field_CanTurnColor();
        }

    }

    private E_firldState FirldState;

    /// <summary>
    /// オセロ盤のターン可能位置の状態を管理。CANTURNで赤いブロックが表示される。
    /// </summary>
    public void Field_CanTurnColor()
    {
        switch (FirldState)
        {
            case E_firldState.NOTTURN:
                CanFirld.enabled = false;
                break;

            case E_firldState.CANTURN:
                CanFirld.enabled = true;
                CanFirld.material.color = Color.red;
                break;
        }
    }

    /// <summary>
    /// ターン可能位置の状態をNOTTURNとし、赤いブロックを非表示にして次ターンへいく
    /// </summary>
    public void Field_Reset()
    {
        if (FirldState == E_firldState.CANTURN) { FirldState = E_firldState.NOTTURN; }
    }

}

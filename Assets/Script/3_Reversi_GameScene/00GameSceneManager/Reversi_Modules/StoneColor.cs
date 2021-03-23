using UnityEngine;

/// <summary>
/// 石の色を管理するクラス
/// </summary>

/// <summary>
/// 石の色の状態管理
/// </summary>
public enum E_StoneState
{
    EMPTY,
    BLACK,
    WHITE,
    CANTURN,
}

public class StoneColor : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer _Stone;

    // プロパティ
    public E_StoneState StoneState
    {
        get => _stoneState;
        set
        {
            _stoneState = value;
            ChangeStoneColor();
        }
    }

    private E_StoneState _stoneState;

    public void StoneColor_Reset()
    {
        if (StoneState == E_StoneState.CANTURN) StoneState = E_StoneState.EMPTY;
    }

    private void ChangeStoneColor()
    {
        switch (StoneState)
        {

            case E_StoneState.EMPTY:
                _Stone.enabled = false;
                break;

            case E_StoneState.CANTURN:
                _Stone.enabled = false;
                break;

            case E_StoneState.BLACK:
                _Stone.enabled = true;
                _Stone.material.color = Color.black;
                break;

            case E_StoneState.WHITE:
                _Stone.enabled = true;
                _Stone.material.color = Color.white;
                break;


        }


    }

}

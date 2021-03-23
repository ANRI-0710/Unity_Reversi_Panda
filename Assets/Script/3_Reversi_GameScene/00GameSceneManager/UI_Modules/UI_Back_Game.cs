using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲーム中断・終了用のポップアップを出すクラス
/// </summary>

public class UI_Back_Game : Popup
{
    [SerializeField]
    private GameObject _Popup_Exit;

    public void Open_Back_Popup() {

        PopupStart(_Popup_Exit);
        _Popup_Exit.SetActive(true);
    }

    public void Yes_Button() {
        SceneManager.LoadScene("2_0_MyPage_Scene");  
    }

    public void No_Cancel_Button()
    {
        Popup_Close(_Popup_Exit);
    }
}

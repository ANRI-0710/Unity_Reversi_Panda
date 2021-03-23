using UnityEngine;

/// <summary>
/// ヘルプ画面の表示
/// </summary>
public class Mypage_Help : Popup
{
    [SerializeField]
    private GameObject _Help_Panel;

    [SerializeField]
   private GameObject _Popup1;

    [SerializeField]
    private GameObject _Popup2;

    public void OpenPanel()
    {
        PopupStart(_Help_Panel);
        
        _Help_Panel.SetActive(true);

        _Popup1.SetActive(true);
        _Popup2.SetActive(false);
    }

    public void Open_Popup1()
    {
        _Popup1.SetActive(true);
        _Popup2.SetActive(false);
    }


    public void  Open_Popup2()
    {
        _Popup1.SetActive(false);
        _Popup2.SetActive(true);
    }


    public void Cancel()
    {
        Popup_Close(_Help_Panel);
        _Popup1.SetActive(true);
        _Popup2.SetActive(false);
    }

}

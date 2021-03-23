using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// マイページのPlayボタンを押したあとのポップアップ画面の管理クラス
/// </summary>

public class Mypage_Play_popup : Popup
{
    [SerializeField]
    private Dropdown _CPU_Level_Dropdown;

    [SerializeField]
    private GameObject _GamePlay_PopupPanel;

    [SerializeField]
    private Image _Serect_Stone_Mark;

    /// <summary>
    /// CPUのレベル選択
    /// </summary>
    public enum ELevel_Select
    {
        uLevel1,
        uLevel2,
        uLevel3,
    }

    /// <summary>
    /// 何色を選んだかの選択
    /// </summary>
    public enum ESerect_Stone
    {
        uNONE,
        uBLACK,
        uWHITE,
    }

    /// <summary>
    /// 黒を選んだらPlayerPrefsに黒をセット
    /// </summary>
    public void Select_Black()
    {
        PlayerPrefs.SetInt(SaveData_Manager.KEY_STONE_SELECT, (int)ESerect_Stone.uBLACK);
        _Serect_Stone_Mark.gameObject.transform.localPosition = new Vector3(-140, 230, 0);
    }

    /// <summary>
    /// 黒を選んだらPlayerPrefsに白をセット
    /// </summary>
    public void Select_White()
    {
        PlayerPrefs.SetInt(SaveData_Manager.KEY_STONE_SELECT, (int)ESerect_Stone.uWHITE);
        _Serect_Stone_Mark.gameObject.transform.localPosition = new Vector3(135, 230, 0);
    }

    /// <summary>
    /// CPUのレベルセレクト・プルダウンメニューで選択
    /// </summary>
    public void UI_CPU_Level_Select()
    {
        var selectnow = _CPU_Level_Dropdown.value;

        switch ((ELevel_Select)selectnow)
        {
            case ELevel_Select.uLevel1:
                PlayerPrefs.SetInt(SaveData_Manager.KEY_CPU_LEVEL, (int)ELevel_Select.uLevel1);
                Debug.Log("レベル1を設定しました");
                break;

            case ELevel_Select.uLevel2:
                PlayerPrefs.SetInt(SaveData_Manager.KEY_CPU_LEVEL, (int)ELevel_Select.uLevel2);
                Debug.Log("レベル2を設定しました");
                break;

            case ELevel_Select.uLevel3:
                PlayerPrefs.SetInt(SaveData_Manager.KEY_CPU_LEVEL, (int)ELevel_Select.uLevel3);
                Debug.Log("レベル3を設定しました");
                break;

            default:
                break;
        }

    }

    /// <summary>
    /// ゲームスタートボタンの開始
    /// </summary>
    public void GameStart()
    {
        SceneManager.LoadScene("3_Reversi_GameScene");
        _GamePlay_PopupPanel.SetActive(false);
    }

    /// <summary>
    /// ☓ボタンを押すとポップアップが非表示になる・今後DoTweenでアニメーションを入れる予定
    /// </summary>
    public void Cancel()
    {
        Popup_Close(_GamePlay_PopupPanel);
    }

    /// <summary>
    /// ポップアップの表示
    /// </summary>
    public void TapPlayButton()
    {
        Select_Black();
        PopupStart(_GamePlay_PopupPanel);
        _GamePlay_PopupPanel.SetActive(true);
    }

}

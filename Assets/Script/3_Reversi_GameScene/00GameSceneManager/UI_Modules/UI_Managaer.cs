using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// UIやメッセージを管理するクラス
/// </summary>
public class UI_Managaer : MonoBehaviour
{
    public enum E_UIStone_color
    {
        eUI_BLACK,
        eUI_WHITE,
    }

    [SerializeField]
    UI_Player_Select_Color _Player_Select_Color;

    [SerializeField]
    UI_CPU_Select_Color _CPU_Select_Color;

    [SerializeField]
    private Image UI_Now_Turn;

    [SerializeField]
    private Text _Result_Image;

    [SerializeField]
    private Image BackImage;

    /// <summary>
    /// ターン情報を元に、先攻、後攻のUIの石の色を変える
    /// </summary>
    public void Select_UI_Stone_Set()
    {
        _Player_Select_Color.Play_UIStoneStart(GameScene_Controller.Instance.Choice_Stone_Color);

        _CPU_Select_Color.CPU_UIStoneStart(GameScene_Controller.Instance.Choice_Stone_Color);

    }

    public void Select_UI_Now()
    {
        if (GameScene_Controller.Instance.Choiceng_Stone == GameScene_Controller.Instance.MyTurn)
        {
            UI_Now_Turn.transform.localPosition = new Vector3(-190, 490, 0);
        }
        else
        {
            UI_Now_Turn.transform.localPosition = new Vector3(190, 490, 0);
        }


    }

    public void Player_Win()
    {
        //BackImage.SetActive(true);
        BackImage.enabled = true;
        var str = "勝ち!";
        _Result_Image.text = str.ToString();
        _Result_Image.enabled = true;
        //Wait_Time_Mypage_Awake();
    }

    public void Player_Lose()
    {
        
        BackImage.enabled = true;
        var str = "負け!";
        _Result_Image.text = str.ToString();
        _Result_Image.enabled = true;
        //Wait_Time_Mypage_Awake();
    }

    public void Player_Draw()
    {
        BackImage.enabled = true;
        var str = "引き分け!";
        _Result_Image.text = str.ToString();
        _Result_Image.enabled = true;
        //Wait_Time_Mypage_Awake();
    }

    public void SkipON()
    {
        BackImage.enabled = true;
        var str = "スキップ";
        _Result_Image.text = str.ToString();
        _Result_Image.enabled = true;

    }

    public void SkipOFF()
    {

        BackImage.enabled = false;
        _Result_Image.enabled = false;

    }


    public void Wait_Time_Mypage_Awake()
    {

        Invoke("BackMyPage", 5f);

    }

    public void BackMyPage()
    {

        SceneManager.LoadScene("2_0_MyPage_Scene");

    }



}

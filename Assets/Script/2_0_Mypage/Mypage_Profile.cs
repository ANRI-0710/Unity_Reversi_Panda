using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// マイページ・プロフィールのポップアップ専用クラス
/// 好感度表示とパンダの名前変更を担当
/// </summary>

public class Mypage_Profile : Popup
{
    [SerializeField]
    private GameObject _Mypage_Profile_Popup;

    [SerializeField]
    private Text _PandaLike;

    [SerializeField]
    private InputField _inputField;

    [SerializeField]
    private Text _NameText;

    private int _Panda_Like_Num;

    //好感度数値
    private readonly int Num_5 = 5;
    private readonly int Num_10 = 10;
    private readonly int Num_15 = 15;
    private readonly int Num_20 = 20;
    private readonly int Num_30 = 30;

    void Start()
    {
        //PlayerPrefs.DeleteAll();
        
        //パンダの名前入力
        _inputField = _inputField.GetComponent<InputField>();
        _NameText = _NameText.GetComponent<Text>();
        _PandaLike = _PandaLike.GetComponent<Text>();

        //既に名前が入力されている場合は呼び出し
        var savename = PlayerPrefs.GetString("PANDANAME", " ");
        _NameText.text = savename.ToString();

        //好感度の表示
        _Panda_Like_Num = PlayerPrefs.GetInt("PANDALIKE", 0);
       // _Panda_Like_Num = 100;

        PlayerPrefs.SetInt("PANDALIKE", _Panda_Like_Num);

        Debug.Log("好感度" + _Panda_Like_Num);
        Panda_Like_Text_Display();
    }

    public void Open_Profile_Popup()
    {
        PopupStart(_Mypage_Profile_Popup);
        _Mypage_Profile_Popup.SetActive(true);
    }

    public void Cancel()
    {
        Popup_Close(_Mypage_Profile_Popup);
    }

    /// <summary>
    /// パンダの名前入力
    /// </summary>
    public void InputText()
    {
        _NameText.text = _inputField.text;
    }
    /// <summary>
    /// 名前変更
    /// </summary>
    public void Name_Registration()
    {
        var text = _NameText.text.ToString();
        PlayerPrefs.SetString("PANDANAME", text);
    }

    public void Panda_Like_Text_Display()
    {
        _Panda_Like_Num = PlayerPrefs.GetInt("PANDALIKE", 0);
       
        if (_Panda_Like_Num <= Num_5)
        {
            var str = "そんなに好きじゃない";
            _PandaLike.text = str;
        }

        if (_Panda_Like_Num >= Num_10)
        {
            var str = "普通";
            _PandaLike.text = str;
        }

        if (_Panda_Like_Num >= Num_15)
        {
            var str = "ちょっと仲良し";
            _PandaLike.text = str;
        }

        if (_Panda_Like_Num >= Num_20)
        {
            var str = "かなり仲良し";
            _PandaLike.text = str;
        }

        if (_Panda_Like_Num >= Num_30)
        {
            var str = "ずっ友";
            _PandaLike.text = str;
        }

    }

}

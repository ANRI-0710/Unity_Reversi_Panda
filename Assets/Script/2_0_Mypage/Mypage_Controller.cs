using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// えさやり専用ボタン・ボタンのテキストに現在のえさの数を表示する
/// </summary>
public class Mypage_Controller : MonoBehaviour
{
    [SerializeField]
    private Text _Esa_text;   
    private int _Esa;

    void Start()
    {
        //データの呼び出し
        _Esa = PlayerPrefs.GetInt(SaveData_Manager.KEY_PANDA_ESA, 0);

        SoundManager.Instance.BGM_Title_Mypage_Play();
        Esa_Display();
    }

    /// <summary>
    /// えさの個数を表示
    /// </summary>
    public void Esa_Display()
    {
        var t = _Esa_text.GetComponent<Text>();
        t.text = "×" + _Esa.ToString();
    }

    /// <summary>
    /// えさマークのボタンを押したときの挙動、１個えさの個数をへらす
    /// </summary>
    public void Esa_Decrease()
    {
        if (_Esa > 0)
        {
            _Esa--;
            PlayerPrefs.SetInt("ESA", _Esa);
            Esa_Display();
        }
    }

}

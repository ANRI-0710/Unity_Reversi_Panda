using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// タイトルに戻る
/// </summary>

public class Mypage_BackButton : MonoBehaviour
{
    public void MyPage_TopButton() {

        SceneManager.LoadScene("01_Title_Scene");    
    }

}

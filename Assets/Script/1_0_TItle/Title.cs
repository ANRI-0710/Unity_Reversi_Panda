using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    private void Start()
    {
        SoundManager.Instance.BGM_Title_Mypage_Play();
    }

    public void Invoke_Tap_StartPage() {

        SoundManager.Instance.Start_Title_Start();
        Invoke("GoToStartPage", 1f);
    }
       
    public void GoToStartPage() {

        SceneManager.LoadScene("2_0_MyPage_Scene");           
    }

}

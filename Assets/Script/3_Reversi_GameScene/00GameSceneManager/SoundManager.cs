using UnityEngine;

/// <summary>
/// サウンド管理クラス
/// </summary>
public class SoundManager : Singleton<SoundManager>
{
    [SerializeField]
    private AudioClip BGM_Title_Mypage;

    [SerializeField]
    private AudioClip BGM_Game_Main;

    [SerializeField]
    private AudioClip SE_Stone_Tap;

    [SerializeField]
    private AudioClip SE_Stone_roll;

    [SerializeField]
    private AudioClip SE_ButtonTap;

    [SerializeField]
    private AudioClip SE_Cancel_Tap;

    [SerializeField]
    private AudioClip SE_Popup_Tap;

    [SerializeField]
    private AudioClip Start_Tap;

    [SerializeField]
    private AudioSource _audioSource;

    /// <summary>
    /// シングルトンの起動
    /// </summary>
    public void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        Instance._audioSource = this.GetComponent<AudioSource>();
        Instance.BGM_Title_Mypage_Play();
    }

    public void BGM_Title_Mypage_Play()
    {
        Instance._audioSource.clip = BGM_Title_Mypage;
        Instance._audioSource.Play();
    }

    public void BGM_Game_Main_Play()
    {
        Instance._audioSource.clip = BGM_Game_Main;
        Instance._audioSource.Play();
    }

    public void SE_Stone_Tap_Play()
    {
        Instance._audioSource.PlayOneShot(SE_Stone_Tap);
    }

    public void SE_Stone_Turn()
    {
        Instance._audioSource.PlayOneShot(SE_Stone_roll);
    }

    public void SE_Button_Tap()
    {
        Instance._audioSource.PlayOneShot(SE_ButtonTap);
    }

    public void SE_Popup()
    {
        Instance._audioSource.PlayOneShot(SE_Popup_Tap);
    }

    public void SE_Cansel()
    {
        Instance._audioSource.PlayOneShot(SE_Cancel_Tap);
    }

    public void Start_Title_Start()
    {
        Instance._audioSource.PlayOneShot(Start_Tap);
    }

}

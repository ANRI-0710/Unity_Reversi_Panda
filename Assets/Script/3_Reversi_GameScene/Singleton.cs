using UnityEngine;

/// <summary>
/// SoundManager,DataManagaerで自動的にシングルトンにするクラス
/// </summary>

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));
            }

            if (instance == null)
            {
                Debug.Log("シングルトンの" + typeof(T) + "が所得エラーが起きています");
            }
            return instance;

        }
    }

}

using UnityEngine;

/// <summary>
/// セーブデータ管理クラス
/// </summary>
public class SaveData_Manager : Singleton<SaveData_Manager>
{ 
  
    public const string KEY_CPU_LEVEL = "CPULEVEL";

    public const string KEY_STONE_SELECT = "STONE_SELECT";

    public const string KEY_PLAYER_WIN = "PLAYERWIN";

    public const string WHITE_WIN = "WHITEWIN";
    public const string BLACK_WIN = "BLACKWIN";

    public const string KEY_ESA_MINUS = "ESA_MINUS";
    public const string KEY_ESA_PLUS = "ESA_PLUS";

    public const string KEY_PANDA_ESA = "ESA";
    public const string KEY_PANDA_LIKE_LEVEL = "PANDALIKE";

    public const string KEY_PANDA_NAME = "PANDANAME";

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

}

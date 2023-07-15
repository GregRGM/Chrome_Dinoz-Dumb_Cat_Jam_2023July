using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealmManager : MonoBehaviour
{
    [Tooltip("Only used for Initialization")]
    public RealmConfig[] realmConfigs;

    public static RealmData[] allRealmData;
    public static int currentRealm;
    public static int currentLevel;
    public static bool dataInitialized;

    static string[] realmNames;
    const string REALM_NAME_PREF = "CurrentRealm";
    const string LEVEL_NAME_PREF = "CurrentLevel";

    public bool log = false;

    public delegate void SaveDataLoadedCallback();
    public static event SaveDataLoadedCallback onSaveLoaded;

    private void Awake()
    {
        allRealmData = new RealmData[realmConfigs.Length];
        realmNames = new string[realmConfigs.Length];

        for (int i = 0; i < realmConfigs.Length; i++)
        {
            realmNames[i] = realmConfigs[i].realmName;
            for (int j = 0; j < realmConfigs[i].levels.Length; j++)
            {
                realmConfigs[i].levels[j].world = i;
                realmConfigs[i].levels[j].level = j;
            }
            CheckRealm(i);
        }

        if (!PlayerPrefs.HasKey(REALM_NAME_PREF))
            PlayerPrefs.SetInt(REALM_NAME_PREF, 0);

        if (!PlayerPrefs.HasKey(LEVEL_NAME_PREF))
            PlayerPrefs.SetInt(LEVEL_NAME_PREF, 0);

        currentRealm = PlayerPrefs.GetInt(REALM_NAME_PREF);
        currentLevel = PlayerPrefs.GetInt(LEVEL_NAME_PREF);

        onSaveLoaded?.Invoke();
        dataInitialized = true;
    }


    void CheckRealm(int realmID)
    {
        allRealmData[realmID] = new RealmData(realmConfigs[realmID].realmName, realmConfigs[realmID].levels.Length);
        if (!PlayerPrefs.HasKey("LevelData" + realmConfigs[realmID].realmName))
        {
            for (int j = 0; j < realmConfigs[realmID].levels.Length; j++)
            {
                allRealmData[realmID].realmName = realmConfigs[realmID].realmName;
                allRealmData[realmID].levels[j] = realmConfigs[realmID].levels[j].levelData;
            }

            string levelDataString = JsonUtility.ToJson(allRealmData[realmID]);
            PlayerPrefs.SetString("LevelData" + realmConfigs[realmID].realmName, levelDataString);
            if (log)
            {
                Debug.Log("---- Creating Realm Data ----");
                Debug.Log(levelDataString);
            }
        }
        else
        {
            string levelDataString = PlayerPrefs.GetString("LevelData" + realmConfigs[realmID].realmName);
            if (log)
            {
                Debug.Log("---- Loading Realm Data ----");
                Debug.Log(levelDataString);
            }

            allRealmData[realmID] = JsonUtility.FromJson<RealmData>(levelDataString);
        }
    }

    public static void SetCurrentLevel(int realm, int level)
    {
        PlayerPrefs.SetInt(REALM_NAME_PREF, realm);
        PlayerPrefs.SetInt(LEVEL_NAME_PREF, level);

        currentRealm = PlayerPrefs.GetInt(REALM_NAME_PREF);
        currentLevel = PlayerPrefs.GetInt(LEVEL_NAME_PREF);
    }

    public static void SaveLevelData(int realm, int level, LevelData data)
    {
        allRealmData[realm].levels[level] = new LevelData(data);
        string levelDataString = JsonUtility.ToJson(allRealmData[realm]);

        PlayerPrefs.SetString("LevelData" + realmNames[realm], levelDataString);
    }
}


[System.Serializable]
public class RealmData
{
    public string realmName;
    public LevelData[] levels;

    public RealmData(string newName, int levelCount)
    {
        realmName = newName;
        levels = new LevelData[levelCount];
    }
}
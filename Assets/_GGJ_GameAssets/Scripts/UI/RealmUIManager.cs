using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RealmUIManager : MonoBehaviour
{
    public GameObject realmPrefab;
    public Transform realmsParent;
    public TMP_Text continueText;

    MainMenu mainMenu;
    RealmUI[] realmUI;

    int currentRealm = 0;

    // Start is called before the first frame update
    void Start()
    {
        mainMenu = GetComponent<MainMenu>();

        if (RealmManager.dataInitialized)
        {
            SetupRealmUI();
        }
        else
        {
            RealmManager.onSaveLoaded += PlayerSaveData_onSaveLoaded;
        }
    }

    private void PlayerSaveData_onSaveLoaded()
    {
        RealmManager.onSaveLoaded -= PlayerSaveData_onSaveLoaded;
        SetupRealmUI();
    }

    void SetupRealmUI()
    {
        continueText.text = "Continue: " + (RealmManager.currentRealm + 1) + "-" + (RealmManager.currentLevel + 1);
        realmUI = new RealmUI[RealmManager.allRealmData.Length];
        for (int i = 0; i < RealmManager.allRealmData.Length; i++)
        {
            realmUI[i] = Instantiate(realmPrefab, realmsParent).GetComponent<RealmUI>();
            realmUI[i].SetupRealmUI(RealmManager.allRealmData[i]);

            for (int j = 0; j < RealmManager.allRealmData[i].levels.Length; j++)
            {
                string levelName = RealmManager.allRealmData[i].levels[j].internalSceneName;
                realmUI[i].levelButtons[j].button.onClick.AddListener(delegate { LevelClicked(levelName); });
            }
            realmUI[i].gameObject.SetActive(i == 0);
        }
    }

    void LevelClicked(string levelName)
    {
        GameManager.isStory = false;
        GameManager.singleton.PlaySceneLoad(levelName);
        mainMenu.SelectPanel(2);
    }

    public void ContinueGame()
    {
        GameManager.isStory = true;
        GameManager.singleton.PlaySceneLoad("World_" + (RealmManager.currentRealm + 1) + "_Level_" + (RealmManager.currentLevel + 1));
        mainMenu.SelectPanel(2);
    }

    public void ChangeRealm(int dir)
    {
        currentRealm = Utils.RealMod(currentRealm + dir, RealmManager.allRealmData.Length);
        SelectRealmUI(currentRealm);
    }

    public void SelectRealmUI(int realmPanel)
    {
        for (int i = 0; i < RealmManager.allRealmData.Length; i++)
        {
            realmUI[i].gameObject.SetActive(i == realmPanel);
        }
    }

}

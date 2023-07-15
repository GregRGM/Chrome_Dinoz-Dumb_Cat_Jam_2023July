using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RealmUI : MonoBehaviour
{
    public TMP_Text realmNameText;
    public LevelSelectButton[] levelButtons;

    public void SetupRealmUI(RealmData realmData)
    {
        realmNameText.text = realmData.realmName;

        for (int i = 0; i < levelButtons.Length; i++)
        {
            LevelData currentData = realmData.levels[i];
            levelButtons[i].levelNameText.text = currentData.displayName;

            levelButtons[i].button.interactable = currentData.unlocked;
            levelButtons[i].precentCompleteText.text = (currentData.percentCompleted * 100).ToString("F0") + "%";
            levelButtons[i].fastestClearText.text = Utils.ScoreboardTimer(currentData.fastestTime);
        }
    }
}
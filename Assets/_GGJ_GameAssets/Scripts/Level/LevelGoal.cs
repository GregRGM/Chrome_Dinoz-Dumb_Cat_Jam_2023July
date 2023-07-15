using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelGoal : MonoBehaviour
{
    [Header("Level Configuration")]
    public LevelConfig currentLevel;
    public LevelConfig nextLevelConfig;
    public string levelToLoadString;
    public TMP_Text completionTime;
    public GameObject canvasObject;

    [Header("Gems")]
    [SerializeField] GameObject gemFinishSlot;
    [SerializeField] Transform gemLayoutGroupTransform;
    [SerializeField] Color acquiredGemColorBG;

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip victoryClip;

    bool goalReached = false;

    Gem[] allGems;
    float timer = 0.0f;

    private void Start()
    {
        allGems = FindObjectsOfType<Gem>();
        canvasObject.SetActive(false);
        timer = 0.0f;

    }

    void Update()
    {
        if(!goalReached)
            timer += Time.deltaTime;
    }

    public virtual void SetComplete(PlayerLevelStats stats) 
    {
        if (!goalReached) 
        {
            audioSource.PlayOneShot(victoryClip);
            canvasObject.SetActive(true);
            LeanTween.scale(canvasObject, canvasObject.transform.localScale * 1.25f, .5f).setEasePunch();
            goalReached = true;
            completionTime.text = Utils.ScoreboardTimer(timer);

            float prevCompletionPerc = RealmManager.allRealmData[currentLevel.world].levels[currentLevel.level].percentCompleted;


            LevelData completedLevelData = new LevelData(RealmManager.allRealmData[currentLevel.world].levels[currentLevel.level])
            {
                unlocked = true,
            };

            int collectedGems = 0;
            for (int i = 0; i < allGems.Length; i++)
            {
                LevelGemUI newGem = Instantiate(gemFinishSlot, gemLayoutGroupTransform).GetComponent<LevelGemUI>();
                newGem.bgImage.color = allGems[i].gemColor * Color.gray;
                if (allGems[i].collected)
                {
                    newGem.bgImage.color = acquiredGemColorBG;
                    newGem.fgColor.color = allGems[i].gemColor;
                    collectedGems++;
                }
            }

            float completionPerc = (float)collectedGems / allGems.Length;
            if (completedLevelData.percentCompleted < completionPerc || completedLevelData.percentCompleted == 0)
            {
                completedLevelData.percentCompleted = completionPerc;
            }

            if(completionPerc > prevCompletionPerc)
            {
                completedLevelData.fastestTime = timer;
            }
            else if(completionPerc == prevCompletionPerc)
            {
                if (completedLevelData.fastestTime > timer || completedLevelData.fastestTime == 0)
                {
                    completedLevelData.fastestTime = timer;
                }
            }

            RealmManager.SaveLevelData(currentLevel.world, currentLevel.level, completedLevelData);

            StartCoroutine(PlayEndSequence());
            if (nextLevelConfig != null)
            {
                LevelData nextLevelData = new LevelData(nextLevelConfig.levelData)
                {
                    unlocked = true
                };

                RealmManager.SaveLevelData(nextLevelConfig.world, nextLevelConfig.level, nextLevelData);

                //Update Current Level (for continue game)
                if(nextLevelConfig.world > RealmManager.currentRealm)
                    RealmManager.SetCurrentLevel(nextLevelConfig.world, nextLevelConfig.level);
                else if(nextLevelConfig.level > RealmManager.currentRealm)
                    RealmManager.SetCurrentLevel(nextLevelConfig.world, nextLevelConfig.level);
            }
        }
    }

    IEnumerator PlayEndSequence()
    {
        yield return new WaitForSeconds(4.0f);
        GameManager.singleton.PlaySceneLoad(levelToLoadString);
    }

    public void OnTriggerEnter2D(Collider2D collider) 
    {
        if (collider.gameObject.tag == "Player") 
        {
            collider.GetComponent<PlayerMovement>().PauseMovement(true);
            SetComplete(collider.GetComponent<PlayerLevelStats>());
        }
    }
}

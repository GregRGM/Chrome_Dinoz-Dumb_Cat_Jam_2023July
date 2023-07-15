using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", 
    menuName = "ScriptableObjects/LevelConfig", 
    order = 3)]
public class LevelConfig : ScriptableObject
{
    [SerializeField]
    public LevelData levelData;

    internal int world, level;

}

[System.Serializable]
public class LevelData
{
    [Tooltip("Name of the level.")]
    public string displayName;

    [Tooltip("The scene that's loaded when the level is played.")]
    public string internalSceneName;


    [HideInInspector] public bool unlocked;
    [HideInInspector] public float fastestTime;
    [HideInInspector] public float percentCompleted;

    public LevelData(string newName, string newSceneName, bool isUnlocked, float newTime, float newPerc)
    {
        displayName = newName;
        internalSceneName = newSceneName;
        unlocked = isUnlocked;
        fastestTime = newTime;
        percentCompleted = newPerc;
    }

    public LevelData(LevelData copyData)
    {
        displayName = copyData.displayName;
        internalSceneName = copyData.internalSceneName;
        unlocked = copyData.unlocked;
        fastestTime = copyData.fastestTime;
        percentCompleted = copyData.percentCompleted;
    }
}

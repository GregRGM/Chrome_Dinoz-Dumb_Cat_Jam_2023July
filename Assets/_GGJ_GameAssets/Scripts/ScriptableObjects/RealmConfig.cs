using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig",
    menuName = "ScriptableObjects/RealmConfig",
    order = 4)]
public class RealmConfig : ScriptableObject
{
    [Tooltip("Name of the level.")]
    public string realmName;
    public LevelConfig[] levels;

    public void SetLevelData(int levelID, LevelData data)
    {
        levels[levelID].levelData = data;
    }
}

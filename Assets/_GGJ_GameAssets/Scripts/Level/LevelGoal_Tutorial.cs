using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoal_Tutorial : LevelGoal
{
    public override void SetComplete(PlayerLevelStats stats)
    {
        base.SetComplete(stats);
        PlayerPrefs.SetInt("Tutorial_Complete", 1);
    }
}

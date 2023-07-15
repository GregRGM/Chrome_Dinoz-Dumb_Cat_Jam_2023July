using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelStats : MonoBehaviour
{
    internal int currentSpecialCount = 0;
    internal void UpdateSpecialCount(int amountToAdd)
    {
        currentSpecialCount += amountToAdd;
    }
}

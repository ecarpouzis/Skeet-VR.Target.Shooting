using UnityEngine;
using System.Collections;

public class Ach_CleaningUpTracker : Shootable
{
    private static int currentShotCount = 0;
    private static readonly int maxShotCount = 39;
    
    void Start()
    {
        currentShotCount = 0;
    }
    
    public override void OnHit()
    {
        currentShotCount++;
        //Debug.Log("[" + currentShotCount + "/" + maxShotCount + "]");
        if (currentShotCount == maxShotCount)
        {
            if (currentShotCount < 25)
            {
                Debug.LogError("Error with cleaning up achievement tracker");
                return;
            }
            VRApiManager.Achievements.GrantAchievement(Achievements.CLEANING_UP);
        }
    }
}

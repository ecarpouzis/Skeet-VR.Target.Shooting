using UnityEngine;
using System.Collections;

public class Ach_NoAnimalsWereHarmedProgress : MonoBehaviour
{
    private static int currentShotCount = 0;
    private static readonly int maxShotCount = 8;

    private Vector3 originalPosition;
    private bool addedShot = false;

    void Start()
    {
        originalPosition = transform.position;
        addedShot = false;
        currentShotCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!addedShot)
        {
            if (Vector3.Distance(originalPosition, transform.position) > .2f)
            {
                addedShot = true;
                currentShotCount++;
                //Debug.Log("[" + currentShotCount + "/" + maxShotCount + "]");
                if (currentShotCount == maxShotCount)
                {
                    VRApiManager.Achievements.GrantAchievement(Achievements.NO_ANIMALS_WERE_HARMED);
                }
            }
        }
    }
}

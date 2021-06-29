using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetwallManager : MonoBehaviour
{

    GameTracker tracker;
    public GameObject targetWall;
    List<WallTarget> targets;
    public bool targetwallStarted = false;
    bool batchInProgress = false;
    public float timePlaying { get; private set; }
    public readonly float maxTime = 60f;

    float timeSinceOpen = 0f;
    float startingTimeOpen = 3f;
    float timeOpen = 3f;


    //Batches for first half will speed up which must be reset.
    float timeSinceEvent = 0f;
    float startingTimeBetweenEvents = 3f;
    float timeBetweenEvents = 3f;

    //Second half the targets will start to be opened faster, which must be reset
    float timeSecondhalfOpenings = .5f;
    float startTimeSecondhalfOpenings = .5f;
    float timeSinceSecondHalfOpening = 0f;

    //Second half the targets will start to close faster, which must be reset
    float secondHalfClosetime = 3.25f;
    float startSecondHalfClosetime = 3.25f;

    //Track total targets opened in second half so we can step up #targets to open
    int targetsOpened = 0;

    //As the second half progresses, we open more and more targets.
    int secondHalfTargetsToOpen = 1;
    int startSecondHalfTargetsToOpen = 1;

    int startingTargetsBatch = 8;
    int targetsBatch = 8;

    void Awake()
    {
        tracker = GameObject.Find("GameTracker").GetComponent<GameTracker>();
        targets = new List<WallTarget>(targetWall.GetComponentsInChildren<WallTarget>());
    }

    public void StartTargetwall()
    {
        foreach(WallTarget target in targets)
            target.InstantClose();
        targetWall.SetActive(true);
        targetwallStarted = true;
    }

    void BatchOpen(int numTargets)
    {
        timeSinceOpen = 0f;
        batchInProgress = true;
        List<WallTarget> targetsToOpen = RandomTargets(numTargets);
        foreach (WallTarget target in targetsToOpen)
        {
            target.Open(1f / (float)targetsBatch);
        }
    }

    void BatchClose()
    {
        timeSinceEvent = 0f;
        batchInProgress = false;
        List<WallTarget> openTargets = new List<WallTarget>();

        //Need to count the targets left open when I batch them closed, to determine volume of close
        foreach (WallTarget target in targets)
        {
            if (target.isOpen)
                openTargets.Add(target);
        }

        foreach (WallTarget target in openTargets)
        {
            target.Close(1f / openTargets.Count);
        }

    }

    List<WallTarget> RandomTargets(int numTargets)
    {
        List<WallTarget> chosenTargets = new List<WallTarget>();

        while (chosenTargets.Count < numTargets)
        {
            WallTarget chosenTarget = targets.GetRandomValue<WallTarget>();
            while (chosenTargets.Contains(chosenTarget))
            {
                chosenTarget = targets.GetRandomValue<WallTarget>();
            }
            chosenTargets.Add(chosenTarget);
        }

        return chosenTargets;
    }

    public void EndTargetwall()
    {
        if (GameTracker.Instance.Points >= 8000)
            VRApiManager.Achievements.GrantAchievement(Achievements.ANOTHER_BRICK);

        BatchClose();
        targetwallStarted = false;
        targetsOpened = 0;
        timeOpen = startingTimeOpen;
        targetsBatch = startingTargetsBatch;
        timeBetweenEvents = startingTimeBetweenEvents;
        timeSecondhalfOpenings = startTimeSecondhalfOpenings;
        secondHalfClosetime = startSecondHalfClosetime;
        secondHalfTargetsToOpen = startSecondHalfTargetsToOpen;
        targetWall.SetActive(false);
        timePlaying = 0f;
        tracker.GameOver();
    }

    void Update()
    {
        if (targetwallStarted)
        {
            //Debug.Log("Playing: " + timePlaying + " Targets opened: "+targetsOpened);
            timePlaying += Time.deltaTime;
            timeSinceEvent += Time.deltaTime;

            if (timePlaying < maxTime / 2)
            {
                //First half of game, batch openings
                if (batchInProgress)
                {
                    timeSinceOpen += Time.deltaTime;
                    if (timeSinceOpen > timeOpen)
                    {
                        //Time to close
                        BatchClose();
                        timeBetweenEvents -= .2f;
                    }
                }
                else if (timeSinceEvent > timeBetweenEvents)
                {
                    BatchOpen(targetsBatch);
                    targetsBatch++;
                }
            }
            else if (timePlaying < maxTime)
            {
                //Second half of game, speed openings
                if (batchInProgress)
                {
                    BatchClose();
                }

                timeSinceSecondHalfOpening += Time.deltaTime;
                if (timeSinceSecondHalfOpening > timeSecondhalfOpenings)
                {
                    OpenRandom(secondHalfTargetsToOpen);
                    timeSinceSecondHalfOpening = 0f;
                }
            }
            else
            {
                EndTargetwall();
            }
        }

    }

    void OpenRandom(int numOpen)
    {
        int numOpened = 0;
        while (numOpened < numOpen)
        {
            WallTarget chosenTarget = targets.GetRandomValue<WallTarget>();
            int targetsTried = 0;
            while (chosenTarget.isOpen && targetsTried < 30)
            {
                targetsTried++;
                chosenTarget = targets.GetRandomValue<WallTarget>();
            }
            targetsOpened++;
            chosenTarget.Open(.5f/(float)numOpen);
            chosenTarget.DelayClose(secondHalfClosetime);


            if (targetsOpened == 30 || targetsOpened == 60)
                secondHalfTargetsToOpen++;
            if (targetsOpened == 5 || targetsOpened == 20 || targetsOpened == 40 || targetsOpened == 60)
                secondHalfClosetime -= .1f;
            if (targetsOpened == 5 || targetsOpened == 20 || targetsOpened == 40)
                timeSecondhalfOpenings -= .025f;

            numOpened++;
        }
    }

}

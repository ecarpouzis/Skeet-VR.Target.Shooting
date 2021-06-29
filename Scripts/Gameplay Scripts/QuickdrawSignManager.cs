using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuickdrawSignManager : MonoBehaviour
{
    public PlayerController player { get { return PlayerController.Instance; } }
    public List<Transform> quickdrawSignLocationsRoomscale;
    public List<Transform> quickdrawSignLocationsFrontFacing;
    public GameObject signPrefab;
    bool quickdrawCalled = false;
    List<GameObject> spawnedSigns = new List<GameObject>();
    public List<GameObject> chosenSigns = new List<GameObject>();
    float timeSinceStart = 0f;
    float timeToDraw = 0f;
    float timeCompleted = 0f;
    bool playedFirstTone = false, playedSecondTone = false, playedThirdTone = false;
    bool quickdrawStarted = false;
    public SoundSubClip Beep1, Beep2, Beep3;
    public GameTracker gameTracker;
    bool isPointingDown = false;
    bool signsRising;
    float signsRisingTime;

    public void StartQuickdrawMode()
    {
        timeCompleted = 0f;
        timeToDraw = Random.Range(3f, 6f);
        List<Transform> quickdrawSignLocations;
        if (VRApiManager.c_IsOculus)
        {
            quickdrawSignLocations = quickdrawSignLocationsFrontFacing;
        }
        else {
            quickdrawSignLocations = quickdrawSignLocationsRoomscale;
        }

        foreach (Transform signSpawn in quickdrawSignLocations)
        {
            GameObject newSign = Instantiate(signPrefab, signSpawn.position, signSpawn.localRotation) as GameObject;
            newSign.transform.parent = signSpawn;
            newSign.transform.localPosition = Vector3.zero;
            newSign.transform.localRotation = Quaternion.identity;
            spawnedSigns.Add(newSign);
        }

        GameObject chosenSign1 = spawnedSigns.GetRandomValue<GameObject>();

        int chosenSignIndex = spawnedSigns.IndexOf(chosenSign1);
        int howManyRight = Random.Range(2, 4);
        int rightIndex = chosenSignIndex + howManyRight;
        if (rightIndex > spawnedSigns.Count - 1)
        {
            rightIndex = rightIndex - spawnedSigns.Count;
        }

        int howManyLeft = Random.Range(2, 4);
        int leftIndex = chosenSignIndex - howManyLeft;
        if (leftIndex < 0)
        {
            leftIndex = leftIndex + spawnedSigns.Count;
        }
        //Debug.Log("Chosen index is:" + chosenSignIndex);
        chosenSigns.Add(spawnedSigns[chosenSignIndex]);
        //Debug.Log("Right Index is: " + rightIndex);
        chosenSigns.Add(spawnedSigns[rightIndex]);
        //Debug.Log("Left Index is: " + leftIndex);
        chosenSigns.Add(spawnedSigns[leftIndex]);

        signsRising = true;
        quickdrawStarted = true;
    }

    public void EndQuickdrawMode()
    {
        quickdrawStarted = false;
        quickdrawCalled = false;
        foreach (GameObject sign in spawnedSigns)
        {
            Destroy(sign);
        }
        spawnedSigns.Clear();
        playedFirstTone = false; playedSecondTone = false; playedThirdTone = false;
        timeSinceStart = 0f;
        timeToDraw = 0f;
        
        if (timeCompleted > 0)
        {
            MainMenu.Instance.LastScore = timeCompleted;
            VRApiManager.Leaderboard.TrySetHighScore(GameTracker.Instance.CurrentMode, PlayerController.Instance.CurrentGun, timeCompleted);
            gameTracker.Delay(1f, () => { MainMenu.Instance.LeaderboardUI.ForceRefresh(); });
            MainMenu.Instance.UpdateUI();
        }

        if (timeCompleted < 2f)
            VRApiManager.Achievements.GrantAchievement(Achievements.SPIN_ME_ROUND);
        gameTracker.GameOver();
    }

    public void RemoveSign(GameObject givenSign)
    {
        chosenSigns.Remove(givenSign);
    }

    void DetectPointdown()
    {
        RaycastHit hit;
        int layerMask = 1 << 14;
        GunInfo equippedGunInfo = PlayerController.Instance.heldGun.GetComponent<GunInfo>();
        Ray newRay = new Ray(PlayerController.Instance.heldGun.transform.position, equippedGunInfo.BarrelForward * 50);
        Debug.DrawRay(newRay.origin, equippedGunInfo.BarrelForward * 50);
        Physics.Raycast(newRay, out hit, 50, layerMask);

        if (hit.collider != null)
        {
            if (hit.collider.name == "QuickdrawGroundbox")
            {
                isPointingDown = true;
            }
            else
            {
                isPointingDown = false;
            }
        }
        else
        {
            isPointingDown = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (signsRising)
        {
            signsRisingTime += Time.deltaTime;
            foreach (GameObject sign in spawnedSigns)
            {
                Vector3 newPos = sign.transform.position;
                newPos.y = 0;
                sign.transform.position = Vector3.Lerp(sign.transform.position, newPos, .5f * signsRisingTime);

                if (sign.transform.position.y >= 0)
                    signsRising = false;
            }

        }
        if (quickdrawStarted)
        {
            DetectPointdown();
            timeSinceStart += Time.deltaTime;
            if (isPointingDown && !playedThirdTone)
            {
                player.ControlsDisplay.quickloadText.text = "";
                if (timeSinceStart >= 1f && !playedFirstTone)
                {
                    Beep1.Play();
                    playedFirstTone = true;
                }
                if (timeSinceStart >= 2f && playedFirstTone && !playedSecondTone)
                {
                    Beep2.Play();
                    playedSecondTone = true;
                }
            }
            else if (!isPointingDown && !playedThirdTone)
            {
                timeSinceStart = 0f;
                playedFirstTone = false; playedSecondTone = false; playedThirdTone = false;
                player.ControlsDisplay.quickloadText.text = "AIM AT GROUND";
            }
            if (timeSinceStart >= timeToDraw && playedSecondTone && !playedThirdTone)
            {
                Beep3.Play();
                playedThirdTone = true;
                quickdrawCalled = true;

            }
            if (quickdrawCalled)
            {
                player.ControlsDisplay.quickloadText.text = (timeSinceStart - timeToDraw).ToString().Substring(0, 5);
                bool isDone = true;
                foreach (GameObject sign in chosenSigns)
                {
                    sign.GetComponent<QuickdrawSign>().chosenSign = true;

                    if (sign != null)
                    {
                        isDone = false;
                    }
                }
                if (isDone)
                {
                    timeCompleted = timeSinceStart - timeToDraw;
                    Debug.Log("Done! " + timeCompleted);
                    //TURN THIS INTO A 00:00 BASED TIME DISPLAY
                    player.ControlsDisplay.quickloadText.text = timeCompleted.ToString().Substring(0, 5);
                    EndQuickdrawMode();
                }
            }
        }
    }
}

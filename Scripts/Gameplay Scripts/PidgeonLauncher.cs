using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PidgeonLauncher : MonoBehaviour
{
    public bool ignoreAmmo = false;
    public List<GameObject> ammo;
    public int pidgeonsLaunched = 0;
    public GameObject pidgeonPrefab;
    public Transform launchTransform;
    float speed = 25f;
    public bool launcherRunning;

    int pidgeonsToLaunch;

    //Pidgeons currently being shot out of the launcher this batch.
    int pidgeonsToShoot = 0;
    GameTracker gameTracker;

    //Ex: Load 3 pidgeons, shoot them, wait, load 2 pidgeons, shoot them.
    float timeBetweenLoads = 4f;
    float timeSinceLoad = 0f;

    float timeBetweenShots = 1f;
    float timeSinceShot = 0f;
    List<int> earlyGame = new List<int> { 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 5 };
    List<int> lateGame = new List<int> { 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 4, 4, 4, 4, 4, 5, 5, 5 };
    //Must always total 100

    void Start()
    {
        pidgeonsToLaunch = ammo.Count;
        gameTracker = GameObject.Find("GameTracker").GetComponent<GameTracker>();
    }

    public int LoadPidgeons()
    {
        int loadedPidgeons = 0;
        List<int> usedList = null;
        if (earlyGame.Count > 0)
        {
            usedList = earlyGame;
        }
        else if (lateGame.Count > 0)
        {
            usedList = lateGame;
        }

        if (usedList != null)
        {
            int randomIndex = Random.Range(0, usedList.Count);
            loadedPidgeons = usedList[randomIndex];
            usedList.RemoveAt(randomIndex);
        }
        return loadedPidgeons;
    }

    private IEnumerator EndGameCoroutine()
    {
        yield return new WaitForSeconds(3f);
        gameTracker.GameOver();
    }

    // Update is called once per frame
    void Update()
    {
        if (launcherRunning)
        {
            if (pidgeonsLaunched == pidgeonsToLaunch)
            {
                    launcherRunning = false;
                    StartCoroutine(EndGameCoroutine());   
            }

            timeSinceLoad += Time.deltaTime;
            timeSinceShot += Time.deltaTime;
            if (timeSinceLoad >= timeBetweenLoads && pidgeonsToShoot < 1)
            {
                pidgeonsToShoot = LoadPidgeons();
                timeSinceLoad = 0f;
            }
            if (timeSinceShot >= timeBetweenShots && pidgeonsToShoot > 0)
            {
                ShootPidgeon();
                timeSinceShot = 0f;
                pidgeonsToShoot--;
            }
        }
    }

    private void ShootPidgeon()
    {
        GameObject pidgeon = Instantiate(pidgeonPrefab, launchTransform.position, launchTransform.rotation) as GameObject;
        pidgeon.name = pidgeonPrefab.name;
        if (!ignoreAmmo)
        {
            Destroy(ammo[pidgeonsLaunched]);
        }
        pidgeonsLaunched++;
        timeBetweenLoads -= .04f;
        timeBetweenShots -= .005f;
        pidgeon.GetComponent<Rigidbody>().velocity = launchTransform.forward * speed;
        gameTracker.OnTrapShoot();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static T GetRandomValue<T>(this List<T> input)
    {
        return input[UnityEngine.Random.Range(0, input.Count)];
    }

    private static Random rng = new Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0,n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static string SecondsToString(this float f)
    {
        f = Mathf.Max(f, 0f);
        string fmtstr = "0:{1:00}";

        var mins = Mathf.Floor(f / 60f);
        var seconds = f - mins * 60f;

        fmtstr = string.Format(fmtstr, mins, seconds);
        return fmtstr;
    }

    public static bool HasDLC(this DLCUnlock given, DLCUnlock flags)
    {
        return (given & flags) > 0;
    }


}

public class GameTracker : MonoBehaviour
{
    public static GameTracker Instance;

    GameObject GameMenu;
    public string loadedMode;

    //Shotgun Pickups
    public GameObject OverUnderPickup;
    public GameObject SideBySidePickup;
    public GameObject PumpActionPickup;
    public GameObject TommygunPickup;

    public GameObject Table;

    //TRAP MODE:
    int targetsLaunched = 0;
    int targetsHit = 0;
    int points = 0;
    public GameObject PidgeonLauncherPrefab;
    public Transform PidgeonLauncherSpawnPosition;
    public PidgeonLauncher pidgeonLauncher;
    bool gameStarted = false;
    public GameObject UFOPrefab;
    public List<TargetSpawn> TargetSpawners;
    public List<GameObject> PigSpawners, UFOSpawners;
    public GameObject PigPrefab;
    public DishThrower dishSpawner;

    public float Points { get { return points; } }

    private const string
          Trap = "Trap",
          Skeet = "Skeet",
          Bounce = "Bounce",
          Quickdraw = "Quickdraw",
          Targetwall = "Targetwall",
          Animalgallery = "Animalgallery";

    public Modes CurrentMode
    {
        get
        {
            switch (loadedMode)
            {
                case Trap:
                    return Modes.Trap;
                case Skeet:
                    return Modes.Skeet;
                case Bounce:
                    return Modes.Bounce;
                case Quickdraw:
                    return Modes.Quickdraw;
                case Targetwall:
                    return Modes.Targetwall;
                case Animalgallery:
                    return Modes.AnimalGallery;
            }
            return (Modes)(-1);
        }
    }

    //SKEET MODE:
    PlayerController player { get { return PlayerController.Instance; } }
    public Transform SkeetStartPosition;

    bool canShoot = false;
    public List<Transform> stations;
    int station = 0;
    //0 next station 1 high 2 low 3 double
    int[] launchPattern = new int[] { 1, 2, 3, 0, 1, 2, 3, 0, 1, 2, 0, 1, 2, 0, 1, 2, 0, 1, 2, 3, 0, 1, 2, 3, 0, 1, 2 };
    private int maxSkeetPoints { get { int max = 0; for (int i = 0; i < launchPattern.Length; i++) { var g = launchPattern[i]; if (g == 1 || g == 2) max += 100; else if (g == 3) max += 200; } return max; } }
    int timesPulled = 0;
    public PidgeonLauncherSkeetTower highTower, lowTower;

    private static bool objectPoolInit = false;

    void Awake()
    {
        GameMenu = GameObject.Find("GameMenu");
    }

    void Start()
    {
        Instance = this;
        loadedMode = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (loadedMode == Skeet)
            SkeetRoundDisplay.Instance.DeactivateUI();

        if (!objectPoolInit)
        {
            ObjectPool.OPPreloadObjects("Bottle1Fragments", 10);
            ObjectPool.OPPreloadObjects("BrownBottleFragments", 10);
            ObjectPool.OPPreloadObjects("SignFragments", 10);
            ObjectPool.OPPreloadObjects("ClayPidgeonFragments", 10);
            ObjectPool.OPPreloadObjects("DishFragments", 10);
            ObjectPool.OPPreloadObjects("PigFragments", 10);
            ObjectPool.OPPreloadObjects("UFOFragments", 10);
            objectPoolInit = true;
        }
    }
    public void Update()
    {

    }

    public void StartGame()
    {
        player.CanTeleport = false;
        VRApiManager.Achievements.PigCounterReset();
        points = 0;
        gameStarted = true;
        GameMenu.SetActive(false);
        HidePickups();

        switch (loadedMode)
        {
            case Trap:
                Destroy(pidgeonLauncher.gameObject);
                GameObject launcher = Instantiate(PidgeonLauncherPrefab, PidgeonLauncherSpawnPosition.position, PidgeonLauncherSpawnPosition.rotation) as GameObject;
                launcher.transform.parent = PidgeonLauncherSpawnPosition;
                launcher.transform.localPosition = Vector3.zero;
                launcher.transform.localRotation = Quaternion.identity;
                pidgeonLauncher = launcher.GetComponent<PidgeonLauncher>();
                targetPoolThisGame = new List<int>(targetPool);
                dishSpawner.ResetDishes();
                pidgeonLauncher.launcherRunning = true;
                break;
            case Skeet:
                if (VRApiManager.c_IsOpenVR)
                {
                    PlayerController.Instance.ControlsDisplay.LeftTouchpadText(false);
                }
                player.transform.parent = stations[0];
                player.transform.localPosition = Vector3.zero;
                player.canPull = true;
                SkeetRoundDisplay.Instance.SetRoundValue(launchPattern[timesPulled]);
                canShoot = true;
                Table.SetActive(false);
                break;
            case Quickdraw:
                player.ControlsDisplay.SetUpQuickdrawUI();
                GameObject.Find("QuickdrawSigns").GetComponent<QuickdrawSignManager>().StartQuickdrawMode();
                Table.SetActive(false);
                break;
            case Bounce:
                player.ControlsDisplay.SetUpQuickdrawUI();
                GameObject.Find("BounceBallManager").GetComponent<BounceBallManager>().StartBounceMode();
                break;
            case Targetwall:
                GameObject.Find("Targetwall").GetComponent<TargetwallManager>().StartTargetwall();
                break;
            case Animalgallery:
                GameObject.Find("AnimalGallery").GetComponent<AnimalGalleryManager>().StartAnimalMode();
                break;
        }
    }
    public void GameOver()
    {
        StartCoroutine(GameOverCoroutine());
    }
    
    private IEnumerator GameOverCoroutine()
    {
        yield return new WaitForSeconds(2f);

        if (loadedMode == Skeet)
        {
            if (VRApiManager.c_IsOpenVR)
            {
                PlayerController.Instance.ControlsDisplay.LeftTouchpadText(true);
            }
        }

        Table.SetActive(true);
        player.CanTeleport = true;
        if (loadedMode != Quickdraw)
        {
            MainMenu.Instance.LastScore = points;
            VRApiManager.Leaderboard.TrySetHighScore(CurrentMode, PlayerController.Instance.CurrentGun, points);
            Delay(1f, () => { MainMenu.Instance.LeaderboardUI.ForceRefresh(); });
            MainMenu.Instance.UpdateUI();
        }

        if (loadedMode == Trap && points >= 12000)
            VRApiManager.Achievements.GrantAchievement(Achievements.WHAT_A_MESS);
        else if (loadedMode == Skeet && points == maxSkeetPoints)
        {
            GameObject.Find("FireworksManager").GetComponent<FireworksManager>().StartFireworks();
            VRApiManager.Achievements.GrantAchievement(Achievements.WINNER_WINNER);
        }

        GameMenu.SetActive(true);
        if (loadedMode == Skeet)
        {
            player.transform.parent = SkeetStartPosition;
            player.transform.localPosition = Vector3.zero;
            player.canPull = false;
            SkeetRoundDisplay.Instance.DeactivateUI();
        }
        ShowPickups();
    }
    public void AddPoints(Shootable shotTransform, int givenPoints)
    {
        PointEffect.InstantiatePointEffect(shotTransform, givenPoints);
        points += givenPoints;
    }

    public void SkeetPull()
    {
        if (loadedMode == Trap)
        {
            StartGame();
        }
        else if (canShoot)
        {
            int thisLaunch = launchPattern[timesPulled];
            //Debug.Log("Times Pulled: " + timesPulled + " Station:" + station + "Launch pattern: " + thisLaunch);
            SkeetRoundDisplay.Instance.DeactivateUI();
            //The towers don't pull if you're hitting the button too fast, so return false and don't increment timesPulled if you didn't pull.
            switch (thisLaunch)
            {
                case 0:
                    SkeetNextStation();
                    break;
                case 1:
                    player.PullShouts.CallPull();
                    StartCoroutine(SkeetHighShot(.75f, 1.25f, true));
                    canShoot = false;
                    break;
                case 2:
                    player.PullShouts.CallPull();
                    StartCoroutine(SkeetLowShot(.75f, 1.25f, true));
                    canShoot = false;
                    break;
                case 3:
                    player.PullShouts.CallPull();
                    StartCoroutine(SkeetHighShot(.75f, 1.25f, false));
                    StartCoroutine(SkeetLowShot(2.75f, 3.25f, true));
                    canShoot = false;
                    break;
            }
        }
    }
    public void SkeetManualEndRound(ClayPidgeon lastPidgeon)
    {
        if (lastPidgeon.SkeetModeRound == timesPulled)
        {
            SkeetEndRound();
        }
    }
    private void SkeetNextStation()
    {
        station++;
        player.transform.parent = stations[station];
        player.transform.localPosition = Vector3.zero;
        SkeetEndRound();
    }
    private IEnumerator SkeetHighShot(float minDelay, float maxDelay, bool finalShot)
    {
        float lerp = Random.Range(0f, 1f);
        float wait = Mathf.Lerp(minDelay, maxDelay, lerp);
        yield return new WaitForSeconds(wait);
        highTower.ShootPidgeon(finalShot, timesPulled);
        if (finalShot)
        {
            StartCoroutine(SkeetFinalShot());
        }
    }
    private IEnumerator SkeetLowShot(float minDelay, float maxDelay, bool finalShot)
    {
        float lerp = Random.Range(0f, 1f);
        float wait = Mathf.Lerp(minDelay, maxDelay, lerp);
        yield return new WaitForSeconds(wait);
        lowTower.ShootPidgeon(finalShot, timesPulled);
        if (finalShot)
        {
            StartCoroutine(SkeetFinalShot());
        }
    }
    private IEnumerator SkeetFinalShot()
    {
        int currentRound = timesPulled;
        yield return new WaitForSeconds(7f);
        if (timesPulled == currentRound)
        {
            SkeetEndRound();
            Debug.LogWarning("last pidgeon not destroyed, automatically advancing");
        }
    }
    private void SkeetEndRound()
    {
        timesPulled++;

        if (timesPulled > launchPattern.Length - 1)
        {
            station = 0;
            timesPulled = 0;
            GameOver();
        }
        else
        {
            canShoot = true;
            SkeetRoundDisplay.Instance.SetRoundValue(launchPattern[timesPulled]);
        }
    }

    List<int> targetPool = new List<int> { 1, 1, 1, 1, 2, 2, 2, 2, 3, 3 };
    List<int> targetPoolThisGame;

    public void OnTrapShoot()
    {
        SpawnUFO();
        SpawnPig();
        SpawnTargets();
        SpawnDish();
    }

    private void SpawnUFO()
    {
        if (pidgeonLauncher.pidgeonsLaunched == 20 || pidgeonLauncher.pidgeonsLaunched == 40 || pidgeonLauncher.pidgeonsLaunched == 60 || pidgeonLauncher.pidgeonsLaunched == 80)
        {
            GameObject UFO = Instantiate(UFOPrefab, UFOSpawners.GetRandomValue<GameObject>().transform.position, Quaternion.identity) as GameObject;
            UFO.name = UFOPrefab.name;
        }
    }

    private void SpawnPig()
    {
        if (pidgeonLauncher.pidgeonsLaunched == 30 || pidgeonLauncher.pidgeonsLaunched == 50 || pidgeonLauncher.pidgeonsLaunched == 70 || pidgeonLauncher.pidgeonsLaunched == 90)
        {
            GameObject chosenSpawner = PigSpawners.GetRandomValue<GameObject>();
            Instantiate(PigPrefab, chosenSpawner.transform.position, chosenSpawner.transform.rotation);
        }
    }

    private void SpawnDish()
    {

        if (pidgeonLauncher.pidgeonsLaunched == 15 || pidgeonLauncher.pidgeonsLaunched == 25 || pidgeonLauncher.pidgeonsLaunched == 45 || pidgeonLauncher.pidgeonsLaunched == 65 || pidgeonLauncher.pidgeonsLaunched == 85 || pidgeonLauncher.pidgeonsLaunched == 95)
        {
            dishSpawner.SpawnDish();
        }
    }

    private void SpawnTargets()
    {
        if (pidgeonLauncher.pidgeonsLaunched % 10 == 0 && pidgeonLauncher.pidgeonsLaunched < 100)
        {
            if (Random.Range(0, 5) < 2)
            {
                int randomIndex = Random.Range(0, targetPoolThisGame.Count);
                int targetsToSpawn = targetPoolThisGame[randomIndex];
                targetPoolThisGame.RemoveAt(randomIndex);
                switch (targetsToSpawn)
                {
                    case 1:
                        int targetToSpawn = Random.Range(0, 3);
                        TargetSpawners[targetToSpawn].SpawnTarget();
                        break;
                    case 2:
                        int targetNotToSpawn = Random.Range(0, 3);
                        if (targetNotToSpawn != 0)
                        {
                            TargetSpawners[0].SpawnTarget();
                        }
                        if (targetNotToSpawn != 1)
                        {
                            TargetSpawners[1].SpawnTarget();
                        }
                        if (targetNotToSpawn != 2)
                        {
                            TargetSpawners[2].SpawnTarget();
                        }
                        break;
                    case 3:
                        foreach (TargetSpawn target in TargetSpawners)
                        {
                            target.SpawnTarget();
                        }
                        break;
                }

            }
        }
    }

    void HidePickups()
    {
        OverUnderPickup.SetActive(false);
        SideBySidePickup.SetActive(false);
        PumpActionPickup.SetActive(false);
        TommygunPickup.SetActive(false);
    }

    void ShowPickups()
    {
        OverUnderPickup.SetActive(true);
        SideBySidePickup.SetActive(true);
        PumpActionPickup.SetActive(true);
        TommygunPickup.SetActive(true);
        switch (player.CurrentGun)
        {
            case Guns.SideBySide:
                SideBySidePickup.SetActive(false);
                break;
            case Guns.OverUnder:
                OverUnderPickup.SetActive(false);
                break;
            case Guns.PumpAction:
                PumpActionPickup.SetActive(false);
                break;
            case Guns.Tommygun:
                TommygunPickup.SetActive(false);
                break;
        }
    }

    public void Delay(float f, System.Action e)
    {
        StartCoroutine(DelayInternal(f, e));
    }
    private IEnumerator DelayInternal(float f, System.Action e)
    {
        yield return new WaitForSeconds(f);
        e();
    }
}

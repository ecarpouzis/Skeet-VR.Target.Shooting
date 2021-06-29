using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardInfo
{
    public string dlc;
    public int rank;
    public string username;
    public float score;

    public LeaderboardInfo(string givenDLC, int givenRank, string givenName, float givenScore)
    {
        dlc = givenDLC;
        rank = givenRank;
        username = givenName;
        score = givenScore;
    }
}

public class LeaderboardUI : MonoBehaviour
{
    public static readonly int MaxEntries = DynamicVRApi.LeaderboardApi.NumLBEntries;

    public Text FilterLabel;
    public Text ConfigurationLabel;
    public Text LeaderboardMessage;
    public GameObject LeaderboardEntryPrefab;

    private List<LeaderboardEntry> leaderboard = new List<LeaderboardEntry>();
    private float heightOfEntries = .26f;

    private LeaderboardFilter RealFilter;
    private Modes RealMode { get { return GameTracker.Instance.CurrentMode; } }
    private Guns RealGuns { get { return PlayerController.Instance.CurrentGun; } }

    private Modes CurrentMode = (Modes)(-1);
    private Guns CurrentGun = (Guns)(-1);
    private LeaderboardFilter CurrentFilter = (LeaderboardFilter)(-1);

    private bool success = false;

    private void Awake()
    {
        for (int i = 0; i < MaxEntries; i++)
        {
            LeaderboardEntry newEntry = Instantiate(LeaderboardEntryPrefab, transform.position, transform.rotation, transform).GetComponent<LeaderboardEntry>();
            newEntry.transform.position = new Vector3(newEntry.transform.position.x, newEntry.transform.position.y - (leaderboard.Count * heightOfEntries), newEntry.transform.position.z);
            newEntry.transform.localScale = transform.localScale;
            newEntry.createEntry(DLCUnlock.None, 0, "", 0);
            leaderboard.Add(newEntry);
            newEntry.gameObject.SetActive(false);
        }
    }

    public void ClearLeaderboard()
    {
        foreach (LeaderboardEntry entry in leaderboard)
        {
            entry.gameObject.SetActive(false);
        }
    }
    public void SetFilter(LeaderboardFilter lbFilter)
    {
        FilterLabel.text = lbFilter.ToString() + " Leaderboard";
        ConfigurationLabel.text = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + " - " + PlayerController.Instance.CurrentGun.ToString();

        RealFilter = lbFilter;

        UpdateUI();
    }
    public void ForceRefresh()
    {
        LoadLeaderboard(RealMode, RealGuns, RealFilter);
    }
    public void UpdateUI()
    {
        if (!success ||
            RealMode != CurrentMode ||
            RealGuns != CurrentGun ||
            RealFilter != CurrentFilter)
        {
            LoadLeaderboard(RealMode, RealGuns, RealFilter);
        }
    }

    private void LoadLeaderboard(Modes m, Guns g, LeaderboardFilter lbFilter)
    {
        ClearLeaderboard();
        success = false;
        LeaderboardMessage.text = "Loading Leaderboard: " + m.ToString() + g.ToString() + lbFilter.ToString();
        VRApiManager.Leaderboard.GetHighScores(m, g, lbFilter, (del) =>
        {
            if (del.Status == DynamicVRApi.Status.Succeess)
            {
                LeaderboardMessage.text = "";

                DynamicVRApi.Score[] scores = del.Payload;
                SetLeaderboard(scores, lbFilter);
                success = true;
            }
            else if (del.Status == DynamicVRApi.Status.NotInitialized)
            {
                LeaderboardMessage.text = "The leaderboard API is not initialized.";
            }
            else if (del.Status == DynamicVRApi.Status.ApiFail)
            {
                Debug.Log("Api Failure");
                LeaderboardMessage.text = "Error getting scores from Leaderboard API.";
            }
        });

        CurrentMode = m;
        CurrentGun = g;
        CurrentFilter = lbFilter;
    }

    private void SetLeaderboard(DynamicVRApi.Score[] scores, LeaderboardFilter lbFilter)
    {
        ClearLeaderboard();
        if (MaxEntries != leaderboard.Count)
            Debug.LogError("Invalid Leaderboard Count");

        for (int i = 0; i < MaxEntries; i++)
        {
            if (i < scores.Length)
            {
                var s = scores[i];
                var lb = leaderboard[i];
                lb.gameObject.SetActive(true);
                lb.rank.text = s.Rank.ToString();
                lb.username.text = s.Name;
                lb.score.text = s.Value.ToString();
                lb.SetDLCIcon(s.DLCUnlock);
            }
            else
            {
                leaderboard[i].gameObject.SetActive(false);
            }
        }
    }
}

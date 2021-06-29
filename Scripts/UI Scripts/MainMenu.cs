using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance { get; private set; }

    public Text
        EquippedDisplay,
        HighScoreDisplay,
        PreviousScoreDisplay;

    public LeaderboardUI LeaderboardUI;


    [HideInInspector]
    public float LastScore = 0f;

    private Modes CurrentMode { get { return GameTracker.Instance.CurrentMode; } }
    private Guns CurrentGun { get { return PlayerController.Instance.CurrentGun; } }

    void Start()
    {
        if (Instance != null)
            Debug.LogWarning("Multiple Main Menu Scripts");
        Instance = this;


        PreviousScoreDisplay.text = "";
    }

    public static string GetGunName(Guns g)
    {
        switch (g)
        {
            case Guns.OverUnder:
                return "Over Under";
            case Guns.PumpAction:
                return "Pump Action";
            case Guns.SideBySide:
                return "Side by Side";
            case Guns.Tommygun:
                return "Tommy Gun";
            default:
                Debug.LogError("Invalid gun");
                return "Null";
        }
    }

    public void UpdateUI()
    {
        if (EquippedDisplay != null && HighScoreDisplay != null && PreviousScoreDisplay != null)
        {
            EquippedDisplay.text = "Equipped: " + GetGunName(CurrentGun);

            if (VRApiManager.Leaderboard.HasHighScore(CurrentMode, CurrentGun))
            {
                if (CurrentMode == Modes.Quickdraw)
                {
                    HighScoreDisplay.text = "Fastest Time: " + VRApiManager.Leaderboard.GetMyHighScore(CurrentMode, CurrentGun) + "s";
                    if (LastScore > 0f)
                        PreviousScoreDisplay.text = "Previous Time: " + LastScore + "s";
                    else
                        PreviousScoreDisplay.text = "";
                }
                else
                {
                    HighScoreDisplay.text = "High Score: " + VRApiManager.Leaderboard.GetMyHighScore(CurrentMode, CurrentGun);
                    if (LastScore > 0f)
                        PreviousScoreDisplay.text = "Previous Score: " + LastScore;
                    else
                        PreviousScoreDisplay.text = "";
                }
            }
            else
            {
                HighScoreDisplay.text = "";
                PreviousScoreDisplay.text = "";
            }

            LeaderboardUI.UpdateUI();
        }
    }
}

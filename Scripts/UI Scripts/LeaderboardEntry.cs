using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LeaderboardEntry : MonoBehaviour
{
    public Image icon;
    public Text rank, username, score;

    public Sprite BronzeIcon, SilverIcon, GoldIcon, EmptyIcon;

    public void createEntry(DLCUnlock dlc, int givenRank, string givenName, float givenScore)
    {
        SetDLCIcon(dlc);

        rank.text = givenRank.ToString();
        username.text = givenName;
        score.text = givenScore.ToString();
    }

    public void SetDLCIcon(DLCUnlock dlc)
    {
        if (dlc.HasDLC(DLCUnlock.Gold))
            icon.sprite = GoldIcon;
        else if (dlc.HasDLC(DLCUnlock.Silver))
            icon.sprite = SilverIcon;
        else if (dlc.HasDLC(DLCUnlock.Bronze))
            icon.sprite = BronzeIcon;
        else
            icon.sprite = EmptyIcon;
    }
}

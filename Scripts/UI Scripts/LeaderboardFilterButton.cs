using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardFilterButton : Shootable {
    public LeaderboardFilter LeaderboardFilter;
    public LeaderboardUI LeaderboardUI;

    override public void OnHit()
    {
        LeaderboardUI.SetFilter(LeaderboardFilter);
    }
}

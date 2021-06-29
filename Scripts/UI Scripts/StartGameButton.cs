using UnityEngine;
using System.Collections;

public class StartGameButton : Shootable
{

    override public void OnHit()
    {
        GameObject.Find("GameTracker").GetComponent<GameTracker>().StartGame();
    }
}

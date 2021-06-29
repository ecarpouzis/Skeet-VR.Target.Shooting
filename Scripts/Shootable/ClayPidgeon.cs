using UnityEngine;
using System.Collections;

public class ClayPidgeon : Shootable
{
    public bool dontScore = false;
    [HideInInspector]
    public bool LastPidgeon = false;
    [HideInInspector]
    public int SkeetModeRound = -1;
    int pointValue = 100;

    override public void OnHit()
    {
        VRApiManager.Achievements.GrantAchievement(Achievements.NUMBER_ONE);
        if (!dontScore)
        {
            GameObject.Find("GameTracker").GetComponent<GameTracker>().AddPoints(this, pointValue);
            if (LastPidgeon)
                GameTracker.Instance.SkeetManualEndRound(this);
        }
        base.OnHit();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Terrain")
        {
            Destroy(gameObject);
            if (LastPidgeon)
            {
                GameTracker.Instance.SkeetManualEndRound(this);
                //Debug.Log("Last Pidgeon Destroyed");
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        if (!dontScore)
        {
            GetComponent<SoundSubClip>().Play();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

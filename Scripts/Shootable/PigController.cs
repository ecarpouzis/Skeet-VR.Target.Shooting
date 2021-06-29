using UnityEngine;
using System.Collections;

public class PigController : Shootable
{
    float speed = 10f;
    int pointValue = 500;
    Vector3 startingPosition;
    float timeSinceStart = 0;
    bool countingUp = true;

    void Start()
    {
        startingPosition = transform.position;
    }

    override public void OnHit()
    {
        VRApiManager.Achievements.PigCounterAdd();
        GameObject.Find("GameTracker").GetComponent<GameTracker>().AddPoints(this, pointValue);
        base.OnHit();
    }

    // Update is called once per frame
    void Update()
    {
        float timescale = 2f;
        float distance = .5f;

        if (countingUp)
            timeSinceStart += Time.deltaTime;
        else
            timeSinceStart -= Time.deltaTime;

        if (timeSinceStart * timescale > 1)
            countingUp = false;
        else if (timeSinceStart * timescale < 0)
            countingUp = true;


        float newY = Mathf.Lerp(startingPosition.y + distance, startingPosition.y - distance, timescale * timeSinceStart);

        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }

}

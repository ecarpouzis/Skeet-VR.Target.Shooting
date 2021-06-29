using UnityEngine;
using System.Collections;

public class Dish : Shootable
{
    int pointValue = 250;
    float startingForce = 7f;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Terrain")
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {
        GetComponent<Rigidbody>().velocity = new Vector3(0f, startingForce, 0f);
    }

    override public void OnHit()
    {
        GameObject.Find("GameTracker").GetComponent<GameTracker>().AddPoints(this, pointValue);
        base.OnHit();
    }
}

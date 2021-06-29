using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceBallManager : MonoBehaviour
{
    public PlayerController player { get { return PlayerController.Instance; } }
    public BouncingBall Ball;
    public GameObject BallPrefab;
    public Transform BounceSpawnPosition;
    float timeSinceStart = 0f;
    float timeToDraw = 0f;
    float timeCompleted = 0f;
    bool bounceStarted = false;
    public GameTracker gameTracker;
    public Transform Northmost;
    public Transform Eastmost;
    public Transform Westmost;
    public Transform Southmost;
    Rigidbody ballBody;
    
    public void StartBounceMode()
    {
        PlayerController.Instance.ControlsDisplay.quickloadText.text = "0";
        timeCompleted = 0f;
        GameObject newBall = Instantiate(BallPrefab, BounceSpawnPosition.position, Quaternion.identity) as GameObject;
        Ball = newBall.GetComponent<BouncingBall>();
        ballBody = Ball.GetComponent<Rigidbody>();
        if(player.CurrentGun == Guns.PumpAction)
        {
            ballBody.mass = 1.3f;
        } if(player.CurrentGun == Guns.Tommygun)
        {
            ballBody.mass = 1.6f;
        }
        else
        {
            ballBody.mass = .75f;
        }
        bounceStarted = true;
    }

    public void EndBounceMode()
    {
        if (gameTracker.Points >= 15)
            VRApiManager.Achievements.GrantAchievement(Achievements.OFF_THE_WALLS);

        bounceStarted = false;
        PlayerController.Instance.ControlsDisplay.quickloadText.text = "";
        gameTracker.GameOver();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (bounceStarted)
        {
            player.ControlsDisplay.quickloadText.text = Ball.bounces.ToString();

            Vector3 newVelocity = ballBody.velocity;

            if (Ball.transform.position.z > Northmost.position.z)
            {
                if (newVelocity.z > 0)
                {
                    newVelocity.z *= -1;
                }
            }
            else if (Ball.transform.position.z < Southmost.position.z)
            {
                if (newVelocity.z < 0)
                {
                    newVelocity.z *= -1;
                }
            }
            if (Ball.transform.position.x < Westmost.position.x)
            {
                if (newVelocity.x < 0)
                {
                    newVelocity.x *= -1;
                }
            }
            else if (Ball.transform.position.x > Eastmost.position.x)
            {
                if (newVelocity.x > 0)
                {
                    newVelocity.x *= -1;
                }
            }
            ballBody.velocity = newVelocity;
        }
    }
}
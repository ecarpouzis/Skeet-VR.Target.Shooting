using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UFOController : Shootable
{
    public Material lightOnMaterial;
    public Material lightOffMaterial;
    float lightTimer = 0f;
    float lightIncrement = .05f;
    int lightOn = 0;
    public List<GameObject> lights;
    GameObject flyTarget;

    public SoundSubClip laserSound;
    public LineRenderer laser;
    float laserCooldown = 1f;
    float timeSinceLaser = 0f;
    bool isLasering = false;
    //Time I've been lasering
    float timeLasering = .0f;
    float timeToBlowUpPidgeon = .2f;
    GameObject targettedPidgeon;

    public override void OnHit()
    {
        VRApiManager.Achievements.UFOCounterAdd();
        base.OnHit();
    }

    //Used for flying
    float timeSinceStart = 0f;

    // Use this for initialization
    void Start()
    {
        flyTarget = GameObject.Find("PidgeonTarget");
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceStart += Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, flyTarget.transform.position, timeSinceStart / 300);
        lightTimer += Time.deltaTime;
        timeSinceLaser += Time.deltaTime;

        if (lightTimer > lightIncrement)
        {
            lights[lightOn].GetComponent<Renderer>().material = lightOffMaterial;
            lightOn++;
            if (lightOn >= lights.Count)
            {
                lightOn = 0;
            }
            lights[lightOn].GetComponent<Renderer>().material = lightOnMaterial;
            lightTimer = 0f;
        }

        if (timeSinceLaser > laserCooldown && !isLasering)
        {
            ShootLaser();
        }

        if (isLasering)
        {
            timeLasering += Time.deltaTime;

            laser.SetVertexCount(2);
            laser.SetPosition(0, laser.transform.position);
            if (targettedPidgeon != null)
            {
                laser.SetPosition(1, targettedPidgeon.transform.position);
            }
            if (timeLasering > timeToBlowUpPidgeon)
            {
                if (targettedPidgeon != null)
                {
                    Destroy(targettedPidgeon);
                }
                targettedPidgeon = null;
                isLasering = false;
                laserSound.Stop();
                timeLasering = 0f;
                timeSinceLaser = 0f;
            }
        }
        else
        {
            laser.SetVertexCount(0);
        }

    }

    void ShootLaser()
    {
        targettedPidgeon = GameObject.Find("ClayPidgeon");
        if (targettedPidgeon != null)
        {
            laserSound.Play();
            isLasering = true;
        }
    }

}

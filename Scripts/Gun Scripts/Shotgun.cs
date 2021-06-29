using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Shotgun : MonoBehaviour
{
    public Rigidbody myRigidbody;
    public BulletCollider bulletCone;
    public SoundSubClip shotSound, reloadSound, dryfireSound;
    public ParticleSystem bullets;
    public ParticleSystem muzzleFlash;
    public ShellEject shellEject;
    float timeBetweenShots = 0f;
    float minTimeBetweenShots = 0f;
    bool canShoot = false;
    int loadedAmmo;
    int ammoPerLoad = 1;
    PlayerController player { get { return PlayerController.Instance; } }

    // Use this for initialization
    void Start()
    {
        minTimeBetweenShots = .1f;
    }

    // Update is called once per frame
    void Update()
    {
        timeBetweenShots += Time.deltaTime;
        if (timeBetweenShots > minTimeBetweenShots)
        {
            canShoot = true;
        }
    }

    public void LoadAmmo()
    {
        //Reload Haptic:
        //Left controller because you grab the gun with left
        player.TriggerHaptics(.1f, Controller.Left, 0);
        reloadSound.Play(.3f, .6f);
        loadedAmmo = ammoPerLoad;
    }

    public void EjectShell()
    {
        //Shotgun Haptic:
        //Left controller because you grab the gun with left
        player.TriggerHaptics(.1f, Controller.Left, 0);
        reloadSound.Play(0f, .25f);
        shellEject.ejectShell();
    }

    public void Shoot()
    {
        if (loadedAmmo > 0 && canShoot)
        {
            //Shotgun Haptic Shoot:
            player.TriggerHaptics(.1f, Controller.Right, 1);
            
            muzzleFlash.Play();
            bullets.Play();
            shotSound.Play(0f, 1f);
            List<Shootable> shotObjects = bulletCone.seenObjects;
            int clayPidgeonCount = 0;
            if (shotObjects.Count > 0)
            {
                foreach (Shootable shootable in shotObjects)
                {
                    if (shootable != null)
                    {
                        if (shootable is ClayPidgeon)
                        {
                            clayPidgeonCount++;
                            if (clayPidgeonCount == 2)
                                VRApiManager.Achievements.GrantAchievement(Achievements.DAMN_FINE);
                        }
                        shootable.OnHit();
                    }
                }
                bulletCone.clearObjects();
            }
            loadedAmmo--;
            canShoot = false;
            timeBetweenShots = 0f;
        }
        else
        {
            //Dryfire
            dryfireSound.Play();
        }
    }

}
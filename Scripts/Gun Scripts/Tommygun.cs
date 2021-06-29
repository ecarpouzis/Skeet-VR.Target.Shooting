using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tommygun : MonoBehaviour
{
    public Rigidbody myRigidbody;
    public BulletCollider bulletCone;
    public SoundSubClip shotSound, reloadSound, dryfireSound;
    public ParticleSystem muzzleFlash;
    public ShellEject shellEject;
    public TommygunBulletShooter bulletShooter;

    public DrumEject drumPosition;
    public GameObject loadedDrum;
    
    float timeBetweenShots = 0f;
    float minTimeBetweenShots = .3f;
    bool canShoot = false;
    int loadedAmmo;
    bool isShooting = false;
    int ammoPerLoad = 1;
    PlayerController player { get { return PlayerController.Instance; } }

    // Use this for initialization
    void Start()
    {
        minTimeBetweenShots = .1f;
        LoadAmmo();
    }

    // Update is called once per frame
    void Update()
    {
        timeBetweenShots += Time.deltaTime;
        if (timeBetweenShots > minTimeBetweenShots)
        {
            canShoot = true;
        }
        if (canShoot && isShooting)
        {
            AutomaticShot();
        }
    }

    public void AutomaticShot()
    {
        if (loadedAmmo > 0)
        {
            //Shotgun Haptic Shoot:
            player.TriggerHaptics(.1f, Controller.Right, 1);

            muzzleFlash.Play();
            bulletShooter.shootBullet();
            EjectShell();
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
            canShoot = false;
            timeBetweenShots = 0f;
        }
    }

    public bool LoadAmmo()
    {
        //Reload Haptic:
        player.TriggerHaptics(.1f, Controller.Left, 0);
        reloadSound.Play(.3f, .6f);
        loadedAmmo = 30;
        loadedDrum.SetActive(true);
        return true;
    }

    public void EjectShell()
    {
        //Shotgun Haptic:
        player.TriggerHaptics(.1f, Controller.Right, 0);
        reloadSound.Play(0f, .25f);
        shellEject.ejectShell();
    }

    public void StartShoot()
    {
        isShooting = true;
    }

    public void StopShoot()
    {
        isShooting = false;

        if (loadedAmmo == 0 && loadedDrum.activeInHierarchy)
        {
            EjectDrum();
        }
    }

    public void EjectDrum()
    {
        loadedDrum.SetActive(false);
        drumPosition.EjectDrum();
    }
}

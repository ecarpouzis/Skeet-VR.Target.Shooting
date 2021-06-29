using UnityEngine;
using System.Collections;

public class BreakbarrelAmmoSpawner : MonoBehaviour
{
    public Transform tommygunHeldAmmoPosition;
    public GameObject ammoPrefab;
    public Transform HeldAmmoPosition;
    public GameObject tommygunAmmoDrumPrefab;
    AudioSource BulletWoosh;

    private float minSpawnDelay = .2f;
    PlayerController player { get { return PlayerController.Instance; } }
    private bool canSpawn = true;
    private Transform heldAmmo;
    private Rigidbody heldRigidbody;
    private BoxCollider heldCollider;
    private bool holdingAmmo = false;

    public void SpawnAmmo()
    {
        if (canSpawn)
        {
            if (holdingAmmo)
                ReleaseAmmo(Vector3.zero, Vector3.zero);
            
            holdingAmmo = true;
            canSpawn = false;
            Delay(minSpawnDelay, () => { canSpawn = true; });
            GameObject ammo;

            if (player.CurrentGun == Guns.Tommygun)
            {
                ammo = Instantiate(tommygunAmmoDrumPrefab) as GameObject;
                heldAmmo = ammo.transform;
                heldAmmo.name = tommygunAmmoDrumPrefab.name;
                heldAmmo.transform.parent = tommygunHeldAmmoPosition;
            }
            else
            {
                ammo = Instantiate(ammoPrefab, transform.position, transform.rotation) as GameObject;
                heldAmmo = ammo.transform;
                heldAmmo.name = ammoPrefab.name;
                heldAmmo.transform.parent = HeldAmmoPosition;
                if (player.CurrentGun == Guns.OverUnder)
                {
                    heldAmmo.transform.localScale = new Vector3(.7f, 1.2f, .7f);
                }
            }
            BulletWoosh.Stop();
            BulletWoosh.Play();
            heldAmmo.transform.localRotation = Quaternion.identity;
            heldAmmo.transform.localPosition = Vector3.zero;
            heldRigidbody = ammo.GetComponent<Rigidbody>();
            heldRigidbody.useGravity = false;
            heldRigidbody.isKinematic = true;
            heldCollider = ammo.GetComponent<BoxCollider>();
            heldCollider.isTrigger = true;
        }
    }

    public void ReleaseAmmo(Vector3 velocity, Vector3 angularVelocity)
    {
        if (holdingAmmo)
        {
            if (heldRigidbody != null)
            {
                heldRigidbody.velocity = velocity;
                heldRigidbody.angularVelocity = angularVelocity;
                heldRigidbody.useGravity = true;
                heldRigidbody.isKinematic = false;
                heldRigidbody.transform.parent = null;
                var g = heldRigidbody.gameObject.AddComponent<SelfDestruct>();
                g.givenTime = 3f;
            }
            if (heldCollider != null)
            {
                heldCollider.isTrigger = false;
            }
            holdingAmmo = false;
        }
    }

    void Awake()
    {
        BulletWoosh = GetComponent<AudioSource>();
    }

    void Start()
    {
        canSpawn = true;
    }
    
    private void Delay(float f, System.Action e)
    {
        StartCoroutine(DelayInternal(f, e));
    }
    private IEnumerator DelayInternal(float f, System.Action e)
    {
        yield return new WaitForSeconds(f);
        e();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TommygunBulletShooter : MonoBehaviour {
    
    public GameObject bulletPrefab;
    public GameObject gun;
    public float bulletForce = 3000f;

    public void shootBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position + transform.forward * Random.Range(0f, 1.5f), transform.rotation) as GameObject;
        bullet.GetComponent<Rigidbody>().AddForce(transform.forward * bulletForce);
    }
}

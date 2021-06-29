using UnityEngine;
using System.Collections;

public class SelfDestruct : MonoBehaviour
{
    public float givenTime;
    private float timeAlive = 0f;

    void OnEnable()
    {
        timeAlive = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        timeAlive += Time.deltaTime;
        if (timeAlive > givenTime)
        {
            Destroy(gameObject);
        }
    }
}

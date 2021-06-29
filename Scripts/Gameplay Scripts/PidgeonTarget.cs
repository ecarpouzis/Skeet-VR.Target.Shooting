using UnityEngine;
using System.Collections;

public class PidgeonTarget : MonoBehaviour
{
    public int highNum;
    public int lowNum;
    int speed = 50;
    bool goingLeft = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3((Mathf.PingPong(Time.time * speed, highNum) - lowNum), transform.position.y, transform.position.z);
    }
}

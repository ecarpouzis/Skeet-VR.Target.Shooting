using UnityEngine;
using System.Collections;

public class DestroyableSign : Shootable
{
    override public void OnHit()
    {
        if (transform.parent.name == "ArcadeSign")
        {
            base.OnHit();
            Destroy(transform.parent.gameObject);
        }
        else
        {
            base.OnHit();
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

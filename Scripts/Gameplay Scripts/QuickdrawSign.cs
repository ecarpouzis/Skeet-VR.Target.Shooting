using UnityEngine;
using System.Collections;

public class QuickdrawSign : MonoBehaviour {
    public bool chosenSign = false;
    public QuickdrawTarget myTarget;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        if (chosenSign) {
            transform.LookAt(PlayerController.Instance.transform.position);
            myTarget.chosenSign = true;
            }
	}
}

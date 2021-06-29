using UnityEngine;
using System.Collections;

public class QuickdrawTarget : Shootable {
    public bool chosenSign = false;

    override public void OnHit()
    {
        if (chosenSign == true)
        {
            GameObject.Find("QuickdrawSigns").GetComponent<QuickdrawSignManager>().RemoveSign(this.transform.parent.gameObject);
            base.OnHit();
        }
    }
}

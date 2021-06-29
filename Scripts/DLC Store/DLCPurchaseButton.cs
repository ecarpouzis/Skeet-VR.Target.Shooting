using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DLCPurchaseButton : Shootable
{
    public DLCUnlock dlc;

    override public void OnHit()
    {
        OculusIAP.TryPurchase(dlc);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OculusIAP
{
    public static void TryPurchase(DLCUnlock dlc)
    {
        if (VRApiManager.p_IsOculus && VRApiManager.p_IsInit)
        {
            if (!VRApiManager.DLC.UnlockedDLC().HasDLC(dlc))
            {
                string sku = VRApiManager.OculusDLC.SkuFromDLC(dlc);

                var req = Oculus.Platform.IAP.LaunchCheckoutFlow(sku);

                req.OnComplete((x) =>
                {
                    if (x.IsError)
                    {
                        Oculus.Platform.Models.Error e = x.GetError();
                        Debug.Log("IAP.LaunchCheckoutFlow() Error code: " + e.Code + " http: " + e.HttpCode + " msg: " + e.Message);
                    }
                    else
                    {
                        VRApiManager.OculusDLC.UpdateCache(() =>
                        {
                            var newdlc = VRApiManager.DLC.UnlockedDLC();
                            Debug.Log("IAPCheckoutFlow complete, unlocked DLC: " + newdlc.ToString());
                        });
                    }
                });
            }
        }
    }
}

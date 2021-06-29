using UnityEngine;
using System.Collections;

public class CreditsButton : Shootable {
    override public void OnHit()
    {
        VRApiManager.Achievements.GrantAchievement(Achievements.RED_ROOM);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Credits");
    }
}

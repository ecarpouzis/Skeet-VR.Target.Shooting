using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeModeButton : Shootable
{
    public Image ButtonFade;

    private static bool transition = false;
    
    void Start()
    {
        transition = false;

        if (ButtonFade == null)
            Debug.LogError("ButtonFade should be set in the inspector for scene change buttons");
    }

    override public void OnHit()
    {
        if (!transition)
            StartCoroutine(FadeCoroutine());
    }

    IEnumerator FadeCoroutine()
    {
        const float fade = .3f;
        transition = true;

        //Fade out
        float dt = 0f;
        while (dt < fade)
        {
            float l = dt / fade;
            l = 1 - l;
            Color c = ButtonFade.color;
            c.a = l;
            ButtonFade.color = c;
            yield return new WaitForEndOfFrame();
            dt += Time.deltaTime;
        }

        //Fade in
        dt = 0f;
        while (dt < fade)
        {
            float l = dt / fade;
            Color c = ButtonFade.color;
            c.a = l;
            ButtonFade.color = c;
            yield return new WaitForEndOfFrame();
            dt += Time.deltaTime;
        }

        //Fade out
        dt = 0f;
        while (dt < fade)
        {
            float l = dt / fade;
            l = 1 - l;
            Color c = ButtonFade.color;
            c.a = l;
            ButtonFade.color = c;
            yield return new WaitForEndOfFrame();
            dt += Time.deltaTime;
        }

        //Fade in
        dt = 0f;
        while (dt < fade)
        {
            float l = dt / fade;
            Color c = ButtonFade.color;
            c.a = l;
            ButtonFade.color = c;
            yield return new WaitForEndOfFrame();
            dt += Time.deltaTime;
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(transform.name);
    }
}

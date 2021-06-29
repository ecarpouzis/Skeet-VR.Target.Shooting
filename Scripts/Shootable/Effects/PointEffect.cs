using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PointEffect : MonoBehaviour
{
    public const string PrefabPath = "PointEffect";

    public Text PointDisplay;
    public Shootable MyShootable;
    public int PointValue;

    float timeSinceStart = 0f;
    float startFadeout = .5f;
    float finishFadeout = 1.5f;

    private float speed = 1.5f;

    public static PointEffect InstantiatePointEffect(Shootable myShootable, int pointValue)
    {
        GameObject effectGo = Instantiate(Resources.Load(PrefabPath)) as GameObject;
        effectGo.transform.position = myShootable.transform.position;
        if (myShootable.pointDisplayOverwrite != null)
        {
            effectGo.transform.position = myShootable.pointDisplayOverwrite.position;
            effectGo.transform.localScale = myShootable.pointDisplayOverwrite.localScale;
        }
        effectGo.transform.LookAt(Camera.main.transform.position);

        PointEffect effect = effectGo.GetComponent<PointEffect>();
        effect.MyShootable = myShootable;
        effect.PointValue = pointValue;
        if(pointValue>0)
            effect.PointDisplay.text = "+" + pointValue.ToString();
        else
            effect.PointDisplay.text = pointValue.ToString();


        return effect;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceStart += Time.deltaTime;
        float dy = Time.deltaTime * speed;
        transform.position = new Vector3(transform.position.x, transform.position.y + dy, transform.position.z);
        speed = Mathf.Lerp(speed, 0f, .15f);
        if(timeSinceStart> startFadeout)
        {
            float lerpProgress = (timeSinceStart - startFadeout) % (finishFadeout - startFadeout);
            Color newColor = new Color();
            newColor = PointDisplay.color;
            newColor.a = Mathf.Lerp(1f, 0f, lerpProgress);
            PointDisplay.color = newColor;
            if (timeSinceStart > finishFadeout)
            {
                Destroy(gameObject);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(MaskableGraphic))]
public class ScreenFader : MonoBehaviour
{
    public float SolidAlpha = 1f;
    public float ClearAlpha = 0;
    public float Delay = 0f;
    public float TimeToFade = 1f;
    MaskableGraphic _graphic;

    void Start()
    {
        _graphic = GetComponent<MaskableGraphic>();        
    }

    IEnumerator FadeRoutine(float alpha)
    {
        yield return new WaitForSeconds(Delay);
        _graphic.CrossFadeAlpha(alpha, TimeToFade, true);
    }

    public void FadeOn()
    {
        StartCoroutine(FadeRoutine(SolidAlpha));
    }

    public void FadeOff()
    {
        StartCoroutine(FadeRoutine(ClearAlpha));
    }
}

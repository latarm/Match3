using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text TimeLeftText;
    public Image ClockImage;
    public int FlashTimerLimit = 10;
    public AudioClip FlashBeep;
    public float FlashInterval = 1f;
    public Color FlashColor = Color.red;

    Coroutine _flashRoutine;

    int _maxTime = 60;
    public bool Paused = false;

    public void InitTimer(int maxTime=60)
    {
        _maxTime = maxTime;
        if(ClockImage!=null)
        {
            ClockImage.type = Image.Type.Filled;
            ClockImage.fillMethod = Image.FillMethod.Radial360;
            ClockImage.fillOrigin = (int)Image.Origin360.Top;
        }

        if(TimeLeftText!=null)
        {
            TimeLeftText.text = maxTime.ToString();
        }
    }

    public void UpdateTimer(int currentTime)
    {
        if(Paused)
        {
            return;
        }
        if(ClockImage!=null)
        {
            ClockImage.fillAmount = (float)currentTime / _maxTime;

            if(currentTime<=FlashTimerLimit)
            {
                _flashRoutine = StartCoroutine(FlashRoutine(ClockImage, FlashColor, FlashInterval));
                if(SoundManager.Instance !=null&& FlashBeep!=null)
                {
                    SoundManager.Instance.PlayClipAtPoint(FlashBeep, Vector3.zero, SoundManager.Instance.FxVolume, false);
                }
            }
        }
        if(TimeLeftText!=null)
        {
            TimeLeftText.text = currentTime.ToString();
        }
    }

    IEnumerator FlashRoutine(Image image, Color targetColor, float interval)
    {
        if (image !=null)
        {
            Color originalColor = image.color;
            image.CrossFadeColor(targetColor, interval * 0.3f, true, true);
            yield return new WaitForSeconds(interval * 0.5f);
            image.CrossFadeColor(originalColor, interval * 0.3f, true, true);
            yield return new WaitForSeconds(interval * 0.5f);
        }
    }

    public void FadeOff()
    {
        if (_flashRoutine != null)
        {
            StopCoroutine(_flashRoutine);
        }
        ScreenFader[] screenFaders = GetComponentsInChildren<ScreenFader>();
        foreach (ScreenFader screenFader in screenFaders)
        {
            screenFader.FadeOff();
        }
    }
}

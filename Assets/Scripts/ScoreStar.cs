using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreStar : MonoBehaviour
{
    public Image Star;
    public ParticlePlayer StarFX;
    public float Delay = 0.15f;
    public AudioClip StarSound;
    public bool Activated = false;

    private void Start()
    {
        SetActive(false);

    }

    void SetActive(bool state)
    {
        if (Star != null)
            Star.gameObject.SetActive(state);
    }

    public void Activate()
    {
        if (Activated)
            return;
        StartCoroutine(ActivateRoutine());
    }

    IEnumerator ActivateRoutine()
    {
        Activated = true;
        if (StarFX != null)
            StarFX.Play();
        if(SoundManager.Instance!=null&& StarSound!=null)
        {
            SoundManager.Instance.PlayClipAtPoint(StarSound, Vector3.zero, SoundManager.Instance.FxVolume);
        }
        yield return new WaitForSeconds(Delay);

        SetActive(true);
    }
}

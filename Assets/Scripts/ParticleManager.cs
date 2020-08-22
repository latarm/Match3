using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public GameObject ClearFXPrefab;
    public GameObject BreakFXPrefab;
    public GameObject DoubleBreakFXPRefab;
    public GameObject BombFXPrefab;

    public void ClearPieceFXAt(int x, int y, int z=0)
    {
        if(ClearFXPrefab!=null)
        {
            GameObject clearFX = Instantiate(ClearFXPrefab, new Vector3(x, y, z), Quaternion.identity);

            ParticlePlayer particlePlayer = clearFX.GetComponent<ParticlePlayer>();
            if (particlePlayer != null)
                particlePlayer.Play();
        }
    }

    public void BreakTileFXAt(int breakableValue, int x, int y, int z = 0)
    {
        GameObject breakFX = null;
        ParticlePlayer particlePlayer = null;

        if (breakableValue > 1)
        {
            if (DoubleBreakFXPRefab != null)
                breakFX = Instantiate(DoubleBreakFXPRefab, new Vector3(x, y, z), Quaternion.identity);
        }
        else
        {
            if (BreakFXPrefab != null)
                breakFX = Instantiate(BreakFXPrefab, new Vector3(x, y, z), Quaternion.identity);
        }
        if (breakFX != null)
        {
            particlePlayer = breakFX.GetComponent<ParticlePlayer>();
            if (particlePlayer != null)
                particlePlayer.Play();
        }
    }

    public void BombFXAt(int x, int y, int z=0)
    {
        if(BombFXPrefab!=null)
        {
            GameObject bombFX = Instantiate(BombFXPrefab, new Vector3(x, y, z), Quaternion.identity) as GameObject;
            ParticlePlayer particlePlayer = bombFX.GetComponent<ParticlePlayer>();
            if(particlePlayer !=null)
            {
                particlePlayer.Play();
            }
        }
    }
}

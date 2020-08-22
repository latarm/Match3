using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePlayer : MonoBehaviour
{
    public ParticleSystem[] AllParticles;
    public float Lifetime = 1f;
    public bool destroyImmediately = true;

    void Start()
    {
        AllParticles = GetComponentsInChildren<ParticleSystem>();
        if (destroyImmediately)
        {
            Destroy(gameObject, Lifetime);
        }
    }

    public void Play()
    {
        foreach (ParticleSystem particle in AllParticles)
        {
            particle.Stop();
            particle.Play();
        }
        Destroy(gameObject, Lifetime);
    }
}

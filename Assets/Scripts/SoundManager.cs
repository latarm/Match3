using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public AudioClip[] MusicClips;
    public AudioClip[] WinClips;
    public AudioClip[] LoseClips;
    public AudioClip[] BonusClips;

    [Range(0, 1)]
    public float MusicVolume = 0.5f;

    [Range(0, 1)]
    public float FxVolume = 1.0f;

    public float LowPitch = 0.95f;
    public float HighPitch = 1.05f;
    
    void Start()
    {
        PlayRandomMusic();
    }

    public AudioSource PlayClipAtPoint(AudioClip clip, Vector3 position, float volume=1f, bool randomizePitch=true)
    {
        if(clip!=null)
        {
            GameObject gObject = new GameObject("SoundFX" + clip.name);
            gObject.transform.position = position;

            AudioSource source = gObject.AddComponent<AudioSource>();
            source.clip = clip;

            if (randomizePitch)
            {
                float randomPitch = Random.Range(LowPitch, HighPitch);
                source.pitch = randomPitch;
            }

            source.volume = volume;

            source.Play();
            Destroy(gObject, clip.length);
            return source;
        }
        return null;
    }
    public AudioSource PlayRandom(AudioClip[] clips, Vector3 position, float volume = 1f)
    {
        if(clips!=null)
        {
            if(clips.Length!=null)
            {
                int randomIndex = Random.Range(0, clips.Length);
                if (clips[randomIndex] != null)
                {
                    AudioSource source = PlayClipAtPoint(clips[randomIndex], position, volume);
                    return source;
                }
            }
        }
        return null;
    }
    public void PlayRandomMusic()
    {
        PlayRandom(MusicClips, Vector3.zero, MusicVolume);
    }
    public void PlayWinSound()
    {
        PlayRandom(WinClips, Vector3.zero, FxVolume);
    }
    public void PlayLoseSound()
    {
        PlayRandom(LoseClips, Vector3.zero, FxVolume*0.35f);
    }
    public void PlayBonusSound()
    {
        PlayRandom(BonusClips, Vector3.zero, FxVolume);
    }
}

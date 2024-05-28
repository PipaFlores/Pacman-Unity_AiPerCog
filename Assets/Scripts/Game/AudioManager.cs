using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static AudioManager Instance { get; private set; }
    public AudioSource BackgroundSound;
    public AudioSource EatingSound;
    public AudioSource DeathSound;
    //public AudioSource WinSound;
    public AudioSource IntermissionSound;
    public AudioSource GhostEatenSound;
    

    void Awake()
    {
        // Singleton pattern
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PlayBackgroundSound()
    {
        BackgroundSound.mute = false;
        if (!BackgroundSound.isPlaying)
        {
            BackgroundSound.Play();
        }
    }

    public void PlayEatingSound()
    {
        if (!EatingSound.isPlaying)
        {
            EatingSound.Play();
        }
    }

    public void PlayDeathSound()
    {
        if (!DeathSound.isPlaying)
        {
            DeathSound.Play();
        }
    }

    public void PlayIntermissionSound(float duration)
    {
        IntermissionSound.Play();
        StartCoroutine(StopIntermissionSound(duration));
        BackgroundSound.mute = true;
        Invoke(nameof(PlayBackgroundSound), duration);
        
    }

    private IEnumerator StopIntermissionSound(float duration)
    {
        yield return new WaitForSeconds(duration);
        IntermissionSound.Stop();
    }

    public void PlayGhostEatenSound()
    {
        GhostEatenSound.Play();
        
    }


}

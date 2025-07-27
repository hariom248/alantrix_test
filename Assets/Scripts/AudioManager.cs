using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource sfxSource;
    
    [Header("Audio Clips")]
    public AudioClip cardFlipSound;
    public AudioClip cardMatchSound;
    public AudioClip cardMismatchSound;
    public AudioClip gameCompleteSound;
    public AudioClip buttonClickSound;
    
    [Header("Audio Settings")]
    [Range(0f, 1f)]
    public float sfxVolume = 1f;
    
    void Start()
    {
        sfxSource.volume = sfxVolume;
    }
    
    public void PlayCardFlipSound()
    {
        PlaySFX(cardFlipSound);
    }
    
    public void PlayCardMatchSound()
    {
        PlaySFX(cardMatchSound);
    }
    
    public void PlayCardMismatchSound()
    {
        PlaySFX(cardMismatchSound);
    }
    
    public void PlayGameCompleteSound()
    {
        PlaySFX(gameCompleteSound);
    }

    public void PlayButtonClickSound()
    {
        PlaySFX(buttonClickSound);
    }
    
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
} 
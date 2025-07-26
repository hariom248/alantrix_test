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
    
    [Header("Audio Settings")]
    [Range(0f, 1f)]
    public float sfxVolume = 1f;
    
    
    void Start()
    {
        InitializeAudio();
    }
    
    void InitializeAudio()
    {
        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFXSource");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
            sfxSource.volume = sfxVolume;
        }
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
    
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
    
    public void Mute()
    {
        if (sfxSource != null)
            sfxSource.mute = true;
    }
    
    public void Unmute()
    {
        if (sfxSource != null)
            sfxSource.mute = false;
    }
} 
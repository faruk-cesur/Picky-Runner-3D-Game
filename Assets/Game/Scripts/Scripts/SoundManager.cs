using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //[SerializeField] private AudioClip collectableSound;
    //[SerializeField] private AudioClip finishSound;
    //[SerializeField] private AudioClip gameOverSound;

    public static SoundManager Instance;
    [HideInInspector] public bool sound;
    private AudioSource _audioSource;


    private void Awake()
    {
        makeSingleton();
        _audioSource = GetComponent<AudioSource>();
    }

    private void makeSingleton()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public void SoundOnOff()
    {
        sound = !sound;
    }

    public void PlaySound(AudioClip clip, float volume)
    {
        if (sound)
        {
            _audioSource.PlayOneShot(clip, volume);
        }
    }
}
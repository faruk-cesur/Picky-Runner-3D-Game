using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioClip starBuffSound,
        collectableSound,
        deathSound,
        finishSound,
        jumpSound,
        slideSound,
        smashWallSound,
        pickItemSound,
        gameOverSound,
        beforeAttackSound,
        afterAttackSound;

    private AudioSource _audioSource;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        _audioSource = GetComponent<AudioSource>();
    }


    public void PlaySound(AudioClip clip, float volume)
    {
        _audioSource.PlayOneShot(clip, volume);
    }

    public IEnumerator GameOverSound()
    {
        PlaySound(Instance.deathSound, 1);
        yield return new WaitForSeconds(2f);
        PlaySound(Instance.gameOverSound, 1);
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class SoundFXManager : Singleton<SoundFXManager>
{
    [SerializeField] private AudioSource soundFXObject;
    [SerializeField] private float lowPitchRng;
    [SerializeField] private float highPitchRng;
    public bool useRandomGlobalPitch;

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume = 1f)
    {
        if (audioClip == null) return;

        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        if (useRandomGlobalPitch)
        {
            float rand = Random.Range(lowPitchRng, highPitchRng);
            audioSource.pitch = rand;
        }
        audioSource.clip = audioClip;
        audioSource.volume = volume;

        audioSource.Play();
        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }
}

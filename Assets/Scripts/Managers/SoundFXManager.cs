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

        //AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        AudioSource audioSource = ObjectPool.instance.GetPooledAudioSource();

        if (audioSource != null)
        {
            if (useRandomGlobalPitch)
            {
                float rand = Random.Range(lowPitchRng, highPitchRng);
                audioSource.pitch = rand;
            }

            audioSource.clip = audioClip;
            audioSource.volume = volume;
            audioSource.gameObject.SetActive(true);
            audioSource.Play();
            float clipLength = audioSource.clip.length;

            //Destroy(audioSource.gameObject, clipLength);
            StartCoroutine(SetAudioInactive(audioSource));
        }
    }

    IEnumerator SetAudioInactive(AudioSource audioSrc)
    {
        yield return new WaitUntil(() => !audioSrc.isPlaying);
        audioSrc.gameObject.SetActive(false);
    }
}

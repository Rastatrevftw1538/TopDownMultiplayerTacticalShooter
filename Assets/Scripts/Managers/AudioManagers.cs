using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManagers : MonoBehaviour
{
    [Header("Bullet Sounds")]
    public AudioClip ARSound;
    public AudioClip ARSound2;
    public AudioClip PistolSound;
    public AudioClip SMGSound;
    public AudioClip ShotgunSound;
    public AudioClip SniperSound;

    [Header("Reload Sounds")]
    public AudioClip ARSoundReload;
    public AudioClip PistolSoundReload;
    public AudioClip SMGSoundReload;
    public AudioClip ShotGunSoundReload;
    public AudioClip SniperSoundReload;

    private AudioSource audioSource;

    private void Awake()
    {
        EvtSystem.EventDispatcher.AddListener<ShootSound>(PlayShootSound);
        EvtSystem.EventDispatcher.AddListener<ReloadSound>(PlayReloadSound);
        audioSource = this.GetComponent<AudioSource>();
    }

    private void PlayShootSound(ShootSound evtData)
    {
        audioSource.transform.position = evtData.position;
        //int rand = Random.Range(0, 2);
        //Debug.LogError("random: " + rand);
        switch (evtData.GunName)
        {
            case "AR":
                //AudioSource.PlayClipAtPoint(ARSound, evtData.position);
                //if (rand == 0)
                    audioSource.PlayOneShot(ARSound);
                //else
                //    audioSource.PlayOneShot(ARSound2);
                break;
            case "Pistol":
                //AudioSource.PlayClipAtPoint(PistolSound, evtData.position);
                audioSource.PlayOneShot(PistolSound);
                break;
            case "SMG":
                //AudioSource.PlayClipAtPoint(SMGSound, evtData.position);
                audioSource.PlayOneShot(SMGSound);
                break;
            case "Shotgun":
                //AudioSource.PlayClipAtPoint(ShotgunSound, evtData.position, 0.3f);
                audioSource.PlayOneShot(ShotgunSound, 0.3f);
                break;
            case "Sniper":
                //AudioSource.PlayClipAtPoint(SniperSound, evtData.position);
                audioSource.PlayOneShot(SniperSound);
                break;
        }
    }

    private void PlayReloadSound(ReloadSound evtData)
    {
        switch (evtData.GunName)
        {
            case "AR":
                AudioSource.PlayClipAtPoint(ARSoundReload, evtData.position);
                break;
            case "Pistol":
                AudioSource.PlayClipAtPoint(PistolSoundReload, evtData.position);
                break;
            case "SMG":
                AudioSource.PlayClipAtPoint(SMGSoundReload, evtData.position);
                break;
            case "Shotgun":
                AudioSource.PlayClipAtPoint(ShotGunSoundReload, evtData.position);
                break;
            case "Sniper":
                AudioSource.PlayClipAtPoint(SniperSoundReload, evtData.position);
                break;
        }
    }
}

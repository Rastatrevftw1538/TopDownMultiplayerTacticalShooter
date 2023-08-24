using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagers : MonoBehaviour
{
    [Header("Bullet Sounds")]
    public AudioClip ARSound;
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
        switch (evtData.GunName)
        {
            case "AR":
                AudioSource.PlayClipAtPoint(ARSound, evtData.position);
                break;
            case "Pistol":
                AudioSource.PlayClipAtPoint(PistolSound, evtData.position);
                break;
            case "SMG":
                AudioSource.PlayClipAtPoint(SMGSound, evtData.position);
                break;
            case "Shotgun":
                AudioSource.PlayClipAtPoint(ShotgunSound, evtData.position, 0.3f);
                break;
            case "Sniper":
                AudioSource.PlayClipAtPoint(SniperSound, evtData.position);
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

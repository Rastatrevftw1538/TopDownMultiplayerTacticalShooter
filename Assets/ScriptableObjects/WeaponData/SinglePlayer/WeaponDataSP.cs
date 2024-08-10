using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData SinglePlayer", menuName = "Weapon Data SinglePlayer")]
public class WeaponDataSP : ScriptableObject
{
    //public float[] shootSound;
    //public float[] reloadSound;
    public AudioClip shootSound;
    public AudioClip shootOnBeatSound;
    public AudioClip reloadSound;
    public int damagePerBullet;
    public float fireRate;
    public bool semiAuto;
    public float fireRange;
    public int numOfBulletsPerShot = 1;
    public Sprite weaponSprite;
    public int ammo;
    public float reloadTime;
    public float spreadIncreasePerSecond;
}

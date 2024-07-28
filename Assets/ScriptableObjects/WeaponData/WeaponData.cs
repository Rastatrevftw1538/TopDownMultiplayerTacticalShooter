using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Weapon Data")]
public class WeaponData : ScriptableObject
{
    public float[] shootSound;
    public float[] reloadSound;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIScript : MonoBehaviour
{
    //FOR NOW, JUST COMMENT OUT THE LINE THAT YOU'RE NOT GOING TO BE USING BASED ON SINGLEPLAYER/MULTIPLAYER
    //WeaponSinglePlayer gun;
    //Weapon gun;
    [SerializeField]
    //TMP_Text ammoUI;
    // Start is called before the first frame update
    void Start()
    {
        //gun = this.GetComponent<WeaponSinglePlayer>();
        //ammoUI.text = gun.getCurrentAmmo().ToString() + " / "+ gun.getTotalMags().ToString();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (gun.isGunReloading())
        {
            ammoUI.text = "Reloading";
        }
        else if (gun.isOutOfAmmo())
        {
            ammoUI.text = "Out of Ammo";
        }
        else
        {
            ammoUI.text = gun.getCurrentAmmo().ToString() + " / " + gun.getTotalMags().ToString();
        }*/
    }
}

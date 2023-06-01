using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;

public class WeaponPickUp : NetworkBehaviour
{
    [SerializeField] private WeaponData[] weapons;
    [SyncVar(hook = nameof(OnButtonPressedChanged))]
    private bool isButtonPressed = false;
    [SyncVar(hook = nameof(OnWeaponIndexChanged))]
    private int currentWeaponIndex = 0;

    private GameObject button;

    private bool isHidden = false;
    private float hideTimer = 30f;
    private float currentTimer = 0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isHidden && !button.activeSelf)
        {
            if (isOwned)
            {
                RpcShowButton();
            }
        }

        if (isButtonPressed && isOwned)
        {
            Weapon weapon = other.GetComponent<Weapon>();
            if (weapon != null)
            {
                currentWeaponIndex = Random.Range(0, weapons.Length);
                CmdSetWeaponSpecs(other.gameObject, weapons[currentWeaponIndex]);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (button.activeSelf)
        {
            if (isOwned)
            {
                RpcHideButton();
            }
        }
    }

    private void Awake()
    {
        button = transform.GetChild(0).GetChild(0).gameObject;
    }

    public void Pressed()
    {
        if (isOwned)
        {
            // Set the flag to true when the button is pressed
            isButtonPressed = true;
        }
        else
        {
            // Send a command to the server to set the flag to true
            CmdSetButtonPressed(true);
        }
    }

    [Command]
    private void CmdSetButtonPressed(bool pressed)
    {
        // Update the button pressed state on the server
        isButtonPressed = pressed;
    }

    [Command]
    private void CmdSetWeaponSpecs(GameObject weaponObject, WeaponData weaponSpecs)
    {
        // Set the weaponSpecs on the weapon object on the server
        Weapon weapon = weaponObject.GetComponent<Weapon>();
        if (weapon != null)
        {
            weapon.weaponSpecs = weaponSpecs;
        }
    }

    private void OnButtonPressedChanged(bool oldValue, bool newValue)
    {
        // This method is called on all clients when the button pressed state changes on the server
        if (newValue)
        {
            ResetWeapon();
            // Perform desired action while the button is pressed
            Debug.Log("Button Pressed!");
        }
    }

    private void OnWeaponIndexChanged(int oldIndex, int newIndex)
    {
        // This method is called on all clients when the current weapon index changes on the server
        if (!isOwned)
        {
            // Retrieve the player's weapon and update it with the new weapon
            Weapon weapon = GetComponent<Weapon>();
            if (weapon != null)
            {
                weapon.weaponSpecs = weapons[newIndex];
            }
        }
    }

    private void Update()
    {
        if (isServer && isHidden)
        {
            currentTimer += Time.deltaTime;
            if (currentTimer >= hideTimer)
            {
                currentTimer = 0f;
                isHidden = false;
                gameObject.SetActive(true);
            }
        }
    }

    public void ResetWeapon()
    {
        if (isOwned)
        {
            currentWeaponIndex = 0;
            gameObject.SetActive(false);
            isHidden = true;
        }
    }

    [ClientRpc]
    private void RpcShowButton()
    {
        button.SetActive(true);
    }

    [ClientRpc]
    private void RpcHideButton()
    {
        button.SetActive(false);
    }
}

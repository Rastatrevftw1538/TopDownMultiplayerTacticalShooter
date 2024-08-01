using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Sentient : MonoBehaviour, IInteractable
{
    private void Update()
    {
        if//(eventsystemTrigger == true)
        {
            Interact();  
        }
    }

    public void Interact()
    {

    }
}

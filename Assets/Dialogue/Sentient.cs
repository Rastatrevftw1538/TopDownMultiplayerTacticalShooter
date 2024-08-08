using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Sentient : MonoBehaviour, IInteractable
{
    //vars for finding the distance between the NPC and the player
    [SerializeField] private SpriteRenderer _interactSprite;
    [SerializeField] private Transform _playerTransform;
    private const float INTERACT_DISTANCE = 5.0f;

    //vars for finding what wave the games on
    public WaveManager waveCheck;
    [SerializeField] public int waveToTalk;

    private void start()
    {

        //finding the player
        //_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        //finding what wave the games on
        waveCheck = FindObjectOfType<WaveManager>();
        //need a way to read the current wave
    }

    private void Update()
    {
        if(Keyboard.current.lKey.wasPressedThisFrame && IsWithinInteractDistance())
        {
            Interact();
        }

        if(_interactSprite.gameObject.activeSelf && !IsWithinInteractDistance())
        {
            //turn off the sprite if not within distance
            _interactSprite.gameObject.SetActive(false);        

        }

        else if (!_interactSprite.gameObject.activeSelf && IsWithinInteractDistance())
        {
            //if the sprite isn't on but we're in interact distance, turn on the sprite
            _interactSprite.gameObject.SetActive(true);
        }

       //need a way to update the current wave


    }

    public abstract void Interact();

    private bool IsWithinInteractDistance()
    {
        if (Vector2.Distance(_playerTransform.position, transform.position) < INTERACT_DISTANCE)
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    /*public bool IsTalkingTime()
    {
        if (whatWave == waveToTalk)
        {
            return true;
        }

        else
        {
            return false;
        }
    }*/


}




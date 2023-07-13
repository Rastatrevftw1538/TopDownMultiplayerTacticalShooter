using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImageSprite : MonoBehaviour
{
    [SerializeField]
    private float activeTime = 0.1f;
    private float timeActivated;

    [SerializeField]
    private float alphaSet = 0.8f;
    private float alphaMultiplier = 0.85f;
    private float alpha;
    
    private float lastImageXpos;
    private float distanceBetweenImages = 0.1f;

    private Transform player;

    private SpriteRenderer SR;
    private SpriteRenderer playerSR;

    private Color color;

    private void OnEnable()
    {
        SR = GetComponent<SpriteRenderer>();

        if(GameObject.FindGameObjectWithTag("PlayerObject") != null){
            player = GameObject.FindGameObjectWithTag("PlayerObject").transform;
            playerSR = player.GetComponent<SpriteRenderer>();
            
            alpha = alphaSet;
            SR.sprite = playerSR.sprite;
            transform.position = player.position;
            transform.rotation = player.rotation;
        }

        timeActivated = Time.time;
    }

    private void FixedUpdate()
    {
        alpha *= alphaMultiplier;
        color = new Color(1f, 1f, 1f, alpha);
        SR.color = color;

        if (activeTime >= 0)
        {
            activeTime -= Time.deltaTime;
            PlayerAfterImagePool.Instance.AddToPool(gameObject);

            if (Mathf.Abs(player.position.x - lastImageXpos) > distanceBetweenImages)
            {
                PlayerAfterImagePool.Instance.GetFromPool();
                lastImageXpos = player.position.x;
            }
        }
    }

}


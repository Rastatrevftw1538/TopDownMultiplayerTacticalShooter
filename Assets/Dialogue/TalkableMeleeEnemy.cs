using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkableMeleeEnemy : Sentient, ITalkable
{
    //variable to read in the text its supposed to say
    [SerializeField] private DialogueText dialogueText;
    [SerializeField] private DialogueController dialogueController;
    private CircleCollider2D interactCircle;
    public float talkDistance;
    public bool canInteract;
    [field:SerializeField] public KeyCode Key { get; set; }
    [field:SerializeField] public Sprite CharacterTalkSprite { get; set; }

    void Start()
    {
        TryCircle();
    }

    void TryCircle()
    {
        TryGetComponent<CircleCollider2D>(out interactCircle);
        if (!interactCircle) interactCircle = gameObject.AddComponent<CircleCollider2D>();
        interactCircle.enabled = true;
        interactCircle.isTrigger = true;

        if (talkDistance <= 0)
            talkDistance = 3f;
        interactCircle.radius = talkDistance;
    }

    [HideInInspector] public bool StartedConvo { get; set; }
    bool isConvoKeyPressed = false;
    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")){
            if (isConvoKeyPressed && !StartedConvo)
            {
                Interact();
            }
        }
    }

    Vector2 press;
    private void Update()
    {
        press = new UnityEngine.Vector2
        {
            x = Input.GetAxisRaw("Talk"),
        };
        if (press.magnitude > 0)
        {
            isConvoKeyPressed = true;
        }
        else
        {
            isConvoKeyPressed = false;
        }
    }

    private void FixedUpdate()
    {

    }

    public override void Interact()
    {
        StartedConvo = true;
        //Debug.LogError("interacting...");
        Talk(dialogueText);
    }

    
    public void Talk(DialogueText dialogueText)
    {
        //start conversation
        dialogueController.DisplayNextParagraph(dialogueText, this, CharacterTalkSprite);
    }
}


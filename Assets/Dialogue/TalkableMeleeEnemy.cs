using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkableMeleeEnemy : Sentient, ITalkable
{
    //variable to read in the text its supposed to say
    [SerializeField] private DialogueText dialogueText;

    public override void Interact()
    {
        Talk(dialogueText);
    }


    public void Talk(DialogueText dialogueText)
    {
        //start conversation
    }
}


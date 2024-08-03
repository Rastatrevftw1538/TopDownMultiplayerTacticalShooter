using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkableMeleeEnemy : Sentient, ITalkable
{
    //variable to read in the text its supposed to say
    [SerializeField] private dialogueText DialogueText;

    public override void Interact()
    {
        Talk(DialogueText);
    }


    public void Talk(dialogueText DialogueText)
    {
        //start conversation
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkableMeleeEnemy : Sentient, ITalkable
{

    [SerializeField] private dialogueText DialogueText;

    public void Talk(dialogueText DialogueText)
    {
        
    }
}

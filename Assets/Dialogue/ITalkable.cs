using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITalkable
{
    public void Talk(DialogueText dialogueText);
    public KeyCode Key {  get; set; }
    public bool StartedConvo {  get; set; }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/New Dialouge - Container")]

public class dialogueText : ScriptableObject
{
    public string speakerName;
    public string speakerType;

    [TextArea (3,5)]
    public string[] paragraphs;
} 

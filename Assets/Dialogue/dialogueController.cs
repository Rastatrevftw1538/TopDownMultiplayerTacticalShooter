using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class dialogueController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI NPCNameText;
    [SerializeField] private TextMeshProUGUI NPCDialogueText;

    private Queue<string> paragraphs = new Queue<string> (); 

    
}

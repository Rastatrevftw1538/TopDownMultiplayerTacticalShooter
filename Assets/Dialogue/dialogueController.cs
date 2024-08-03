using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI NPCNameText;
    [SerializeField] private TextMeshProUGUI NPCDialogueText;

    private Queue<string> paragraphs = new Queue<string> ();

    private bool conversationEnded;

    public void DisplayNextParagraph(DialogueText dialogueText)
    {
        //if there's nothing in the paragraph queue
        if(paragraphs.Count == 0)
        {
            if (!conversationEnded)
            {
                //start a conversation
                StartConversation(dialogueText);
            }

            else
            {
                //end a conversation
                EndConversation(dialogueText);
                return;

            }

        }

        //if there's something in the paragraph queue
    }

    private void StartConversation(DialogueText dialogueText)
    {
        //activate a DialogueText game object
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        //update the speaker name
        NPCNameText.text = dialogueText.speakerName;

        //add dialogue text to paragraph queue
        for (int i = 0; i < dialogueText.paragraphs.Length; i++)
        {
            paragraphs.Enqueue(dialogueText.paragraphs[i]);
        }
    }

    private void EndConversation(DialogueText dialogueText)
    {
        //clear the queue
        paragraphs.Clear();

        //return bool to false
        conversationEnded = false;

        //deactivate game object
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
    
}

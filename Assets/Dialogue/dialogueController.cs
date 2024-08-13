using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.Rendering;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI NPCNameText;
    [SerializeField] private TextMeshProUGUI NPCDialogueText;
    [SerializeField] private GameObject dialogueHolder;

    private Queue<string> paragraphs = new Queue<string> ();

    private bool conversationEnded;

    public void DisplayNextParagraph(DialogueText dialogueText, ITalkable NPC)
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
        //update conversation text
        NPCDialogueText.text = paragraphs.Dequeue();
        if(paragraphs.Count == 0)
        {
            conversationEnded = true;
        }
        lastNPC = NPC;
    }

    ITalkable lastNPC;
    DialogueText latestConvo;
    private Queue<string> latestParagraphs = new Queue<string>();
    static int idx;
    public void DisplayNextParagraph()
    {
        /*NPCDialogueText.text = paragraphs.Dequeue();
        if (paragraphs.Count == 0)
        {
            conversationEnded = true;
            EndConversation();
        }*/

        while(idx <= latestParagraphs.Count)
        {
            NPCDialogueText.text = latestConvo.paragraphs[idx];
            idx++;
            return;
        }

        conversationEnded = true;
        StartCoroutine(nameof(EndConversation));
    }

    private void StartConversation(DialogueText dialogueText)
    {
        //activate a DialogueText game object
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        if (!dialogueHolder.activeSelf)
        {
            dialogueHolder.SetActive(true);
        }

        //GET CONVO DETAILS
        latestConvo = dialogueText;

        //update the speaker name
        NPCNameText.text = dialogueText.speakerName;

        //filling the dialogue tree with dialogue
        for (int i = 0; i < dialogueText.paragraphs.Length; i++)
        {
            paragraphs.Enqueue(dialogueText.paragraphs[i]);
        }
        latestParagraphs = paragraphs;
        //Debug.LogError("conversation has " + paragraphs.Count + " dialogues");
    }

    IEnumerator EndConversation()
    {
        //clear the queue
        paragraphs.Clear();

        //set the bool to true
        conversationEnded = false;

        //buffer so that the player cant get stuck in a dialogue loop
        dialogueHolder.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        idx = 0;
        if (lastNPC != null) lastNPC.StartedConvo = false;

        //deactivate game object
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }

    }

    private void EndConversation(DialogueText dialogueText)
    {
        //clear the queue
        paragraphs.Clear();

        //set the bool to true
        conversationEnded = false;
        dialogueHolder.SetActive(false);
        idx = 0;
        if (lastNPC != null) lastNPC.StartedConvo = false;

        //deactivate game object
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
    
}

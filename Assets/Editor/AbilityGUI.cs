using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(Ability))]
public class AbilityGUI : Editor
{
    public new string name;

    public override VisualElement CreateInspectorGUI()
    {
        var editorAsset = AssetDatabase.
            LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/AbilityGUI.uxml");

        var root = editorAsset.CloneTree();

        //targetLine = target as DialogueLineData;

        root.style.flexGrow = 1.0f;

        var nameLabel = root.Query<Label>("NameLabel").First();
        //nameLabel.text = targetLine.name;

        var groupBox = root.Query<GroupBox>("Options").First();

        //CREATING OBJECT FIELD FOR AUDIO CLIPS
        var audioClipDialogue = new ObjectField();
        audioClipDialogue.objectType = typeof(AudioClip); //CREATE NEW OBJECT FIELD THAT TAKES IN 'AudioClip'
        audioClipDialogue.bindingPath = "dialogueAudio"; //BIND IT TO DIALOGUE AUDIO
        audioClipDialogue.Bind(serializedObject); //BIND OBJECT
        audioClipDialogue.label = "Dialogue Clip"; // LABELS THE BOX
        audioClipDialogue.style.paddingTop = 10f;

        var audioClipStartDialogue = new ObjectField();
        audioClipStartDialogue.objectType = typeof(AudioClip);
        audioClipStartDialogue.bindingPath = "startAudioSound";
        audioClipStartDialogue.Bind(serializedObject);
        audioClipStartDialogue.label = "Dialogue Clip Start Sound";

        //ADDS THE AUDIO CLIP TO THE GROUP BOX, SO IT SHOWS UP IN THE UI
        groupBox.Add(audioClipDialogue); // <-- ACTUALLY ADDS TO THE UI
        groupBox.Add(audioClipStartDialogue); // <-- ACTUALLY ADDS TO THE UI

        //ADDS THE RESPONSES TO THE GROUP BOX, SO IT SHOWS UP IN THE UI
        SerializedProperty responseArray = serializedObject.FindProperty("responses"); //LIST VIEW FOR THE DIALOGUE RESPONSES
        var responseField = new PropertyField(responseArray); //CREATES A NEW PROPERTY FIELD FOR THE DIALOGUE RESPONSES
        responseField.style.paddingTop = 10f;
        responseField.style.width = 500f;
        responseField.style.overflow.Equals(true);
        root.Add(responseField); // <-- ACTUALLY ADDS TO THE UI, OUTSIDE OF THE 'DetailBox'

        return root;
    }
}

using UnityEngine;
using System.IO;
using Sanford.Multimedia.Midi;

public class MIDIManager : MonoBehaviour
{
    public string filePath;

    string filePathToRead;
    public GameObject RedButP1;
    public GameObject BlueButP1;
    public GameObject GreenButP1;
    public GameObject YellowButP1;

    [SerializeField]
    private GameObject NoteManager;

    [SerializeField]
    private GameObject RedNote;

    [SerializeField]
    private GameObject BlueNote;

    [SerializeField]
    private GameObject GreenNote;

    [SerializeField]
    private GameObject YellowNote;

    void Start()
    {
        // Call EnumerateFiles in a foreach-loop.
        SearchForMidiData();
        LoadMidiData(filePathToRead);
    }
    public void SearchForMidiData()
    {
        filePathToRead = Application.streamingAssetsPath + "/" + filePath;
        //Debug.Log(filePathToRead);
    }
    public void LoadMidiData(string filePath)
    {
        FileStream midiStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        LoadMidiData(midiStream);
        //Debug.Log("hi");
    }

    public void LoadMidiData(Stream midiStream)
    {
        ReadMidiFile(midiStream);
    }

    void ReadMidiFile(Stream midiStream)
    {
        Sequence midiSequence = new Sequence(midiStream);
        float length = midiSequence.GetLength() / midiSequence.Division;
        // Debug.Log(length);
        //Debug.Log(midiSequence.Division);
        // Debug.Log(midiSequence.GetLength());

        foreach (Track midiTrack in midiSequence)
            ReadMidiTrack(midiTrack, midiSequence.Division);
    }

    void ReadMidiTrack(Track midiTrack, int sequencerDivision)
    {
        for (int i = 0; i < midiTrack.Count; ++i)
        {
            //Debug.Log(i);
            //Debug.Log(midiTrack.Count);
            MidiEvent midiEvent = midiTrack.GetMidiEvent(i);
            if (midiEvent.MidiMessage.GetBytes().Length < 3)
                continue;

            byte midiType = (byte)(midiEvent.MidiMessage.GetBytes()[0] & 0xFF);
            byte note = (byte)(midiEvent.MidiMessage.GetBytes()[1] & 0xFF);
            byte velocity = (byte)(midiEvent.MidiMessage.GetBytes()[2] & 0xFF);
            float time = midiEvent.AbsoluteTicks / (float)sequencerDivision;

            if (midiType == (byte)ChannelCommand.NoteOff || (midiType == (byte)ChannelCommand.NoteOn) && velocity == 0)
            {
                //Debug.Log("No Note Played "+time);
            }
            else if (midiType == (byte)ChannelCommand.NoteOn)
            {
                //Debug.Log("notePlayed");
                //var rnd = Random.Range(1, 5);
                Debug.Log(note);
                if (note == 58)
                {
                    //Debug.Log(note+" "+time+" "+velocity);
                    Instantiate(RedNote, new Vector3(RedButP1.transform.position.x, time + 2f, 0f), RedNote.transform.rotation, NoteManager.transform);
                }
                else if (note == 55)
                {
                    //Debug.Log(note+" "+time+" "+velocity);
                    Instantiate(GreenNote, new Vector3(GreenButP1.transform.position.x, time + 2f, 0f), GreenNote.transform.rotation, NoteManager.transform);
                }
                else if (note == 56)
                {
                    //Debug.Log(note+" "+time+" "+velocity);
                    Instantiate(BlueNote, new Vector3(BlueButP1.transform.position.x, time + 2f, 0f), BlueNote.transform.rotation, NoteManager.transform);
                }
                else if (note == 59)
                {
                    //Debug.Log(note+" "+time+" "+velocity);
                    Instantiate(YellowNote, new Vector3(YellowButP1.transform.position.x, time + 2f, 0f), YellowNote.transform.rotation, NoteManager.transform);
                }
                if (note == 69)
                {
                    //Debug.Log(note+" "+time+" "+velocity);
                    Instantiate(RedNote, new Vector3(RedButP1.transform.position.x, time + 2f, 0f), RedNote.transform.rotation, NoteManager.transform);
                }
                else if (note == 70)
                {
                    //Debug.Log(note+" "+time+" "+velocity);
                    Instantiate(GreenNote, new Vector3(GreenButP1.transform.position.x, time + 2f, 0f), GreenNote.transform.rotation, NoteManager.transform);
                }
                else if (note == 71)
                {
                    //Debug.Log(note+" "+time+" "+velocity);
                    Instantiate(BlueNote, new Vector3(BlueButP1.transform.position.x, time + 2f, 0f), BlueNote.transform.rotation, NoteManager.transform);
                }
                else if (note == 72)
                {
                    //Debug.Log(note+" "+time+" "+velocity);
                    Instantiate(YellowNote, new Vector3(YellowButP1.transform.position.x, time + 2f, 0f), YellowNote.transform.rotation, NoteManager.transform);
                }
                else
                {
                    //Debug.Log("MissedNote");
                }
            }
        }
    }
    void Wait()
    {
        //Debug.Log("wait");
    }
}
// MidiTest script and how to make them spawn on beat and not all at once
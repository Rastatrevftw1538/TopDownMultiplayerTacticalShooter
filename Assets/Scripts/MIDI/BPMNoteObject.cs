using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport.Error;
using UnityEngine;

public class BPMNoteObject : MonoBehaviour
{
    bool canBePressed;
    public KeyCode keyToPress;
    BPMManager bpmManager;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        if (!hasChecked)
        {
            normNoteDist = BPMManager.instance.normErrorWindow;
            goodNoteDist = BPMManager.instance.goodErrorWindow;
            perfectNoteDist = BPMManager.instance.perfectErrorWindow;
            hasStarted = BPMManager.instance.startPlaying;
            beatTempo = BPMManager.instance.BPM;
            hitLocation = GameObject.FindGameObjectWithTag("Activator").transform;
            hasChecked = true;
        }

        TryGetComponent<Rigidbody2D>(out rb);
    }
    static float beatTempo;
    static bool hasStarted;
    static bool hasChecked;
    static float normNoteDist;
    static float goodNoteDist;
    static float perfectNoteDist;
    static Transform hitLocation;

    const float why = 15f;
    // Update is called once per frame

    Vector2 beatTempoDir;

    void Update()
    {
        if (Input.GetKeyDown(keyToPress))
        {
            if (canBePressed)
            {
                gameObject.SetActive(false);

                //BPMManager.Instance.NoteHit();

                //check dist between the note and the hit target in abs value
                float dist = Mathf.Abs(Vector2.Distance(transform.position, hitLocation.position) - why);
                Debug.LogError("Distance between that hit was " + dist);
                if (dist > normNoteDist)
                {
                    Debug.LogError("Norm");
                    BPMManager.instance.NormalHit();
                }
                else if (dist > goodNoteDist)
                {
                    Debug.LogError("Good");
                    BPMManager.instance.GoodHit();
                }
                else if(dist > perfectNoteDist)
                {
                    Debug.LogError("Perfect");
                    BPMManager.instance.PerfectHit(); 
                }

                Destroy(this.gameObject, 0.1f);
            }
        }

        if (hasStarted) //rb.MovePosition(new Vector2(rb.position.x * Time.deltaTime * beatTempo, 0f));
            transform.position += new Vector3(beatTempo/50 * Time.deltaTime, 0f, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Activator"))
        {
            canBePressed = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!gameObject.activeSelf) return;

        if (collision.CompareTag("Activator"))
        {
            canBePressed = false;

            BPMManager.instance.NoteMissed(this.gameObject);
        }
    }
}

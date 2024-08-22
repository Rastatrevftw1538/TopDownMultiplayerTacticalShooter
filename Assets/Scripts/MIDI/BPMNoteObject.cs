using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport.Error;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class BPMNoteObject : MonoBehaviour
{
    bool canBePressed;
    public KeyCode keyToPress;
    private Rigidbody2D rb;
    public PlayerInputActions playerInputAction;
    private InputAction inputAction;
    static PauseMenu pauseCheck;
    // Start is called before the first frame update

    private void Awake()
    {
        playerInputAction = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputAction = playerInputAction.Player.Fire;
        inputAction.Enable();
        inputAction.performed += Click;
    }

    private void OnDisable()
    {
        inputAction.Disable();
    }

    private void Click(InputAction.CallbackContext context)
    {
        didClick = true;
    }

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

            pauseCheck = PauseMenu.instance;
        }

        TryGetComponent<Rigidbody2D>(out rb);
    }
    static float beatTempo = 0f;
    static bool hasStarted;
    static bool hasChecked;
    static float normNoteDist;
    static float goodNoteDist;
    static float perfectNoteDist;
    static Transform hitLocation;

    bool didClick;
    //const float why = 30f;
    // Update is called once per frame

    Vector2 beatTempoDir;

    void Update()
    {
        if(!hasStarted) hasStarted = BPMManager.instance.startPlaying;
        if (beatTempo == 0f) beatTempo = BPMManager.instance.BPM;
        if (didClick && !pauseCheck._isPaused)
        {
            didClick = false;
            if (canBePressed)
            {
                //BPMManager.instance.canClick = Color.green;
                gameObject.SetActive(false);

                //BPMManager.Instance.NoteHit();

                //check dist between the note and the hit target in abs value
                float dist = Mathf.Abs(Vector2.Distance(transform.position, hitLocation.position));
                //Debug.LogError("Distance between that hit was " + dist);
                if (dist > normNoteDist)
                {
                    //Debug.LogError("Norm");
                    BPMManager.instance.NormalHit();
                }
                else if (dist > goodNoteDist)
                {
                    //Debug.LogError("Good");
                    BPMManager.instance.GoodHit();
                }
                else if (dist > perfectNoteDist)
                {
                    // Debug.LogError("Perfect");
                    BPMManager.instance.PerfectHit();
                }

                Destroy(this.gameObject, 0.1f);
            }
        }
    }

    Vector2 movement;
    private void FixedUpdate()
    {
        movement = new Vector2(0, 1).normalized;
        Vector3 moveVector = beatTempo * Time.fixedDeltaTime * Time.timeScale * transform.TransformDirection(movement);
        if (hasStarted) rb.velocity = new Vector2(moveVector.x, -moveVector.y) * 2;
            //transform.position = Vector3.Lerp(transform.position, hitLocation.position, Time.fixedDeltaTime * Time.timeScale);
            //transform.position -= new Vector3(0f, beatTempo * Time.fixedDeltaTime * Time.timeScale, 0f);

        //so that the beat will actually continue to move past the hit location, and not get stuck directly on top of it
        /*if (Mathf.Abs(Vector2.Distance(transform.position, hitLocation.position)) <= 0.3f)
        {
            Vector2 past = new Vector2(hitLocation.position.x, hitLocation.position.y - 5);
            transform.position = Vector3.MoveTowards(transform.position, past, beatTempo * Time.fixedDeltaTime * Time.timeScale);
        }*/
        //transform.position += new Vector3(beatTempo * Time.deltaTime, 0f, 0f);
        //transform.position -= new Vector3(0f, beatTempo * Time.deltaTime, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Activator"))
        {
            canBePressed = true;
            BPMManager.instance.canClick = Color.green;
        }

        if (collision.CompareTag("Destroy Note"))
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!gameObject.activeSelf) return;

        if (collision.CompareTag("Activator"))
        {
            //BPMManager.instance.canClick = Color.red;
            /*if (!BPMManager.instance)
            {
                bpmManager = FindObjectOfType<BPMManager>();

                canBePressed = false;
                bpmManager.canClick = Color.red;
                bpmManager.NoteMissed(this.gameObject);
            }
            else
            {
                canBePressed = false;
                BPMManager.instance.canClick = Color.red;
                BPMManager.instance.NoteMissed(this.gameObject);
            }*/
            canBePressed = false;
            BPMManager.instance.NoteMissed(this.gameObject);
            Destroy(this.gameObject, 0.1f);
        }

        if(collision.CompareTag("Destroy Note"))
        {
            Destroy(this.gameObject);
        }
    }
}

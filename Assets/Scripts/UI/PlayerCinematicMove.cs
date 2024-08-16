using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerCinematicMove : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform target;
    public bool spawnObjectOnceDone;
    public GameObject spawnObject;
    public Transform spawnObjectTransform;
    CinemachineVirtualCamera cinemachineCam;
    PlayerScriptSinglePlayer player;
    Animator playerAnim;
    NavMeshAgent playerNavMesh;
    Transform playerTransform;
    bool reached;

    void Start()
    {
        reached = false;
        cinemachineCam = GameObject.FindGameObjectWithTag("Cinemachine Camera").GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    float debugTimer = 3f; // if the player doesnt make it to the destination in 3 seconds, just teleport them there
    float timer = 0f;
    bool startedCo = false;
    void Update()
    {
        if (!player || !playerTransform || !startedCo) return;
        //if(playerNavMesh.isPathStale || playerNavMesh.pathStatus == NavMeshPathStatus.PathComplete) reached = true;

        timer += Time.deltaTime;
        if (timer > debugTimer && !reached)
        {
            playerTransform.position = target.position;
        }

        if (Mathf.Abs(Vector3.Distance(playerTransform.position, target.position)) <= 5)
        {
            reached = true;
        }
        else
        {
            //Debug.LogError($"distance is {Mathf.Abs(Vector2.Distance(playerTransform.position, target.position))}");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent<PlayerScriptSinglePlayer>(out player))
            {
               // player.TryGetComponent<NavMeshAgent>(out playerNavMesh);
                playerTransform = player.gameObject.transform;
                playerAnim = player.Anim;
                StartCoroutine(nameof(PlayerToTarget));
            }
        }
    }

    //yes ideally i use a navmesh but it wasnt working so
    IEnumerator PlayerToTarget()
    {
        startedCo = true;
        player.setCanMove(false); // set player binds off
        //playerNavMesh.SetDestination(target.position);
        //float dist = (Vector2.Distance(playerTransform.position, target.position)) + 4;
        playerTransform.position = Vector3.Lerp(playerTransform.position, target.position, .03f); //Move the player towards the target

        //set player collider to trigger so they cant get stuck on walls
        BoxCollider2D boxCollider;
        if(player.gameObject.TryGetComponent<BoxCollider2D>(out boxCollider))
            boxCollider.isTrigger = true;

        playerAnim.SetFloat("Moving", 1); //set the players walk animation to true

        yield return new WaitUntil(() => reached == true); //wait until the player reachs the destination

        if(boxCollider) boxCollider.isTrigger = false;

        //Debug.LogError("done..!");
        player.setCanMove(true); //set their binds to active again
        if (spawnObjectOnceDone) Instantiate(spawnObject, spawnObjectTransform.position, Quaternion.identity); //if you choose to spawn an object once done, this does so

        yield return new WaitForSeconds(1f); //wait a little
        this.gameObject.SetActive(false); // set this gameobject to inactive
    }
}

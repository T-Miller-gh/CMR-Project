using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 

public class horseWanderAI : MonoBehaviour
{
    // all assigned IN EDITOR
    public characterManager characterManagerScript;
    public Transform player;
    public NavMeshAgent nav;
    public Animator looseHorseAnim;
    [SerializeField] private AudioSource horseAudioSrc;
    [SerializeField] private AudioClip horseNeighClip; 

    private float moveSpeed = 3f;
    private float rotSpeed = 70f;

    private bool isWandering = false;
    private bool isRotatingRight = false;
    private bool isRotatingLeft = false;
    private bool isWalking = false;
    public bool isCaught = false;

    bool audioPlaying = false; 

    public bool startedFollowingAlready = false; 


    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        nav.enabled = false; 
    }

    void FixedUpdate()
    {
        if(isCaught /*&& !startedFollowingAlready*/)
        {
            StopAllCoroutines();
            Destroy(gameObject); 
            // startFollowingPlayer();
            // isCaught = true;
            startedFollowingAlready = true; 
        }

        if(!audioPlaying)
        {
            StartCoroutine(triggerNeigh());
        }
         

        // this bool is set to true in characterManger.cs
        //if (isCaught)
        //{
        //    // stop the horse custom horse AI, enable NavMeshAgent
        //    // StopAllCoroutines();
        //    // startFollowingPlayer();

        //    float distanceThreshold = 2f;

        //    if (nav.remainingDistance <= (nav.stoppingDistance + distanceThreshold))
        //    {
        //        looseHorseAnim.SetBool("isWalkingForward", false);

        //    }
        //    else
        //    {
        //        looseHorseAnim.SetBool("isWalkingForward", true);
        //    }
        //}

        // if the horse isn't wandering already, start the wandering process
        if (isWandering == false)
        {
            StartCoroutine(Wander());
        }

        // if this is true (set in the coroutine) rotate right
        if (isRotatingRight == true)
        {
            looseHorseAnim.SetBool("isTurningRight", true);
            transform.Rotate(transform.up * rotSpeed * Time.deltaTime);
        }

        // if this is true (set in the coroutine) rotate left
        if (isRotatingLeft == true)
        {
            looseHorseAnim.SetBool("isTurningLeft", true);
            transform.Rotate(transform.up * -rotSpeed * Time.deltaTime);
        }

        // if this is true (set in the coroutine) walk forward
        if(isWalking == true)
        {
            looseHorseAnim.SetBool("isWalkingForward", true);
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }

        if(!isWalking && !isRotatingLeft && !isRotatingRight)
        {
            looseHorseAnim.SetBool("isWalkingForward", false);
            looseHorseAnim.SetBool("isTurningLeft", false);
            looseHorseAnim.SetBool("isTurningRight", false); 
        }
    }

    IEnumerator triggerNeigh()
    {
        audioPlaying = true; 
        horseAudioSrc.PlayOneShot(horseNeighClip);
        yield return new WaitForSeconds(15f);
        audioPlaying = false; 
    }

    IEnumerator Wander()
    {
        // rot time is amount of time capsule will be rotating
        int rotTime = Random.Range(1, 3);
        // rot wait is amount of time between capsule rotations
        int rotWait = Random.Range(3, 10);
        // this determines whether or not it is going to rotate left or right (basically a bool)
        int rotateLorR = Random.Range(1, 3);
        // amount of time between walking
        int walkWait = Random.Range(3, 10);
        // amount of time it will be walking
        int walkTime = Random.Range(1, 5);

        isWandering = true;

        // gets random walk time from above
        yield return new WaitForSeconds(walkWait);
        // turns walking on 
        isWalking = true;
        // starts walking timer 
        yield return new WaitForSeconds(walkTime);
        // turns walking off after timer is done
        isWalking = false;
        // seconds before ai rotates
        yield return new WaitForSeconds(rotWait);

        if (rotateLorR == 1)
        {
            isRotatingRight = true;
            yield return new WaitForSeconds(rotTime);
            isRotatingRight = false;
        }
        else if (rotateLorR == 2)
        {
            isRotatingLeft = true;
            yield return new WaitForSeconds(rotTime);
            isRotatingLeft = false;
        }
        //if (rotateLorR == 1)
        //{
        //    isRotatingRight = true;
        //}
        //    yield return new WaitForSeconds(rotTime);
        //{ 
        //    isRotatingLeft = false;
        //}
        //if (rotateLorR == 2)
        //{
        //    isRotatingLeft = true;
        //}
        //    yield return new WaitForSeconds(rotTime);
        //{ 
        //    isRotatingRight = false;
        //}
        // stops AI from wandering again so the cycle can start all over (in fixed update)
        isWandering = false;
    }

    void startFollowingPlayer()
    {
        // set movSpeed and rotSpeed to zero so horse has no independant movemant after it has been captured
        moveSpeed = 0f;
        rotSpeed = 0f;
        // float distanceThreshold = 2f; 
        // turn on navMesh Agent
        nav.enabled = true; 
        // set destination of horse to player position (adjust stopping distance and such IN EDITOR)
        nav.SetDestination(player.position);

        // Check if the horse is initially moving
        //if (nav.velocity.magnitude > 0)
        //{
        //    looseHorseAnim.SetBool("isWalkingForward", true);
        //}
        //else
        //{
        //    looseHorseAnim.SetBool("isWalkingForward", false);
        //}

        // looseHorseAnim.Rebind();

        looseHorseAnim.Play("DefaultState"); 

         
    }
}

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

    public float moveSpeed = 3f;
    public float rotSpeed = 70f;

    private bool isWandering = false;
    private bool isRotatingRight = false;
    private bool isRotatingLeft = false;
    private bool isWalking = false;

    public bool scriptCommunicationTest = true; 


    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        nav.enabled = false; 
    }

    private void FixedUpdate()
    {
        // PULLING THIS BOOL FROM characterManager
        if(characterManagerScript.changeHorseBehavior == true)
        {
            // Debug.Log("this horse has been caught");
            // Destroy(gameObject);
            // SWITCH HORSE BEHAVIOR HERE
            isWandering = true;
            // stop the horse custom horse AI, enable NavMeshAgent
            StopAllCoroutines(); 
            startFollowingPlayer(); 
        }

        //characterManager cManager = FindObjectOfType<characterManager>();

        //for(int i = 0; i < 1; i++)
        //{
        //    //Debug.Log(cManager.timerOn); 
        //}

        // if the horse isn't wandering already, start the wandering process
        if (isWandering == false)
        {
            StartCoroutine(Wander());
        }

        // if this is true (set in the coroutine) rotate right
        if (isRotatingRight == true)
        {
            transform.Rotate(transform.up * rotSpeed * Time.deltaTime);
        }

        // if this is true (set in the coroutine) rotate left
        if (isRotatingLeft == true)
        {
            transform.Rotate(transform.up * -rotSpeed * Time.deltaTime);
        }

        // if this is true (set in the coroutine) walk forward
        if(isWalking == true)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
    }

    IEnumerator Wander()
    {
        // rot time is amount of time capsule will be rotating
        int rotTime = Random.Range(1, 3);
        // rot wait is amount of time between capsule rotations
        int rotWait = Random.Range(1, 4);
        // this determines whether or not it is going to rotate left or right (basically a bool)
        int rotateLorR = Random.Range(1, 2);
        // amount of time between walking
        int walkWait = Random.Range(1, 4);
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
        }
            yield return new WaitForSeconds(rotTime);
        { 
            isRotatingLeft = false;
        }
        if (rotateLorR == 2)
        {
            isRotatingLeft = true;
        }
            yield return new WaitForSeconds(rotTime);
        { 
            isRotatingRight = false;
        }
        // stops AI from wandering again so the cycle can start all over (in fixed update)
        isWandering = false;
    }

    public void startFollowingPlayer()
    {
        // set movSpeed and rotSpeed to zero so horse has no independant movemant after it has been captured
        moveSpeed = 0f;
        rotSpeed = 0f; 
        // turn on navMesh Agent
        nav.enabled = true; 
        // set destination of horse to player position (adjust stopping distance and such IN EDITOR)
        nav.SetDestination(player.position);

    }
}

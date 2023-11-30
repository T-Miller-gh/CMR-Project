using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class horseWanderAI : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float rotSpeed = 70f;

    private bool isWandering = false;
    private bool isRotatingRight = false;
    private bool isRotatingLeft = false;
    private bool isWalking = false;


    // Start is called before the first frame update
    void Start()
    {

    }

    private void FixedUpdate()
    {
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
}

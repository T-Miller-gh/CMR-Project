using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterManager : MonoBehaviour
{
    public bool playerInRadius;
    public float countdownTimer; 

    private void OnTriggerEnter(Collider other)
    {
        /* dont think we need on trigger enter but we might be able to use it to add sound fx and such
        if (other.gameObject.tag == "horse")
        {
            Debug.Log("I found the horse"); 
        }
        */
    }

    private void OnTriggerStay(Collider other)
    {
        playerInRadius = true; 
        // starts counting down till the player catches the horse
        countdownTimer -= Time.deltaTime; 
        // Debug.Log(countdownTimer); 
        // if the player stays in the horses radius until 0 seconds, they catch it
        if(countdownTimer <= 0)
        {
            Debug.Log("i caught the horse completely");
            Destroy(other.gameObject); 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        playerInRadius = false;
        countdownTimer = 10; 
        Debug.Log("I've lost the horse"); 
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class characterManager : MonoBehaviour
{
    public TextMeshProUGUI gameTimerTxt; 

    public float horseCaptureTimer;
    public float gameTimeLeft;

    public int horsesCollected; 

    public bool timerOn = false;
    public bool playerInRadius = false;
    public bool playerInSafeZone = false;
    public bool allHorsesCollected = false; 

    public void Start()
    {
        // starts the overall game timer (players need to return to base before this runs out)
        timerOn = true;
        playerInSafeZone = false; 
    }

    public void FixedUpdate()
    {
        // checks if the game started, if so, it counts down
        if(timerOn)
        {
            if(gameTimeLeft > 0)
            {
                gameTimeLeft -= Time.deltaTime;
                updateTimer(gameTimeLeft); 
            }
            else
            {
                Debug.Log("timer is up!");
                gameTimeLeft = 0;
                timerOn = false; 
            }
        }

        if (playerInSafeZone && allHorsesCollected && gameTimeLeft > 0)
        {
            Debug.Log("You've won the game!");
        }
    }

    // updates the UI text displaying timer time
    void updateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        gameTimerTxt.text = string.Format("{0:00} : {1:00}", minutes, seconds); 
    }

    public void horseCounter()
    {
        horsesCollected += 1; 

        if(horsesCollected > 0)
        {
            allHorsesCollected = true; 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "safeZone")
        {
            Debug.Log("I've entered safe zone"); 
        }
    }

    private void OnTriggerStay(Collider other)
    {
        playerInRadius = true; 
        // starts counting down till the player catches the horse

        if(other.tag == "horse")
        {
            horseCaptureTimer -= Time.deltaTime;

            // Debug.Log(countdownTimer); 
            // if the player stays in the horses radius until 0 seconds, they catch it
            if (horseCaptureTimer <= 0)
            {
                Debug.Log("i caught the horse completely");
                horseCounter();
                Destroy(other.gameObject);
            }
        }

        if (other.tag == "safeZone")
        {
            playerInSafeZone = true; 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        playerInRadius = false;
        horseCaptureTimer = 10; 
        Debug.Log("I've lost the horse"); 
    }

}

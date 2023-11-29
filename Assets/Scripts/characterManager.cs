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
    public int horseCount; 

    public bool timerOn = false;
    public bool playerInSafeZone = false;
    public bool allHorsesCollected = false;
    // public bool playerInRadius = false;

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
                // Debug.Log("GAME OVER");
                // gameTimeLeft = 0;
                // timerOn = false; 
            }
        }

        if (playerInSafeZone && allHorsesCollected && gameTimeLeft > 0)
        {
            Debug.Log("You've won the game!");
        }
        else if(gameTimeLeft <= 0)
        {
            Debug.Log("Game Over"); 
        }

    }

    // updates the UI text displaying timer time remaining
    void updateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        gameTimerTxt.text = string.Format("{0:00} : {1:00}", minutes, seconds); 
    }

    // each time a player collets a horse, this adds one, and checks to see if all of the lost horses have been collected
    public void horseCounter()
    {
        horsesCollected += 1; 

        if(horsesCollected == horseCount)
        {
            // this has to be true in order for player to win
            allHorsesCollected = true; 
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // playerInRadius = true; 
        // starts counting down till the player catches the horse

        if(other.tag == "horse")
        {
            horseCaptureTimer -= Time.deltaTime;

            // Debug.Log(countdownTimer); 
            // if the player stays in the horses radius until 0 seconds, they catch it
            if (horseCaptureTimer <= 0)
            {
                horseCounter();
                horseCaptureTimer = 10; 
                // eventually turn this into the horse following player
                Destroy(other.gameObject);
            }
        }

        // check if player is in safe zone, this has to be active in order for player to win
        if (other.tag == "safeZone")
        {
            playerInSafeZone = true; 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "horse")
        {
            horseCaptureTimer = 10;
            // Debug.Log("I've lost the horse");
        }

        if (other.tag == "safeZone")
        {
            playerInSafeZone = false;
        }
    }

}

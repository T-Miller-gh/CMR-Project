using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class characterManager : MonoBehaviour
{
    public ParticleSystem playerSnowParticles; 

    public TextMeshProUGUI gameTimerTxt;
    public TextMeshProUGUI horsesCapturedTxt;

    public float horseCaptureTimer;
    public float gameTimeLeft;
    public float totalGameTime = 120f; 
    public float maxParticleSize = 2;
    public float maxEmissionRate = 350;
    //public float sizeChangeSpeed = .0001f; // Adjust as needed
    //public float rateChangeSpeed = 2f; // Adjust as needed

    public int horsesCollected;
    public int horseCount; 

    public bool timerOn = false;
    public bool playerInSafeZone = false;
    public bool allHorsesCollected = false;
    public bool changeHorseBehavior = false; 

    public void Start()
    {
        // starts the overall game timer (players need to return to base before this runs out)
        timerOn = true;
        playerInSafeZone = false;

        var particleSize = playerSnowParticles.main;
        var particleRateOverTime = playerSnowParticles.emission; 

        // these two variables create a great white-out...so it should build up to this as time goes on. 
        particleSize.startSize = .1f;
        particleRateOverTime.rateOverTime = 10f; 
    }

    public void FixedUpdate()
    {
        // checks if the game started, if so, it counts down
        if(timerOn)
        {
            if(gameTimeLeft > 0)
            {
                gameTimeLeft -= Time.deltaTime;
                updatePlayerParticles(); 
                updateTimer(gameTimeLeft); 
            }
            else
            {
                // add game over functionality here 
            }
        }

        if (playerInSafeZone && allHorsesCollected && gameTimeLeft > 0)
        {
            // Debug.Log("You've won the game!");
            gameTimerTxt.text = "You won!!";
            horsesCapturedTxt.text = "You won!!";
            // pauses the game, change this later
            Time.timeScale = 0; 
        }
        else if(gameTimeLeft <= 0)
        {
            // Debug.Log("Game Over");
            gameTimerTxt.text = "Game over";
            horsesCapturedTxt.text = "Game over";
            // pauses the game, change later; 
            Time.timeScale = 0; 
        }

    }

    void updatePlayerParticles()
    {
        var particleSize = playerSnowParticles.main;
        var particleRateOverTime = playerSnowParticles.emission;

        float remainingGameTime = Mathf.Max(0f, gameTimeLeft - Time.time);

        //float sizeChangeRate = sizeChangeSpeed * Time.deltaTime;
        //float emissionChangeRate = rateChangeSpeed * Time.deltaTime;


        //float newParticleSize = Mathf.MoveTowards(particleSize.startSize.constant, maxParticleSize, sizeChangeRate);
        //float newParticleRate = Mathf.MoveTowards(particleRateOverTime.rateOverTime.constant, maxEmissionRate, emissionChangeRate); 

        float newParticleSize = Mathf.Lerp(0.1f, maxParticleSize, 1f - (remainingGameTime / totalGameTime));
        float newParticleRate = Mathf.Lerp(10f, maxEmissionRate, 1f - (remainingGameTime / totalGameTime));

        particleSize.startSize = newParticleSize;
        particleRateOverTime.rateOverTime = newParticleRate;
    }

    // updates the UI text displaying timer time remaining
    void updateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        gameTimerTxt.text = string.Format("Time remaining: " + "{0:00} : {1:00}", minutes, seconds); 
    }

    // each time a player collets a horse, this adds one, and checks to see if all of the lost horses have been collected
    public void horseCounter()
    {
        horsesCollected += 1;
        // update horses collected UI
        horsesCapturedTxt.text = "Horses collected: " + horsesCollected + "/3";

        if (horsesCollected == horseCount)
        {
            // this has to be true in order for player to win
            allHorsesCollected = true; 
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "horse")
        {
            // get the script off the horse you are capturing
            horseWanderAI horseScript = other.GetComponent<horseWanderAI>();

            horseCaptureTimer -= Time.deltaTime;

            // if the player stays in the horses radius until 0 seconds, they catch it
            if (horseCaptureTimer <= 0)
            {
                horseCounter();
                horseCaptureTimer = 10;

                // change that horses behavior within it's script
                horseScript.isCaught = true; 
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

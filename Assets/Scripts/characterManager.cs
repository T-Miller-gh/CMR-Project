using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; 

public class characterManager : MonoBehaviour
{
    [SerializeField] private InputActionReference menuInputActionReference;

    public ParticleSystem playerSnowParticles; 

    public TextMeshProUGUI gameTimerTxt;
    public TextMeshProUGUI horsesCapturedTxt;

    public GameObject quitMenu;
    public GameObject loadingUI; 

    public float horseCaptureTimer;
    float gameTimeLeft = 300;
    float totalGameTime = 300;
    float gameProgress; 
    float maxParticleSize = 3f;
    float maxEmissionRate = 100f;
    float maxParticleSpeed = 5f; 

    public int horsesCollected;
    public int horseCount; 

    public bool timerOn = false;
    public bool playerInSafeZone = false;
    public bool allHorsesCollected = false;
    public bool gameStarted = false; 
    // public bool changeHorseBehavior = false;

    void OnEnable()
    {
        menuInputActionReference.action.started += MenuPressed;
    }

    private void OnDisable()
    {
        menuInputActionReference.action.started -= MenuPressed;
    }

    private void Awake()
    {
        totalGameTime = 300;
        gameTimeLeft = totalGameTime;
    }

    public void Start()
    {
        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            // starts the overall game timer (players need to return to base before this runs out)
            gameStarted = true;
            timerOn = true;
            horsesCollected = 0;
        }
        else
        {
            gameStarted = false;
            timerOn = false;
            horsesCollected = 0; 
        }

        // add functions here that start the game 
        // intro to game by pete vann (mini tutorial, etc) 
        // fade to black then back to game view
        // timer starts now
    }

    public void ResetGame()
    {
        timerOn = false;
        gameStarted = false; 
        playerInSafeZone = false;
        allHorsesCollected = false;
        horsesCollected = 0;
        gameTimeLeft = 300;
        totalGameTime = 300;
    }

    void MenuPressed(InputAction.CallbackContext context)
    {
        Time.timeScale = 0; 
        quitMenu.SetActive(true); 
        // Debug.Log("Menu pressed");
        // SceneSelectionManager.LoadMenuScene();
    }

    public void HideQuitMenu()
    {
        quitMenu.SetActive(false);
        Time.timeScale = 1; 
    }

    public void ReturnToMenu()
    {
        Debug.Log("returning to menu"); 
        quitMenu.SetActive(false); 
        loadingUI.SetActive(true);
        Time.timeScale = 1;

        Destroy(gameObject);

        ResetGame();
        Debug.Log("Game reset"); 
        SceneSelectionManager.LoadMenuScene();
    }

    public void FixedUpdate()
    {
        if (gameStarted)
        {
            // checks if the game started, if so, it counts down
            if (timerOn)
            {
                if (gameTimeLeft > 0)
                {
                    gameTimeLeft -= Time.deltaTime;
                    gameProgress = 1f - (gameTimeLeft / totalGameTime);
                    updateTimer(gameTimeLeft);
                    updatePlayerParticles(); 
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
            else if (gameTimeLeft <= 0)
            {
                // Debug.Log("Game Over");
                gameTimerTxt.text = "Game over";
                horsesCapturedTxt.text = "Game over";
                // pauses the game, change later; 
                Time.timeScale = 0;
            }
        }
    }

    void updatePlayerParticles()
    {
        if(timerOn)
        {
            var particleSize = playerSnowParticles.main;
            var particleRateOverTime = playerSnowParticles.emission;
            var particleSpeed = playerSnowParticles.velocityOverLifetime;

            float newParticleSize = Mathf.Lerp(0.03f, maxParticleSize, gameProgress);
            float newParticleRate = Mathf.Lerp(10f, maxEmissionRate, gameProgress);
            float newParticleSpeed = Mathf.Lerp(1f, maxParticleSpeed, gameProgress);

            particleSize.startSize = newParticleSize;
            particleRateOverTime.rateOverTime = newParticleRate;
            particleSpeed.speedModifier = newParticleSpeed;
        }
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

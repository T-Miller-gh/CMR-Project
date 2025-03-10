using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit; 

public class characterManager : MonoBehaviour
{
    [SerializeField] private InputActionReference menuInputActionReference;
    [SerializeField] private InputActionReference primaryButtonReference;

    float threshold = 0.5f; 

    public ActionBasedContinuousMoveProvider playerXrMovementSpeed;
    public XRInteractorLineVisual[] lineVisuals; 

    public ParticleSystem playerSnowParticles;

    public AudioSource playerAudioSource;
    public AudioSource horseAudioSource;
    public AudioClip whistleCaught;
    public AudioClip horseWalk;
    public AudioClip horseRun;
    public AudioClip wind; 
    public AudioClip[] narrativeAudio; 

    public Animator horseAnim; 

    public TextMeshProUGUI gameTimerTxt;
    public TextMeshProUGUI horsesCapturedTxt;

    public GameObject quitMenu;
    public GameObject inactivityMenu; 
    public GameObject loadingUI;
    public GameObject youLoseUIGO;
    public GameObject youWinUIGO;
    public GameObject introUIGO;
    public GameObject introUIInstructions; 

    public CanvasGroup youLoseUI;
    public CanvasGroup youWinUI;
    public CanvasGroup introUI;

    public Image peteImage;
    public TextMeshProUGUI textComponent;
    public string[] dialogueLines;
    public float textSpeed; 
    int index; 

    public TextMeshProUGUI[] dialogue;
    int currentIndex = 0;
    int lastIndex = 0;

    public float horseCaptureTimer;
    public float gameTimeLeft = 300;
    public float totalGameTime = 300;
    public float gameProgress; 
    float maxParticleSize = 3f;
    float maxEmissionRate = 100f;
    float maxParticleSpeed = 5f;
    float fadeDuration = 2f; 

    public int horsesCollected = 0;
    public int horseCount;

    bool throughIntroInstructions = false;
    bool cutsceneStarted = false; 
    public bool timerOn = false;
    public bool playerInSafeZone = false;
    public bool allHorsesCollected = false;
    public bool gameStarted = false;
    bool throughCutscene = false;
    bool isFading = false;
    bool horseWalkPlaying = false;
    bool horseRunPlaying = false; 
    // public bool changeHorseBehavior = false;

    void OnEnable()
    {
        menuInputActionReference.action.started += MenuPressed;
        primaryButtonReference.action.started += PrimaryButtonPressed;
        // primaryButtonReference.action.started += PlayerInstructions; 
        CameraMovementTracker.pauseGameForInactivity += GamePausedForInactivity;
    }

    private void OnDisable()
    {
        menuInputActionReference.action.started -= MenuPressed;
        primaryButtonReference.action.started -= PrimaryButtonPressed;
        // primaryButtonReference.action.started -= PlayerInstructions; 
        CameraMovementTracker.pauseGameForInactivity -= GamePausedForInactivity; 
    }

    private void Awake()
    {
        // totalGameTime = 30;
        gameTimeLeft = totalGameTime;
    }

    public void Start()
    {
        //if(SceneManager.GetActiveScene().buildIndex == 1)
        //{
        //    // starts the overall game timer (players need to return to base before this runs out)
        //    gameStarted = true;
        //    timerOn = true;
        //    horsesCollected = 0;
        //}
        //else
        //{
        //    gameStarted = false;
        //    timerOn = false;
        //    horsesCollected = 0; 
        //}

        lineVisuals[0].enabled = true;
        lineVisuals[1].enabled = true;
        youLoseUIGO.SetActive(false);
        youWinUIGO.SetActive(false); 
        // introUIGO.SetActive(true); 

        Time.timeScale = 0;

        textComponent.text = string.Empty;
        //StartCoroutine(StartCutscene()); 
        PlayerInstructions();

        //if(throughCutscene == true)
        //{
        //    Debug.Log("game should have started"); 
        //    Time.timeScale = 1; 
        //    gameStarted = true;
        //    timerOn = true;
        //    horsesCollected = 0;

        //    youWinUI.alpha = 0f;
        //    youLoseUI.alpha = 0f;
        //}
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
        lineVisuals[0].enabled = true;
        lineVisuals[1].enabled = true;
        quitMenu.SetActive(true); 
        // Debug.Log("Menu pressed");
        // SceneSelectionManager.LoadMenuScene();
    }

    void GamePausedForInactivity()
    {
        Time.timeScale = 0;
        lineVisuals[0].enabled = true;
        lineVisuals[1].enabled = true;
        inactivityMenu.SetActive(true);
        // Debug.Log("Menu pressed");
    }

    public void HideQuitMenu()
    {
        quitMenu.SetActive(false);
        lineVisuals[0].enabled = false;
        lineVisuals[1].enabled = false;
        Time.timeScale = 1; 
    }

    public void HideInactivityMenu()
    {
        inactivityMenu.SetActive(false);
        lineVisuals[0].enabled = false;
        lineVisuals[1].enabled = false;
        Time.timeScale = 1;
    }

    public void ReturnToMenu()
    {
        // Debug.Log("returning to menu"); 
        quitMenu.SetActive(false); 
        loadingUI.SetActive(true);
        Time.timeScale = 1;

        StartCoroutine(waitForReferencesToUnsubscribe()); 

        // Destroy(gameObject);

        StopAllCoroutines(); 
        ResetGame();
        // Debug.Log("Game reset"); 
        SceneSelectionManager.LoadMenuScene();
    }

    public void RestartGame()
    {
        // Destroy(gameObject); 
        StopAllCoroutines(); 
        ResetGame();
        SceneManager.LoadScene(1);
        Time.timeScale = 1; 
    }

    public void FixedUpdate()
    {
        if (throughCutscene)
        {
            // Debug.Log("game should have started");
            gameStarted = true;
            timerOn = true;
            lineVisuals[0].enabled = false;
            lineVisuals[1].enabled = false;
            // horsesCollected = 0;

            youWinUI.alpha = 0f;
            youLoseUI.alpha = 0f;
        }

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

                    // Particles handled in particle settings; replaced size increase with fog increase; speed increase was too much for quest
                    //updatePlayerParticles(); 
                }
                else
                {
                    // add game over functionality here 
                }
            }

            if (playerInSafeZone && allHorsesCollected && gameTimeLeft > 0)
            {
                StartCoroutine(FadeInYouWinUI());  
                // youWinUI.GetComponent<CanvasGroup>().alpha = 1; 

                // Debug.Log("You've won the game!");
                gameTimerTxt.text = "";
                horsesCapturedTxt.text = "";
                // pauses the game, change this later
                Time.timeScale = 0;
            }
            else if (gameTimeLeft <= 0)
            {
                // Debug.Log("Game Over");
                StartCoroutine(FadeInYouLoseUI()); 
                gameTimerTxt.text = "";
                horsesCapturedTxt.text = "";
                // pauses the game, change later; 
                Time.timeScale = 0;
            }
        }

        float leftThumbstickVertical = Input.GetAxis("XRI_Left_Primary2DAxis_Vertical");
        float rightThumbstickHorizontal = Input.GetAxis("XRI_Right_Primary2DAxis_Horizontal");

        if (Mathf.Abs(leftThumbstickVertical) > threshold)
        {
            if (leftThumbstickVertical > 0)
            {
                // Debug.Log("Thumbstick pushed forward: " + leftThumbstickVertical);
                horseAnim.SetBool("isWalkingForward", false);
                horseAnim.SetBool("isWalkingBackward", true);
                playerXrMovementSpeed.moveSpeed = 2.5f;

                if (!horseWalkPlaying)
                {
                    horseAudioSource.clip = horseWalk;
                    horseAudioSource.Play();
                    horseWalkPlaying = true;
                    horseRunPlaying = false;
                }
            }
            else
            {
                if (Input.GetButton("XRI_Left_GripButton"))
                {
                    // Debug.Log(" within gallop ");
                    horseAnim.SetBool("isGalloping", true);
                    horseAnim.SetBool("isWalkingForward", false);
                    horseAnim.SetBool("isWalkingBackward", false);
                    playerXrMovementSpeed.moveSpeed = 5f;

                    if (!horseRunPlaying)
                    {
                        horseAudioSource.clip = horseRun;
                        horseAudioSource.Play();
                        horseRunPlaying = true;
                        horseWalkPlaying = false;
                        // Debug.Log("into horse running"); 
                    }
                }
                else
                {
                    // Debug.Log("thumbstick pushed backwards: " + leftThumbstickVertical);
                    horseAnim.SetBool("isWalkingBackward", false);
                    horseAnim.SetBool("isGalloping", false);
                    horseAnim.SetBool("isWalkingForward", true);
                    playerXrMovementSpeed.moveSpeed = 2.5f;

                    if (!horseWalkPlaying)
                    {
                        horseAudioSource.clip = horseWalk;
                        horseAudioSource.Play();
                        horseWalkPlaying = true;
                        horseRunPlaying = false;
                    }
                }
            }
        }
        else
        {
            horseAnim.SetBool("isWalkingForward", false);
            horseAnim.SetBool("isWalkingBackward", false);
            horseAnim.SetBool("isGalloping", false);

            horseAudioSource.Stop();
            horseWalkPlaying = false;
            horseRunPlaying = false;
        }

        if (Mathf.Abs(rightThumbstickHorizontal) > threshold)
        {
            if (rightThumbstickHorizontal > 0)
            {
                // Debug.Log("Thumbstick pushed right: " + rightThumbstickHorizontal);
                horseAnim.SetBool("isTurningRight", true);
                horseAnim.SetBool("isTurningLeft", false);
            }
            else if (rightThumbstickHorizontal < 0)
            {
                // Debug.Log("thumbstick pushed left: " + rightThumbstickHorizontal);
                horseAnim.SetBool("isTurningLeft", true);
                horseAnim.SetBool("isTurningRight", false);
            }
        }
        else
        {
            horseAnim.SetBool("isTurningRight", false);
            horseAnim.SetBool("isTurningLeft", false);
        }
    }

    IEnumerator waitForReferencesToUnsubscribe()
    {
        yield return new WaitForSeconds(4f);
    }

    // systems for fading in UI when player either wins or loses game
    IEnumerator FadeInYouWinUI()
    {
        horseAudioSource.Stop();
        playerAudioSource.Stop();
        playerAudioSource.PlayOneShot(narrativeAudio[2]);
        youWinUIGO.SetActive(true);
        lineVisuals[0].enabled = true;
        lineVisuals[1].enabled = true;

        // Debug.Log("I am within the coroutine"); 
        float elapsedTime = 0f;
        float startAlpha = 0f;
        float targetAlpha = 1f;

        while (elapsedTime < fadeDuration)
        {
            youWinUI.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
            // Debug.Log("im within the while loop"); 
        }

        youWinUI.alpha = targetAlpha;
    }

    IEnumerator FadeInYouLoseUI()
    {
        horseAudioSource.Stop();
        playerAudioSource.Stop();
        playerAudioSource.PlayOneShot(narrativeAudio[3]);
        youLoseUIGO.SetActive(true);
        lineVisuals[0].enabled = true;
        lineVisuals[1].enabled = true;
        // Debug.Log("I am within the coroutine");

        float elapsedTime = 0f;
        float startAlpha = 0f;
        float targetAlpha = 1f;

        while (elapsedTime < fadeDuration)
        {
            youLoseUI.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
            // Debug.Log("im within the while loop");
        }

        youLoseUI.alpha = targetAlpha;
    }

    // increases the intensity of snow as the game time gets closer to 0
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
        horseAudioSource.PlayOneShot(whistleCaught); 

        if (horsesCollected == horseCount)
        {
            // this has to be true in order for player to win
            allHorsesCollected = true; 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        {
            if (other.tag == "horse")
            {
                // get the script off the horse you are capturing
                horseWanderAI horseScript = other.GetComponent<horseWanderAI>();

                // horseCaptureTimer -= Time.deltaTime;

                // if the player stays in the horses radius until 0 seconds, they catch it
                //if (horseCaptureTimer <= 0)
                //{
                //    horseCounter();
                //    horseCaptureTimer = 10;

                //    // change that horses behavior within it's script
                //    horseScript.isCaught = true; 
                //}
                if(!horseScript.isCaught)
                {
                    horseCounter();
                    horseScript.isCaught = true;
                }

            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //if(other.tag == "horse")
        //{
        //    // get the script off the horse you are capturing
        //    horseWanderAI horseScript = other.GetComponent<horseWanderAI>();

        //    // horseCaptureTimer -= Time.deltaTime;

        //    // if the player stays in the horses radius until 0 seconds, they catch it
        //    //if (horseCaptureTimer <= 0)
        //    //{
        //    //    horseCounter();
        //    //    horseCaptureTimer = 10;

        //    //    // change that horses behavior within it's script
        //    //    horseScript.isCaught = true; 
        //    //}
        //    horseCounter();
        //    horseScript.isCaught = true; 
        //}

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

    // CUTSCENE CODE ALL BELOW

    void PlayerInstructions()
    {
        introUIInstructions.SetActive(true);
        throughIntroInstructions = true;

        playerAudioSource.PlayOneShot(narrativeAudio[0]);
    }

    void PrimaryButtonPressed(InputAction.CallbackContext context)
    {
        if(throughIntroInstructions && !cutsceneStarted)
        {
            Debug.Log("start cutscene");
            introUIInstructions.SetActive(false); 
            introUIGO.SetActive(true); 
            StartCoroutine(StartCutscene());
            cutsceneStarted = true;
        }

        //Debug.Log("primary button pressed");
        if (textComponent.text == dialogueLines[index])
        {
            NextLine();
        }
        else
        {
            StopAllCoroutines();
            textComponent.text = dialogueLines[index];
        }
    }

    IEnumerator StartCutscene()
    {
        peteImage.gameObject.SetActive(true); 
        // yield return StartCoroutine(FadeIn(peteImage));
        index = 0;
        // Debug.Log("past fadeimage" + index);

        playerAudioSource.Stop(); 
        playerAudioSource.PlayOneShot(narrativeAudio[1]); 

        StartCoroutine(TypeLine());
        // yield return StartCoroutine(FadeInText(dialogue[0]));
        yield return null; 
    }

    IEnumerator TypeLine()
    {
        // Debug.Log("within type line");
        foreach (char c in dialogueLines[index].ToCharArray())
        {
            // Debug.Log("within foreach");
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < dialogueLines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            Debug.Log("start game now");
            throughCutscene = true;
            Time.timeScale = 1;
            Debug.Log(throughCutscene);
            playerAudioSource.Stop(); 
            playerAudioSource.clip = wind;
            playerAudioSource.Play(); 
            StartCoroutine(FadeOutIntro(introUI));
        }
    }

    IEnumerator FadeIn(Image image)
    {
        CanvasGroup canvasGroup = image.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = image.gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0;

        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.unscaledDeltaTime / 2; // Adjust the speed of fade-in
            yield return null;
        }
    }

    IEnumerator FadeOutIntro(CanvasGroup canvasGroup)
    {
        while(canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.unscaledDeltaTime / fadeDuration;
            yield return null;
            introUIGO.SetActive(false); 
        }
    }
}

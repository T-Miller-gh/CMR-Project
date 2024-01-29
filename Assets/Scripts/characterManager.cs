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

    //public XRNode leftMovementInputSource;
    //public XRNode rightMovementInputSource; 

    float threshold = 0.5f; 

    //List<XRNodeState> nodeStates = new List<XRNodeState>();
    //Vector3 lastPosition;

    public ActionBasedContinuousMoveProvider playerXrMovementSpeed;
    public XRInteractorLineVisual[] lineVisuals; 

    public ParticleSystem playerSnowParticles; 

    public Animator horseAnim; 

    public TextMeshProUGUI gameTimerTxt;
    public TextMeshProUGUI horsesCapturedTxt;

    public GameObject quitMenu;
    public GameObject loadingUI;
    public GameObject youLoseUIGO;
    public GameObject youWinUIGO;
    public GameObject introUIGO; 

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
    float gameTimeLeft = 300;
    float totalGameTime = 300;
    float gameProgress; 
    float maxParticleSize = 3f;
    float maxEmissionRate = 100f;
    float maxParticleSpeed = 5f;
    float fadeDuration = 2f; 

    public int horsesCollected = 0;
    public int horseCount; 

    public bool timerOn = false;
    public bool playerInSafeZone = false;
    public bool allHorsesCollected = false;
    public bool gameStarted = false;
    bool throughCutscene = false;
    bool isFading = false; 
    // public bool changeHorseBehavior = false;

    void OnEnable()
    {
        menuInputActionReference.action.started += MenuPressed;
        primaryButtonReference.action.started += PrimaryButtonPressed; 
    }

    private void OnDisable()
    {
        menuInputActionReference.action.started -= MenuPressed;
        primaryButtonReference.action.started -= PrimaryButtonPressed; 
    }

    private void Awake()
    {
        totalGameTime = 300;
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
        introUIGO.SetActive(true); 

        Time.timeScale = 0;

        textComponent.text = string.Empty; 
        StartCoroutine(StartCutscene()); 

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
        lineVisuals[0].enabled = true;
        lineVisuals[1].enabled = true;
        quitMenu.SetActive(true); 
        // Debug.Log("Menu pressed");
        // SceneSelectionManager.LoadMenuScene();
    }

    public void HideQuitMenu()
    {
        quitMenu.SetActive(false);
        lineVisuals[0].enabled = false;
        lineVisuals[1].enabled = false;
        Time.timeScale = 1; 
    }

    public void ReturnToMenu()
    {
        Debug.Log("returning to menu"); 
        quitMenu.SetActive(false); 
        loadingUI.SetActive(true);
        Time.timeScale = 1;

        // Destroy(gameObject);

        StopAllCoroutines(); 
        ResetGame();
        Debug.Log("Game reset"); 
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
                    updatePlayerParticles(); 
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

        //InputTracking.GetNodeStates(nodeStates);
        //XRNodeState state = nodeStates.Find(s => s.nodeType == movementInputSource); 

        float leftThumbstickVertical = Input.GetAxis("XRI_Left_Primary2DAxis_Vertical");
        float rightThumbstickHorizontal = Input.GetAxis("XRI_Right_Primary2DAxis_Horizontal");

        //if (Input.GetButton("XRI_Left_GripButton"))
        //{
        //    Debug.Log("Run now");
        //}

        //{
        //    Debug.Log("running now");  
        //}

        //if (leftTrigger)
        //{
        //    Debug.Log("Left trigger"); 
        //}

        if (Mathf.Abs(leftThumbstickVertical) > threshold)
        {
            if (leftThumbstickVertical > 0)
            {
                // Debug.Log("Thumbstick pushed forward: " + leftThumbstickVertical);
                horseAnim.SetBool("isWalkingForward", false);
                horseAnim.SetBool("isWalkingBackward", true);
                playerXrMovementSpeed.moveSpeed = 2.5f; 
            }
            else if(leftThumbstickVertical < 0)
            {
                // Debug.Log("thumbstick pushed backwards: " + leftThumbstickVertical);
                horseAnim.SetBool("isWalkingBackward", false);
                horseAnim.SetBool("isWalkingForward", true);

                if (Input.GetButton("XRI_Left_GripButton"))
                {
                    horseAnim.SetBool("isGalloping", true);
                    horseAnim.SetBool("isWalkingForward", false);
                    horseAnim.SetBool("isWalkingBackward", false); 
                    playerXrMovementSpeed.moveSpeed = 5f; 
                }
                else
                {
                    // add animation logic here
                    horseAnim.SetBool("isWalkingForward", true);
                    horseAnim.SetBool("isGalloping", false);
                    horseAnim.SetBool("isWalkingBackward", false); 
                    playerXrMovementSpeed.moveSpeed = 2.5f; 
                }
            }

        }
        else
        {
            horseAnim.SetBool("isWalkingForward", false);
            horseAnim.SetBool("isWalkingBackward", false);
            horseAnim.SetBool("isGalloping", false); 
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

    // systems for fading in UI when player either wins or loses game
    IEnumerator FadeInYouWinUI()
    {
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

    void PrimaryButtonPressed(InputAction.CallbackContext context)
    {
        Debug.Log("primary button pressed");

        if (textComponent.text == dialogueLines[index])
        {
            NextLine();
        }
        else
        {
            StopAllCoroutines();
            textComponent.text = dialogueLines[index];
        }

        //// Check if there are more dialogue elements
        //if (currentIndex < dialogue.Length - 1)
        //{
        //    // Increment the index to switch to the next dialogue
        //    currentIndex++;
        //    lastIndex = currentIndex - 1;

        //    // Start the fade -in for the new dialogue element
        //    StartCoroutine(FadeInText(dialogue[currentIndex]));
        //    if (currentIndex > 0)
        //    {
        //        StartCoroutine(FadeOutText(dialogue[0]));
        //    }

        //    if (lastIndex < currentIndex)
        //    {
        //        StartCoroutine(FadeOutText(dialogue[lastIndex]));
        //    }
        //}
        //else
        //{
        //    Debug.Log("start game now");
        //    throughCutscene = true;
        //    Time.timeScale = 1;
        //    Debug.Log(throughCutscene); 
        //    StartCoroutine(FadeOutIntro(introUI)); 
        //}
    }

    IEnumerator StartCutscene()
    {
        yield return StartCoroutine(FadeIn(peteImage));
        index = 0;
        Debug.Log("past fadeimage" + index);
        StartCoroutine(TypeLine());
        // yield return StartCoroutine(FadeInText(dialogue[0]));
    }

    IEnumerator TypeLine()
    {
        Debug.Log("within type line");
        foreach (char c in dialogueLines[index].ToCharArray())
        {
            Debug.Log("within foreach");
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

    //IEnumerator FadeInText(TextMeshProUGUI text)
    //{
    //    CanvasGroup canvasGroup = text.GetComponent<CanvasGroup>();

    //    // Reset the alpha to 0 for the current text
    //    canvasGroup.alpha = 0;

    //    // Fade in the current text
    //    while (canvasGroup.alpha < 1)
    //    {
    //        canvasGroup.alpha += Time.unscaledDeltaTime / fadeDuration; // Adjust the speed of fade-in

    //        yield return null;
    //    }
    //}

    //IEnumerator FadeOutText(TextMeshProUGUI text)
    //{
    //    CanvasGroup canvasGroup = text.GetComponent<CanvasGroup>();

    //    // Fade out the text
    //    while (canvasGroup.alpha > 0)
    //    {
    //        canvasGroup.alpha -= Time.unscaledDeltaTime / fadeDuration; // Adjust the speed of fade-out
    //        yield return null;
    //    }
    //}

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

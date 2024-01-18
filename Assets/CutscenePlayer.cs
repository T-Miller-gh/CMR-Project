using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class CutscenePlayer : MonoBehaviour
{
    [SerializeField] private InputActionReference menuInputActionReference;
    [SerializeField] private InputActionReference primaryButtonReference;

    public CanvasGroup introCutsceneUI;

    public Image peteImage;
    public TextMeshProUGUI[] dialogue;
    int currentIndex = 0;
    int lastIndex = 0; 

    float fadeDuration = 2f;

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

    private void Start()
    {
        StartCoroutine(StartCutscene()); 
    }

    void MenuPressed(InputAction.CallbackContext context)
    {
        Time.timeScale = 0;
        Debug.Log("menu button pressed"); 
        // quitMenu.SetActive(true);
    }

    void PrimaryButtonPressed(InputAction.CallbackContext context)
    {
        Debug.Log("primary button pressed");

        // Check if there are more dialogue elements
        if (currentIndex < dialogue.Length - 1)
        {
            // Increment the index to switch to the next dialogue
            currentIndex++;
            lastIndex = currentIndex - 1;

            StartCoroutine(HandleFading()); 

            // Start the fade-in for the new dialogue element
            //StartCoroutine(FadeInText(dialogue[currentIndex]));


            //if(currentIndex > 0)
            //{
            //    StartCoroutine(FadeOutText(dialogue[0]));
            //}
            
            //if(lastIndex < currentIndex)
            //{
            //    StartCoroutine(FadeOutText(dialogue[lastIndex])); 
            //}
        }
        else
        {
            // If this was the last dialogue element, load the next scene
            SceneManager.LoadScene("Level_main");
        }
    }

    IEnumerator HandleFading()
    {
        StartCoroutine(FadeOutText(dialogue[lastIndex]));

        yield return new WaitForSeconds(fadeDuration);

        StartCoroutine(FadeInText(dialogue[currentIndex]));
    }

    public void HideQuitMenu()
    {
        //quitMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void ReturnToMenu()
    {
        Debug.Log("returning to menu");
        //quitMenu.SetActive(false);
        //loadingUI.SetActive(true);
        Time.timeScale = 1;

        Destroy(gameObject);

        Debug.Log("Game reset");
        SceneSelectionManager.LoadMenuScene();
    }

    IEnumerator StartCutscene()
    {
        yield return StartCoroutine(FadeIn(peteImage));

        yield return StartCoroutine(FadeInText(dialogue[0]));
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
            canvasGroup.alpha += Time.deltaTime / 2; // Adjust the speed of fade-in
            yield return null;
        }
    }

    IEnumerator FadeInText(TextMeshProUGUI text)
    {
        CanvasGroup canvasGroup = text.GetComponent<CanvasGroup>(); 
 
        // Reset the alpha to 0 for the current text
        canvasGroup.alpha = 0;

        // Fade in the current text
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime / fadeDuration; // Adjust the speed of fade-in

            yield return null;
        }
    }

    IEnumerator FadeOutText(TextMeshProUGUI text)
    {
        CanvasGroup canvasGroup = text.GetComponent<CanvasGroup>(); 

        // Fade out the text
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime / fadeDuration; // Adjust the speed of fade-out
            yield return null;
        }
    }
}

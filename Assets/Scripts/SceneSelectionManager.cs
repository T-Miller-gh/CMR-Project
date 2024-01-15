using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; 


public class SceneSelectionManager : MonoBehaviour
{
    // [SerializeField] private InputActionReference menuInputActionReference; 

    public delegate void ButtonClickMethod(); 
    Dictionary<string, ButtonClickMethod> buttonDictionay = new Dictionary<string, ButtonClickMethod>();

    public GameObject nightwranglerBtn;
    public GameObject playBtn; 
    public TextMeshProUGUI gameDescriptionText;

    public int sceneSelection = 0;

    public GameObject loadingScreen;
    float loadingDelay = 2.0f; 

    void Start()
    {
        Button nightWranglerPlay = nightwranglerBtn.GetComponent<Button>();
        Button playGameBtn = playBtn.GetComponent<Button>();
        playBtn.SetActive(false);

        buttonDictionay["nightwrangler"] = LoadNightwranglerScene;
        buttonDictionay["play"] = PlayGame; 
        // buttonDictionay["menu"] = LoadMenuScene;

        nightWranglerPlay.onClick.AddListener(() => OnButtonClick("nightwrangler"));
        playGameBtn.onClick.AddListener(() => OnButtonClick("play")); 
    }


    public void OnButtonClick(string buttonName)
    {
        if(buttonDictionay.ContainsKey(buttonName))
        {
            buttonDictionay[buttonName].Invoke(); 
            // StartCoroutine(LoadSceneWithDelay(buttonDictionay[buttonName]));
        }
    }

    IEnumerator LoadSceneWithDelay(ButtonClickMethod sceneLoadMethod)
    {
        Debug.Log("loading screen activated"); 
        // Show the loading screen
        loadingScreen.SetActive(true);

        // Wait for the specified delay
        yield return new WaitForSeconds(loadingDelay);

        // Invoke the scene loading method
        sceneLoadMethod.Invoke();

        // loadingScreen.SetActive(false);
        Debug.Log("loading screen deactivated"); 

        // Hide the loading screen
        // loadingScreen.SetActive(false);
    }

    void LoadNightwranglerScene()
    {
        Debug.Log("loading nightwrangler game");
        gameDescriptionText.text = "The cowboys are in need of rest after a long days work, but someone needs to stay " +
            "up and watch the horses during the night, and herd them in if necessary. Can you do it? Can you brave the " +
            "cold and be the night wrangler?";
        // loading cutscene scene (level_main for nightwrangler game is loaded in CutscenePlayer.cs) 
        sceneSelection = 2; 
        playBtn.SetActive(true); 
    }

    public static void LoadMenuScene()
    {
        // sceneSelection = 0; 
        SceneManager.LoadScene(0); 
        Debug.Log("loading menu scene");
    }

    void PlayGame()
    {
        
        string sceneSelected = DetermineSceneSelection(sceneSelection);
        //SceneManager.LoadScene(sceneSelected); 
        StartCoroutine(LoadSceneWithDelay(() => SceneManager.LoadScene(sceneSelected)));
        Debug.Log("Play whatever game was selected in here"); 
    }

    string DetermineSceneSelection(int selection)
    {
        switch(selection)
        {
            case 0:
                return "menu";
            case 1:
                return "Level_main";
            case 2:
                return "cutscene";
            default:
                Debug.LogWarning("invalid scene selection");
                return "menu"; 
        }
    }
}

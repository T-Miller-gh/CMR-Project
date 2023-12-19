using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; 
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class menuManager : MonoBehaviour
{
    public delegate void ButtonClickMethod(); 
    Dictionary<string, ButtonClickMethod> buttonDictionay = new Dictionary<string, ButtonClickMethod>();

    public GameObject nightwranglerBtn;
    public GameObject playBtn; 
    public TextMeshProUGUI gameDescriptionText;

    public int sceneSelection = 0;

    // Start is called before the first frame update
    void Start()
    {
        Button nightWranglerPlay = nightwranglerBtn.GetComponent<Button>();
        Button playGameBtn = playBtn.GetComponent<Button>();
        playBtn.SetActive(false);

        buttonDictionay["nightwrangler"] = LoadNightwranglerScene;
        buttonDictionay["play"] = PlayGame; 
        buttonDictionay["menu"] = LoadMenuScene;

        nightWranglerPlay.onClick.AddListener(() => OnButtonClick("nightwrangler"));
        playGameBtn.onClick.AddListener(() => OnButtonClick("play")); 
    }

    //private void Update()
    //{
    //    if (OVRInput.Get(OVRInput.Button.One))
    //    {
    //        Debug.Log("a button was pressed");
    //    }
    //}

    public void OnButtonClick(string buttonName)
    {
        if(buttonDictionay.ContainsKey(buttonName))
        {
            buttonDictionay[buttonName].Invoke(); 
        }
    }

    void LoadNightwranglerScene()
    {
        Debug.Log("loading nightwrangler game");
        gameDescriptionText.text = "Long ago..." + "<br>" + "there were night wranglers";
        sceneSelection = 1; 
        playBtn.SetActive(true); 
    }

    void LoadMenuScene()
    {
        sceneSelection = 0; 
        Debug.Log("loading menu scene");
    }

    void PlayGame()
    {
        string sceneSelected = DetermineSceneSelection(sceneSelection);
        SceneManager.LoadScene(sceneSelected); 
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
            default:
                Debug.LogWarning("invalid scene selection");
                return "menu"; 
        }
    }
}

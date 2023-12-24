using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; 
// using UnityEngine.XR.Interaction.Toolkit;
// using UnityEngine.XR;

public class menuManager : MonoBehaviour
{
    // private XRNode xrNodeleft = XRNode.LeftHand; 
    // private UnityEngine.XR.InputDevice device;
    [SerializeField] private InputActionReference menuInputActionReference; 

    public delegate void ButtonClickMethod(); 
    Dictionary<string, ButtonClickMethod> buttonDictionay = new Dictionary<string, ButtonClickMethod>();

    public GameObject nightwranglerBtn;
    public GameObject playBtn; 
    public TextMeshProUGUI gameDescriptionText;

    public int sceneSelection = 0;

    //void GetDevice()
    //{
    //    // List<UnityEngine.XR.InputDevice> devices = new List<UnityEngine.XR.InputDevice>();
    //    // UnityEngine.XR.InputDevices.GetDevicesAtXRNode(xrNodeleft, devices);
    //    // InputDevices.GetDevicesAtXRNode(xrNodeleft, devices); 

    //    if(devices.Count > 0)
    //    {
    //        device = devices[0];
    //        Debug.Log("Device aquired: " + device.name); 
    //    }
    //    else
    //    {
    //        Debug.LogError("No valid deavice found at XRNODE: " + xrNodeleft);
    //    }
    //    // InputDevices.GetDeviceAtXRNode(xrNodeleft); 
    //}

    //void OnEnable()
    //{
    //    menuInputActionReference.action.started += MenuPressed; 
    //}

    //private void OnDisable()
    //{
    //    menuInputActionReference.action.started -= MenuPressed; 
    //}


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
        }
    }


    //void MenuPressed(InputAction.CallbackContext context)
    //{
    //    // Debug.Log("Menu pressed");
    //    LoadMenuScene(); 
    //}

    void LoadNightwranglerScene()
    {
        Debug.Log("loading nightwrangler game");
        gameDescriptionText.text = "The cowboys are in need of rest after a long days work, but someone needs to stay " +
            "up and watch the horses during the night, and herd them in if necessary. Can you do it? Can you brave the " +
            "cold and be the night wrangler?";
        sceneSelection = 1; 
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

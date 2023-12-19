using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
// using UnityEngine.InputSystem; 
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class menuManager : MonoBehaviour
{
    private XRNode xrNodeleft = XRNode.LeftHand; 
    private UnityEngine.XR.InputDevice device; 


    public delegate void ButtonClickMethod(); 
    Dictionary<string, ButtonClickMethod> buttonDictionay = new Dictionary<string, ButtonClickMethod>();

    public GameObject nightwranglerBtn;
    public GameObject playBtn; 
    public TextMeshProUGUI gameDescriptionText;

    public int sceneSelection = 0;

    void GetDevice()
    {
        List<UnityEngine.XR.InputDevice> devices = new List<UnityEngine.XR.InputDevice>();
        // UnityEngine.XR.InputDevices.GetDevicesAtXRNode(xrNodeleft, devices);
        InputDevices.GetDevicesAtXRNode(xrNodeleft, devices); 

        if(devices.Count > 0)
        {
            device = devices[0];
            Debug.Log("Device aquired: " + device.name); 
        }
        else
        {
            Debug.LogError("No valid deavice found at XRNODE: " + xrNodeleft);
        }
        // InputDevices.GetDeviceAtXRNode(xrNodeleft); 
    }

    void OnEnable()
    {
        if(!device.isValid)
        {
            GetDevice(); 
        }
    }

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

    void Update()
    {
        Debug.Log("update method called"); 

        if(!device.isValid)
        {
            GetDevice();
            Debug.Log("device not valid, try again"); 
        }

        bool menuButton = false; 

        if(device.TryGetFeatureValue(CommonUsages.menuButton, out menuButton) && menuButton)
        {
            Debug.Log("menu button was pressed"); 
        }
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

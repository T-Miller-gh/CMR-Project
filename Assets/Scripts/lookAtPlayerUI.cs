using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class lookAtPlayerUI : MonoBehaviour
{
    public GameObject player;
    public characterManager characterManager;

    public bool timerAtZero = false; 

    void FixedUpdate()
    {
        TextMeshProUGUI horseTimerTxt = GetComponent<TextMeshProUGUI>();

        // get the time left to capture from characterManager (do conversions as well)
        float timeToCapture = characterManager.horseCaptureTimer;
        int timeToCaptureInt = Mathf.FloorToInt(timeToCapture); 

        // make text look/rotate towards player
        transform.rotation = Quaternion.LookRotation(transform.position - player.transform.position);
        horseTimerTxt.text = timeToCaptureInt.ToString();

        if (timerAtZero == true)
        {
            // turn off text until it is needed again
            horseTimerTxt.text = " "; 
        }

    }
}

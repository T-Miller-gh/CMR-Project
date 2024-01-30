using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class modifyFog : MonoBehaviour
{
    public characterManager characterManager;

    // Adjust these via the Inspector in Unity
    [SerializeField] private float startDensity = 0.02f; 
    [SerializeField] private float targetDensity = 0.2f;

    private void Update()
    {
        updateFog(); 
    }

    void updateFog()
    {
        if(characterManager.timerOn)
        {
            float newFogDensity = Mathf.Lerp(startDensity, targetDensity, characterManager.gameProgress);
            RenderSettings.fogDensity = newFogDensity;
            Debug.Log(newFogDensity); 
        }
    }
}

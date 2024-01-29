using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class modifyFog : MonoBehaviour
{
    // Adjust these via the Inspector in Unity
    [SerializeField] private float targetDensity = 0.2f;

    //[Tooltip(2Fade duration in seconds)]
    [SerializeField] private float fadeDuration = 300.0f;

    private bool fogTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!fogTriggered)
        {   
            //So it will only trigger once per play
            fogTriggered = true;
            StartCoroutine(FadeFog());
        }      
    }

    private IEnumerator FadeFog()
    {
        var timePassed = .01f;
        while (timePassed <= fadeDuration)
        {
            //Increase fog from current density to target density over total duration
            var factor = timePassed / fadeDuration;
            RenderSettings.fogDensity = Mathf.Lerp(0.02f, targetDensity, factor);
            timePassed += .001f;

            yield return null;
        }
    }
}

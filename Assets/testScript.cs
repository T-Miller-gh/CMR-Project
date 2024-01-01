using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class testScript : MonoBehaviour
{
    private XRController xrController;

    void Start()
    {
        xrController = GetComponent<XRController>();
    }

    void Update()
    {
        if (xrController.inputDevice.IsPressed(InputHelpers.Button.PrimaryButton, out bool primaryButtonValue) && primaryButtonValue)
        {
            Debug.Log("a btn was pressed on the right"); 
        }
    }
}

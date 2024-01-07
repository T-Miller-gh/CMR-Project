//using Cinemachine;
using Polyphoria;
using UnityEngine;

//[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CinemachineLink : MonoBehaviour
{
    public ModularCharacter Character;

   // private CinemachineVirtualCamera _camera;

    private void OnEnable()
    {
        //_camera = GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        //_camera.Follow = Character.SkeletonAsset.transform;
        //_camera.LookAt = Character.SkeletonAsset.transform;
    }
}
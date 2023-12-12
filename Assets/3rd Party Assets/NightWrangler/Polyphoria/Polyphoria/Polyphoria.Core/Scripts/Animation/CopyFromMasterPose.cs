using UnityEngine;

namespace Polyphoria
{
    /// <summary>
    /// Used by modular characters to initiate mesh baking on startup
    /// </summary>
    public class CopyFromMasterPose : MonoBehaviour
    {
        [SerializeField] private Transform _masterPose;

        private void Awake()
        {
            if (_masterPose == null)
                _masterPose = transform.parent.GetComponentInParent<ModularCharacter>().SkeletonAsset.transform;

            //SkinnedMeshTools.AddSkinnedMeshTo(gameObject, _masterPose.transform, true);
            SkinnedMeshTools.AddSkinnedMeshTo(gameObject, _masterPose.transform, true);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Polyphoria
{
    /// <summary>
    /// A modular part for a character. Saves all information for the baking of the SkinnedMeshRenderers to
    /// allow unbaking of parts
    /// </summary>
    public class ModularCharacterPart : MonoBehaviour
    {
        /// <summary>
        /// A list of all baked objects. These are saved to allow the unbaking of parts
        /// </summary>
        [SerializeField] private List<GameObject> _bakedObjects;

        public List<Texture> SkinMasks;

        /// <summary>
        /// The parent character. Will be autodiscovered for parts that are in the scene at level start and
        /// automatically assigned by script for parts that are added during runtime.
        /// </summary>
        public ModularCharacter ModularCharacter;

        private IEnumerator Start()
        {
            ModularCharacter = GetComponentInParent<ModularCharacter>();
            RegisterTriggers();
            RegisterReceivers();

            yield return new WaitForEndOfFrame();
            Bake();
            ModularCharacter.RebuildNextUpdate = true;
        }

        public void RegisterReceivers()
        {
            string[] blendShapeReceivers = GetComponentsInChildren<ModularCharacterElement>().SelectMany(e => e.BlendShapeReceivers).Distinct().ToArray();
            foreach (string receiver in blendShapeReceivers)
            {
                ModularCharacter.RegisterReceiver(receiver, this);
            }
        }

        public void RegisterTriggers()
        {
            string[] blendShapeTriggers = GetComponentsInChildren<ModularCharacterElement>().SelectMany(e => e.BlendShapeTriggers).Distinct().ToArray();
            foreach (string trigger in blendShapeTriggers)
            {
                ModularCharacter.RegisterTrigger(trigger, this);
            }
        }

        public SkinnedMeshRenderer[] SKRenderers
        {
            get
            {
                if (Application.isPlaying)
                {
                    return _bakedObjects
                        .SelectMany(c => c.GetComponentsInChildren<SkinnedMeshRenderer>())
                        .Where(s => s.sharedMesh != null).ToArray();
                }

                return GetComponentsInChildren<SkinnedMeshRenderer>();
            }
        }

        public void UnregisterTriggers()
        {
            string[] blendShapeTriggers = GetComponentsInChildren<ModularCharacterElement>()
                .SelectMany(e => e.BlendShapeTriggers).Distinct().ToArray();
            foreach (string trigger in blendShapeTriggers)
            {
                ModularCharacter.UnregisterTrigger(trigger, this);
            }
        }

        public void UnregisterReceivers()
        {
            string[] blendShapeReceivers = GetComponentsInChildren<ModularCharacterElement>()
                .SelectMany(e => e.BlendShapeReceivers).Distinct().ToArray();
            foreach (string receiver in blendShapeReceivers)
            {
                ModularCharacter.UnregisterReceiver(receiver, this);
            }
        }
        
        public void Bake()
        {
            SkinMasks = new List<Texture>();
            ModularCharacter character = transform.GetComponentInParent<ModularCharacter>();

            if (character.SkeletonAsset == null)
            {
                Debug.LogError($"[Polyphoria] Baking precheck failed: Missing SkeletonAsset in {character.gameObject.name}");
                return;
            }

            _bakedObjects = SkinnedMeshTools.AddSkinnedMeshTo(gameObject, character.SkeletonAsset.transform, true);
            

            foreach (GameObject obj in _bakedObjects)
            {
                foreach (ModularCharacterElement sk in obj.GetComponentsInChildren<ModularCharacterElement>(true))
                {
                    SkinMasks.AddRange(sk.SkinMasks);
                }
            }
        }

        public void Unbake()
        {
            if (_bakedObjects != null)
            {
                foreach (GameObject bakedObject in _bakedObjects)
                {
                    DestroyImmediate(bakedObject);
                }
                _bakedObjects.Clear();
            }
        }
    }
}
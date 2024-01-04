using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Polyphoria
{
    /// <summary>
    /// Container object for modular characters.
    /// </summary>
    [SelectionBase]
    public class ModularCharacter : MonoBehaviour
    {
        public GameObject ElementParentPrefab;
        public GameObject SkeletonAsset;

        /// <summary>
        /// Contains a list of all modular parts this character contains. This list is used for
        /// runtime modifications as well as some editor scripts. This list should not be modified
        /// directly, unless you know what you are doing.
        /// </summary>
        public List<ModularCharacterPart> Parts;

        public Dictionary<string, List<ModularCharacterPart>> BlendShapeTriggers;
        public Dictionary<string, List<ModularCharacterPart>> BlendShapeReceivers;

        /// <summary>
        /// If this is set to true, the opacity masks and blend shapes will be refreshed in the next update. Is automatically
        /// set to true when attaching new parts or once after PlayMode started and initial bake completed
        /// </summary>
        public bool RebuildNextUpdate;

        private void Awake()
        {
            BlendShapeTriggers = new Dictionary<string, List<ModularCharacterPart>>();
            BlendShapeReceivers = new Dictionary<string, List<ModularCharacterPart>>();

            if(!SkeletonAsset.scene.IsValid())
                SpawnSkeleton(SkeletonAsset);
        }

        public void SpawnSkeleton(GameObject prefab)
        {
            if (SkeletonAsset != null && SkeletonAsset.scene.IsValid())
            {
                DestroyImmediate(SkeletonAsset);
                SkeletonAsset = null;
            }
            if (prefab != null)
            {
                SkeletonAsset = Instantiate(prefab, transform);
                SkeletonAsset.gameObject.name = "Skeleton";
            }
        }


        private void Update()
        {
            if (RebuildNextUpdate)
            {
                RebuildNextUpdate = false;
                ApplyBlendshapes();
                BakeOpacityMaps();
            }
        }

        /// <summary>
        /// Attaches a new part to this modular character. Is used by editor scripts and can be used for runtime
        /// manipulation of the character. The prefab has to contain a ModularCharacterElement component.
        /// </summary>
        /// <param name="prefab"></param>
        public void AttachPart(GameObject prefab)
        {
            ModularCharacterElement element = prefab.GetComponent<ModularCharacterElement>();
            if (element == null)
            {
                Debug.LogError($"[Polyphoria] The object {prefab.name} is not a valid ModularCharacterElement");
                return;
            }

            GameObject parent;
            #if UNITY_EDITOR
                parent = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(ElementParentPrefab, transform);
            #else
                parent = Instantiate(ElementParentPrefab, transform);
            #endif
            parent.transform.localPosition = Vector3.zero;
            parent.transform.localRotation = Quaternion.identity;
            parent.transform.localScale = Vector3.one;
            parent.name = $"Part - {prefab.name}";
            ModularCharacterPart part = parent.GetComponent<ModularCharacterPart>();
            if (part == null)
            {
                Debug.LogError($"[Polyphoria] The ElementParentPrefab is invalid");
                return;
            }

            Parts.Add(part);

            GameObject newComponent;
            #if UNITY_EDITOR
                newComponent = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab, parent.transform);
            #else
                newComponent = Instantiate(prefab, parent.transform);
            #endif

            newComponent.transform.localPosition = Vector3.zero;
            newComponent.transform.localRotation = Quaternion.identity;
            newComponent.transform.localScale = Vector3.one;
            newComponent.name = prefab.name;
            if (Application.isPlaying)
            {
                part.Bake();
                BakeOpacityMaps();
                part.ModularCharacter = this;
                part.RegisterTriggers();
                part.RegisterReceivers();
                ApplyBlendshapes();
                _cachedAnimator = null;
            }
        }

        private void BakeOpacityMaps()
        {
            RenderTexture rtA = new RenderTexture(512, 512, 32);
            RenderTexture rtB = new RenderTexture(512, 512, 32);
            RenderTexture.active = rtA;
            GL.Clear(true, true, Color.white);
            RenderTexture.active = null;

            Material m = new Material(Shader.Find("Hidden/Polyphoria/OpacityBaker"));

            foreach (ModularCharacterElement part in GetComponentsInChildren<ModularCharacterElement>(true))
            {
                foreach (Texture2D skinMask in part.SkinMasks)
                {
                    m.SetTexture("_MapA", rtA);
                    m.SetTexture("_MapB", skinMask);

                    Graphics.Blit(rtA, rtB, m);
                    RenderTexture tmp = rtA;
                    rtA = rtB;
                    rtB = tmp;
                }
            }

            foreach(ModularCharacterElement part in GetComponentsInChildren<ModularCharacterElement>(true))
            {
                foreach (Renderer r in part.GetComponentsInChildren<Renderer>())
                {
                    foreach (Material mat in r.materials)
                    {
                        if (mat.HasProperty("_SkinMask"))
                        {
                            mat.SetTexture("_SkinMask", rtA);
                        }
                    }
                }
            }
            Debug.Log("[Polyphoria] Skin baking completed");
        }

        /// <summary>
        /// Removes a baked part from a modular character. This function is used by some editor scripts, but can also
        /// be used to modify the parts on runtime.
        /// </summary>
        /// <param name="obj"></param>
        public void RemovePart(GameObject obj)
        {
            ModularCharacterPart part = obj.GetComponent<ModularCharacterPart>();
            if (part == null)
            {
                Debug.LogError($"[Polyphoria] {obj.name} is not Modular Character Part");
                return;
            }

            part.Unbake();
            Parts.Remove(part);

            if (Application.isPlaying)
            {
                part.UnregisterReceivers();
                part.UnregisterTriggers();
                ApplyBlendshapes();
            }

            DestroyImmediate(obj);
        }

        private void UnbakeAll()
        {
            for (int i = Parts.Count - 1; i >= 0; i--)
            {
                Parts[i].Unbake();
            }
        }

        private void BakeAll()
        {
            for (int i = Parts.Count - 1; i >= 0; i--)
            {
                Parts[i].Bake();
            }
        }

        public void ChangeSkeleton(GameObject skeletonAsset)
        {
            if (!Application.isPlaying)
            {
                Debug.Log("Changing Skeletons using this method is only required during runtime (after baking of all parts)");
                return;
            }

            UnbakeAll();

            if (SkeletonAsset != null)
                DestroyImmediate(SkeletonAsset);

            SpawnSkeleton(skeletonAsset);
            BakeAll();
            _cachedAnimator = null;
        }

        public void Clear()
        {
            for (int i = Parts.Count - 1; i >= 0; i--)
            {
                RemovePart(Parts[i].gameObject);
            }
        }

        private Animator _cachedAnimator;

        private void LateUpdate()
        {
            TransferSkeletonMotion();
        }

        private void TransferSkeletonMotion()
        {
            if (_cachedAnimator == null || !_cachedAnimator.gameObject.activeInHierarchy)
                _cachedAnimator = GetComponentInChildren<Animator>();
            Vector3 world = _cachedAnimator.transform.position;
            transform.position = world;
            _cachedAnimator.transform.localPosition = Vector3.zero;
        }

        public void RegisterTrigger(string trigger, ModularCharacterPart part)
        {
            if (!BlendShapeTriggers.ContainsKey(trigger))
            {
                BlendShapeTriggers.Add(trigger, new List<ModularCharacterPart>());
            }

            BlendShapeTriggers[trigger].Add(part);
            ApplyBlendshapes();
        }

        public void RegisterReceiver(string receiver, ModularCharacterPart part)
        {
            if (!BlendShapeReceivers.ContainsKey(receiver))
            {
                BlendShapeReceivers.Add(receiver, new List<ModularCharacterPart>());
            }

            BlendShapeReceivers[receiver].Add(part);
            ApplyBlendshapes();
        }

        public void ApplyBlendshapes()
        {
            if (BlendShapeReceivers == null)    // During Edit-Mode
                return;

            foreach (KeyValuePair<string, List<ModularCharacterPart>> receiver in BlendShapeReceivers)
            {
                foreach (ModularCharacterPart part in receiver.Value)
                {
                    foreach (SkinnedMeshRenderer sk in part.SKRenderers)
                    {
                        for (int i = 0; i < sk.sharedMesh.blendShapeCount; i++)
                        {
                            if (sk.sharedMesh.GetBlendShapeName(i).EndsWith(receiver.Key))
                            {
                                sk.SetBlendShapeWeight(i, BlendShapeTriggers.ContainsKey(receiver.Key) ? 100f : 0);
                            }
                        }
                    }
                }
            }
        }

        public void UnregisterTrigger(string trigger, ModularCharacterPart part)
        {
            BlendShapeTriggers[trigger].Remove(part);
            if (!BlendShapeTriggers[trigger].Any())
            {
                BlendShapeTriggers.Remove(trigger);
            }

            ApplyBlendshapes();
        }

        public void UnregisterReceiver(string receiver, ModularCharacterPart part)
        {
            BlendShapeReceivers[receiver].Remove(part);
            if (!BlendShapeReceivers[receiver].Any())
            {
                BlendShapeReceivers.Remove(receiver);
            }
            ApplyBlendshapes();
        }

        public bool IsSlotFree(PartSlot slot)
        {
            return Parts.Select(p => p.GetComponent<BakedPartReferences>())
                .All(p => p.Element.Slot != slot);
        }

        public BakedPartReferences GetSlotPart(PartSlot slot)
        {
            return Parts
                .Select(p => p.GetComponent<BakedPartReferences>())
                .FirstOrDefault(p => p.Element.Slot == slot);
        }

        public void RemoveSlotPart(PartSlot slot)
        {
            BakedPartReferences part = GetSlotPart(slot);
            if (part != null)
            {
                RemovePart(part.gameObject);
            }
        }
    }
}
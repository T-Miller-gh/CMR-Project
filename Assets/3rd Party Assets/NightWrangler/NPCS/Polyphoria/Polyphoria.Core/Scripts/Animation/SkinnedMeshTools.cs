using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Polyphoria
{
    /// <summary>
    /// Helper class to merge multiple SkinnedMeshRenderer sharing the same rig to enable usage of a single animator
    /// </summary>
    public static class SkinnedMeshTools
    {
        private static void ApplyFromMasterPose(SkinnedMeshRenderer master, SkinnedMeshRenderer child)
        {
            Dictionary<string, Transform> bones = master.bones.ToDictionary(bone => bone.name, bone => bone);
            List<Transform> list = new List<Transform>();
            for (int i = 0; i < child.bones.Length; i++)
            {
                if (!bones.ContainsKey(child.bones[i].name))
                {
                    Debug.LogError($"[Polyphoria] Could not retrieve bone (Target: {child.name}, Bone: {child.bones[i].name})");
                }
                else
                {
                    list.Add(bones[child.bones[i].name]);
                }
            }

            child.bones = list.ToArray();
        }

        public static List<GameObject> AddSkinnedMeshTo(GameObject obj, Transform root, bool hideFromObj)
        {
            SkinnedMeshRenderer[] bonedObjects = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
            List<GameObject> result = bonedObjects.Select(smr => ProcessBonedObject(smr, root)).ToList();
            if (hideFromObj)
                obj.SetActive(false);

            return result;
        }

        private static GameObject ProcessBonedObject(SkinnedMeshRenderer thisRenderer, Transform root)
        {
            GameObject newObject = new GameObject(thisRenderer.gameObject.name);
            newObject.transform.parent = root;
            SkinnedMeshRenderer newRenderer = newObject.AddComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
            Transform[] myBones = new Transform[thisRenderer.bones.Length];
            for (int i = 0; i < thisRenderer.bones.Length; i++)
                myBones[i] = FindChildByName(thisRenderer.bones[i].name, root);

            newRenderer.bones = myBones;
            newRenderer.sharedMesh = thisRenderer.sharedMesh;
            newRenderer.materials = thisRenderer.materials;

            return newObject;
        }

        private static Transform FindChildByName(string thisName, Transform thisGObj)
        {
            if (thisGObj.name == thisName)
                return thisGObj.transform;

            foreach (Transform child in thisGObj)
            {
                Transform returnObj = FindChildByName(thisName, child);

                if (returnObj != null)
                    return returnObj;
            }

            return null;
        }
    }
}
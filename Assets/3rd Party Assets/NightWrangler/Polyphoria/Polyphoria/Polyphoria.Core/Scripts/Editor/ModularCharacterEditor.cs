using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEditorHelper;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Polyphoria
{
    [CustomEditor(typeof(ModularCharacter))]
    public class ModularCharacterEditor : Editor
    {
        private SerializedProperty _elementParentProperty;
        private SerializedProperty _skeletonAssetProperty;

        private void OnEnable()
        {
            _elementParentProperty = serializedObject.FindProperty(nameof(ModularCharacter.ElementParentPrefab));
            if (_elementParentProperty.objectReferenceValue == null)
            {
                string asset = AssetDatabase.FindAssets("l:Polyphoria_ElementParent").FirstOrDefault();
                if (asset == null)
                {
                    Debug.LogWarning($"[Polyphoria] Could not locate ElementParent prefab. Assign manually or ensure file is present and labelled with \"Polyphoria_ElementParent\"");
                }
                else
                {
                    _elementParentProperty.objectReferenceValue = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(asset));
                    serializedObject.ApplyModifiedProperties();
                }
            }
            _skeletonAssetProperty = serializedObject.FindProperty(nameof(ModularCharacter.SkeletonAsset));
        }

        public override void OnInspectorGUI()
        {
            ModularCharacter character = target as ModularCharacter;
            GUI.enabled = false;
            EditorGUILayout.PropertyField(_elementParentProperty);
            GUI.enabled = true;

            GameObject oldSkeleton = character.SkeletonAsset;
            GameObject newSkeleton = (GameObject)EditorGUILayout.ObjectField("Skeleton Asset",
                character.SkeletonAsset, typeof(GameObject), true);

            if (oldSkeleton != newSkeleton)
            {
                if (Application.isPlaying)
                {
                    character.ChangeSkeleton(newSkeleton);
                }
                else
                {
                    character.SkeletonAsset = newSkeleton;
                }
            }

            try
            {
                if (character == null)
                {
                    EditorGUILayout.HelpBox("Internal error. Target is not a ModularCharacter", MessageType.Error,
                        true);
                    return;
                }

                if (character.Parts == null)
                    return;


                EditorGUILayout.LabelField("Modular Parts", EditorStyles.boldLabel);
                using (new IndentBlock())
                {
                    if (character.Parts.Count == 0)
                    {
                        EditorGUILayout.LabelField("No parts attached");
                    }
                    else
                    {
                        character.Parts.RemoveAll(p => p == null);
                        for (int index = 0; index < character.Parts.Count; index++)
                        {
                            ModularCharacterPart part = character.Parts[index];
                            using (new EditorBlock(EditorBlock.Orientation.Horizontal))
                            {
                                EditorGUILayout.LabelField(part.name);
                                if (GUILayout.Button("Find", GUILayout.Width(40)))
                                {
                                    GameObject prefabInstance = PrefabUtility.GetNearestPrefabInstanceRoot(part.transform.GetChild(0));
                                    if (prefabInstance != null)
                                    {
                                        string prefabAssetPath =
                                            PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefabInstance);
                                        if (!string.IsNullOrEmpty(prefabAssetPath))
                                            EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(prefabAssetPath));
                                    }
                                }

                                if (GUILayout.Button("Remove", GUILayout.Width(70)))
                                {
                                    if (PrefabUtility.IsPartOfAnyPrefab(character))
                                    {
                                        if (!EditorUtility.DisplayDialog("Remove character part",
                                            "This character is part of a prefab and thus cannot be modified directly. Do you want to remove the prefab connection? (if not, edit the prefab itself by opening it in prefab mode)",
                                            "Yes", "No"))
                                        {
                                            return;
                                        }
                                        else
                                        {
                                            PrefabUtility.UnpackPrefabInstance(character.gameObject,
                                                PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
                                        }
                                    }

                                    part.Unbake();
                                    character.RemovePart(part.gameObject);
                                    character.ApplyBlendshapes();
                                    EditorUtility.SetDirty(character);
                                    EditorUtility.SetDirty(character.gameObject);
                                    break;
                                }
                            }
                        }
                    }
                }

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Drag modular prefab here to attach");
                using (new IndentBlock())
                {
                    GameObject go = (GameObject) EditorGUILayout.ObjectField(null, typeof(GameObject), false);
                    if (go != null)
                    {
                        if (!PrefabUtility.IsPartOfAnyPrefab(go))
                        {
                            Debug.LogError($"[Polyphoria] A prefab is required for baking modular characters");
                            return;
                        }

                        if (go.GetComponent<ModularCharacterElement>() == null)
                        {
                            Debug.LogError($"[Polyphoria] The prefab must contain a ModularCharacterElement component");
                            return;
                        }

                        character.AttachPart(go);
                        EditorUtility.SetDirty(character);
                        EditorUtility.SetDirty(character.gameObject);
                    }
                }
            }
            finally
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
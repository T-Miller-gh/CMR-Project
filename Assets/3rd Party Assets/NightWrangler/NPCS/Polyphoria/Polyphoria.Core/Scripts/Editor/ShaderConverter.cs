using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditorHelper;
using UnityEngine;

namespace Polyphoria
{
    [Serializable]
    public abstract class MaterialConverterPreset
    {
        public string Name;
        public Shader ShaderBase;
        public Shader ShaderComplex;
        public Shader ShaderSkin;
        public Shader ShaderHair;

        public List<ShaderMapping> MappingsBase;
        public List<ShaderMapping> MappingsComplex;
        public List<ShaderMapping> MappingsSkin;
        public List<ShaderMapping> MappingsHair;
    }

    public class PolyphoriaHDRP : MaterialConverterPreset
    {
        public PolyphoriaHDRP()
        {
            Name = "Polyphoria HDRP";
            ShaderBase = Shader.Find("Shader Graphs/SG_Base_HDRP");
            ShaderComplex = Shader.Find("Shader Graphs/SG_Base_Complex_HDRP");
            ShaderHair = Shader.Find("HDRP/Lit");
            ShaderSkin = Shader.Find("M_Base_Skin_HDRP");
            MappingsBase = new List<ShaderMapping>
            {
                new ShaderMapping("_MainTex", "Albedo", ShaderMappingType.Texture)
            };
            MappingsComplex = new List<ShaderMapping>();
            MappingsSkin = new List<ShaderMapping>();
            MappingsHair = new List<ShaderMapping>();
        }
    }

    public class PolyphoriaNoSRP : MaterialConverterPreset
    {
        public PolyphoriaNoSRP()
        {
            Name = "Polyphoria (No SRP)";
            ShaderBase = Shader.Find("M_Base");
            ShaderComplex = Shader.Find("M_Base_Complex");
            ShaderHair = Shader.Find("M_Hair");
            ShaderSkin = Shader.Find("M_Base_Skin");

            MappingsBase = new List<ShaderMapping>();
            MappingsComplex = new List<ShaderMapping>();
            MappingsSkin = new List<ShaderMapping>();
            MappingsHair = new List<ShaderMapping>();
        }
    }

    public enum ShaderMappingType { Texture, Float, Color, Int, Vector }

    [Serializable]
    public class ShaderMapping
    {
        public string InputProperty;
        public string OutputProperty;
        public ShaderMappingType MappingType;

        public ShaderMapping(string inputProperty, string outputProperty, ShaderMappingType mappingType)
        {
            InputProperty = inputProperty;
            OutputProperty = outputProperty;
            MappingType = mappingType;
        }
    }

    public class ShaderValueCache
    {
        public Dictionary<string, Texture> Textures;
        public Dictionary<string, float> Floats;
        public Dictionary<string, Color> Colors;
        public Dictionary<string, int> Integers;
        public Dictionary<string, Vector4> Vectors;

        public ShaderValueCache()
        {
            Textures = new Dictionary<string, Texture>();
            Floats = new Dictionary<string, float>();
            Colors = new Dictionary<string, Color>();
            Integers = new Dictionary<string, int>();
            Vectors = new Dictionary<string, Vector4>();
        }

        public void Clear()
        {
            Textures.Clear();
            Floats.Clear();
            Colors.Clear();
            Integers.Clear();
            Vectors.Clear();
        }
    }

    public class ShaderConverter : EditorWindow
    {
        public Shader ShaderBase;
        public Shader ShaderComplex;
        public Shader ShaderSkin;
        public Shader ShaderHair;

        private string _corePath;

        public bool UseBaseMappings;
        public List<ShaderMapping> MappingsBase;
        public bool UseComplexMappings;
        public List<ShaderMapping> MappingsComplex;
        public bool UseHairMappings;
        public List<ShaderMapping> MappingsHair;
        public bool UseSkinMappings;
        public List<ShaderMapping> MappingsSkin;

        [MenuItem("Tools/Polyphoria/Material Converter")]
        private static void Init()
        {
            GetWindow<ShaderConverter>().Show();
        }

        private void OnEnable()
        {
            string path = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            _corePath = Regex.Match(path, @"(.*Polyphoria\.Core)").Groups[1].Value;
        }

        private void OnGUI()
        {
            Rect r = EditorGUILayout.GetControlRect(false, 32);
            EditorHelper.GUIDrawRect(r, Color.black);
            r.width = 256;

            GUI.DrawTexture(r,
                AssetDatabase.LoadAssetAtPath<Texture>(_corePath + "/Scripts/Editor/MaterialConverter.png"));

            DrawShaderFields();
            DrawPresets();
            DrawActions();
            DrawMappings();
        }

        private void DrawMappings()
        {
            void DrawSection(string name, ref bool useMappings, List<ShaderMapping> shaderMappings)
            {
                useMappings = EditorGUILayout.Toggle($"Customize ({name})", useMappings);
                if (useMappings)
                {
                    foreach (ShaderMapping mapping in shaderMappings)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("\u2192 In", GUILayout.Width(40));
                            mapping.InputProperty = EditorGUILayout.TextField(mapping.InputProperty);
                            EditorGUILayout.LabelField("\u2192 Out", GUILayout.Width(40));
                            mapping.OutputProperty = EditorGUILayout.TextField(mapping.OutputProperty);
                            mapping.MappingType = (ShaderMappingType) EditorGUILayout.EnumPopup(mapping.MappingType);
                            if (GUILayout.Button("X", GUILayout.Width(20)))
                            {
                                shaderMappings.Remove(mapping);
                                break;
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    if (GUILayout.Button("Add Mapping"))
                    {
                        shaderMappings.Add(new ShaderMapping("", "", ShaderMappingType.Texture));
                    }
                }
            }

            GUILayout.Label("Custom Mappings", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("You can use custom mappings to allow the Material Converter to copy over shader property values, even if the properties are named differently. Use this to allow a conversion for you own shader variants. This setting is not needed if you work with the Polyphoria shaders", MessageType.Info);
            DrawSection("Base", ref UseBaseMappings, MappingsBase);
            DrawSection("Complex", ref UseComplexMappings, MappingsComplex);
            DrawSection("Skin", ref UseSkinMappings, MappingsSkin);
            DrawSection("Hair", ref UseHairMappings, MappingsHair);
        }

        private void DrawActions()
        {
            List<Material> baseMaterials = new List<Material>();
            List<Material> complexMaterials = new List<Material>();
            List<Material> skinMaterials = new List<Material>();
            List<Material> hairMaterials = new List<Material>();

            GUILayout.Label("Actions", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                if (GUILayout.Button("Convert Selected Materials"))
                {
                    List<Material> selectedAssets = Selection.assetGUIDs
                        .Select(ag =>
                            AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(ag), typeof(Material)))
                        .Where(a => a != null)
                        .OfType<Material>()
                        .ToList();

                    if (selectedAssets.Count > 30 && !EditorUtility.DisplayDialog("Polyphoria Batch Conversion",
                            "This conversion might take a while. Do wou want to continue?", "Start Conversion",
                            "Cancel"))
                        return;

                    baseMaterials.AddRange(selectedAssets.Where(a => AssetDatabase.GetLabels(a).Contains("Polyphoria_Base")));
                    complexMaterials.AddRange(selectedAssets.Where(a => AssetDatabase.GetLabels(a).Contains("Polyphoria_Complex")));
                    skinMaterials.AddRange(selectedAssets.Where(a => AssetDatabase.GetLabels(a).Contains("Polyphoria_Skin")));
                    hairMaterials.AddRange(selectedAssets.Where(a => AssetDatabase.GetLabels(a).Contains("Polyphoria_Hair")));
                    ConvertMaterials(baseMaterials, complexMaterials, skinMaterials, hairMaterials);
                }

                if (GUILayout.Button("Convert all Polyphoria Materials"))
                {
                    if (!EditorUtility.DisplayDialog("Polyphoria Batch Conversion",
                        "This conversion might take a while. Do wou want to continue?", "Start Conversion", "Cancel"))
                        return;
                    baseMaterials.AddRange(AssetDatabase.FindAssets("l:Polyphoria_Base").Select(AssetDatabase.LoadAssetAtPath<Material>));
                    complexMaterials.AddRange(AssetDatabase.FindAssets("l:Polyphoria_Complex").Select(AssetDatabase.LoadAssetAtPath<Material>));
                    skinMaterials.AddRange(AssetDatabase.FindAssets("l:Polyphoria_Skin").Select(AssetDatabase.LoadAssetAtPath<Material>));
                    hairMaterials.AddRange(AssetDatabase.FindAssets("l:Polyphoria_Hair").Select(AssetDatabase.LoadAssetAtPath<Material>));
                    ConvertMaterials(baseMaterials, complexMaterials, skinMaterials, hairMaterials);
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void ConvertMaterials(List<Material> baseMaterials, List<Material> complexMaterials, List<Material> skinMaterials, List<Material> hairMaterials)
        {
            try
            {
                int total = baseMaterials.Count + complexMaterials.Count + skinMaterials.Count + hairMaterials.Count;
                float current = 0;

                ShaderValueCache cache = new ShaderValueCache();

                EditorUtility.DisplayProgressBar("Polyphoria Batch Conversion", "Converting Materials", 0);
                foreach (Material baseMaterial in baseMaterials)
                {
                    if (UseBaseMappings) SaveProperties(cache, baseMaterial, MappingsBase);
                    baseMaterial.shader = ShaderBase;
                    if (UseBaseMappings) ApplyProperties(cache, baseMaterial);
                    EditorUtility.DisplayProgressBar("Polyphoria Batch Conversion", "Converting Materials", ++current / total);
                }

                foreach (Material complexMaterial in complexMaterials)
                {
                    if (UseComplexMappings) SaveProperties(cache, complexMaterial, MappingsComplex);
                    complexMaterial.shader = ShaderComplex;
                    if (UseComplexMappings) ApplyProperties(cache, complexMaterial);
                    EditorUtility.DisplayProgressBar("Polyphoria Batch Conversion", "Converting Materials", ++current / total);
                }

                foreach (Material skinMaterial in skinMaterials)
                {
                    if (UseSkinMappings) SaveProperties(cache, skinMaterial, MappingsSkin);
                    skinMaterial.shader = ShaderSkin;
                    if (UseSkinMappings) ApplyProperties(cache, skinMaterial);
                    EditorUtility.DisplayProgressBar("Polyphoria Batch Conversion", "Converting Materials", ++current / total);
                }

                foreach (Material hairMaterial in hairMaterials)
                {
                    if (UseHairMappings) SaveProperties(cache, hairMaterial, MappingsHair);
                    hairMaterial.shader = ShaderHair;
                    if (UseHairMappings) ApplyProperties(cache, hairMaterial);
                    EditorUtility.DisplayProgressBar("Polyphoria Batch Conversion", "Converting Materials", ++current / total);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private void SaveProperties(ShaderValueCache cache, Material material, List<ShaderMapping> mappings)
        {
            foreach (ShaderMapping mapping in mappings.Where(m => material.HasProperty(m.InputProperty)))
            {
                switch (mapping.MappingType)
                {
                    case ShaderMappingType.Texture:
                        Texture val = material.GetTexture(mapping.InputProperty);
                        if(val != null)
                            cache.Textures.Add(mapping.OutputProperty, val);
                        break;
                    case ShaderMappingType.Float:
                        float floatVal = material.GetFloat(mapping.InputProperty);
                        cache.Floats.Add(mapping.OutputProperty, floatVal);
                        break;
                    case ShaderMappingType.Color:
                        Color col = material.GetColor(mapping.InputProperty);
                        cache.Colors.Add(mapping.OutputProperty, col);
                        break;
                    case ShaderMappingType.Int:
                        int i = material.GetInt(mapping.InputProperty);
                        cache.Integers.Add(mapping.OutputProperty, i);
                        break;
                    case ShaderMappingType.Vector:
                        Vector4 v = material.GetVector(mapping.InputProperty);
                        cache.Vectors.Add(mapping.OutputProperty, v);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void ApplyProperties(ShaderValueCache cache, Material material)
        {
            foreach (KeyValuePair<string, Texture> cacheTexture in cache.Textures.Where(cacheTexture => material.HasProperty(cacheTexture.Key)))
            {
                material.SetTexture(cacheTexture.Key, cacheTexture.Value);
            }
            foreach (KeyValuePair<string, Color> cacheColor in cache.Colors.Where(cacheColor => material.HasProperty(cacheColor.Key)))
            {
                material.SetColor(cacheColor.Key, cacheColor.Value);
            }
            foreach (KeyValuePair<string, float> cacheFloat in cache.Floats.Where(cacheFloat => material.HasProperty(cacheFloat.Key)))
            {
                material.SetFloat(cacheFloat.Key, cacheFloat.Value);
            }
            foreach (KeyValuePair<string, int> cacheInteger in cache.Integers.Where(cacheInteger => material.HasProperty(cacheInteger.Key)))
            {
                material.SetInt(cacheInteger.Key, cacheInteger.Value);
            }
            foreach (KeyValuePair<string, Vector4> cacheVector in cache.Vectors.Where(cacheVector => material.HasProperty(cacheVector.Key)))
            {
                material.SetVector(cacheVector.Key, cacheVector.Value);
            }

            cache.Clear();
        }

        private void DrawShaderFields()
        {
            GUILayout.Label("Target Shader", EditorStyles.boldLabel);
            ShaderBase = (Shader)EditorGUILayout.ObjectField("Base", ShaderBase, typeof(Shader), false);
            ShaderComplex = (Shader)EditorGUILayout.ObjectField("Complex", ShaderComplex, typeof(Shader), false);
            ShaderSkin = (Shader)EditorGUILayout.ObjectField("Skin", ShaderSkin, typeof(Shader), false);
            ShaderHair = (Shader)EditorGUILayout.ObjectField("Hair", ShaderHair, typeof(Shader), false);
        }

        private static IEnumerable<T> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class
        {
            return Assembly.GetAssembly(typeof(T)).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))).Select(type => (T)Activator.CreateInstance(type, constructorArgs)).ToList();
        }

        private void DrawPresets()
        {
            GUILayout.Label("Presets", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                foreach (MaterialConverterPreset preset in GetEnumerableOfType<MaterialConverterPreset>())
                {
                    if (GUILayout.Button(preset.Name))
                    {
                        ShaderBase = preset.ShaderBase;
                        ShaderComplex = preset.ShaderComplex;
                        ShaderHair = preset.ShaderHair;
                        ShaderSkin = preset.ShaderSkin;
                        UseBaseMappings = preset.MappingsBase != null && preset.MappingsBase.Count > 0;
                        UseComplexMappings = preset.MappingsComplex != null && preset.MappingsComplex.Count > 0;
                        UseSkinMappings = preset.MappingsSkin != null && preset.MappingsSkin.Count > 0;
                        UseHairMappings = preset.MappingsHair != null && preset.MappingsHair.Count > 0;
                        MappingsBase = preset.MappingsBase;
                        MappingsComplex = preset.MappingsComplex;
                        MappingsHair = preset.MappingsHair;
                        MappingsSkin = preset.MappingsSkin;
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
}
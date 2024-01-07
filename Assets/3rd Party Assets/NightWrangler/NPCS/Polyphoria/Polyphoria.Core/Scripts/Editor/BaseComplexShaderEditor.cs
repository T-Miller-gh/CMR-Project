using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BaseComplexShaderEditor : ShaderGUI
{
    public enum Page { General, Tinting, Pattern }

    private Page _currentPage;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        Material target = materialEditor.target as Material;

        EditorGUILayout.BeginHorizontal();
        foreach (Page page in Enum.GetValues(typeof(Page)))
        {
            if (GUILayout.Toggle(_currentPage == page, page.ToString("G"), "Button"))
            {
                _currentPage = page;
            }
        }
        EditorGUILayout.EndHorizontal();

        switch (_currentPage)
        {
            case Page.General:
                DrawGeneralPage(target, materialEditor, properties);
                break;
            case Page.Tinting:
                DrawTintingPage(target, materialEditor, properties);
                break;
            case Page.Pattern:
                DrawPatternPage(target, materialEditor, properties);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void DrawGeneralPage(Material material, MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        materialEditor.TextureProperty(FindProperty("_Albedo", properties), "Albedo");
        materialEditor.TextureProperty(FindProperty("_MSO", properties), "ORM (Occlusion (R), Roughness (G), Metallic (M)");
        materialEditor.TextureProperty(FindProperty("_Normal", properties), "Normal");
        materialEditor.TextureProperty(FindProperty("_OpacityMap", properties), "Opacity");
    }

    private void DrawTintingPage(Material material, MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        materialEditor.TexturePropertySingleLine(new GUIContent(material.IsKeywordEnabled("_USECOMPLEXTINTMASK") ? "TintMask (RGBA+CMY)" : "TintMask (RGBA)"), FindProperty("_TintMask", properties));

        bool isComplexMask = EditorGUILayout.ToggleLeft(new GUIContent("Use complex TintMask", "Use 7 Channels instead of 4. Slightly increased shader complexity"), material.IsKeywordEnabled("_USECOMPLEXTINTMASK"));
        if (isComplexMask)
            material.EnableKeyword("_USECOMPLEXTINTMASK");
        else
            material.DisableKeyword("_USECOMPLEXTINTMASK");
        EditorGUILayout.Space(15);
        EditorGUILayout.LabelField("Tint Values", EditorStyles.boldLabel);
        materialEditor.ColorProperty(FindProperty("_RPrimary", properties), "Red (Primary)");
        materialEditor.ColorProperty(FindProperty("_GDetails1", properties), "Green");
        materialEditor.ColorProperty(FindProperty("_BDetails2", properties), "Blue");
        materialEditor.ColorProperty(FindProperty("_ADetails3Skin", properties), "Alpha");
        if (isComplexMask)
        {
            materialEditor.ColorProperty(FindProperty("_C", properties), "Cyan");
            materialEditor.ColorProperty(FindProperty("_M", properties), "Magenta");
            materialEditor.ColorProperty(FindProperty("_Y", properties), "Yellow");
        }

        EditorGUILayout.Space(15);
        EditorGUILayout.LabelField("Smoothness Multipliers", EditorStyles.boldLabel);
        materialEditor.FloatProperty(FindProperty("_RSmoothnessMultiplier", properties), "Red");
        materialEditor.FloatProperty(FindProperty("_GSmoothnessMultiplier", properties), "Green");
        materialEditor.FloatProperty(FindProperty("_BSmoothnessMultiplier", properties), "Blue");
        materialEditor.FloatProperty(FindProperty("_ASmoothnessMultiplier", properties), "Alpha");
        if (isComplexMask)
        {
            materialEditor.FloatProperty(FindProperty("_CSmoothnessMultiplier", properties), "Cyan");
            materialEditor.FloatProperty(FindProperty("_MSmoothnessMultiplier", properties), "Magenta");
            materialEditor.FloatProperty(FindProperty("_YSmoothnessMultiplier", properties), "Yellow");
        }
    }


    private void DrawPatternPage(Material material, MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        bool patternEnabled = EditorGUILayout.ToggleLeft(new GUIContent("Enable Pattern", "Allows to place single or repeating patterns. Increased shader complexity"), material.IsKeywordEnabled("_PATTERN"));
        if (patternEnabled)
            material.EnableKeyword("_PATTERN");
        else
            material.DisableKeyword("_PATTERN");

        if (patternEnabled)
        {
            using (new VisualBlock())
            {
                materialEditor.TextureProperty(FindProperty("_PatternTexture", properties), "Pattern Texture");
                materialEditor.ColorProperty(FindProperty("_PatternTint", properties), "Tint");
            }

            EditorGUILayout.Space(15);
            using (new VisualBlock())
            {

                materialEditor.TextureProperty(FindProperty("_PatternORM", properties), "ORM (Occlusion (R), Roughness (G), Metallic (M)");
                if (FindProperty("_PatternORM", properties).textureValue != null)
                {
                    EditorGUI.indentLevel++;
                    {
                        EditorGUILayout.LabelField("ORM Multiplier", EditorStyles.boldLabel);
                        EditorGUI.indentLevel++;
                        {
                            materialEditor.FloatProperty(FindProperty("_PatternOcclusionMultiplier", properties), "Occlusion");
                            materialEditor.FloatProperty(FindProperty("_PatternSmoothnessMultiplier", properties), "Roughness");
                            materialEditor.FloatProperty(FindProperty("_PatternMetallicMultiplier", properties), "Metallic");
                        }
                        EditorGUI.indentLevel--;

                        MaterialProperty origORM = FindProperty("_UseOriginalORM", properties);
                        origORM.floatValue = EditorGUILayout.ToggleLeft("Use main ORM", origORM.floatValue != 0) ? 1 : 0;
                    }
                    EditorGUI.indentLevel--;
                }
            }

            EditorGUILayout.Space(15);
            using (new VisualBlock())
            {
                MaterialProperty useNormalProp = FindProperty("_UseCustomNormal", properties);
                useNormalProp.floatValue = EditorGUILayout.ToggleLeft("Use Custom Normal for pattern", useNormalProp.floatValue != 0) ? 1 : 0;
                if (useNormalProp.floatValue != 0)
                {
                    materialEditor.TextureProperty(FindProperty("_PatternNormal", properties), "Normal");
                    EditorGUI.indentLevel++;
                    {
                        materialEditor.FloatProperty(FindProperty("_PatternNormalIntensity", properties), "Normal Intensity");
                    }
                    EditorGUI.indentLevel--;
                }
            }

            EditorGUILayout.Space(15);
            using (new VisualBlock())
            {
                EditorGUILayout.LabelField("Position", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                MaterialProperty uvProp = FindProperty("_PatternUV", properties);
                float u = uvProp.vectorValue.x;
                float v = uvProp.vectorValue.y;
                u = EditorGUILayout.Slider("U", u, 0, 1);
                v = EditorGUILayout.Slider("V", v, 0, 1);
                uvProp.vectorValue = new Vector4(u, v, 0, 0);

                MaterialProperty rotationProperty = FindProperty("_PatternRotation", properties);
                rotationProperty.floatValue = EditorGUILayout.Slider("Rotation (Degrees)", rotationProperty.floatValue, 0, 360);

                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("Scaling", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                MaterialProperty scaleProp = FindProperty("_PatternScale_W_H_Uniform", properties);
                float x = scaleProp.vectorValue.x;
                float y = scaleProp.vectorValue.y;
                float uniform = scaleProp.vectorValue.y;
                x = EditorGUILayout.FloatField("X", x);
                y = EditorGUILayout.FloatField("Y", y);
                uniform = EditorGUILayout.FloatField("Uniform", uniform);
                scaleProp.vectorValue = new Vector4(x, y, uniform, 0);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(15);
            using (new VisualBlock())
            {
                MaterialProperty displayOnSeparateTintChannels = FindProperty("_DisplayPatternOnSeparateTintMaskChannels", properties);
                displayOnSeparateTintChannels.floatValue = EditorGUILayout.ToggleLeft("Split displaying using TintMask channels", displayOnSeparateTintChannels.floatValue != 0) ? 1 : 0;
                if (displayOnSeparateTintChannels.floatValue != 0)
                {
                    EditorGUI.indentLevel++;
                    {
                        MaterialProperty useChannelR = FindProperty("_DisplayPatternOnR", properties);
                        useChannelR.floatValue = EditorGUILayout.ToggleLeft("Red", useChannelR.floatValue != 0) ? 1 : 0;
                        MaterialProperty useChannelG = FindProperty("_DisplayPatternOnG", properties);
                        useChannelG.floatValue = EditorGUILayout.ToggleLeft("Green", useChannelG.floatValue != 0) ? 1 : 0;
                        MaterialProperty useChannelB = FindProperty("_DisplayPatternOnB", properties);
                        useChannelB.floatValue = EditorGUILayout.ToggleLeft("Blue", useChannelB.floatValue != 0) ? 1 : 0;
                        MaterialProperty useChannelA = FindProperty("_DisplayPatternOnA", properties);
                        useChannelA.floatValue = EditorGUILayout.ToggleLeft("Alpha", useChannelA.floatValue != 0) ? 1 : 0;
                    }
                    EditorGUI.indentLevel--;
                }
            }
        }
    }
}

public class VisualBlock : IDisposable
{
    public VisualBlock()
    {
        EditorGUILayout.BeginVertical("Box");
    }

    public void Dispose()
    {
        EditorGUILayout.EndVertical();
    }
}

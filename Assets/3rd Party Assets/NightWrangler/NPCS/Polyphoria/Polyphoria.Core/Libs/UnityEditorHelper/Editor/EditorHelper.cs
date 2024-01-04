using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityEditorHelper
{
    public static class EditorHelper
    {
        private static readonly GUIStyle SimpleRectStyle;

        static EditorHelper()
        {
            Texture2D simpleTexture = new Texture2D(1, 1);
            simpleTexture.SetPixel(0, 0, Color.white);
            simpleTexture.Apply();

            SimpleRectStyle = new GUIStyle { normal = { background = simpleTexture } };
        }

        public static bool DrawIconHeader(string key, Texture icon, string caption, Color captionColor, bool canToggle = true)
        {
            bool state = EditorPrefs.GetBool(key, true);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(new GUIContent(icon), EditorStyles.miniLabel, GUILayout.Width(22), GUILayout.Height(22));
                using (new SwitchTextColor(captionColor))
                {
                    EditorGUILayout.BeginVertical();
                    GUILayout.Space(5);
                    GUIStyle style = new GUIStyle { normal = { textColor = captionColor }, fontStyle = FontStyle.Bold };
                    EditorGUILayout.LabelField(caption, style);
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.Space();
                string cap = state ? "\u25bc" : "\u25b2";
                if (canToggle)
                {
                    if (GUILayout.Button(cap, EditorStyles.label, GUILayout.Width(16), GUILayout.Height(16)))
                    {
                        state = !state;
                        EditorPrefs.SetBool(key, state);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(2);
            return state;
        }

        public static string GetAssetPath(string assetFileName)
        {
            if (!AssetDatabase.GetAllAssetPaths().Any(p => p.EndsWith(assetFileName)))
            {
                AssetDatabase.Refresh();
            }
            string basePath = AssetDatabase.GetAllAssetPaths().First(p => p.EndsWith(assetFileName));
            int lastDelimiter = basePath.LastIndexOf('/') + 1;
            basePath = basePath.Remove(lastDelimiter, basePath.Length - lastDelimiter);
            return basePath;
        }

        public static GUIStyle GetEditorStyle(string style)
        {
            return EditorGUIUtility.GetBuiltinSkin(EditorGUIUtility.isProSkin ? EditorSkin.Scene : EditorSkin.Inspector).GetStyle(style);
        }

        public static void GUIDrawRect(Rect position, Color color)
        {
            using (new SwitchColor(color))
            {
                GUI.Box(position, GUIContent.none, SimpleRectStyle);
            }
        }

        public static int IndexOf(this SerializedProperty prop, string value)
        {
            if (!prop.isArray)
                return -1;
            for (int i = 0; i < prop.arraySize; i++)
            {
                if (prop.GetArrayElementAtIndex(i).stringValue.Equals(value))
                    return i;
            }
            return -1;
        }

        public static void DrawTiledTexture(Rect rect, Texture tex)
        {
            GUI.BeginGroup(rect);
            {
                int width = Mathf.RoundToInt(rect.width);
                int height = Mathf.RoundToInt(rect.height);

                for (int y = 0; y < height; y += tex.height)
                {
                    for (int x = 0; x < width; x += tex.width)
                    {
                        GUI.DrawTexture(new Rect(x, y, tex.width, tex.height), tex);
                    }
                }
            }
            GUI.EndGroup();
        }

        private static Texture2D _checkerboard;

        public static Texture2D Checkerboard
        {
            get
            {
                return _checkerboard ?? (_checkerboard = CreateCheckerTex(
                    new Color(0.1f, 0.1f, 0.1f, 0.5f),
                    new Color(0.2f, 0.2f, 0.2f, 0.5f)));
            }
        }

        private static Texture2D CreateCheckerTex(Color c0, Color c1)
        {
            Texture2D tex = new Texture2D(16, 16)
            {
                name = "[EditorHelper] Checker Texture",
                hideFlags = HideFlags.DontSave
            };

            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    tex.SetPixel(x, y, ((y < 8 && x < 8) || (y >= 8 && x >= 8)) ? c1 : c0);
                }
            }
            tex.Apply();
            tex.filterMode = FilterMode.Point;
            return tex;
        }

        public static Rect ConvertToTexCoords(Rect rect, int width, int height)
        {
            Rect final = rect;
            if (width != 0f && height != 0f)
            {
                final.xMin = rect.xMin / width;
                final.xMax = rect.xMax / width;
                final.yMin = 1f - rect.yMax / height;
                final.yMax = 1f - rect.yMin / height;
            }
            return final;
        }

        public static void DrawOutline(Rect rect, Color color)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Texture2D tex = EditorGUIUtility.whiteTexture;
                GUI.color = color;
                GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, 1f, rect.height), tex);
                GUI.DrawTexture(new Rect(rect.xMax, rect.yMin, 1f, rect.height), tex);
                GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, 1f), tex);
                GUI.DrawTexture(new Rect(rect.xMin, rect.yMax, rect.width, 1f), tex);
                GUI.color = Color.white;
            }
        }

        public static void CopyToClipboard(this string s)
        {
            TextEditor te = new TextEditor();
            te.text = s;
            te.SelectAll();
            te.Copy();
        }
    }

    public sealed class HighlightBox : IDisposable
    {
        public HighlightBox() : this(new Color(0.1f, 0.1f, 0.2f))
        {
        }

        public HighlightBox(Color color)
        {
            GUILayout.Space(8f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(4f);
            using (new SwitchColor(color))
            {
                EditorGUILayout.BeginHorizontal(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).GetStyle("sv_iconselector_labelselection"), GUILayout.MinHeight(10f));
            }
            GUILayout.BeginVertical();
            GUILayout.Space(4f);
        }

        public void Dispose()
        {
            GUILayout.Space(3f);
            GUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(3f);
            GUILayout.EndHorizontal();
            GUILayout.Space(3f);
        }
    }

    public sealed class EditorFrame : IDisposable
    {
        public EditorFrame(string caption, Color color, int width = -1)
        {
            if(width == -1)
                GUILayout.BeginVertical();
            else
                GUILayout.BeginVertical(GUILayout.Width(width));               

            GUILayout.Space(8);
            using (new EditorBlock(EditorBlock.Orientation.Horizontal))
            {
                GUILayout.Space(11);
                using (new EditorBlock(EditorBlock.Orientation.Horizontal, "TL LogicBar 1"))
                {
                    GUILayout.Space(4);
                    GUIStyle style = new GUIStyle
                    {
                        normal = {textColor = color},
                        fontStyle = FontStyle.Bold
                    };
                    EditorGUILayout.LabelField(caption, style);
                }
                GUILayout.FlexibleSpace();
                GUILayout.Space(11);
            }
            GUILayout.Space(-11);

            EditorGUILayout.BeginVertical("GroupBox");
        }


        public void Dispose()
        {
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }
    }

    public sealed class EditorBlock : IDisposable
    {
        public enum Orientation
        {
            Horizontal,
            Vertical
        }

        private readonly Orientation _orientation;

        public EditorBlock(Orientation orientation, string style, params GUILayoutOption[] options)
        {
            _orientation = orientation;
            if (orientation == Orientation.Horizontal)
            {
                EditorGUILayout.BeginHorizontal(string.IsNullOrEmpty(style) ? GUIStyle.none : style, options);
            }
            else
            {
                EditorGUILayout.BeginVertical(string.IsNullOrEmpty(style) ? GUIStyle.none : style, options);
            }
        }

        public EditorBlock(Orientation orientation, string style) : this(orientation, style, new GUILayoutOption[] { })
        {
        }

        public EditorBlock(Orientation orientation) : this(orientation, null, new GUILayoutOption[] { })
        {
        }

        public EditorBlock(Orientation orientation, params GUILayoutOption[] layoutOptions) : this(orientation, null, layoutOptions)
        {
            
        }

        public void Dispose()
        {
            if (_orientation == Orientation.Horizontal)
            {
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.EndVertical();
            }
        }
    }

    public sealed class SwitchColor : IDisposable
    {
        private readonly Color _savedColor;

        public SwitchColor(Color newColor)
        {
            _savedColor = GUI.color;
            GUI.color = newColor;
        }

        public void Dispose()
        {
            GUI.color = _savedColor;
        }
    }

    public sealed class SwitchBackgroundColor : IDisposable
    {
        private readonly Color _savedColor;

        public SwitchBackgroundColor(Color newColor)
        {
            _savedColor = GUI.backgroundColor;
            GUI.backgroundColor = newColor;
        }

        public void Dispose()
        {
            GUI.backgroundColor = _savedColor;
        }
    }

    public sealed class SwitchTextColor : IDisposable
    {
        private readonly Color _savedColor;

        public SwitchTextColor(Color newColor)
        {
            _savedColor = GUI.backgroundColor;
            GUI.backgroundColor = newColor;
        }

        public void Dispose()
        {
            GUI.backgroundColor = _savedColor;
        }
    }

    public sealed class SwitchGUIDepth : IDisposable
    {
        private readonly int _savedDepth;

        public SwitchGUIDepth(int depth)
        {
            _savedDepth = GUI.depth;
            GUI.depth = depth;
        }

        public void Dispose()
        {
            GUI.depth = _savedDepth;
        }
    }

    public class IndentBlock : IDisposable
    {
        public IndentBlock()
        {
            EditorGUI.indentLevel++;
        }

        public void Dispose()
        {
            EditorGUI.indentLevel--;
        }
    }

    public class ScrollViewBlock : IDisposable
    {
        public ScrollViewBlock(ref Vector2 scrollPosition, params GUILayoutOption[] options)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, options);
        }

        public void Dispose()
        {
            EditorGUILayout.EndScrollView();
        }
    }

    public sealed class FoldableBlock : IDisposable
    {
        private readonly Color _defaultBackgroundColor;

        private bool _expanded;

        public FoldableBlock(ref bool expanded, string header) : this(ref expanded, header, null)
        {
        }

        public FoldableBlock(ref bool expanded, string header, Texture2D icon)
        {
            _defaultBackgroundColor = GUI.backgroundColor;
            GUILayout.Space(3f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(3f);
            GUI.changed = false;
            if (!GUILayout.Toggle(true, new GUIContent("<b><size=11>" + header + "</size></b>", icon), "dragtab", GUILayout.MinWidth(20f)))
                expanded = !expanded;
            GUILayout.Space(2f);
            GUILayout.EndHorizontal();
            if (!expanded)
            {
                GUILayout.Space(3f);
            }
            else
            {
                GroupStart();
            }
            _expanded = expanded;
        }

        private void GroupStart()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(3f);
            EditorGUILayout.BeginVertical();
            GUILayout.Space(-2f);
            EditorGUILayout.BeginHorizontal("TextArea", GUILayout.MinHeight(10f));
            GUILayout.BeginVertical();
            GUILayout.Space(2f);
        }

        private void GroupEnd()
        {
            GUILayout.Space(3f);
            GUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            GUILayout.Space(3f);
            GUILayout.EndHorizontal();
            GUILayout.Space(3f);
            GUI.backgroundColor = _defaultBackgroundColor;
        }

        public void Dispose()
        {
            if (_expanded)
                GroupEnd();
        }
    }
}
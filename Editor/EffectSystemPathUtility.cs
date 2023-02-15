using UnityEditor;
using UnityEngine;
using StudioScor.EffectSystem;
using StudioScor.Utilities.Editor;

namespace StudioScor.EffectSystem.Editor
{
    [CustomEditor(typeof(EffectSystemComponent))]
    [CanEditMultipleObjects]
    public sealed class EffectSystemComponentEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Application.isPlaying)
            {
                GUILayout.Space(5f);
                SEditorUtility.GUI.DrawLine(4f);
                GUILayout.Space(5f);

                var effectSystem = (EffectSystemComponent)target;

                var effects = effectSystem.Effects;

                GUIStyle title = new();
                GUIStyle normal = new();

                title.normal.textColor = Color.white;
                title.alignment = TextAnchor.MiddleCenter;
                title.fontStyle = FontStyle.Bold;

                normal.normal.textColor = Color.white;

                GUILayout.Label("[ Effects ]", title);

                if (effects is not null)
                {
                    foreach (var effect in effects)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(effect.Effect.name, normal);
                        GUILayout.FlexibleSpace();
                        GUILayout.Label("[ Level : " + effect.Level.ToString() + " ]", normal);
                        if(effect.Effect.Type.Equals(EEffectType.Infinity))
                        {
                            GUILayout.Label(" [ Infinity ]", normal);
                        }
                        else
                        {
                            GUILayout.Label(" [ " + effect.RemainTime.ToString("N1") + " sec ] ", normal);
                        }
                        GUILayout.Space(10f);
                        GUILayout.EndHorizontal();

                        SEditorUtility.GUI.DrawLine(1f);
                    }
                }
            }
        }
    }

    public static class EffectSystemPathUtility
    {
        private static string _RootFolder;
        public static string RootFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_RootFolder))
                {
                    _RootFolder = PathOf("studioscor_effectsystem_root");
                }

                return _RootFolder;
            }
        }

        private static string PathOf(string fileName)
        {
            var files = AssetDatabase.FindAssets(fileName);

            if (files.Length == 0)
                return string.Empty;

            var assetPath = AssetDatabase.GUIDToAssetPath(files[0]).Replace(fileName, string.Empty);

            return assetPath;
        }
    }
}
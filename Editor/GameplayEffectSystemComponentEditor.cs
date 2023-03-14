using UnityEditor;
using UnityEngine;
using StudioScor.Utilities.Editor;

namespace StudioScor.GameplayEffectSystem.Editor
{
    [CustomEditor(typeof(GameplayEffectSystemComponent))]
    [CanEditMultipleObjects]
    public sealed class GameplayEffectSystemComponentEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Application.isPlaying)
            {
                GUILayout.Space(5f);
                SEditorUtility.GUI.DrawLine(4f);
                GUILayout.Space(5f);

                var effectSystem = (GameplayEffectSystemComponent)target;

                var effects = effectSystem.GameplayEffects;

                GUIStyle title = new();
                GUIStyle normal = new();

                title.normal.textColor = Color.white;
                title.alignment = TextAnchor.MiddleCenter;
                title.fontStyle = FontStyle.Bold;

                normal.normal.textColor = Color.white;

                GUILayout.Label("[ Gameplay Effects ]", title);

                if (effects is not null)
                {
                    foreach (var effect in effects)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(effect.GameplayEffect.name, normal);
                        GUILayout.FlexibleSpace();
                        GUILayout.Label("[ Level : " + effect.Level.ToString() + " ]", normal);
                        if(effect.GameplayEffect.Type.Equals(EGameplayEffectType.Infinity))
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
}
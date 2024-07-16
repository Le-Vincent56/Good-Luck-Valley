using UnityEngine;
using UnityEditor;

namespace GoodLuckValley.UI.Editor
{
    public abstract class FadePanelEditorBase : UnityEditor.Editor
    {
        private SerializedObject serializedFadePanel;
        protected SerializedProperty fadeDurationProperty;
        protected SerializedProperty exclusiveFadeProperty;
        private SerializedProperty objectsToFadeProperty;

        private void OnEnable()
        {
            serializedFadePanel = new SerializedObject(target);
            fadeDurationProperty = serializedFadePanel.FindProperty("fadeDuration");
            exclusiveFadeProperty = serializedFadePanel.FindProperty("exclusiveFade");
            objectsToFadeProperty = serializedFadePanel.FindProperty("objectsToFade");
        }

        public void ShowFadePanelUI()
        {
            serializedFadePanel.Update();

            EditorGUILayout.PropertyField(fadeDurationProperty, new GUIContent("Fade Duration"));
            EditorGUILayout.PropertyField(exclusiveFadeProperty, new GUIContent("Exclusive Fade"));

            if (exclusiveFadeProperty.boolValue)
            {
                EditorGUILayout.PropertyField(objectsToFadeProperty, new GUIContent("Objects to Fade"), true);
            }

            serializedFadePanel.ApplyModifiedProperties();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GoodLuckValley.UI.Elements.Editor
{
    [CustomEditor(typeof(SliderInput))]
    public class InputSliderEditor : UnityEditor.Editor
    {
        SerializedProperty normalStep; 
        SerializedProperty applyAltMod;
        SerializedProperty altStep;
        SerializedProperty applyShiftMod;
        SerializedProperty shiftStep;

        private void OnEnable()
        {
            normalStep = serializedObject.FindProperty("normalStep");
            applyAltMod = serializedObject.FindProperty("applyAltMod");
            altStep = serializedObject.FindProperty("altStep");
            applyShiftMod = serializedObject.FindProperty("applyShiftMod");
            shiftStep = serializedObject.FindProperty("shiftStep");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // Update the serialized object
            serializedObject.Update();

            // Draw properties
            EditorGUILayout.LabelField("Input Slider", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(normalStep);
            EditorGUILayout.PropertyField(applyAltMod);
            EditorGUILayout.PropertyField(altStep);
            EditorGUILayout.PropertyField(applyShiftMod);
            EditorGUILayout.PropertyField(shiftStep);

            // Apply changes
            serializedObject.ApplyModifiedProperties();
        }
    }
}
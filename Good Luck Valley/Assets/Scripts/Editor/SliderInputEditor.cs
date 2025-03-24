using GoodLuckValley.UI.Menus.Audio;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace GoodLuckValley.Editors.UI
{
    [CustomEditor(typeof(SliderInput))]
    public class SliderInputEditor : OdinEditor
    {
        private SerializedProperty normalStep;
        private SerializedProperty applyAltMod;
        private SerializedProperty altStep;
        private SerializedProperty applyShiftMod;
        private SerializedProperty shiftStep;

        protected override void OnEnable()
        {
            base.OnEnable();

            normalStep = serializedObject.FindProperty("normalStep");
            applyAltMod = serializedObject.FindProperty("applyAltMod");
            altStep = serializedObject.FindProperty("altStep");
            applyShiftMod = serializedObject.FindProperty("applyShiftMod");
            shiftStep = serializedObject.FindProperty("shiftStep");
        }

        public override void OnInspectorGUI()
        {
            // Draw the default inspector
            DrawDefaultInspector();

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

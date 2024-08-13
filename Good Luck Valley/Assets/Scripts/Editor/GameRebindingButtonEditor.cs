using UnityEditor;
using UnityEngine.InputSystem;
using System.Linq;
using System;

namespace GoodLuckValley.UI.Settings.Controls.Editor
{
    [CustomEditor(typeof(GameRebindingButton))]
    public class RebindingButtonEditor : UnityEditor.Editor
    {
        SerializedProperty bindingInfoProp;
        SerializedProperty buttonProp;
        SerializedProperty validRebind;

        private void OnEnable()
        {
            bindingInfoProp = serializedObject.FindProperty("bindingInfo");
            buttonProp = serializedObject.FindProperty("targetButton");
            validRebind = serializedObject.FindProperty("validRebind");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.ObjectField(buttonProp);
            EditorGUILayout.Space(10f);

            EditorGUILayout.LabelField("Binding Info", EditorStyles.boldLabel);
            ShowBindingInfo(bindingInfoProp, "Action Map", "Action", "Binding");
            EditorGUILayout.Space(10f);

            EditorGUILayout.PropertyField(validRebind);

            serializedObject.ApplyModifiedProperties();
        }

        private void ShowBindingInfo(SerializedProperty bindingInfoProp, string mapNamePopupString, string actionNamePopupName, string bindingName)
        {
            SerializedProperty actionAssetProp = bindingInfoProp.FindPropertyRelative("actionAsset");
            SerializedProperty actionMapNameProp = bindingInfoProp.FindPropertyRelative("actionMapName");
            SerializedProperty actionNameProp = bindingInfoProp.FindPropertyRelative("actionName");
            SerializedProperty bindingIndexProp = bindingInfoProp.FindPropertyRelative("bindingIndex");
            EditorGUILayout.PropertyField(actionAssetProp);

            if (actionAssetProp.objectReferenceValue != null)
            {
                InputActionAsset actionAsset = (InputActionAsset)actionAssetProp.objectReferenceValue;

                string[] actionMapNames = GetActionMapNames(actionAsset);
                int selectedActionMap = Array.IndexOf(actionMapNames, actionMapNameProp.stringValue);
                if (selectedActionMap < 0 || selectedActionMap >= actionMapNames.Length)
                {
                    selectedActionMap = 0;
                }
                selectedActionMap = EditorGUILayout.Popup(mapNamePopupString, selectedActionMap, actionMapNames);
                actionMapNameProp.stringValue = actionMapNames[selectedActionMap];

                string[] actionNames = GetActionNames(actionAsset, actionMapNameProp.stringValue);
                int selectedAction = Array.IndexOf(actionNames, actionNameProp.stringValue);
                if (selectedAction < 0 || selectedAction >= actionNames.Length)
                {
                    selectedAction = 0;
                }
                selectedAction = EditorGUILayout.Popup(actionNamePopupName, selectedAction, actionNames);
                actionNameProp.stringValue = actionNames[selectedAction];

                InputAction action = actionAsset.FindActionMap(actionMapNameProp.stringValue)?.FindAction(actionNameProp.stringValue);
                if (action != null)
                {
                    string[] bindingOptions = GetBindingOptions(action);
                    if (bindingIndexProp.intValue < 0 || bindingIndexProp.intValue >= bindingOptions.Length)
                    {
                        bindingIndexProp.intValue = 0;
                    }
                    bindingIndexProp.intValue = EditorGUILayout.Popup(bindingName, bindingIndexProp.intValue, bindingOptions);
                }
            }
        }

        private string[] GetActionMapNames(InputActionAsset actionAsset) => 
            actionAsset.actionMaps.Select(map => map.name).ToArray();

        private string[] GetActionNames(InputActionAsset actionAsset, string actionMapName)
        {
            InputActionMap actionMap = actionAsset.FindActionMap(actionMapName);
            return actionMap != null ? actionMap.actions.Select(action => action.name).ToArray() : new string[0];
        }

        private string[] GetBindingOptions(InputAction action) => 
            action.bindings.Select((binding, index) => $"{index}: {binding.ToDisplayString()}").ToArray();
    }
}
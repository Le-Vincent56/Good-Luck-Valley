using GoodLuckValley.UI.Menus.Controls;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GoodLuckValley.Editors.UI
{
    [CustomEditor(typeof(TutorialKeybind))]
    public class TutorialKeybindEditor : Editor
    {
        private SerializedProperty bindingInfoProp;
        private SerializedProperty buttonProp;
        private SerializedProperty animatorProp;
        private SerializedProperty imageProp;
        private SerializedProperty validRebind;
        private SerializedProperty rebinded;

        private void OnEnable()
        {
            bindingInfoProp = serializedObject.FindProperty("bindingInfo");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Binding Info", EditorStyles.boldLabel);
            ShowBindingInfo(bindingInfoProp, "Action Map", "Action", "Binding");
            EditorGUILayout.Space(10f);

            serializedObject.ApplyModifiedProperties();
        }

        private void ShowBindingInfo(SerializedProperty bindingInfoProp, string mapNamePopupString, string actionNamePopupName, string bindingName)
        {
            SerializedProperty actionAssetProp = bindingInfoProp.FindPropertyRelative("ActionAsset");
            SerializedProperty actionMapNameProp = bindingInfoProp.FindPropertyRelative("ActionMapName");
            SerializedProperty actionNameProp = bindingInfoProp.FindPropertyRelative("ActionName");
            SerializedProperty bindingIndexProp = bindingInfoProp.FindPropertyRelative("BindingIndex");
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

        private string[] GetBindingOptions(InputAction action) =>
            action.bindings.Select((binding, index) => $"{index}: {binding.ToDisplayString()}").ToArray();

        private string[] GetActionNames(InputActionAsset actionAsset, string actionMapName)
        {
            InputActionMap actionMap = actionAsset.FindActionMap(actionMapName);
            return actionMap != null ? actionMap.actions.Select(action => action.name).ToArray() : new string[0];
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace GoodLuckValley.Patterns.Blackboard.Editor
{
    [CustomEditor(typeof(BlackboardData))]
    public class BlackboardDataEditor : UnityEditor.Editor
    {
        ReorderableList entryList;

        private void OnEnable()
        {
            entryList = new ReorderableList(serializedObject, serializedObject.FindProperty("entries"), true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(new Rect(rect.x + 15, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight), "Key");
                    EditorGUI.LabelField(new Rect(rect.x + rect.width * 0.3f + 20, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight), "Type");
                    EditorGUI.LabelField(new Rect(rect.x + rect.width * 0.6f + 15, rect.y, rect.width * 0.4f, EditorGUIUtility.singleLineHeight), "Value");
                }
            };

            entryList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                SerializedProperty element = entryList.serializedProperty.GetArrayElementAtIndex(index);

                rect.y += 2;
                SerializedProperty keyName = element.FindPropertyRelative("keyName");
                SerializedProperty valueType = element.FindPropertyRelative("valueType");
                SerializedProperty value = element.FindPropertyRelative("value");

                Rect keyNameRect = new Rect(rect.x, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);
                Rect valueTypeRect = new Rect(rect.x + rect.width * 0.3f + 10, rect.y, rect.width * 0.3f - 10, EditorGUIUtility.singleLineHeight);
                Rect valueRect = new Rect(rect.x + rect.width * 0.6f + 10, rect.y, rect.width * 0.4f - 10, EditorGUIUtility.singleLineHeight);

                EditorGUI.PropertyField(keyNameRect, keyName, GUIContent.none);
                EditorGUI.PropertyField(valueTypeRect, valueType, GUIContent.none);

                switch((AnyValue.ValueType)valueType.enumValueIndex)
                {
                    case AnyValue.ValueType.Int:
                        SerializedProperty intValue = value.FindPropertyRelative("intValue");
                        EditorGUI.PropertyField(valueRect, intValue, GUIContent.none);
                        break;
                    case AnyValue.ValueType.Float:
                        SerializedProperty floatValue = value.FindPropertyRelative("floatValue");
                        EditorGUI.PropertyField(valueRect, floatValue, GUIContent.none);
                        break;
                    case AnyValue.ValueType.Bool:
                        SerializedProperty boolValue = value.FindPropertyRelative("boolValue");
                        EditorGUI.PropertyField(valueRect, boolValue, GUIContent.none);
                        break;
                    case AnyValue.ValueType.String:
                        SerializedProperty strValue = value.FindPropertyRelative("stringValue");
                        EditorGUI.PropertyField(valueRect, strValue, GUIContent.none);
                        break;
                    case AnyValue.ValueType.Vector2:
                        SerializedProperty vec2Value = value.FindPropertyRelative("vector2Value");
                        EditorGUI.PropertyField(valueRect, vec2Value, GUIContent.none);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            entryList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
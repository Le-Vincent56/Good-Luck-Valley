using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GoodLuckValley.World.Tiles;

[CustomPropertyDrawer(typeof(ShroomTile.UpDirection))]
public class UpDirectionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Get the enum value
        int enumValueIndex = property.enumValueIndex;

        // Define the labels for each enum value
        string[] enumLabels = { "(0, 1)", "(1, 0)", "(0, -1)", "(-1, 0)" };

        // Display the enum value using a Popup
        enumValueIndex = EditorGUI.Popup(position, label.text, enumValueIndex, enumLabels);

        // Set the new enum value
        property.enumValueIndex = enumValueIndex;

        EditorGUI.EndProperty();
    }
}

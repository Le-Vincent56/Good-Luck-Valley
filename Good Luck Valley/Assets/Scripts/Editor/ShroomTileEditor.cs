using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using GoodLuckValley.World.Tiles;

[CustomEditor(typeof(ShroomTile))]
public class ShroomTileEditor : Editor
{
    private bool expandedRotations = true;

    private SerializedProperty tileType;
    private SerializedProperty shroomType;

    private SerializedProperty triangleTop;
    private SerializedProperty triangleBottom;
    private SerializedProperty triangleSide;

    private SerializedProperty rectangleTop;
    private SerializedProperty rectangleBottom;
    private SerializedProperty rectangleLeft;
    private SerializedProperty rectangleRight;

    private void OnEnable()
    {
        tileType = serializedObject.FindProperty("tileType");
        shroomType = serializedObject.FindProperty("shroomType");

        triangleTop = serializedObject.FindProperty("triangleTop");
        triangleBottom = serializedObject.FindProperty("triangleBottom");
        triangleSide = serializedObject.FindProperty("triangleSide");

        rectangleTop = serializedObject.FindProperty("rectangleTop");
        rectangleBottom = serializedObject.FindProperty("rectangleBottom");
        rectangleLeft = serializedObject.FindProperty("rectangleLeft");
        rectangleRight = serializedObject.FindProperty("rectangleRight");
    }

    public override void OnInspectorGUI()
    {
        // Update the object if data is changed
        serializedObject.UpdateIfRequiredOrScript();

        // Build tile details
        EditorGUILayout.LabelField("Tile Details", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(tileType, new GUIContent("Tile Type"));
        EditorGUILayout.PropertyField(shroomType, new GUIContent("Shroom Type"));

        expandedRotations = EditorGUILayout.BeginFoldoutHeaderGroup(expandedRotations, new GUIContent("Rotations"));

        if(expandedRotations)
        {
            if (tileType.intValue == 3)
            {
                EditorGUILayout.PropertyField(triangleTop, new GUIContent("Top"));
                EditorGUILayout.PropertyField(triangleBottom, new GUIContent("Bottom"));
                EditorGUILayout.PropertyField(triangleSide, new GUIContent("Side"));
            }
            else if (tileType.intValue == 4)
            {
                EditorGUILayout.PropertyField(rectangleTop, new GUIContent("Top"));
                EditorGUILayout.PropertyField(rectangleBottom, new GUIContent("Bottom"));
                EditorGUILayout.PropertyField(rectangleLeft, new GUIContent("Left"));
                EditorGUILayout.PropertyField(rectangleRight, new GUIContent("Right"));
            }
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        // Apply changes
        serializedObject.ApplyModifiedProperties();
    }
}

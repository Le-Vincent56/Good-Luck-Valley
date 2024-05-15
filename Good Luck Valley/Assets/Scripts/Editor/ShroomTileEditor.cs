using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using GoodLuckValley.World.Tiles;

[CustomEditor(typeof(ShroomTile))]
public class ShroomTileEditor : Editor
{
    private bool expandedBounds = true;
    private bool expandedRotations = true;

    private SerializedProperty tileType;
    private SerializedProperty shroomType;

    private SerializedProperty contactBuffer;

    private SerializedProperty center;
    private SerializedProperty width;
    private SerializedProperty height;

    private SerializedProperty spawnUp;
    private SerializedProperty spawnRight;
    private SerializedProperty spawnDown;
    private SerializedProperty spawnLeft;

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

        contactBuffer = serializedObject.FindProperty("contactBuffer");

        center = serializedObject.FindProperty("center");
        width = serializedObject.FindProperty("width");
        height = serializedObject.FindProperty("height");

        spawnUp = serializedObject.FindProperty("spawnUp");
        spawnRight = serializedObject.FindProperty("spawnRight");
        spawnDown = serializedObject.FindProperty("spawnDown");
        spawnLeft = serializedObject.FindProperty("spawnLeft");

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
        EditorGUILayout.LabelField("Type Details", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(tileType, new GUIContent("Tile Type"));
        EditorGUILayout.PropertyField(shroomType, new GUIContent("Shroom Type"));

        EditorGUILayout.Space(10f);
        EditorGUILayout.PropertyField(contactBuffer, new GUIContent("Contact Buffer"));

        if(tileType.intValue == 3)
        {
            EditorGUILayout.LabelField("Direction Details", EditorStyles.boldLabel);
        } else if (tileType.intValue == 4)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LabelField("Spawn Details", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(center, new GUIContent("Center Point"));
            EditorGUILayout.PropertyField(width, new GUIContent("Width Extents"));
            EditorGUILayout.PropertyField(height, new GUIContent("Height Extents"));
            EditorGUILayout.PropertyField(spawnUp, new GUIContent("Spawn Up"));
            EditorGUILayout.PropertyField(spawnRight, new GUIContent("Spawn Right"));
            EditorGUILayout.PropertyField(spawnDown, new GUIContent("Spawn Down"));
            EditorGUILayout.PropertyField(spawnLeft, new GUIContent("Spawn Left"));
            EditorGUI.EndDisabledGroup();
        }

        EditorGUILayout.Space(10f);

        // Draw rotations
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

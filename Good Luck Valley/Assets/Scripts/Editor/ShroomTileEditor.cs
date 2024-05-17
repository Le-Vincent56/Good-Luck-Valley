using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using GoodLuckValley.World.Tiles;
using UnityEngine.Rendering;

[CustomEditor(typeof(ShroomTile))]
public class ShroomTileEditor : Editor
{
    private bool expandedRotations = true;

    private SerializedProperty tileType;
    private SerializedProperty shroomType;
    private SerializedProperty prioritySide;

    private SerializedProperty contactBuffer;

    private SerializedProperty center;
    private SerializedProperty width;
    private SerializedProperty height;

    // Triangle fields
    private SerializedProperty diagDirection;
    private SerializedProperty hypotenusePoints;
    private SerializedProperty spawnHor;
    private SerializedProperty spawnVer;

    private SerializedProperty diagonalRot;
    private SerializedProperty horizontalRot;
    private SerializedProperty verticalRot;

    // Rectangle fields
    private SerializedProperty spawnUp;
    private SerializedProperty spawnRight;
    private SerializedProperty spawnDown;
    private SerializedProperty spawnLeft;

    private SerializedProperty topRot;
    private SerializedProperty bottomRot;
    private SerializedProperty leftRot;
    private SerializedProperty rightRot;

    private void OnEnable()
    {
        tileType = serializedObject.FindProperty("tileType");
        shroomType = serializedObject.FindProperty("shroomType");
        prioritySide = serializedObject.FindProperty("prioritySide");

        contactBuffer = serializedObject.FindProperty("contactBuffer");

        center = serializedObject.FindProperty("center");
        width = serializedObject.FindProperty("width");
        height = serializedObject.FindProperty("height");

        diagDirection = serializedObject.FindProperty("diagDirection");
        hypotenusePoints = serializedObject.FindProperty("hypotenusePoints");
        spawnHor = serializedObject.FindProperty("spawnHor");
        spawnVer = serializedObject.FindProperty("spawnVer");

        diagonalRot = serializedObject.FindProperty("diagonalRot");
        horizontalRot = serializedObject.FindProperty("horizontalRot");
        verticalRot = serializedObject.FindProperty("verticalRot");

        spawnUp = serializedObject.FindProperty("spawnUp");
        spawnRight = serializedObject.FindProperty("spawnRight");
        spawnDown = serializedObject.FindProperty("spawnDown");
        spawnLeft = serializedObject.FindProperty("spawnLeft");

        topRot = serializedObject.FindProperty("topRot");
        bottomRot = serializedObject.FindProperty("bottomRot");
        leftRot = serializedObject.FindProperty("leftRot");
        rightRot = serializedObject.FindProperty("rightRot");
    }

    public override void OnInspectorGUI()
    {
        // Update the object if data is changed
        serializedObject.UpdateIfRequiredOrScript();

        // Build tile details
        EditorGUILayout.LabelField("Type Details", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(tileType, new GUIContent("Tile Type"));
        EditorGUILayout.PropertyField(shroomType, new GUIContent("Shroom Type"));
        EditorGUILayout.PropertyField(prioritySide, new GUIContent("Prioritize"));
        EditorGUILayout.PropertyField(contactBuffer, new GUIContent("Contact Buffer"));
        EditorGUILayout.Space(10f);

        if(tileType.intValue == 3)
        {
            EditorGUILayout.LabelField("Direction Details", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(diagDirection, new GUIContent("Diagonal Direction"));
            EditorGUILayout.Space(10f);

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LabelField("Spawn Details", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(center, new GUIContent("Center Point"));
            EditorGUILayout.PropertyField(width, new GUIContent("Width Extents"));
            EditorGUILayout.PropertyField(height, new GUIContent("Height Extents"));
            EditorGUILayout.PropertyField(hypotenusePoints, new GUIContent("Hypotenuse Points"));
            EditorGUILayout.PropertyField(spawnHor, new GUIContent("Horizontal Spawn"));
            EditorGUILayout.PropertyField(spawnVer, new GUIContent("Vertical Spawn"));
            EditorGUI.EndDisabledGroup();
        } else if (tileType.intValue == 4)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LabelField("Spawn Details", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(center, new GUIContent("Center Point"));
            EditorGUILayout.PropertyField(width, new GUIContent("Width Extents"));
            EditorGUILayout.PropertyField(height, new GUIContent("Height Extents"));
            EditorGUILayout.PropertyField(spawnUp, new GUIContent("Up Spawn"));
            EditorGUILayout.PropertyField(spawnRight, new GUIContent("Right Spawn"));
            EditorGUILayout.PropertyField(spawnDown, new GUIContent("Down Spawn"));
            EditorGUILayout.PropertyField(spawnLeft, new GUIContent("Left Spawn"));
            EditorGUI.EndDisabledGroup();
        }

        EditorGUILayout.Space(10f);

        // Draw rotations
        expandedRotations = EditorGUILayout.BeginFoldoutHeaderGroup(expandedRotations, new GUIContent("Rotations"));
        if(expandedRotations)
        {
            if (tileType.intValue == 3)
            {
                EditorGUILayout.PropertyField(diagonalRot, new GUIContent("Diagonal"));
                EditorGUILayout.PropertyField(horizontalRot, new GUIContent("Horizontal"));
                EditorGUILayout.PropertyField(verticalRot, new GUIContent("Vertical"));
            }
            else if (tileType.intValue == 4)
            {
                EditorGUILayout.PropertyField(topRot, new GUIContent("Top"));
                EditorGUILayout.PropertyField(bottomRot, new GUIContent("Bottom"));
                EditorGUILayout.PropertyField(leftRot, new GUIContent("Left"));
                EditorGUILayout.PropertyField(rightRot, new GUIContent("Right"));
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        // Apply changes
        serializedObject.ApplyModifiedProperties();
    }
}

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
    private SerializedProperty upDirection;

    private SerializedProperty up;
    private SerializedProperty right;
    private SerializedProperty down;
    private SerializedProperty left;

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
        upDirection = serializedObject.FindProperty("upDirection");

        up = serializedObject.FindProperty("up");
        right = serializedObject.FindProperty("right");
        down = serializedObject.FindProperty("down");
        left = serializedObject.FindProperty("left");

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

        EditorGUILayout.LabelField("Direction Details", EditorStyles.boldLabel);
        if(tileType.intValue == 3)
        {

        } else if (tileType.intValue == 4)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(upDirection, new GUIContent("Up Direction"));
            if(EditorGUI.EndChangeCheck())
            {
                switch(upDirection.intValue)
                {
                    // Up
                    case 0:
                        up.vector2Value = new Vector2(0, 1);
                        break;

                    // Right
                    case 1:
                        up.vector2Value = new Vector2(1, 0);
                        break;

                    // Down
                    case 2:
                        up.vector2Value = new Vector2(0, -1);
                        break;

                    // Left
                    case 3:
                        up.vector2Value = new Vector2(-1, 0);
                        break;
                }

                right.vector2Value = new Vector2(up.vector2Value.y, -up.vector2Value.x);
                down.vector2Value = -up.vector2Value;
                left.vector2Value = new Vector2(-up.vector2Value.y, up.vector2Value.x);
            }

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(up, new GUIContent("Up Vector"));
            EditorGUILayout.PropertyField(down, new GUIContent("Down Vector"));
            EditorGUILayout.PropertyField(left, new GUIContent("Left Vector"));
            EditorGUILayout.PropertyField(right, new GUIContent("Right Vector"));
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

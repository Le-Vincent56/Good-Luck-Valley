using UnityEngine;
using UnityEditor;
using GoodLuckValley.World.AreaTriggers;
using UnityEngine.SceneManagement;
using GoodLuckValley.Scenes.Data;
using System.Collections.Generic;

[CustomEditor(typeof(TransitionTrigger))]
public class TransitionTriggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Get the target object
        TransitionTrigger transitionTrigger = (TransitionTrigger)target;

        // Ensure the serialized object is up to date
        serializedObject.Update();

        // Create a header
        EditorGUILayout.LabelField("Fields", EditorStyles.boldLabel);

        SerializedProperty levelPositionDataProperty = serializedObject.FindProperty("levelPositionData");
        EditorGUILayout.PropertyField(levelPositionDataProperty);

        // Show the TransitionType enum
        transitionTrigger.TransitionType = (TransitionType)EditorGUILayout.EnumPopup("Transition Type", transitionTrigger.TransitionType);

        // Show the move direction
        transitionTrigger.MoveDirection = EditorGUILayout.IntSlider("Move Direction", transitionTrigger.MoveDirection, -1, 1);

        // Get the number of scenes in the build
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        // Create an array to store scene names
        string[] sceneNames = new string[sceneCount];

        // Loop through each scene in the build settings to get the scene names
        for(int i = 0; i < sceneCount; i++)
        {
            // Get the scene path by index
            string scenepath = SceneUtility.GetScenePathByBuildIndex(i);

            // Get the scene name from the scene path
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenepath);

            // Store the scene name in the array
            sceneNames[i] = sceneName;
        }

        // Find the index of the current selected scene in the array
        int currentIndex = System.Array.IndexOf(sceneNames, transitionTrigger.SceneToTransitionTo);

        // Show a popup to select a scene from the list
        int selectedIndex = EditorGUILayout.Popup("Scene to Transition To", currentIndex, sceneNames);

        // Update the sceneToTransitionTo if a new scene is selected
        if(selectedIndex >= 0 && selectedIndex < sceneNames.Length)
        {
            transitionTrigger.SceneToTransitionTo = sceneNames[selectedIndex];
        }

        // Ensure that the level position data is assigned and the scene is selected
        if(transitionTrigger.LevelPositionData !=  null && !string.IsNullOrEmpty(transitionTrigger.SceneToTransitionTo))
        {
            // Get the data container for the selected scene
            LevelDataContainer levelData = transitionTrigger.LevelPositionData.GetLevelData(transitionTrigger.SceneToTransitionTo);

            if(levelData != null)
            {
                List<Vector2> positions = transitionTrigger.TransitionType == TransitionType.Exit 
                    ? levelData.Entrances 
                    : levelData.Exits;

                string[] positionOptions = new string[positions.Count];

                for(int i = 0; i < positions.Count; i++)
                {
                    Vector2 position = transitionTrigger.TransitionType == TransitionType.Exit ? levelData.Entrances[i] : levelData.Exits[i];

                    positionOptions[i] = $"Position {i + 1} ({position})";
                }

                if(positionOptions.Length > 0)
                {
                    transitionTrigger.LoadIndex = EditorGUILayout.Popup("Load Index", transitionTrigger.LoadIndex, positionOptions);
                } else
                {
                    EditorGUILayout.LabelField("No positions available for the selected Transition Type");
                }
            }
        }

        // Check for GUI changes to set dirty
        if(GUI.changed)
        {
            EditorUtility.SetDirty(transitionTrigger);
        }

        // Save any changes to the object
        serializedObject.ApplyModifiedProperties();
    }
}

using UnityEngine;
using UnityEditor;
using GoodLuckValley.World.AreaTriggers;
using UnityEngine.SceneManagement;

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

        // Check for GUI changes to set dirty
        if(GUI.changed)
        {
            EditorUtility.SetDirty(transitionTrigger);
        }

        // Save any changes to the object
        serializedObject.ApplyModifiedProperties();
    }
}

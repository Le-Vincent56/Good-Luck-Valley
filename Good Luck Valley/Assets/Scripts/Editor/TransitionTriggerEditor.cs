using UnityEngine;
using UnityEditor;
using GoodLuckValley.World.AreaTriggers;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(TransitionTrigger))]
public class TransitionTriggerEditor : Editor
{
    TransitionTrigger transitionTrigger;

    private void OnEnable()
    {
        // Get the target object
        transitionTrigger = (TransitionTrigger)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Add some space
        EditorGUILayout.Space();

        // Create a header
        EditorGUILayout.LabelField("Fields", EditorStyles.boldLabel);

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
        int selectedIndex = EditorGUILayout.Popup("Scene to Transition to", currentIndex, sceneNames);

        // Update the sceneToTransitionTo if a new scene is selected
        if(selectedIndex >= 0 && selectedIndex < sceneNames.Length)
        {
            transitionTrigger.SceneToTransitionTo = sceneNames[selectedIndex];
        }

        // Save any changes to the object
        if(GUI.changed)
        {
            EditorUtility.SetDirty(transitionTrigger);
        }
    }
}

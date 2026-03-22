using GoodLuckValley.World.LevelManagement.Adapters;
using UnityEditor;
using UnityEngine;

namespace GoodLuckValley.Editor.LevelManagement
{
    /// <summary>
    /// Custom inspector for <see cref="LevelTransitionTrigger"/>. Validates
    /// that the Collider2D is set to trigger mode and that target IDs are configured.
    /// </summary>
    [CustomEditor(typeof(LevelTransitionTrigger))]
    public class LevelTransitionTriggerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            LevelTransitionTrigger trigger = (LevelTransitionTrigger)target;

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Validation", EditorStyles.boldLabel);

            // Check Collider2D is set to trigger
            Collider2D collider = trigger.GetComponent<Collider2D>();
            if (collider && !collider.isTrigger)
            {
                EditorGUILayout.HelpBox(
                    "Collider2D must have 'Is Trigger' enabled for OnTriggerEnter2D to fire.",
                    MessageType.Error
                );
            }

            if (string.IsNullOrEmpty(trigger.TargetSceneID))
            {
                EditorGUILayout.HelpBox(
                    "Target Scene ID is empty. This trigger will not function.",
                    MessageType.Error
                );
            }

            if (string.IsNullOrEmpty(trigger.TargetSpawnPointID))
            {
                EditorGUILayout.HelpBox(
                    "Target Spawn Point ID is empty. Player positioning "
                    + "may fail after transition.",
                    MessageType.Warning
                );
            }
        }
    }
}
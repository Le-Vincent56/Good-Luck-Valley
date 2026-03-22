using System;
using System.Collections.Generic;
using System.Reflection;
using GoodLuckValley.Core.SceneManagement.Data;
using UnityEditor;

namespace GoodLuckValley.Editor.SceneManagement
{
    /// <summary>
    /// Custom inspector for <see cref="SceneRegistry"/>. Validates scene entries
    /// for duplicate IDs, missing Addressable references, invalid installer types,
    /// and verifies that special scene IDs reference existing entries.
    /// </summary>
    [CustomEditor(typeof(SceneRegistry))]
    public class SceneRegistryEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SceneRegistry registry = (SceneRegistry)target;

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Validation", EditorStyles.boldLabel);

            bool hasErrors = false;

            // Validate TransitionSceneId references an entry
            if (string.IsNullOrEmpty(registry.TransitionSceneID))
            {
                EditorGUILayout.HelpBox(
                    "Transition Scene ID is not set. Scene management initialization will fail.",
                    MessageType.Error
                );
                
                hasErrors = true;
            }
            else if (registry.GetEntry(registry.TransitionSceneID) == null)
            {
                EditorGUILayout.HelpBox(
                    $"Transition Scene ID '{registry.TransitionSceneID}' " +
                    "does not match any entry in the Entries list.",
                    MessageType.Error
                );
                
                hasErrors = true;
            }

            // Validate InitialSceneId references an entry
            if (!string.IsNullOrEmpty(registry.InitialSceneID)
                && registry.GetEntry(registry.InitialSceneID) == null)
            {
                EditorGUILayout.HelpBox(
                    $"Initial Scene ID '{registry.InitialSceneID}' does not match " +
                    $"any entry in the Entries list.",
                    MessageType.Error
                );
                
                hasErrors = true;
            }

            // Validate entries
            HashSet<string> seenIDs = new HashSet<string>();
            IReadOnlyList<SceneEntry> entries = registry.Entries;

            for (int i = 0; i < entries.Count; i++)
            {
                SceneEntry entry = entries[i];

                if (string.IsNullOrEmpty(entry.SceneID))
                {
                    EditorGUILayout.HelpBox(
                        $"Entry {i} has an empty Scene ID.",
                        MessageType.Error
                    );
                    
                    hasErrors = true;
                    continue;
                }

                if (!seenIDs.Add(entry.SceneID))
                {
                    EditorGUILayout.HelpBox(
                        $"Duplicate Scene ID '{entry.SceneID}' at entry {i}.",
                        MessageType.Error
                    );
                    
                    hasErrors = true;
                }

                if (entry.SceneReference == null || !entry.SceneReference.RuntimeKeyIsValid())
                {
                    EditorGUILayout.HelpBox(
                        $"Entry '{entry.SceneID}'' has no valid Addressable reference. " +
                        $"GetAddress() will fall back to the Scene ID string.",
                        MessageType.Warning
                    );
                }

                if (entry.SkipContainerInstallation || string.IsNullOrEmpty(entry.InstallerTypeName)) continue;
                
                Type installerType = ResolveType(entry.InstallerTypeName);
                if (installerType != null) continue;
                
                EditorGUILayout.HelpBox(
                    $"Entry '{entry.SceneID}': installer type '{entry.InstallerTypeName}' " +
                    $"could not be resolved. Ensure the fully qualified type name is correct.",
                    MessageType.Error
                );
                        
                hasErrors = true;
            }

            if (!hasErrors)
            {
                EditorGUILayout.HelpBox(
                    $"All entries valid. {entries.Count} scene(s) configured.",
                    MessageType.Info
                );
            }
        }
        
        private static Type ResolveType(string typeName)
        {
            int commaIndex = typeName.IndexOf(',');
            string nameOnly = commaIndex >= 0
                ? typeName.Substring(0, commaIndex).Trim()
                : typeName;

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            for (int i = 0; i < assemblies.Length; i++)
            {
                Type type = assemblies[i].GetType(nameOnly);
                
                if (type == null) continue;
                
                return type;
            }

            return null;
        }
    }
}
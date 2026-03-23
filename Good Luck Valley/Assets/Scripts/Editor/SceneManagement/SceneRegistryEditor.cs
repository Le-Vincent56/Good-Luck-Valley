using System;
using System.Collections.Generic;
using System.Reflection;
using GoodLuckValley.Core.DI.Interfaces;
using GoodLuckValley.Core.SceneManagement.Data;
using GoodLuckValley.Core.Utilities;
using UnityEditor;
using UnityEngine;

namespace GoodLuckValley.Editor.SceneManagement
{
    /// <summary>
    /// Custom inspector for <see cref="SceneRegistry"/>. Draws each scene entry
    /// with an installer type dropdown (populated from assembly-scanned IInstaller /
    /// IScopedInstaller types), auto-computed stable IDs, and validation.
    /// </summary>
    [CustomEditor(typeof(SceneRegistry))]
    public class SceneRegistryEditor : UnityEditor.Editor
    {
        private static string[] _installerTypeNames;
        private static string[] _installerDisplayNames;
        private static bool[] _installerIsScoped;

        private SerializedProperty _entriesProperty;
        private SerializedProperty _transitionSceneIDProperty;
        private SerializedProperty _initialSceneIDProperty;

        private void OnEnable()
        {
            _entriesProperty = serializedObject.FindProperty("entries");
            _transitionSceneIDProperty = serializedObject.FindProperty("transitionSceneID");
            _initialSceneIDProperty = serializedObject.FindProperty("initialSceneID");

            if (_installerTypeNames == null)
                CacheInstallerTypes();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_transitionSceneIDProperty);
            EditorGUILayout.PropertyField(_initialSceneIDProperty);

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Scene Entries", EditorStyles.boldLabel);

            for (int i = 0; i < _entriesProperty.arraySize; i++)
            {
                if (!DrawEntry(i))
                {
                    // Entry was deleted — don't increment, re-check this index
                    i--;
                }
            }

            if (GUILayout.Button("Add Entry"))
                _entriesProperty.InsertArrayElementAtIndex(_entriesProperty.arraySize);

            serializedObject.ApplyModifiedProperties();

            // Validation (uses public API, runs after Apply)
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Validation", EditorStyles.boldLabel);
            DrawValidation();
        }

        /// <summary>
        /// Draws an entry in the Scene Registry editor. Handles rendering properties for an individual entry,
        /// such as scene ID, scene reference, installer type, and additional flags. Provides functionality to
        /// remove the entry or update its values.
        /// </summary>
        /// <param name="index">The index of the entry to draw within the serialized entries array.</param>
        /// <returns>
        /// True if the entry is successfully rendered and retained, or false if the entry is removed.
        /// </returns>
        private bool DrawEntry(int index)
        {
            SerializedProperty entry = _entriesProperty.GetArrayElementAtIndex(index);

            SerializedProperty sceneIDProp = entry.FindPropertyRelative("sceneID");
            SerializedProperty sceneRefProp = entry.FindPropertyRelative("sceneReference");
            SerializedProperty installerProp = entry.FindPropertyRelative("installerTypeName");
            SerializedProperty isScopedProp = entry.FindPropertyRelative("isScoped");
            SerializedProperty skipProp = entry.FindPropertyRelative("skipContainerInstallation");
            SerializedProperty stableIDProp = entry.FindPropertyRelative("stableID");

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            // Header with remove button
            EditorGUILayout.BeginHorizontal();
            string label = string.IsNullOrEmpty(sceneIDProp.stringValue)
                ? $"Entry {index}"
                : sceneIDProp.stringValue;
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                _entriesProperty.DeleteArrayElementAtIndex(index);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                return false;
            }

            EditorGUILayout.EndHorizontal();

            // Scene ID — track changes for auto-computing stable ID
            string previousSceneID = sceneIDProp.stringValue;
            EditorGUILayout.PropertyField(sceneIDProp);

            if (sceneIDProp.stringValue != previousSceneID
                && !string.IsNullOrEmpty(sceneIDProp.stringValue))
            {
                stableIDProp.intValue = HashUtility.ComputeStableHash(sceneIDProp.stringValue);
            }

            // Addressable reference
            EditorGUILayout.PropertyField(sceneRefProp);

            // Skip container installation
            EditorGUILayout.PropertyField(skipProp);

            if (!skipProp.boolValue)
            {
                // Installer type dropdown
                int currentIndex = FindInstallerIndex(installerProp.stringValue);
                int newIndex = EditorGUILayout.Popup("Installer Type", currentIndex, _installerDisplayNames);

                if (newIndex != currentIndex)
                {
                    installerProp.stringValue = _installerTypeNames[newIndex];
                    isScopedProp.boolValue = _installerIsScoped[newIndex];
                }

                // Show IsScoped as read-only (auto-set from installer type)
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(isScopedProp);
                EditorGUI.EndDisabledGroup();
            }

            // Stable ID (read-only)
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(stableIDProp);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2);

            return true;
        }

        /// <summary>
        /// Finds the index of the installer type in the list of installer type names.
        /// Returns 0 if the type name does not match any entries or if it is null or empty.
        /// </summary>
        /// <param name="typeName">The name of the installer type to locate.</param>
        /// <returns>The index of the specified installer type in the installer type names array.</returns>
        private static int FindInstallerIndex(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) return 0;

            for (int i = 0; i < _installerTypeNames.Length; i++)
            {
                if (_installerTypeNames[i] != typeName) continue;
                
                return i;
            }

            return 0;
        }

        private static void CacheInstallerTypes()
        {
            List<Type> types = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            for (int i = 0; i < assemblies.Length; i++)
            {
                Type[] assemblyTypes;

                try
                {
                    assemblyTypes = assemblies[i].GetTypes();
                }
                catch (ReflectionTypeLoadException)
                {
                    continue;
                }

                for (int j = 0; j < assemblyTypes.Length; j++)
                {
                    Type type = assemblyTypes[j];

                    if (type.IsAbstract || type.IsInterface)
                        continue;

                    bool isInstaller = typeof(IInstaller).IsAssignableFrom(type);
                    bool isScopedInstaller = typeof(IScopedInstaller).IsAssignableFrom(type);

                    if (!isInstaller && !isScopedInstaller)
                        continue;

                    types.Add(type);
                }
            }

            int count = types.Count;
            _installerTypeNames = new string[count + 1];
            _installerDisplayNames = new string[count + 1];
            _installerIsScoped = new bool[count + 1];

            _installerTypeNames[0] = "";
            _installerDisplayNames[0] = "(None)";
            _installerIsScoped[0] = false;

            for (int i = 0; i < count; i++)
            {
                Type type = types[i];
                _installerTypeNames[i + 1] = type.FullName;
                _installerDisplayNames[i + 1] = type.Name;
                _installerIsScoped[i + 1] = typeof(IScopedInstaller).IsAssignableFrom(type);
            }
        }

        private void DrawValidation()
        {
            SceneRegistry registry = (SceneRegistry)target;
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
                    $"Transition Scene ID '{registry.TransitionSceneID}' "
                    + "does not match any entry in the Entries list.",
                    MessageType.Error
                );

                hasErrors = true;
            }

            // Validate InitialSceneID references an entry
            if (!string.IsNullOrEmpty(registry.InitialSceneID) && registry.GetEntry(registry.InitialSceneID) == null)
            {
                EditorGUILayout.HelpBox(
                    $"Initial Scene ID '{registry.InitialSceneID}' does not match "
                    + "any entry in the Entries list.",
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
                        $"Entry '{entry.SceneID}' has no valid Addressable reference. "
                        + "GetAddress() will fall back to the Scene ID string.",
                        MessageType.Warning
                    );
                }

                if (entry.SkipContainerInstallation || string.IsNullOrEmpty(entry.InstallerTypeName)) continue;
                Type installerType = ResolveType(entry.InstallerTypeName);

                if (installerType != null) continue;
                
                EditorGUILayout.HelpBox(
                    $"Entry '{entry.SceneID}': installer type '{entry.InstallerTypeName}' "
                    + "could not be resolved. Ensure the fully qualified type name is correct.",
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

        /// <summary>
        /// Resolves a type from a given type name. This method searches all loaded
        /// assemblies in the current application domain to find a type that matches
        /// the specified name.
        /// </summary>
        /// <param name="typeName">
        /// The fully qualified or partial name of the type to resolve.
        /// If the type name includes an assembly name, it will be trimmed out for the search.
        /// </param>
        /// <returns>
        /// The resolved <see cref="Type"/> if found; otherwise, null if no matching type is located.
        /// </returns>
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using GoodLuckValley.Core.DI.Interfaces;
using GoodLuckValley.Core.SceneManagement.Data;
using GoodLuckValley.Core.Utilities;
using GoodLuckValley.World.LevelManagement.Adapters;
using GoodLuckValley.World.LevelManagement.Data;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace GoodLuckValley.Editor.LevelManagement
{
    /// <summary>
    /// Editor window that automates new level creation. Creates a scene file,
    /// <see cref="LevelData"/> ScriptableObject, <see cref="SceneEntry"/> in the
    /// <see cref="SceneRegistry"/>, Addressable entry, <see cref="LevelRegistry"/>
    /// entry, and bakes a default spawn point — all in one operation.
    /// </summary>
    public class LevelCreationWizard : EditorWindow
    {
        private const string DefaultScenePath = "Assets/Scenes/Levels/";
        private const string DefaultLevelDataPath = "Assets/Data/LevelManagement/Levels/";

        // UI elements
        private TextField _sceneNameField;
        private TextField _displayNameField;
        private TextField _areaIDField;
        private TextField _sceneSavePathField;
        private DropdownField _installerDropdown;
        private HelpBox _validationBox;
        private Button _createButton;

        // Cached data
        private string[] _installerTypeNames;
        private string[] _installerDisplayNames;
        private bool[] _installerIsScoped;
        private SceneRegistry _sceneRegistry;
        private LevelRegistry _levelRegistry;

        [MenuItem("Good Luck Valley/Level Tools/Create New Level")]
        private static void ShowWindow()
        {
            LevelCreationWizard window = GetWindow<LevelCreationWizard>("Create New Level");
            window.minSize = new Vector2(400, 340);
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            root.style.paddingTop = 8;
            root.style.paddingBottom = 8;
            root.style.paddingLeft = 12;
            root.style.paddingRight = 12;

            // Title
            Label title = new Label("Create New Level");
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.fontSize = 14;
            title.style.marginBottom = 8;
            root.Add(title);

            // Scene Name
            _sceneNameField = new TextField("Scene Name (ID)");
            _sceneNameField.RegisterValueChangedCallback(OnFieldChanged);
            root.Add(_sceneNameField);

            // Display Name
            _displayNameField = new TextField("Display Name");
            _displayNameField.RegisterValueChangedCallback(OnFieldChanged);
            root.Add(_displayNameField);

            // Area ID
            _areaIDField = new TextField("Area ID");
            root.Add(_areaIDField);

            // Scene Save Path with folder picker
            VisualElement pathRow = new VisualElement();
            pathRow.style.flexDirection = FlexDirection.Row;
            pathRow.style.marginTop = 4;

            _sceneSavePathField = new TextField("Scene Save Path");
            _sceneSavePathField.value = DefaultScenePath;
            _sceneSavePathField.style.flexGrow = 1;
            pathRow.Add(_sceneSavePathField);

            Button browseButton = new Button(OnBrowseClicked) { text = "..." };
            browseButton.style.width = 30;
            browseButton.style.alignSelf = Align.FlexEnd;
            pathRow.Add(browseButton);

            root.Add(pathRow);

            // Installer Type dropdown
            CacheInstallerTypes();
            CacheRegistries();

            List<string> installerChoices = new List<string>();
            int defaultIndex = 0;

            for (int i = 0; i < _installerDisplayNames.Length; i++)
            {
                installerChoices.Add(_installerDisplayNames[i]);

                if (i >= _installerTypeNames.Length || !_installerTypeNames[i].EndsWith("SampleLevelInstaller"))
                    continue;
                
                defaultIndex = i;
            }

            _installerDropdown = new DropdownField(
                "Installer Type", 
                installerChoices, 
                defaultIndex
            );
            _installerDropdown.style.marginTop = 4;
            root.Add(_installerDropdown);

            // Validation box
            _validationBox = new HelpBox("Scene Name is required.", HelpBoxMessageType.Error);
            _validationBox.style.marginTop = 12;
            root.Add(_validationBox);

            // Create button
            _createButton = new Button(OnCreateClicked) { text = "Create Level" };
            _createButton.style.height = 30;
            _createButton.style.marginTop = 8;
            _createButton.SetEnabled(false);
            root.Add(_createButton);
        }

        /// <summary>
        /// Handles the event triggered when the value of an input field changes within the Level Creation Wizard.
        /// This method is used to validate the user input to ensure compliance with required conditions.
        /// </summary>
        /// <param name="evt">The change event containing the new value of the field.</param>
        private void OnFieldChanged(ChangeEvent<string> evt) => Validate();

        /// <summary>
        /// Handles the click event for the browse button in the Level Creation Wizard.
        /// This method opens a folder selection dialog, allowing the user to specify a path
        /// for saving the scene. If a valid folder is selected, the scene save path field is updated accordingly.
        /// </summary>
        private void OnBrowseClicked()
        {
            string current = _sceneSavePathField.value;
            string selected = EditorUtility.OpenFolderPanel(
                "Select Scene Save Folder", 
                current, 
                ""
            );

            if (string.IsNullOrEmpty(selected))
                return;

            string dataPath = Application.dataPath;

            if (selected.StartsWith(dataPath))
            {
                _sceneSavePathField.value = "Assets" + selected.Substring(dataPath.Length) + "/";
            }
            else
            {
                _sceneSavePathField.value = selected + "/";
            }
        }

        /// <summary>
        /// Validates the user's input within the Level Creation Wizard to ensure correctness and compliance with existing project constraints before proceeding with the level creation process.
        /// This method checks the scene name, scene registry, level registry, file system for duplicate scenes, and the display name for potential issues or warnings.
        /// Based on the validation results, it provides appropriate feedback to the user.
        /// </summary>
        private void Validate()
        {
            string sceneName = _sceneNameField.value;

            if (string.IsNullOrEmpty(sceneName))
            {
                SetValidation("Scene Name is required.", HelpBoxMessageType.Error);
                return;
            }

            if (!_sceneRegistry)
            {
                SetValidation(
                    "No SceneRegistry asset found in the project.",
                    HelpBoxMessageType.Error
                );
                return;
            }

            if (!_levelRegistry)
            {
                SetValidation(
                    "No LevelRegistry asset found in the project.",
                    HelpBoxMessageType.Error
                );
                return;
            }

            if (_sceneRegistry.GetEntry(sceneName) != null)
            {
                SetValidation(
                    $"Scene ID '{sceneName}' already exists in SceneRegistry.",
                    HelpBoxMessageType.Error
                );
                return;
            }

            string scenePath = _sceneSavePathField.value + sceneName + ".unity";

            if (File.Exists(scenePath))
            {
                SetValidation(
                    $"Scene file already exists at '{scenePath}'.",
                    HelpBoxMessageType.Error
                );
                return;
            }

            if (string.IsNullOrEmpty(_displayNameField.value))
            {
                SetValidation(
                    "Display Name is empty — it will appear blank in UI.",
                    HelpBoxMessageType.Warning
                );
                return;
            }

            SetValidation("Ready to create level.", HelpBoxMessageType.Info);
        }

        /// <summary>
        /// Updates the validation message and message type displayed in the Level Creation Wizard
        /// and enables or disables the create button based on the validation state.
        /// </summary>
        /// <param name="message">The text message to display in the validation HelpBox.</param>
        /// <param name="messageType">
        /// The type of the message, indicating its severity or purpose
        /// (e.g., Info, Warning, or Error).
        /// </param>
        private void SetValidation(string message, HelpBoxMessageType messageType)
        {
            _validationBox.text = message;
            _validationBox.messageType = messageType;
            _createButton.SetEnabled(messageType != HelpBoxMessageType.Error);
        }

        /// <summary>
        /// Handles the event triggered when the "Create" button is clicked in the Level Creation Wizard.
        /// This method executes the logic for creating a new level based on the user-defined parameters.
        /// </summary>
        private void OnCreateClicked()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            string sceneName = _sceneNameField.value;
            string displayName = _displayNameField.value;
            string areaID = _areaIDField.value;
            string sceneSavePath = _sceneSavePathField.value;
            int installerIndex = _installerDropdown.index;
            string scenePath = sceneSavePath + sceneName + ".unity";

            // Ensure directories exist
            EnsureDirectoryExists(sceneSavePath);
            EnsureDirectoryExists(DefaultLevelDataPath);

            // 1. Create new empty scene
            Scene newScene = EditorSceneManager.NewScene(
                NewSceneSetup.EmptyScene, 
                NewSceneMode.Single
            );

            // 2. Add default SpawnPoint_01
            GameObject spawnPointObj = new GameObject("SpawnPoint_01");
            SpawnPointMarker marker = spawnPointObj.AddComponent<SpawnPointMarker>();

            SerializedObject serializedMarker = new SerializedObject(marker);
            serializedMarker.FindProperty("spawnPointID").stringValue = "SpawnPoint_01";
            serializedMarker.FindProperty("faceRight").boolValue = true;
            serializedMarker.ApplyModifiedPropertiesWithoutUndo();

            // 3. Save scene
            EditorSceneManager.SaveScene(newScene, scenePath);
            AssetDatabase.Refresh();

            // 4. Create LevelData SO
            LevelData levelData = CreateInstance<LevelData>();
            string levelDataPath = DefaultLevelDataPath + sceneName + ".asset";

            SerializedObject levelDataSO = new SerializedObject(levelData);
            levelDataSO.FindProperty("sceneID").stringValue = sceneName;
            levelDataSO.FindProperty("displayName").stringValue = displayName;
            levelDataSO.FindProperty("areaID").stringValue = areaID;
            levelDataSO.FindProperty("stableID").intValue = HashUtility.ComputeStableHash(sceneName);
            levelDataSO.ApplyModifiedPropertiesWithoutUndo();

            AssetDatabase.CreateAsset(levelData, levelDataPath);

            // 5. Add SceneEntry to SceneRegistry
            string sceneGUID = AssetDatabase.AssetPathToGUID(scenePath);
            string installerTypeName = _installerTypeNames[installerIndex];
            bool isScoped = _installerIsScoped[installerIndex];

            SerializedObject registrySO = new SerializedObject(_sceneRegistry);
            SerializedProperty entriesProp = registrySO.FindProperty("_entries");
            entriesProp.InsertArrayElementAtIndex(entriesProp.arraySize);

            SerializedProperty newEntry = entriesProp.GetArrayElementAtIndex(entriesProp.arraySize - 1);
            newEntry.FindPropertyRelative("sceneID").stringValue = sceneName;
            newEntry.FindPropertyRelative("installerTypeName").stringValue = installerTypeName;
            newEntry.FindPropertyRelative("isScoped").boolValue = isScoped;
            newEntry.FindPropertyRelative("skipContainerInstallation").boolValue = false;
            newEntry.FindPropertyRelative("stableID").intValue = HashUtility.ComputeStableHash(sceneName);
            newEntry.FindPropertyRelative("sceneReference").FindPropertyRelative("m_AssetGUID").stringValue = sceneGUID;

            registrySO.ApplyModifiedProperties();

            // 6. Mark scene as Addressable
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

            if (settings)
            {
                AddressableAssetGroup scenesGroup = settings.FindGroup("Scenes");

                if (!scenesGroup)
                {
                    Debug.LogWarning(
                        "[LevelCreationWizard] Addressable group 'Scenes' not found. " +
                        "Falling back to the default group."
                    );
                    scenesGroup = settings.DefaultGroup;
                }
                
                AddressableAssetEntry addressableEntry = settings.CreateOrMoveEntry(
                    sceneGUID, 
                    scenesGroup
                );
                addressableEntry.address = sceneName;
            }
            else
            {
                Debug.LogWarning(
                    "[LevelCreationWizard] Addressable settings not found. "
                    + "Scene was not marked as Addressable — do this manually."
                );
            }

            // 7. Add LevelData to LevelRegistry.AllLevels
            SerializedObject levelRegistrySO = new SerializedObject(_levelRegistry);
            SerializedProperty allLevelsProp = levelRegistrySO.FindProperty("_allLevels");
            allLevelsProp.InsertArrayElementAtIndex(allLevelsProp.arraySize);

            SerializedProperty newLevelRef = allLevelsProp.GetArrayElementAtIndex(allLevelsProp.arraySize - 1);
            newLevelRef.objectReferenceValue = levelData;

            levelRegistrySO.ApplyModifiedProperties();

            // 8. Bake spawn points
            SpawnPointBaker.BakeSpawnPointsFromOpenScene(levelData, newScene);

            // 9. Save all modified assets
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // 10. Scene is already open from step 1
            Debug.Log(
                $"[LevelCreationWizard] Created level '{sceneName}' — "
                + $"scene at '{scenePath}', LevelData at '{levelDataPath}'."
            );

            Close();
        }

        /// <summary>
        /// Caches references to the SceneRegistry and LevelRegistry assets used in the editor.
        /// This method searches the project for registry assets and loads them into memory if found.
        /// </summary>
        private void CacheRegistries()
        {
            string[] sceneRegistryGuids = AssetDatabase.FindAssets("t:SceneRegistry");

            if (sceneRegistryGuids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(sceneRegistryGuids[0]);
                _sceneRegistry = AssetDatabase.LoadAssetAtPath<SceneRegistry>(path);
            }

            string[] levelRegistryGuids = AssetDatabase.FindAssets("t:LevelRegistry");

            if (levelRegistryGuids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(levelRegistryGuids[0]);
                _levelRegistry = AssetDatabase.LoadAssetAtPath<LevelRegistry>(path);
            }
        }

        /// <summary>
        /// Gathers and caches a list of installer types available in the current application domain.
        /// This includes determining their fully qualified names, display names, and whether they implement specific interfaces.
        /// Used to populate and manage installer options for the Level Creation Wizard.
        /// </summary>
        private void CacheInstallerTypes()
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

        /// <summary>
        /// Ensures the specified directory exists by checking its existence and creating it if necessary.
        /// This is used to guarantee that the required directory structure is available for further operations.
        /// </summary>
        /// <param name="path">The path of the directory to check and create if absent.</param>
        private static void EnsureDirectoryExists(string path)
        {
            if (Directory.Exists(path)) return;
            
            Directory.CreateDirectory(path);
            AssetDatabase.Refresh();
        }
    }
}
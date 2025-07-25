/*******************************************************************************
The content of this file includes portions of the proprietary AUDIOKINETIC Wwise
Technology released in source code form as part of the game integration package.
The content of this file may not be used without valid licenses to the
AUDIOKINETIC Wwise Technology.
Note that the use of the game engine is subject to the Unity(R) Terms of
Service at https://unity3d.com/legal/terms-of-service
 
License Usage
 
Licensees holding valid licenses to the AUDIOKINETIC Wwise Technology may use
this file in accordance with the end user license agreement provided with the
software or, alternatively, in accordance with the terms contained
in a written agreement between you and Audiokinetic Inc.
Copyright (c) 2025 Audiokinetic Inc.
*******************************************************************************/

#if !(UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using System.Linq;
using UnityEngine.Serialization;

[System.Serializable]
public class WwiseSettings
{
	public const string Filename = "WwiseSettings.xml";

	public static string GitRepositoryLink
	{
		get
		{
			string wwiseVersion = AkUnitySoundEngine.WwiseVersion;
			string shortWwiseVersion = wwiseVersion.Substring(2, wwiseVersion.IndexOf("Build")-3); //-3 for the space and the 2 first character that are skipped.
			string repositoryLink = "https://github.com/audiokinetic/WwiseUnityAddressables.git";
			repositoryLink += $"#v{shortWwiseVersion}";
			return repositoryLink;
		}
	}

	public static string Path
	{
		get { return System.IO.Path.Combine(UnityEngine.Application.dataPath, Filename); }
	}

	public static bool Exists { get { return System.IO.File.Exists(Path); } }

	public bool CopySoundBanksAsPreBuildStep = true;
	public bool GenerateSoundBanksAsPreBuildStep = false;
	public string WwiseStreamingAssetsPath = AkBasePathGetter.DefaultBasePath;
	[FormerlySerializedAs("GeneratedSoundbanksPath")] public string RootOutputPath;

	public bool CreatedPicker = false;
	public bool CreateWwiseGlobal = true;
	public bool CreateWwiseListener = true;
	public bool ObjectReferenceAutoCleanup = true;
	public bool LoadSoundEngineInEditMode = true;
	public bool ShowMissingRigidBodyWarning = true;
	public bool ShowSpatialAudioWarningMsg = true;
	public string WwiseInstallationPathMac;
	public string WwiseInstallationPathWindows;
	public string WwiseProjectPath;

	public bool UseWaapi = true;
	public string WaapiPort = "8080";
	public string WaapiIP = "127.0.0.1";
	public bool AutoSyncWaapi = false;

	public bool UseGitRepository = true;
	public string PackageSource = "";
	public string AddressableBankFolder = "WwiseData/Bank";
	public bool UseCustomBuildScript = false;
	public string AddressableAssetBuilderPath =  "Assets/AddressableAssetsData/DataBuilders/BuildScriptWwisePacked.asset";
	public bool AutomaticallyUpdateExternalSourcesPath = false;
	public string ExternalSourcesPath = "WwiseData/Bank";

	public string XMLTranslatorTimeout = "0";	//Timeout (in ms) for error translator through SoundBanksInfo.xml. Set to 0 to disable.
	public string WaapiTranslatorTimeout = "0"; //Timeout (in ms) for error translator through WAAPI. Set to 0 to disable.


	[System.Xml.Serialization.XmlIgnore]
	public string WwiseInstallationPath
	{
#if UNITY_EDITOR_OSX
		get { return WwiseInstallationPathMac; }
		set { WwiseInstallationPathMac = value; }
#else
		get { return WwiseInstallationPathWindows; }
		set { WwiseInstallationPathWindows = value; }
#endif
	}

	internal static WwiseSettings LoadSettings()
	{
		var settings = new WwiseSettings();

		try
		{
			var path = Path;
			if (System.IO.File.Exists(path))
			{
				var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(WwiseSettings));
				using (var xmlFileStream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
					settings = xmlSerializer.Deserialize(xmlFileStream) as WwiseSettings;
			}
			else
			{
				var projectDir = System.IO.Path.GetDirectoryName(UnityEngine.Application.dataPath);
				var foundWwiseProjects = System.IO.Directory.GetFiles(projectDir, "*.wproj", System.IO.SearchOption.AllDirectories);
				if (foundWwiseProjects.Length > 0)
					settings.WwiseProjectPath = AkUtilities.MakeRelativePath(UnityEngine.Application.dataPath, foundWwiseProjects[0]);
				else
					settings.WwiseProjectPath = string.Empty;

				settings.WwiseStreamingAssetsPath = AkBasePathGetter.DefaultBasePath;
			}
		}
		catch
		{
		}

#if AK_WWISE_ADDRESSABLES && UNITY_ADDRESSABLES
		settings.CheckGeneratedBanksPath();
#endif
		return settings;
	}

#if AK_WWISE_ADDRESSABLES && UNITY_ADDRESSABLES
	public void CheckGeneratedBanksPath()
	{
			var fullRootOutputPath = AkUtilities.GetFullPath(UnityEngine.Application.dataPath, RootOutputPath);
			var appDataPath = UnityEngine.Application.dataPath.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);

			if (!fullRootOutputPath.Contains(appDataPath))
			{
				UnityEngine.Debug.LogWarning("RootOutputPath is currently set to a path outside of the Assets folder. Generated SoundBanks will not be properly imported for Addressables. Please change this in Project Settings > Wwise Integration.");
			}
	}
#endif


	public void SaveSettings()
	{
		try
		{
			var xmlDoc = new System.Xml.XmlDocument();
			var xmlSerializer = new System.Xml.Serialization.XmlSerializer(GetType());
			using (var xmlStream = new System.IO.MemoryStream())
			{
				var streamWriter = new System.IO.StreamWriter(xmlStream, System.Text.Encoding.UTF8);
				xmlSerializer.Serialize(streamWriter, this);
				xmlStream.Position = 0;
				xmlDoc.Load(xmlStream);
				xmlDoc.Save(Path);
			}
		}
		catch
		{
			UnityEngine.Debug.LogErrorFormat("WwiseUnity: Unable to save settings to file <{0}>. Please ensure that this file path can be written to.", Path);
		}
	}
}

public class AkWwiseEditorSettings
{
	private static WwiseSettings s_Instance;

	public static WwiseSettings Instance
	{
		get
		{
			if (s_Instance == null)
				s_Instance = WwiseSettings.LoadSettings();
			return s_Instance;
		}
	}

	public static void Reload()
	{
		s_Instance = WwiseSettings.LoadSettings();
	}

	public static string WwiseProjectAbsolutePath
	{
		get { return AkUtilities.GetFullPath(UnityEngine.Application.dataPath, Instance.WwiseProjectPath); }
	}

	public static string WwiseScriptableObjectRelativePath
	{
		get { return System.IO.Path.Combine(System.IO.Path.Combine("Assets", "Wwise"), "ScriptableObjects"); }
	}

#region GUI
#if UNITY_2018_3_OR_NEWER
	class SettingsProvider : UnityEditor.SettingsProvider
#else
	class EditorWindow : UnityEditor.EditorWindow
#endif
	{
		class Styles
		{
			public static string WwisePaths = "Wwise Paths";
			public static UnityEngine.GUIContent WwiseProjectPath = new UnityEngine.GUIContent("Wwise Project", "Location of the Wwise project associated with this game. It is recommended to put it in the Unity Project root folder, outside the Assets folder.");

			public static UnityEngine.GUIContent WwiseInstallationPath = new UnityEngine.GUIContent("Wwise Application Path", "Location of the Wwise application. This is required to generate the SoundBanks in Unity.");

			public static UnityEngine.GUIContent WwiseRootOutputPathContent = new UnityEngine.GUIContent("Wwise Root Output Path", "Location of the Generated SoundBanks. This should be the same as the Root Output Path in Wwise.");
			
			public static string AssetManagement = "Asset Management";
			public static UnityEngine.GUIContent StreamingAssetsPath = new UnityEngine.GUIContent("Wwise StreamingAssets Path", "Location of the SoundBanks relative to (and within) the StreamingAssets folder.");
			public static UnityEngine.GUIContent CopySoundBanksAsPreBuildStep = new UnityEngine.GUIContent("Copy SoundBanks at pre-Build step", "Copies the SoundBanks in the appropriate location for building and deployment. It is recommended to leave this box checked.");
			public static UnityEngine.GUIContent GenerateSoundBanksAsPreBuildStep = new UnityEngine.GUIContent("Generate SoundBanks at pre-Build step", "Generates the SoundBanks before copying them during pre-Build step. It is recommended to leave this box unchecked if SoundBanks are generated on a specific build machine.");

			public static string GlobalSettings = "Global Settings";
			public static UnityEngine.GUIContent CreateWwiseGlobal = new UnityEngine.GUIContent("Create WwiseGlobal GameObject", "The WwiseGlobal object is a GameObject that contains the Initializing and Terminating scripts for the Wwise Sound Engine. In the Editor workflow, it is added to every scene, so that it can be properly previewed in the Editor. In the game, only one instance is created, in the first scene, and it is persisted throughout the game. It is recommended to leave this box checked.");
			public static UnityEngine.GUIContent CreateWwiseListener = new UnityEngine.GUIContent("Add Listener to Main Camera", "In order for positioning to work, the AkAudioListener script needs to be attached to the main camera in every scene. If you wish for your listener to be attached to another GameObject, uncheck this box.");
			public static UnityEngine.GUIContent ObjectReferenceAutoCleanup = new UnityEngine.GUIContent("Auto-delete WwiseObjectReferences", "Components that reference Wwise objects such as Events, Banks, and Busses track these references using WwiseObjectReference assets that are created in the Wwise/ScriptableObjects folder. If this option is checked and a Wwise Object has been removed from the Wwise Project, when parsing the Wwise project structure, the corresponding asset in the Wwise/ScriptableObjects folder will be deleted.");
			public static UnityEngine.GUIContent LoadSoundEngineInEditMode = new UnityEngine.GUIContent("Load Sound Engine in Edit Mode", "Load the Sound Engine in Edit Mode. Disable this setting to verify the Sound Engine is properly enabled in-game.");

			public static string InEditorWarnings = "In Editor Warnings";
			public static UnityEngine.GUIContent ShowSpatialAudioWarningMsg = new UnityEngine.GUIContent("Show Spatial Audio Warnings", "Warnings will be displayed on Wwise components that are not configured for Spatial Audio to function properly. It is recommended to leave this box checked.");

			public static string WaapiSection = "Wwise Authoring API (WAAPI)";
			public static UnityEngine.GUIContent UseWaapi = new UnityEngine.GUIContent("Connect to Wwise");
			public static UnityEngine.GUIContent WaapiIP = new UnityEngine.GUIContent("WAAPI IP address");
			public static UnityEngine.GUIContent WaapiPort = new UnityEngine.GUIContent("WAAPI port");
			public static UnityEngine.GUIContent AutosyncSelection = new UnityEngine.GUIContent("Autosync Wwise Browser Selection");
			
			public static string AddressableInstallerSection = "Wwise Addressable Installer";
			public static UnityEngine.GUIContent UseGitRepository = new UnityEngine.GUIContent("Install the addressable from a git repository", "If true, will expect a git repository link to import the package, if false will install from a local folder.");
			public static UnityEngine.GUIContent PackageSource = new UnityEngine.GUIContent("Package Source", "Can be a git repository link or a local folder depending on the setting above.");
			public static UnityEngine.GUIContent AddressableBankFolder = new UnityEngine.GUIContent("Addressable SoundBanks Folder", "Where the banks will be generated. The path should be relative to the Asset Folder");
			public static UnityEngine.GUIContent UseCustomBuildScript = new UnityEngine.GUIContent("Use Custom Build Script", "If toggled on, specify a path to the custom build script in the AddressableAssetBuilderPath field. Otherwise the default Wwise Build script will be automatically created during the installation.");
			public static UnityEngine.GUIContent AddressableAssetBuilderPath = new UnityEngine.GUIContent("Addressable Asset Build Path", "Where the custom asset builder is located.");
			public static UnityEngine.GUIContent AutomaticallyUpdateExternalSourcesPath = new UnityEngine.GUIContent("Automatically Update External Sources Path", "If toggled on, the external sources path will be updated during the installation.");
			public static UnityEngine.GUIContent ExternalSourcesPath = new UnityEngine.GUIContent("External Sources Path", "The new external sources path after the addressable package installation.");
			
			public static string TranslatorSection = "Wwise Error Message Translator";
			public static UnityEngine.GUIContent XMLTranslatorTimeout = new UnityEngine.GUIContent("XML Translator Timeout", "Maximum time (ms) taken to convert numeric ID in errors through SoundBankInfo.xml. Set to 0 to disable. Change will be applied next time play mode is entered.");
			public static UnityEngine.GUIContent WaapiTranslatorTimeout = new UnityEngine.GUIContent("WAAPI Translator Timeout", "Maximum time (ms) taken to convert numeric ID in errors through WAAPI. Set to 0 to disable. Change will be applied next time play mode is entered.");

			private static UnityEngine.GUIStyle version;
			public static UnityEngine.GUIStyle Version
			{
				get
				{
					if (version != null)
						return version;

					version = new UnityEngine.GUIStyle(UnityEditor.EditorStyles.whiteLargeLabel);
					if (!UnityEngine.Application.HasProLicense())
					{
						version.active.textColor =
							version.focused.textColor =
							version.hover.textColor =
							version.normal.textColor = UnityEngine.Color.black;
					}
					return version;
				}
			}

			private static UnityEngine.GUIStyle textField;
			public static UnityEngine.GUIStyle TextField
			{
				get
				{
					if (textField == null)
						textField = new UnityEngine.GUIStyle("textfield");
					return textField;
				}
			}
		}

		private static bool Ellipsis()
		{
			return UnityEngine.GUILayout.Button("...", UnityEngine.GUILayout.Width(30));
		}

#if UNITY_2018_3_OR_NEWER
		private SettingsProvider(string path) : base(path, UnityEditor.SettingsScope.Project) { }

		[UnityEditor.SettingsProvider]
		public static UnityEditor.SettingsProvider CreateWwiseIntegrationSettingsProvider()
		{
			return new SettingsProvider("Project/Wwise Integration") { keywords = GetSearchKeywordsFromGUIContentProperties<Styles>() };
		}

		public override void OnDeactivate()
		{
			base.OnDeactivate();
			if(Instance.LoadSoundEngineInEditMode && !AkUnitySoundEngine.IsInitialized())
			{
				AkUnitySoundEngineInitialization.Instance.InitializeSoundEngine();
			}
			else if (!Instance.LoadSoundEngineInEditMode && AkUnitySoundEngine.IsInitialized())
			{
				AkUnitySoundEngineInitialization.Instance.TerminateSoundEngine();
			}
		}

		private bool IsFolderWwiseApplicationPath(string path)
		{
#if UNITY_EDITOR_OSX
			return path.Contains("Wwise.app");
#else
			string fullPath = Path.GetFullPath(Path.Combine(path, "Authoring\\x64\\Release\\bin\\Wwise.exe"));
			return File.Exists(fullPath);
#endif
		}

		public override void OnGUI(string searchContext)
#else
		[UnityEditor.MenuItem("Edit/Wwise Settings...", false, (int)AkWwiseWindowOrder.WwiseSettings)]
		public static void Init()
		{
			// Get existing open window or if none, make a new one:
			var window = GetWindow(typeof(EditorWindow));
			window.position = new UnityEngine.Rect(100, 100, 850, 360);
			window.titleContent = new UnityEngine.GUIContent("Wwise Settings");
		}

		private void OnGUI()
#endif
		{
			bool changed = false;

			var labelWidth = UnityEditor.EditorGUIUtility.labelWidth;
			UnityEditor.EditorGUIUtility.labelWidth += 100;

			var settings = Instance;

			UnityEngine.GUILayout.Label(string.Format("Wwise v{0} Settings.", AkUnitySoundEngine.WwiseVersion), Styles.Version);
			UnityEngine.GUILayout.Label(Styles.WwisePaths, UnityEditor.EditorStyles.boldLabel);

			using (new UnityEngine.GUILayout.VerticalScope("box"))
			{
				using (new UnityEngine.GUILayout.HorizontalScope())
				{
					UnityEditor.EditorGUILayout.PrefixLabel(Styles.WwiseProjectPath);
					UnityEditor.EditorGUILayout.SelectableLabel(settings.WwiseProjectPath, Styles.TextField, UnityEngine.GUILayout.Height(17));

					if (Ellipsis())
					{
						var OpenInPath = System.IO.Path.GetDirectoryName(AkUtilities.GetFullPath(UnityEngine.Application.dataPath, settings.WwiseProjectPath));
						var WwiseProjectPathNew = UnityEditor.EditorUtility.OpenFilePanel("Select your Wwise Project", OpenInPath, "wproj");
						if (WwiseProjectPathNew.Length != 0)
						{
							if (WwiseProjectPathNew.EndsWith(".wproj") == false)
							{
								UnityEditor.EditorUtility.DisplayDialog("Error", "Please select a valid .wproj file", "Ok");
							}
							else
							{
								settings.WwiseProjectPath = AkUtilities.MakeRelativePath(UnityEngine.Application.dataPath, WwiseProjectPathNew);
								changed = true;
							}
						}
					}
				}

				using (new UnityEngine.GUILayout.HorizontalScope())
				{
					UnityEditor.EditorGUILayout.PrefixLabel(Styles.WwiseInstallationPath);
					UnityEditor.EditorGUILayout.SelectableLabel(settings.WwiseInstallationPath, Styles.TextField, UnityEngine.GUILayout.Height(17));

					if (Ellipsis())
					{
#if UNITY_EDITOR_OSX
						var path = UnityEditor.EditorUtility.OpenFilePanel("Select your Wwise application.", "/Applications/", "");
#else
						var path = UnityEditor.EditorUtility.OpenFolderPanel("Select your Wwise application.", System.Environment.GetEnvironmentVariable("ProgramFiles(x86)"), "");
#endif
						if (path != "" && !IsFolderWwiseApplicationPath(path))
						{
							EditorUtility.DisplayDialog("Wwise Application Path could not be set", $"WwiseUnity: {path} did not contain a Wwise Authoring application.", "OK");
						}
						else if (path.Length != 0)
						{
							settings.WwiseInstallationPath = System.IO.Path.GetFullPath(path);
							changed = true;
						}
					}
				}

				using (new UnityEngine.GUILayout.HorizontalScope())
				{
					UnityEditor.EditorGUILayout.PrefixLabel(Styles.WwiseRootOutputPathContent);
					UnityEditor.EditorGUILayout.SelectableLabel(settings.RootOutputPath, Styles.TextField, UnityEngine.GUILayout.Height(17));
					if (Ellipsis())
					{
#if UNITY_ADDRESSABLES && AK_WWISE_ADDRESSABLES
						var OpenInPath = System.IO.Path.GetDirectoryName(settings.RootOutputPath);
						var path = UnityEditor.EditorUtility.OpenFolderPanel("Select your generated SoundBanks destination folder", OpenInPath, settings.RootOutputPath.Substring(OpenInPath.Length + 1));
						if (path.Length != 0)
						{
							if (!path.Contains(UnityEngine.Application.dataPath))
							{
								UnityEditor.EditorUtility.DisplayDialog("Error", "The SoundBanks destination folder must be located within the Unity project 'Assets' folder.", "Ok");
							}
							else if (path == UnityEngine.Application.dataPath)
							{
								UnityEditor.EditorUtility.DisplayDialog("Error", "The SoundBanks destination folder cannot be the 'Assets' folder.", "Ok");
							}
							else
							{
								var previousPath = settings.RootOutputPath;
								if (previousPath != path)
								{
									settings.RootOutputPath = AkUtilities.MakeRelativePath(UnityEngine.Application.dataPath, path);
									var projectPath = AkUtilities.GetFullPath(UnityEngine.Application.dataPath, settings.WwiseProjectPath);
									var relPath = AkUtilities.MakeRelativePath(System.IO.Path.GetDirectoryName(projectPath), path);
									AkUtilities.SetSoundbanksDestinationFoldersInWproj(projectPath, relPath);
									var appDataPath = UnityEngine.Application.dataPath.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);

									if (!string.IsNullOrEmpty(previousPath) && System.IO.Directory.Exists(previousPath)) 
									{
										UnityEditor.AssetDatabase.Refresh();
										if (previousPath.Contains(appDataPath))
										{
											var newPath = AkUtilities.MakeRelativePath(UnityEngine.Application.dataPath, path);
											var destination = System.IO.Path.Combine("Assets", newPath);
											AkUtilities.MoveAssetsFromDirectory(previousPath, destination, true);
										}
										else
										{
											AkUtilities.DirectoryCopy(previousPath, path, true);
										}
										UnityEditor.AssetDatabase.Refresh();
									}
									changed = true;
								} 
							}
						}
#else
						var FullPath = AkUtilities.GetFullPath(UnityEngine.Application.streamingAssetsPath,
							settings.WwiseStreamingAssetsPath);
						var OpenInPath = System.IO.Path.GetDirectoryName(FullPath);
						var path = UnityEditor.EditorUtility.OpenFolderPanel("Select your Root Output Path", OpenInPath, FullPath.Substring(OpenInPath.Length + 1));
						if (path.Length != 0)
						{
							settings.RootOutputPath = AkUtilities.MakeRelativePath(UnityEngine.Application.dataPath, path);
							changed = true;
						}
#endif
					}
				}

#if !(AK_WWISE_ADDRESSABLES && UNITY_ADDRESSABLES)

				using (new UnityEngine.GUILayout.HorizontalScope())
				{
					UnityEditor.EditorGUILayout.PrefixLabel(Styles.StreamingAssetsPath);
					UnityEditor.EditorGUILayout.SelectableLabel(settings.WwiseStreamingAssetsPath, Styles.TextField, UnityEngine.GUILayout.Height(17));

					if (Ellipsis())
					{
						var FullPath = AkUtilities.GetFullPath(UnityEngine.Application.streamingAssetsPath, settings.WwiseStreamingAssetsPath);
						var OpenInPath = System.IO.Path.GetDirectoryName(FullPath);
						var path = UnityEditor.EditorUtility.OpenFolderPanel("Select your SoundBanks destination folder", OpenInPath, FullPath.Substring(OpenInPath.Length + 1));
						if (path.Length != 0)
						{
							var stremingAssetsIndex = UnityEngine.Application.dataPath.Split('/').Length;
							var folders = path.Split('/');

							if (folders.Length - 1 < stremingAssetsIndex || !string.Equals(folders[stremingAssetsIndex], "StreamingAssets", System.StringComparison.OrdinalIgnoreCase))
							{
								UnityEditor.EditorUtility.DisplayDialog("Error", "The SoundBank destination folder must be located within the Unity project 'StreamingAssets' folder.", "Ok");
							}
							else
							{
								var previousPath = settings.WwiseStreamingAssetsPath;
								var newPath = AkUtilities.MakeRelativePath(UnityEngine.Application.streamingAssetsPath, path);

								if (previousPath != newPath)
								{
									settings.WwiseStreamingAssetsPath = newPath;
									AkWwiseInitializationSettings.Instance.UserSettings.m_BasePath = newPath;
									changed = true;
								}
							}
						}
					}
				}
#endif
			}
#if !(AK_WWISE_ADDRESSABLES && UNITY_ADDRESSABLES)
			UnityEngine.GUILayout.Label(Styles.AssetManagement, UnityEditor.EditorStyles.boldLabel);
			using (new UnityEngine.GUILayout.VerticalScope("box"))
			{
				UnityEngine.GUILayout.Space(UnityEditor.EditorGUIUtility.standardVerticalSpacing);
				UnityEditor.EditorGUI.BeginChangeCheck();
				settings.CopySoundBanksAsPreBuildStep = UnityEditor.EditorGUILayout.Toggle(Styles.CopySoundBanksAsPreBuildStep, settings.CopySoundBanksAsPreBuildStep);
				UnityEngine.GUI.enabled = settings.CopySoundBanksAsPreBuildStep;
				settings.GenerateSoundBanksAsPreBuildStep = UnityEditor.EditorGUILayout.Toggle(Styles.GenerateSoundBanksAsPreBuildStep, settings.GenerateSoundBanksAsPreBuildStep);
				UnityEngine.GUI.enabled = true;
				if (UnityEditor.EditorGUI.EndChangeCheck())
					changed = true;
			}
#endif

			UnityEngine.GUILayout.Space(UnityEditor.EditorGUIUtility.standardVerticalSpacing);
			UnityEngine.GUILayout.Label(Styles.GlobalSettings, UnityEditor.EditorStyles.boldLabel);

			UnityEditor.EditorGUI.BeginChangeCheck();
			using (new UnityEngine.GUILayout.VerticalScope("box"))
			{
				settings.CreateWwiseGlobal = UnityEditor.EditorGUILayout.Toggle(Styles.CreateWwiseGlobal, settings.CreateWwiseGlobal);
				settings.CreateWwiseListener = UnityEditor.EditorGUILayout.Toggle(Styles.CreateWwiseListener, settings.CreateWwiseListener);
				settings.ObjectReferenceAutoCleanup = UnityEditor.EditorGUILayout.Toggle(Styles.ObjectReferenceAutoCleanup, settings.ObjectReferenceAutoCleanup);
				settings.LoadSoundEngineInEditMode = UnityEditor.EditorGUILayout.Toggle(Styles.LoadSoundEngineInEditMode, settings.LoadSoundEngineInEditMode);
			}

			UnityEngine.GUILayout.Space(UnityEditor.EditorGUIUtility.standardVerticalSpacing);
			UnityEngine.GUILayout.Label(Styles.InEditorWarnings, UnityEditor.EditorStyles.boldLabel);

			using (new UnityEngine.GUILayout.VerticalScope("box"))
			{
				settings.ShowSpatialAudioWarningMsg = UnityEditor.EditorGUILayout.Toggle(Styles.ShowSpatialAudioWarningMsg, settings.ShowSpatialAudioWarningMsg);
			}

			UnityEngine.GUILayout.Space(UnityEditor.EditorGUIUtility.standardVerticalSpacing);
			UnityEngine.GUILayout.Label(Styles.WaapiSection, UnityEditor.EditorStyles.boldLabel);
			using (new UnityEngine.GUILayout.VerticalScope("box"))
			{
				settings.UseWaapi = UnityEditor.EditorGUILayout.Toggle(Styles.UseWaapi, settings.UseWaapi);
				settings.WaapiPort = UnityEditor.EditorGUILayout.TextField(Styles.WaapiPort, settings.WaapiPort);
				settings.WaapiIP = UnityEditor.EditorGUILayout.TextField(Styles.WaapiIP, settings.WaapiIP);
				settings.AutoSyncWaapi = UnityEditor.EditorGUILayout.Toggle(Styles.AutosyncSelection, settings.AutoSyncWaapi);
			}
			
			UnityEngine.GUILayout.Space(UnityEditor.EditorGUIUtility.standardVerticalSpacing);
			UnityEngine.GUILayout.Label(Styles.AddressableInstallerSection, UnityEditor.EditorStyles.boldLabel);
			using (new UnityEngine.GUILayout.VerticalScope("box"))
			{
				settings.UseGitRepository = UnityEditor.EditorGUILayout.Toggle(Styles.UseGitRepository, settings.UseGitRepository);
				if (settings.UseGitRepository)
				{
					EditorGUI.BeginDisabledGroup(true); 
					settings.PackageSource = UnityEditor.EditorGUILayout.TextField(Styles.PackageSource, WwiseSettings.GitRepositoryLink);
					EditorGUI.EndDisabledGroup();
				}
				else
				{
					if (settings.PackageSource.StartsWith("https://"))
					{
						settings.PackageSource = "";
					}
					using (new UnityEngine.GUILayout.HorizontalScope())
					{
						UnityEditor.EditorGUILayout.PrefixLabel(Styles.PackageSource);
						UnityEditor.EditorGUILayout.SelectableLabel(settings.PackageSource, Styles.TextField, UnityEngine.GUILayout.Height(17));
						if (Ellipsis())
						{
#if UNITY_EDITOR_OSX
							var path = UnityEditor.EditorUtility.OpenFilePanel("Select your Wwise Addressable source.", "/Applications/", "");
#else
							var path = UnityEditor.EditorUtility.OpenFolderPanel("Select your Wwise Addressable source.", System.Environment.GetEnvironmentVariable("ProgramFiles(x86)"), "");
#endif
							if (path.Length != 0)
							{
								settings.PackageSource = System.IO.Path.GetFullPath(path);
								changed = true;
							}
						}
					}
				}
				using (new UnityEngine.GUILayout.HorizontalScope())
				{
					UnityEditor.EditorGUILayout.PrefixLabel(Styles.AddressableBankFolder);
					UnityEditor.EditorGUILayout.SelectableLabel(settings.AddressableBankFolder, Styles.TextField, UnityEngine.GUILayout.Height(17));
					if (Ellipsis())
					{
						var FullPath = AkUtilities.GetFullPath(UnityEngine.Application.dataPath,settings.RootOutputPath);
						var OpenInPath = System.IO.Path.GetDirectoryName(FullPath);
						var path = UnityEditor.EditorUtility.OpenFolderPanel(
							"Select Addressable Bank folder", OpenInPath,
							FullPath.Substring(OpenInPath.Length + 1));
						if (path.Length != 0)
						{
							if (!path.Contains(UnityEngine.Application.dataPath))
							{
								UnityEditor.EditorUtility.DisplayDialog("Error",
									"The SoundBanks destination folder must be located within the Unity project 'Assets' folder.",
									"Ok");
							}
							else if (path == UnityEngine.Application.dataPath)
							{
								UnityEditor.EditorUtility.DisplayDialog("Error",
									"The SoundBanks destination folder cannot be the 'Assets' folder.", "Ok");
							}
							else
							{
								settings.AddressableBankFolder = AkUtilities.MakeRelativePath(UnityEngine.Application.dataPath, path);
								changed = true;
							}
						}
					}
				}
				settings.UseCustomBuildScript = UnityEditor.EditorGUILayout.Toggle(Styles.UseCustomBuildScript, settings.UseCustomBuildScript);
				if (settings.UseCustomBuildScript)
				{
					using (new UnityEngine.GUILayout.HorizontalScope())
					{
						UnityEditor.EditorGUILayout.PrefixLabel(Styles.AddressableAssetBuilderPath);
						UnityEditor.EditorGUILayout.SelectableLabel(settings.AddressableAssetBuilderPath, Styles.TextField, UnityEngine.GUILayout.Height(17));
						if (Ellipsis())
						{
							var path = UnityEditor.EditorUtility.OpenFilePanel("Select the Addressable Asset Builder Path.", UnityEngine.Application.dataPath, "asset");
							if (path.Length != 0)
							{
								if (!path.Contains(UnityEngine.Application.dataPath))
								{
									UnityEditor.EditorUtility.DisplayDialog("Error", "The SoundBanks destination folder must be located within the Unity project 'Assets' folder.", "Ok");
								}
								else if (path == UnityEngine.Application.dataPath)
								{
									UnityEditor.EditorUtility.DisplayDialog("Error", "The SoundBanks destination folder cannot be the 'Assets' folder.", "Ok");
								}
								else
								{
									settings.AddressableAssetBuilderPath = AkUtilities.MakeRelativePath(UnityEngine.Application.dataPath, path);
									changed = true;
								}
							}
						}
					}
				}
				settings.AutomaticallyUpdateExternalSourcesPath = UnityEditor.EditorGUILayout.Toggle(Styles.AutomaticallyUpdateExternalSourcesPath, settings.AutomaticallyUpdateExternalSourcesPath);
				if (settings.AutomaticallyUpdateExternalSourcesPath)
				{
					using (new UnityEngine.GUILayout.HorizontalScope())
					{
						UnityEditor.EditorGUILayout.PrefixLabel(Styles.ExternalSourcesPath);
						UnityEditor.EditorGUILayout.SelectableLabel(settings.ExternalSourcesPath, Styles.TextField, UnityEngine.GUILayout.Height(17));
						if (Ellipsis())
						{
							var path = UnityEditor.EditorUtility.OpenFolderPanel(
								"Select External Sources folder", UnityEngine.Application.dataPath,
								"");
							if (path.Length != 0)
							{
								if (!path.Contains(UnityEngine.Application.dataPath))
								{
									UnityEditor.EditorUtility.DisplayDialog("Error",
										"The External Sources destination folder must be located within the Unity project 'Assets' folder.",
										"Ok");
								}
								else if (path == UnityEngine.Application.dataPath)
								{
									UnityEditor.EditorUtility.DisplayDialog("Error",
										"The External Sources destination folder cannot be the 'Assets' folder.", "Ok");
								}
								else
								{
									settings.ExternalSourcesPath = AkUtilities.MakeRelativePath(UnityEngine.Application.dataPath, path);
									changed = true;
								}
							}
						}
					}
				}
			}
		
			UnityEngine.GUILayout.Space(UnityEditor.EditorGUIUtility.standardVerticalSpacing);
			UnityEngine.GUILayout.Label(Styles.TranslatorSection, UnityEditor.EditorStyles.boldLabel);
			using (new UnityEngine.GUILayout.VerticalScope("box"))
			{
				settings.XMLTranslatorTimeout = UnityEditor.EditorGUILayout.TextField(Styles.XMLTranslatorTimeout, settings.XMLTranslatorTimeout);
				settings.WaapiTranslatorTimeout = UnityEditor.EditorGUILayout.TextField(Styles.WaapiTranslatorTimeout, settings.WaapiTranslatorTimeout);
			}

			if (UnityEditor.EditorGUI.EndChangeCheck())
				changed = true;

			UnityEngine.GUILayout.Space(UnityEditor.EditorGUIUtility.standardVerticalSpacing);

			UnityEditor.EditorGUIUtility.labelWidth = labelWidth;

			if (changed)
			{
				settings.SaveSettings();
			}
		}
#endregion
	}
}
#endif // UNITY_EDITOR
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.

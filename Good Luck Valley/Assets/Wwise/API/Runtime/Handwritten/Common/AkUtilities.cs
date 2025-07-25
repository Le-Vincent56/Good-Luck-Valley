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
using System;
using System.Linq;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;

public enum AkWwiseMenuOrder
{
	ConvertIDs = 200
}

public enum AkWwiseWindowOrder
{
	WwiseSettings = 305,
	WwiseInitializationSettings = 306,
	WwisePicker = 2300
}

public enum AkWwiseHelpOrder
{
	WwiseHelpOrder = 200
}

public partial class AkUtilities
{
	#region Migration
	/// <summary>
	/// These values represent the maximum value of the "Unity Integration Version" number in the Version.txt file that will migrated.
	/// For example, in Wwise v2019.1.0, "Unity Integration Version" is 18 which means that all migrations up until this version are required.
	/// </summary>
	public enum MigrationStep
	{
		AkGameObjListenerMask_v2016_1_0 = 9,
		AkGameObjPositionOffsetData_v2016_2_0 = 10,
		AkAudioListener_v2017_1_0 = 14,
		InitializationSettings_v2018_1_0 = 15,
		WwiseTypes_v2018_1_6 = 16,
		AkEventCallback_v2018_1_6 = 16,
		AkAmbient_v2019_1_0 = 17,
		NewScriptableObjectFolder_v2019_2_0 = 18,
		AutoDefinedSoundBanks_v2023_1_0 = 19,
		RootOutputPath_v2025_1_0 = 20,
		
		/// <summary>
		/// The value that is currently in the Version.txt file.
		/// </summary>
		Current
	}

	public static bool IsMigrationRequired(MigrationStep step)
	{
		return MigrationStartIndex <= (int)step;
	}

	public static bool IsMigrating
	{
		get { return MigrationStartIndex < MigrationStopIndex; }
	}

	public static void BeginMigration(int startIndex)
	{
		if (startIndex < MigrationStopIndex)
			MigrationStartIndex = startIndex;
	}

	public static void EndMigration()
	{
		MigrationStartIndex = MigrationStopIndex;
	}

	public const int MigrationStopIndex = (int)MigrationStep.Current;

	public static int MigrationStartIndex
	{
		private set { migrationStartIndex = value; }
		get { return migrationStartIndex; }
	}

	private static int migrationStartIndex = MigrationStopIndex;
	
	private static void MigrateRootOutputPath(string WwiseProjectPath)
	{
		if (string.IsNullOrEmpty(AkWwiseEditorSettings.Instance.RootOutputPath))
		{
			var doc = new System.Xml.XmlDocument { PreserveWhitespace = true };
			doc.Load(WwiseProjectPath);
			var Navigator = doc.CreateNavigator();

			// Navigate the wproj file (XML format) to where our setting should be
			var pathInXml = string.Format("/WwiseDocument/ProjectInfo/Project/PropertyList/Property[@Name='{0}']", "SoundBankHeaderFilePath");
			var expression = System.Xml.XPath.XPathExpression.Compile(pathInXml);
			var rootOutputPath = Navigator.SelectSingleNode(expression).GetAttribute("Value", "");
			rootOutputPath = AkUtilities.GetFullPath(System.IO.Path.GetDirectoryName(WwiseProjectPath), rootOutputPath);
			rootOutputPath = AkUtilities.MakeRelativePath(UnityEngine.Application.dataPath, rootOutputPath);
			AkWwiseEditorSettings.Instance.RootOutputPath = rootOutputPath;
			AkWwiseEditorSettings.Instance.SaveSettings();
			UnityEngine.Debug.Log($"wwiseunity: MIGRATION: migrating RootOutputPath to {rootOutputPath}");
		}
	}
	#endregion

	private static readonly System.Collections.Generic.Dictionary<string, string> s_ProjectBankPaths =
		new System.Collections.Generic.Dictionary<string, string>();

	private static System.DateTime s_LastBankPathUpdate = System.DateTime.MinValue;
	private static bool s_AutoBankEnabled = true;

	private static readonly System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>>
		s_BaseToCustomPF = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>>();

	public static bool IsWwiseProjectAvailable { set; get; }

	public static bool IsSoundbankGenerationAvailable()
	{
		return GetWwiseConsole() != null;
	}

	/// Executes a command-line. Blocks the calling thread until the new process has completed. Returns the logged stdout in one big string.
	public static string ExecuteCommandLine(string command, string arguments)
	{
		var process = new System.Diagnostics.Process();
		process.StartInfo.FileName = command;
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.CreateNoWindow = true;
		process.StartInfo.Arguments = arguments;
		process.Start();

		// Synchronously read the standard output of the spawned process. 
		var reader = process.StandardOutput;
		var output = reader.ReadToEnd();

		// Waiting for the process to exit directly in the UI thread. Similar cases are working that way too.

		// TODO: Is it better to provide a timeout avoid any issues of forever blocking the UI thread? If so, what is
		// a relevant timeout value for SoundBank generation?
		process.WaitForExit();
		process.Close();

		return output;
	}

	private static string GetWwiseConsole()
	{
		string result = null;

		var settings = AkWwiseEditorSettings.Instance;

#if UNITY_EDITOR_WIN
		if (!string.IsNullOrEmpty(settings.WwiseInstallationPathWindows))
		{
			result = System.IO.Path.Combine(settings.WwiseInstallationPathWindows, @"Authoring\x64\Release\bin\WwiseConsole.exe");

			if (!System.IO.File.Exists(result))
			{
				result = System.IO.Path.Combine(settings.WwiseInstallationPathWindows, @"Authoring\Win32\Release\bin\WwiseConsole.exe");
			}
		}
#elif UNITY_EDITOR_OSX
		if (!string.IsNullOrEmpty(settings.WwiseInstallationPathMac))
		{
			result = System.IO.Path.Combine(settings.WwiseInstallationPathMac, "Contents/Tools/WwiseConsole.sh");
		}
#endif

		if (result != null && System.IO.File.Exists(result))
		{
			return result;
		}

		return null;
	}

	public static bool GeneratingSoundBanks = false;

	// Generate all the SoundBanks for all the supported platforms in the Wwise project. This effectively calls Wwise for the project
	// that is configured in the UnityWwise integration.
	public static void GenerateSoundbanks(System.Collections.Generic.List<string> platforms = null)
	{
		GeneratingSoundBanks = true;
#if AK_WWISE_ADDRESSABLES && UNITY_ADDRESSABLES
		AkWwiseEditorSettings.Instance.CheckGeneratedBanksPath();
#endif
		var wwiseProjectFullPath = AkWwiseEditorSettings.WwiseProjectAbsolutePath;
		if (IsSoundbankOverrideEnabled(wwiseProjectFullPath))
		{
			UnityEngine.Debug.LogWarning(
				"The SoundBank generation process ignores the SoundBank Settings' Overrides currently enabled in the User settings. The project's SoundBank settings will be used.");
		}

		var wwiseConsole = GetWwiseConsole();
		if (wwiseConsole == null)
		{
			UnityEngine.Debug.LogError("Couldn't locate WwiseConsole, unable to generate SoundBanks.");
			return;
		}

#if UNITY_EDITOR_WIN
		var command = wwiseConsole;
		var arguments = "";
#elif UNITY_EDITOR_OSX
		var command = "/bin/sh";
		var arguments = "\"" + wwiseConsole + "\"";
#else
		var command = "";
		var arguments = "";
#endif
		arguments += " generate-soundbank";

		arguments += " \"" + wwiseProjectFullPath.Replace("\"","") + "\"";

		if (platforms != null && platforms.Count() >0)
		{
			arguments += " --platform";
			foreach (var platform in platforms)
			{
				if (!string.IsNullOrEmpty(platform))
				{
					arguments += " " + platform;
				}
			}
		}

		System.Threading.Tasks.Task.Run(() => RunSoundBankGeneration(command, arguments));
	}

	private static void RunSoundBankGeneration(string command, string arguments)
	{
		var output = ExecuteCommandLine(command, arguments);
		if (output.Contains("Process completed successfully."))
		{
			UnityEngine.Debug.LogFormat("WwiseUnity: SoundBanks generation successful:\n{0}", output);
		}
		else if (output.Contains("Process completed with warning"))
		{
			UnityEngine.Debug.LogWarningFormat("WwiseUnity: SoundBanks generation has warning(s):\n{0}", output);
		}
		else
		{
			UnityEngine.Debug.LogErrorFormat("WwiseUnity: SoundBanks generation error:\n{0}", output);
		}
		GeneratingSoundBanks = false;
		UnityEditor.AssetDatabase.Refresh();
	}

	/// Reads the user settings (not the project settings) to check if there is an override currently defined for the SoundBank generation folders.
	public static bool IsSoundbankOverrideEnabled(string wwiseProjectPath)
	{
		var userConfigFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(wwiseProjectPath),
			System.IO.Path.GetFileNameWithoutExtension(wwiseProjectPath) + "." + System.Environment.UserName + ".wsettings");

		if (!System.IO.File.Exists(userConfigFile))
		{
			return false;
		}

		var userConfigDoc = new System.Xml.XmlDocument();
		userConfigDoc.Load(userConfigFile);
		var userConfigNavigator = userConfigDoc.CreateNavigator();

		var userConfigNode = userConfigNavigator.SelectSingleNode(
			System.Xml.XPath.XPathExpression.Compile("//Property[@Name='SoundBankPathUserOverride' and @Value = 'True']"));

		return userConfigNode != null;
	}

	public static bool IsAutoBankEnabled()
	{
		return s_AutoBankEnabled;
	}

	public static System.Collections.Generic.IDictionary<string, System.Collections.Generic.List<string>> PlatformMapping
	{
		get { return s_BaseToCustomPF; }
	}

	public static System.Collections.Generic.IDictionary<string, string> GetAllBankPaths(string xmlFilePath)
	{
		UpdateSoundbanksDestinationFolders(xmlFilePath);
		return s_ProjectBankPaths;
	}


	public static System.Collections.Generic.IDictionary<string, string> GetAllBankPaths()
	{
		UpdateSoundbanksDestinationFolders(AkWwiseEditorSettings.WwiseProjectAbsolutePath);
		return s_ProjectBankPaths;
	}

	// Parses the .wproj to find out where SoundBanks are generated for the given path.
	public static string GetWwiseSoundBankDestinationFolder(string Platform)
	{
		try
		{
			UpdateSoundbanksDestinationFolders(AkWwiseEditorSettings.WwiseProjectAbsolutePath);
			return s_ProjectBankPaths[Platform];
		}
		catch
		{
			return "";
		}
	}

	public delegate void GetEventDurationsFunc(uint eventID, ref float maximum, ref float minimum);
	public static GetEventDurationsFunc GetEventDurations = (uint eventID, ref float maximum, ref float minimum) => { maximum = minimum = -1.0f; };

	private static void UpdateSoundbanksDestinationFolders(string WwiseProjectPath)
	{
		try
		{
			if (WwiseProjectPath.Length == 0)
			{
				return;
			}

			if (!AkUtilities.IsWwiseProjectAvailable)
			{
				IsWwiseProjectAvailable = System.IO.File.Exists(WwiseProjectPath);
				if (!IsWwiseProjectAvailable)
				{
					return;
				}
			}

			s_ProjectBankPaths.Clear();
			var doc = new System.Xml.XmlDocument();
			doc.Load(WwiseProjectPath);
			var Navigator = doc.CreateNavigator();

			// Gather the mapping of Custom platform to Base platform
			var itpf = Navigator.Select("//Platform");
			s_BaseToCustomPF.Clear();
			foreach (System.Xml.XPath.XPathNavigator node in itpf)
			{
				System.Collections.Generic.List<string> customList = null;
				var basePF = node.GetAttribute("ReferencePlatform", "");
				if (!s_BaseToCustomPF.TryGetValue(basePF, out customList))
				{
					customList = new System.Collections.Generic.List<string>();
					s_BaseToCustomPF[basePF] = customList;
				}

				customList.Add(node.GetAttribute("Name", ""));
			}

			// Navigate the wproj file (XML format) to where generated SoundBank paths are stored
			var it = Navigator.Select("//Property[@Name='SoundBankPaths']/ValueList/Value");
			foreach (System.Xml.XPath.XPathNavigator node in it)
			{
				var path = node.Value;
				FixSlashes(ref path);
				var pf = node.GetAttribute("Platform", "");
				s_ProjectBankPaths[pf] = path;
			}
		}
		catch (System.Exception ex)
		{
			UnityEngine.Debug.LogError("WwiseUnity: Error while reading project " + WwiseProjectPath + ". Exception: " + ex.Message);
		}
	}
	
	private static void UpdateAutoBankSetting(string WwiseProjectPath)
	{
		var doc = new System.Xml.XmlDocument { PreserveWhitespace = true };
		doc.Load(WwiseProjectPath);
		var Navigator = doc.CreateNavigator();

		// Navigate the wproj file (XML format) to where our setting should be
		var pathInXml = string.Format("/WwiseDocument/ProjectInfo/Project/PropertyList/Property[@Name='{0}']", "AutoSoundBankEnabled");
		var expression = System.Xml.XPath.XPathExpression.Compile(pathInXml);
		var node = Navigator.SelectSingleNode(expression);
		s_AutoBankEnabled = node != null;
		AkWwiseInitializationSettings.Instance.IsAutoBankEnabled = s_AutoBankEnabled;
	}
	
	public static string GetRootOutputPath()
	{
		if (AkWwiseEditorSettings.Instance.RootOutputPath == null)
		{
			MigrateRootOutputPath(AkUtilities.GetFullPath(UnityEngine.Application.dataPath,AkWwiseEditorSettings.Instance.WwiseProjectPath));
		}
		return GetFullPath(UnityEngine.Application.dataPath, AkWwiseEditorSettings.Instance.RootOutputPath);
	}
	
	public static void SetWwiseRootOutputPath(string WwiseProjectPath, string destinationPath)
	{
		try
		{
			if (WwiseProjectPath.Length == 0)
			{
				return;
			}

			if (!System.IO.File.Exists(WwiseProjectPath))
			{
				return;
			}

			s_ProjectBankPaths.Clear();

			var doc = new System.Xml.XmlDocument();
			doc.Load(WwiseProjectPath);
			var Navigator = doc.CreateNavigator();

			// Navigate the wproj file (XML format) to where generated SoundBank paths are stored
			var it = Navigator.Select("//Property[@Name='SoundBankHeaderFilePath']");
			foreach (System.Xml.XPath.XPathNavigator node in it)
			{
				if (node.MoveToAttribute("Value", ""))
				{
					var path = $"{destinationPath}";
					FixSlashes(ref path);
					node.SetValue(path);
				}
			}
			doc.Save(WwiseProjectPath);
		}
		catch (System.Exception ex)
		{
			UnityEngine.Debug.LogError("WwiseUnity: Error while reading project " + WwiseProjectPath + ". Exception: " + ex.Message);
		}
	}
	
	public static void SetPlatformsSoundBankPath(string WwiseProjectPath, string destinationPath)
	{
		try
		{
			if (WwiseProjectPath.Length == 0)
			{
				return;
			}

			if (!System.IO.File.Exists(WwiseProjectPath))
			{
				return;
			}

			s_ProjectBankPaths.Clear();

			var doc = new System.Xml.XmlDocument();
			doc.Load(WwiseProjectPath);
			var Navigator = doc.CreateNavigator();

			// Gather the mapping of Custom platform to Base platform
			var itpf = Navigator.Select("//Platform");
			s_BaseToCustomPF.Clear();
			foreach (System.Xml.XPath.XPathNavigator node in itpf)
			{
				System.Collections.Generic.List<string> customList = null;
				var basePF = node.GetAttribute("ReferencePlatform", "");
				if (!s_BaseToCustomPF.TryGetValue(basePF, out customList))
				{
					customList = new System.Collections.Generic.List<string>();
					s_BaseToCustomPF[basePF] = customList;
				}

				customList.Add(node.GetAttribute("Name", ""));
			}

			// Navigate the wproj file (XML format) to where generated SoundBank paths are stored
			var it = Navigator.Select("//Property[@Name='SoundBankPaths']/ValueList/Value");
			foreach (System.Xml.XPath.XPathNavigator node in it)
			{
				var pf = node.GetAttribute("Platform", "");
				var path = $"{destinationPath}/{pf}";
				FixSlashes(ref path);
				node.SetValue(path);
				s_ProjectBankPaths[pf] = path;
			}
			doc.Save(WwiseProjectPath);
		}
		catch (System.Exception ex)
		{
			UnityEngine.Debug.LogError("WwiseUnity: Error while reading project " + WwiseProjectPath + ". Exception: " + ex.Message);
		}
	}

	public static void SetSoundbanksDestinationFoldersInWproj(string WwiseProjectPath, string destinationPath)
	{
		try
		{
			if (WwiseProjectPath.Length == 0)
			{
				return;
			}

			if (!System.IO.File.Exists(WwiseProjectPath))
			{
				return;
			}

			s_ProjectBankPaths.Clear();

			var doc = new System.Xml.XmlDocument();
			doc.Load(WwiseProjectPath);
			var Navigator = doc.CreateNavigator();

			// Gather the mapping of Custom platform to Base platform
			var itpf = Navigator.Select("//Platform");
			s_BaseToCustomPF.Clear();
			foreach (System.Xml.XPath.XPathNavigator node in itpf)
			{
				System.Collections.Generic.List<string> customList = null;
				var basePF = node.GetAttribute("ReferencePlatform", "");
				if (!s_BaseToCustomPF.TryGetValue(basePF, out customList))
				{
					customList = new System.Collections.Generic.List<string>();
					s_BaseToCustomPF[basePF] = customList;
				}

				customList.Add(node.GetAttribute("Name", ""));
			}

			// Navigate the wproj file (XML format) to where generated SoundBank paths are stored
			var it = Navigator.Select("//Property[@Name='SoundBankPaths']/ValueList/Value");
			foreach (System.Xml.XPath.XPathNavigator node in it)
			{
				var pf = node.GetAttribute("Platform", "");
				var path = $"{destinationPath}/{pf}";
				FixSlashes(ref path);
				node.SetValue(path);
				s_ProjectBankPaths[pf] = path;
			}
			it = Navigator.Select("//Property[@Name='SoundBankHeaderFilePath']");
			foreach (System.Xml.XPath.XPathNavigator node in it)
			{
				if (node.MoveToAttribute("Value", ""))
				{
					var path = $"{destinationPath}";
					FixSlashes(ref path);
					node.SetValue(path);
				}
			}
			doc.Save(WwiseProjectPath);
		}
		catch (System.Exception ex)
		{
			UnityEngine.Debug.LogError("WwiseUnity: Error while reading project " + WwiseProjectPath + ". Exception: " + ex.Message);
		}
	}
	
	public static void SetExternalSourceDestinationFolderInWproj(string WwiseProjectPath, string destinationPath)
	{
		try
		{
			if (WwiseProjectPath.Length == 0)
			{
				return;
			}

			if (!System.IO.File.Exists(WwiseProjectPath))
			{
				return;
			}

			s_ProjectBankPaths.Clear();

			var doc = new System.Xml.XmlDocument();
			doc.Load(WwiseProjectPath);
			var navigator = doc.CreateNavigator();

			// Navigate the wproj file (XML format) to where generated SoundBank paths are stored
			var it = navigator.Select("//Property[@Name='ExternalSourcesOutputPath']/ValueList/Value");
			foreach (System.Xml.XPath.XPathNavigator node in it)
			{
				var pf = node.GetAttribute("Platform", "");
				var path = $"{destinationPath}/{pf}";
				FixSlashes(ref path);
				node.SetValue(path);
				s_ProjectBankPaths[pf] = path;
			}
			doc.Save(WwiseProjectPath);
		}
		catch (System.Exception ex)
		{
			UnityEngine.Debug.LogError("WwiseUnity: Error while reading project " + WwiseProjectPath + ". Exception: " + ex.Message);
		}
	}

	private static void CheckWwiseProjectUpdate(string WwiseProjectPath)
	{
		try
		{
			if (WwiseProjectPath.Length == 0)
				return;

			if (!AkUtilities.IsWwiseProjectAvailable)
				return;

			var t = System.IO.File.GetLastWriteTime(WwiseProjectPath);
			if (t <= s_LastBankPathUpdate)
				return;
			s_LastBankPathUpdate = t;
			UpdateSoundbanksDestinationFolders(WwiseProjectPath);
			UpdateAutoBankSetting(WwiseProjectPath);
		}
		catch (System.Exception ex)
		{
			UnityEngine.Debug.LogError("WwiseUnity: Error while reading project " + WwiseProjectPath + ". Exception: " + ex.Message);
		}
	}

	public static void WwiseProjectUpdated(string WwiseProjectPath)
	{
		CheckWwiseProjectUpdate(WwiseProjectPath);
	}

	// Set SoundBank-related bool settings in the wproj file.
	public static bool ToggleBoolSoundbankSettingInWproj(string[] SettingName, string WwiseProjectPath, bool Enable = true)
	{
		try
		{
			if (WwiseProjectPath.Length == 0)
			{
				return true;
			}
			
			var doc = new System.Xml.XmlDocument { PreserveWhitespace = true };
			doc.Load(WwiseProjectPath);
			var Navigator = doc.CreateNavigator();
			bool WprojWasEdited = false;

			foreach (var name in SettingName)
			{
				// Navigate the wproj file (XML format) to where our setting should be
				var pathInXml = string.Format("/WwiseDocument/ProjectInfo/Project/PropertyList/Property[@Name='{0}']",
					name);
				var expression = System.Xml.XPath.XPathExpression.Compile(pathInXml);
				var node = Navigator.SelectSingleNode(expression);
				if (node == null)
				{
					// Setting isn't in the wproj, add it
					// Navigate to the SoundBankHeaderFilePath property (it is always there)
					expression =
						System.Xml.XPath.XPathExpression.Compile(
							"/WwiseDocument/ProjectInfo/Project/PropertyList/Property[@Name='SoundBankHeaderFilePath']");
					node = Navigator.SelectSingleNode(expression);
					if (node == null)
					{
						// SoundBankHeaderFilePath not in wproj, invalid wproj file
						UnityEngine.Debug.LogError(
							"WwiseUnity: Could not find SoundBankHeaderFilePath property in Wwise project file. File is invalid.");
						return false;
					}

					// Add the setting right above SoundBankHeaderFilePath
					var propertyToInsert = string.Format("<Property Name=\"{0}\" Type=\"bool\" Value=\"{1}\"/>", name, Enable ? "True" : "False");
					node.InsertBefore(propertyToInsert);
					WprojWasEdited = true;
				}
				else if (node.GetAttribute("Value", "") == (Enable ? "False" : "True"))
				{
					// Value is present, we simply have to modify it.
					if (!node.MoveToAttribute("Value", ""))
					{
						return false;
					}

					// Modify the value to true
					node.SetValue(Enable ? "True" : "False");
					WprojWasEdited = true;
				}
			}

			if (WprojWasEdited)
			{
				doc.Save(WwiseProjectPath);
			}
			return true;
		}
		catch
		{
			return false;
		}
	}

	public static bool SetSoundbankHeaderFilePath(string WwiseProjectPath, string SoundbankPath)
	{
		if (string.IsNullOrEmpty(AkWwiseEditorSettings.Instance.RootOutputPath))
		{
			var defaultRootOutputPath = AkBasePathGetter.GetDefaultRootOutputPath();
			var pathRelativeToApplicationDataPath = AkUtilities.MakeRelativePath(UnityEngine.Application.dataPath, defaultRootOutputPath);
			AkWwiseEditorSettings.Instance.RootOutputPath = pathRelativeToApplicationDataPath;
			AkWwiseEditorSettings.Instance.SaveSettings();
		}
		string pathRelativeToWwiseProject = AkUtilities.MakeRelativePath(System.IO.Path.GetDirectoryName(WwiseProjectPath), SoundbankPath);
		try
		{
			if (WwiseProjectPath.Length == 0)
			{
				return true;
			}

			var doc = new System.Xml.XmlDocument { PreserveWhitespace = true };
			doc.Load(WwiseProjectPath);
			var Navigator = doc.CreateNavigator();

			// Navigate to where the header file path is saved. The node has to be there, or else the wproj is invalid.
			var expression =
				System.Xml.XPath.XPathExpression.Compile(
					"/WwiseDocument/ProjectInfo/Project/PropertyList/Property[@Name='SoundBankHeaderFilePath']");
			var node = Navigator.SelectSingleNode(expression);
			if (node == null)
			{
				UnityEngine.Debug.LogError(
					"Could not find SoundBankHeaderFilePath property in Wwise project file. File is invalid.");
				return false;
			}

			// Change the "Value" attribute
			if (!node.MoveToAttribute("Value", ""))
			{
				return false;
			}

			node.SetValue(pathRelativeToWwiseProject);
			if (IsMigrationRequired(MigrationStep.RootOutputPath_v2025_1_0))
			{
				MigrateRootOutputPath(WwiseProjectPath);
			}

			doc.Save(WwiseProjectPath);
			return true;
		}
		catch
		{
			return false;
		}
	}
	
	public static bool IsSettingEnabled(string wProjPath, string settingName)
	{
		var doc = new System.Xml.XmlDocument { PreserveWhitespace = true };
		doc.Load(wProjPath);
		var Navigator = doc.CreateNavigator();

		// Navigate the wproj file (XML format) to where or setting should be
		var pathInXml = string.Format("/WwiseDocument/ProjectInfo/Project/PropertyList/Property[@Name='{0}']", settingName);
		var expression = System.Xml.XPath.XPathExpression.Compile(pathInXml);
		var node = Navigator.SelectSingleNode(expression);
		var IsJsonFileGenerationEnabled = node != null ? node.GetAttribute("Value", "") : "False";
		return IsJsonFileGenerationEnabled == "True";
	}

	// Make two paths relative to each other
	public static string MakeRelativePath(string fromPath, string toPath)
	{
		// MONO BUG: https://github.com/mono/mono/pull/471
		// In the editor, Application.dataPath returns <Project Folder>/Assets. There is a bug in
		// mono for method Uri.GetRelativeUri where if the path ends in a folder, it will
		// ignore the last part of the path. Thus, we need to add fake depth to get the "real"
		// relative path.
		fromPath += "/fake_depth";
		try
		{
			if (string.IsNullOrEmpty(fromPath))
			{
				return toPath;
			}

			if (string.IsNullOrEmpty(toPath))
			{
				return "";
			}

			var fromUri = new System.Uri(fromPath);
			var toUri = new System.Uri(toPath);

			if (fromUri.Scheme != toUri.Scheme)
			{
				return toPath;
			}

			var relativeUri = fromUri.MakeRelativeUri(toUri);
			var relativePath = System.Uri.UnescapeDataString(relativeUri.ToString());

			return relativePath;
		}
		catch
		{
			return toPath;
		}
	}

	// Reconcile a base path and a relative path to give a full path without any ".."
	public static string GetFullPath(string BasePath, string RelativePath)
	{
		if (string.IsNullOrEmpty(BasePath))
		{
			return "";
		}

		var wrongSeparatorChar = System.IO.Path.DirectorySeparatorChar == '/' ? '\\' : '/';

		if (string.IsNullOrEmpty(RelativePath))
		{
			return BasePath.Replace(wrongSeparatorChar, System.IO.Path.DirectorySeparatorChar);
		}

		if (System.IO.Path.GetPathRoot(RelativePath) != "")
		{
			return RelativePath.Replace(wrongSeparatorChar, System.IO.Path.DirectorySeparatorChar);
		}

		return System.IO.Path.GetFullPath(System.IO.Path.Combine(BasePath, RelativePath));
	}

	public static bool DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
	{
		var dir = new System.IO.DirectoryInfo(sourceDirName);
		if (!dir.Exists)
		{
			UnityEngine.Debug.LogError("WwiseUnity: Source directory doesn't exist");
			return false;
		}

		if (!System.IO.Directory.Exists(destDirName))
		{
			System.IO.Directory.CreateDirectory(destDirName);
		}

		var files = dir.GetFiles();
		foreach (var file in files)
		{
			var destFilePath = System.IO.Path.Combine(destDirName, file.Name);
			if (System.IO.File.Exists(destFilePath))
			{
				UnityEngine.Debug.LogWarningFormat("WwiseUnity: Destination file path will be overwritten: {0}", destFilePath);
			}

			file.CopyTo(destFilePath, true);
		}

		if (!copySubDirs)
		{
			return true;
		}

		var dirs = dir.GetDirectories();
		foreach (var subdir in dirs)
		{
			var destSubDirName = System.IO.Path.Combine(destDirName, subdir.Name);
			DirectoryCopy(subdir.FullName, destSubDirName, copySubDirs);
		}

		return true;
	}

	public static bool MoveAssetsFromDirectory(string sourceDirName, string destDirName, bool copySubDirs)
	{
		var dir = new System.IO.DirectoryInfo(sourceDirName);
		if (!dir.Exists)
		{
			UnityEngine.Debug.LogError("WwiseUnity: Source directory doesn't exist");
			return false;
		}

		if (!System.IO.Directory.Exists(destDirName))
		{
			AssetDatabase.CreateFolder(System.IO.Path.GetDirectoryName(destDirName), System.IO.Path.GetFileName(destDirName));
		}

		var files = dir.GetFiles();
		string error, source, destFilePath;
		foreach (var file in files)
		{
			if (file.Extension == ".meta")
			{
				continue;
			}

			destFilePath = System.IO.Path.Combine(destDirName, file.Name);
			if (System.IO.File.Exists(destFilePath))
			{
				UnityEngine.Debug.LogWarningFormat("WwiseUnity: Destination file path will be overwritten: {0}", destFilePath);
			}

			source = System.IO.Path.Combine("Assets", AkUtilities.MakeRelativePath(UnityEngine.Application.dataPath, file.FullName));
			source = source.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);

			error = AssetDatabase.MoveAsset(source, destFilePath);
			if (!string.IsNullOrEmpty(error))
			{
				UnityEngine.Debug.LogErrorFormat("WwiseUnity: Error while attempting to move <{0}> to <{1}>: {2}", source, destFilePath, error);
			}

		}

		if (!copySubDirs)
		{
			return true;
		}

		var dirs = dir.GetDirectories();
		foreach (var subdir in dirs)
		{
			source = System.IO.Path.Combine("Assets", AkUtilities.MakeRelativePath(UnityEngine.Application.dataPath, subdir.FullName));
			source = source.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);

			var destSubDirName = System.IO.Path.Combine(destDirName, subdir.Name);
			error = UnityEditor.AssetDatabase.MoveAsset(source, destSubDirName);

			if (!string.IsNullOrEmpty(error))
			{
				UnityEngine.Debug.LogErrorFormat("WwiseUnity: Error while attempting to move <{0}> to <{1}>: {2}", source, destSubDirName, error);
			}
		}

		return true;
	}

	public static bool CreateFolder(string folderToCreate)
	{
		var created = false;

		var folder = string.Empty;
		var folders = folderToCreate.Split(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);
		for (int i = 0; i < folders.Length; ++i)
		{
			var parentFolder = folder;
			folder = string.IsNullOrEmpty(parentFolder) ? folders[i] : System.IO.Path.Combine(parentFolder, folders[i]);

			if (UnityEditor.AssetDatabase.IsValidFolder(folder))
			{
				continue;
			}

			var error = UnityEditor.AssetDatabase.CreateFolder(parentFolder, folders[i]);
			if (string.IsNullOrEmpty(error))
			{
				UnityEngine.Debug.LogFormat("WwiseUnity: Created folder <{0}> in <{0}>", folders[i], parentFolder);
				created = true;
				continue;
			}

			return false;
		}

		if (created)
		{
			UnityEditor.AssetDatabase.SaveAssets();
		}

		return true;
	}

	/// <summary>
	/// Renames or moves a folder using UnityEditor.Database API.
	/// </summary>
	/// <param name="oldPath"></param>
	/// <param name="newPath"></param>
	/// <returns>Returns true if the operation was successful.</returns>
	public static bool MoveFolder(string oldPath, string newPath)
	{
		oldPath = oldPath.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);
		newPath = newPath.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);
		
		if (oldPath.Equals(newPath, System.StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}

		if (!AssetDatabase.IsValidFolder(oldPath))
		{
			UnityEngine.Debug.LogWarningFormat("WwiseUnity: Refusing to move nonexistent folder <{0}>", oldPath);
			return false;
		}

		var error = string.Empty;
		var newParentFolder = System.IO.Path.GetDirectoryName(newPath);
		if (System.IO.Path.GetDirectoryName(oldPath) == newParentFolder)
		{
			error = UnityEditor.AssetDatabase.RenameAsset(oldPath, newPath.Substring(newParentFolder.Length + 1));
			if (string.IsNullOrEmpty(error))
			{
				return true;
			}

			UnityEngine.Debug.LogErrorFormat("WwiseUnity: Error while attempting to rename folder <{0}> to <{1}>: {2}", oldPath, newPath, error);
			return false;
		}

		if (!CreateFolder(newParentFolder))
		{
			return false;
		}

		error = UnityEditor.AssetDatabase.MoveAsset(oldPath, newPath);
		if (string.IsNullOrEmpty(error))
		{
			return true;
		}

		UnityEngine.Debug.LogWarningFormat("WwiseUnity: Error while attempting to move folder <{0}> to <{1}>: {2}", oldPath, newPath, error);
		return false;
	}

	public static void RepaintInspector()
	{
		var windows = UnityEngine.Resources.FindObjectsOfTypeAll<UnityEditor.EditorWindow>();
		foreach (var win in windows)
			if (win.titleContent.text == "Inspector")
			{
				win.Repaint();
			}
	}

	public static string ParseOsxPathFromWinePath(string path)
	{
		string ret = path.Replace("Y:", System.Environment.GetEnvironmentVariable("HOME"));
		ret = ret.Replace("Z:", "");
		ret = ret.Replace('\\', '/');
		return ret;
	}

	#region Tooltip Workaround
	private static System.Reflection.FieldInfo GetFieldInfoFromProperty(UnityEditor.SerializedProperty property)
	{
		var serializedProperty = property.serializedObject.FindProperty("m_Script");
		if (serializedProperty == null)
		{
			return null;
		}

		var monoScript = serializedProperty.objectReferenceValue as UnityEditor.MonoScript;
		if (monoScript == null)
		{
			return null;
		}

		var scriptTypeFromProperty = monoScript.GetClass();
		if (scriptTypeFromProperty == null)
		{
			return null;
		}

		return GetFieldInfoFromPropertyPath(scriptTypeFromProperty, property.propertyPath);
	}

	private static System.Reflection.FieldInfo GetFieldInfoFromPropertyPath(System.Type host, string path)
	{
		System.Reflection.FieldInfo fieldInfo = null;

		var type = host;
		var array = path.Split('.');
		for (int i = 0; i < array.Length; ++i)
		{
			string text = array[i];
			if (i < array.Length - 1 && text == "Array" && array[i + 1].StartsWith("data["))
			{
				if (type.IsArray)
				{
					type = type.GetElementType();
				}
				else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.List<>))
				{
					type = type.GetGenericArguments()[0];
				}

				i++;
			}
			else
			{
				var type2 = type;
				while (type2 != null)
				{
					fieldInfo = type2.GetField(text, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
					if (fieldInfo != null)
					{
						type = fieldInfo.FieldType;
						break;
					}

					type2 = type2.BaseType;
					if (type2 == null)
					{
						return null;
					}
				}
			}
		}

		return fieldInfo;
	}

	private static string GetTooltip(System.Reflection.FieldInfo field, bool inherit)
	{
		var attributes = field.GetCustomAttributes(typeof(UnityEngine.TooltipAttribute), inherit) as UnityEngine.TooltipAttribute[];
		if (attributes != null && attributes.Length > 0)
		{
			return attributes[0].tooltip;
		}

		return string.Empty;
	}

	public static string GetTooltip(UnityEditor.SerializedProperty property)
	{
		return GetTooltip(GetFieldInfoFromProperty(property), true);
	}
	#endregion
}
#endif // UNITY_EDITOR

public partial class AkUtilities
{
	public static void FixSlashes(ref string path, char separatorChar, char badChar, bool addTrailingSlash)
	{
		if (string.IsNullOrEmpty(path))
		{
			return;
		}

		path = path.Trim().Replace(badChar, separatorChar).TrimStart('\\');

		// Append a trailing slash to play nicely with Wwise
		if (addTrailingSlash && !path.EndsWith(separatorChar.ToString()))
		{
			path += separatorChar;
		}
	}
	
	public static void FixSlashes(ref string path)
	{
		var separatorChar = System.IO.Path.DirectorySeparatorChar;
		var badChar = separatorChar == '\\' ? '/' : '\\';
		FixSlashes(ref path, separatorChar, badChar, true);
	}

	public static string GetPathInPackage(string relativePath)
	{
		const string AssetWwisePathParent = "Assets/Wwise/API/";
		const string PackageWwisePathParent = "Packages/com.audiokinetic.wwise.api/";

		string rootpath = "";
		if (System.IO.Directory.Exists(System.IO.Path.GetFullPath(PackageWwisePathParent)))
		{
			rootpath = PackageWwisePathParent;
		}
		else if (System.IO.Directory.Exists(System.IO.Path.GetFullPath(AssetWwisePathParent)))
		{

			rootpath = AssetWwisePathParent;
		}
		else 
		{
			return string.Empty;
		} 

		var relativePathFolders = new System.Collections.Generic.List<string>(relativePath.Split('/'));
		var rootPathFolders = new System.Collections.Generic.List<string>(rootpath.Split('/'));
		var overlap = relativePathFolders.Intersect(rootPathFolders);
		if (overlap.Count() > 0)
		{
			UnityEngine.Debug.LogWarning("AkUtilities.GetPathInPackage(): relativePath contains overlapping folder names with root path.\nrelativePath: " 
				+ relativePath
				+ "\nroot path: "
				+ rootpath
				+ "\n This could cause issues with plugins activation and packaging.");
		}

		return System.IO.Path.Combine(rootpath, relativePath);
	}

	/// <summary>
	///     This is based on FNVHash as used by the DataManager
	///     to assign short IDs to objects. Be sure to keep them both in sync
	///     when making changes!
	/// </summary>
	public class ShortIDGenerator
	{
		private const uint s_prime32 = 16777619;
		private const uint s_offsetBasis32 = 2166136261;

		private static byte s_hashSize;
		private static uint s_mask;

		static ShortIDGenerator()
		{
			HashSize = 32;
		}

		public static byte HashSize
		{
			get { return s_hashSize; }

			set
			{
				s_hashSize = value;
				s_mask = (uint)((1 << s_hashSize) - 1);
			}
		}

		public static uint Compute(string in_name)
		{
			var buffer = System.Text.Encoding.UTF8.GetBytes(in_name.ToLower());

			// Start with the basis value
			var hval = s_offsetBasis32;

			for (var i = 0; i < buffer.Length; i++)
			{
				// multiply by the 32 bit FNV magic prime mod 2^32
				hval *= s_prime32;

				// xor the bottom with the current octet
				hval ^= buffer[i];
			}

			if (s_hashSize == 32)
			{
				return hval;
			}

			// XOR-Fold to the required number of bits
			return (hval >> s_hashSize) ^ (hval & s_mask);
		}
	}
}

#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
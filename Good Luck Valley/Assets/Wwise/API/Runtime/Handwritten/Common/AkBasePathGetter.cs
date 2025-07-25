#if !(UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
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

using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;

/// <summary>
///     This class is responsible for determining the path where sound banks are located. When using custom platforms, this
///     class needs to be extended.
/// </summary>
public partial class AkBasePathGetter
{
	/// <summary>
	///     User hook called to retrieve the custom platform name used to determine the base path. Do not modify platformName
	///     to use default platform names.
	/// </summary>
	/// <param name="platformName">The custom platform name. Leave unaffected if the default location is acceptable.</param>
	public delegate void CustomPlatformNameGetter(ref string platformName);

	public static CustomPlatformNameGetter GetCustomPlatformName;

	/// <summary>
	///     Determines the platform name which is also the sub-folder within the base path where sound banks are located for
	///     this platform.
	/// </summary>
	/// <returns>The platform name.</returns>
	public static string GetPlatformName()
	{
		var platformSubDir = string.Empty;
		GetCustomPlatformName?.Invoke(ref platformSubDir);

		if (!string.IsNullOrEmpty(platformSubDir))
			return platformSubDir;

		return DefaultPlatformName;
	}
}

public partial class AkBasePathGetter
{
	public static readonly string DefaultBasePath = System.IO.Path.Combine("Audio", "GeneratedSoundBanks");
	private const string DecodedBankFolder = "DecodedBanks";

	private static bool LogWarnings_Internal = true;
	public static bool LogWarnings
	{
		get { return LogWarnings_Internal; }
		set { LogWarnings_Internal = value; }
	}

	/// <summary>
	///     Returns the absolute path to the platform specific sound banks.
	/// </summary>
	/// <returns>The absolute path to the platform specific sound banks.</returns>
	public static string GetPlatformBasePath()
	{
		var platformName = GetPlatformName();

#if UNITY_EDITOR
		var platformBasePathEditor = GetPlatformBasePathEditor(platformName);
		if (!string.IsNullOrEmpty(platformBasePathEditor))
			return platformBasePathEditor;

		var fullBasePath = AkWwiseEditorSettings.Instance.WwiseStreamingAssetsPath;
#else
		var fullBasePath = string.Empty;
#endif

		if (string.IsNullOrEmpty(fullBasePath))
			fullBasePath = AkWwiseInitializationSettings.ActivePlatformSettings.SoundbankPath;

		AdjustFullBasePathForPlatform(ref fullBasePath);

		// Combine base path with platform sub-folder
		var platformBasePath = System.IO.Path.Combine(fullBasePath, platformName);
		AkUtilities.FixSlashes(ref platformBasePath);
		return platformBasePath;
	}

#if UNITY_EDITOR
	public static bool GetSoundBankPaths(string platformName, out string sourcePlatformBasePath, out string destinationPlatformBasePath)
	{
		sourcePlatformBasePath = GetPlatformBasePathEditor(platformName);
		if (string.IsNullOrEmpty(sourcePlatformBasePath))
		{
			if (LogWarnings)
				UnityEngine.Debug.LogErrorFormat("WwiseUnity: Could not find source folder for <{0}> platform. Did you remember to generate your banks?", platformName);

			destinationPlatformBasePath = string.Empty;
			return false;
		}

		destinationPlatformBasePath = System.IO.Path.Combine(GetFullSoundBankPathEditor(), platformName);
		if (string.IsNullOrEmpty(destinationPlatformBasePath))
		{
			if (LogWarnings)
				UnityEngine.Debug.LogErrorFormat("WwiseUnity: Could not find destination folder for <{0}> platform", platformName);

			return false;
		}

		return true;
	}

	/// <summary>
	///     Returns the absolute path to the folder above the platform specific sound banks sub-folders.
	/// </summary>
	/// <returns>The absolute sound bank base path.</returns>
	public static string GetFullSoundBankPathEditor()
	{
		string fullBasePath = System.IO.Path.Combine(UnityEngine.Application.streamingAssetsPath, AkWwiseEditorSettings.Instance.WwiseStreamingAssetsPath);
		AkUtilities.FixSlashes(ref fullBasePath);
		return fullBasePath;
	}

	public static string GetWwiseProjectPath()
	{
		var Settings = AkWwiseEditorSettings.Instance;
		return AkUtilities.GetFullPath(UnityEngine.Application.dataPath, Settings.WwiseProjectPath);
	}

	public static string GetWwiseProjectDirectory(string wwiseAbsolutePath = "")
	{
		var projectPath= AkUtilities.GetFullPath(UnityEngine.Application.dataPath, wwiseAbsolutePath == "" ? AkWwiseEditorSettings.Instance.WwiseProjectPath : wwiseAbsolutePath);
		return System.IO.Path.GetDirectoryName(projectPath);
	}

	public static string GetDefaultRootOutputPath()
	{
		string path = Path.GetDirectoryName(GetWwiseProjectPath());
		if (path == null)
		{
			return null;
		}
		return System.IO.Path.Combine(path, "GeneratedSoundBanks");
	}

	/// <summary>
	///     Determines the platform base path for use within the Editor.
	/// </summary>
	/// <param name="platformName">The platform name.</param>
	/// <returns>The full path to the sound banks for use within the Editor.</returns>
	private static string GetPlatformBasePathEditor(string platformName)
	{
		var WwiseProjectFullPath = GetWwiseProjectPath();
		var SoundBankDest = AkUtilities.GetWwiseSoundBankDestinationFolder(platformName);

		try
		{
			if (System.IO.Path.GetPathRoot(SoundBankDest) == "")
			{
				// Path is relative, make it full
				SoundBankDest = AkUtilities.GetFullPath(System.IO.Path.GetDirectoryName(WwiseProjectFullPath), SoundBankDest);
			}
		}
		catch
		{
			SoundBankDest = string.Empty;
		}

		if (LogWarnings)
		{
			if (string.IsNullOrEmpty(SoundBankDest))
			{
				UnityEngine.Debug.LogWarning("WwiseUnity: The platform SoundBank subfolder within the Wwise project could not be found.");
				return null;
			}

			try
			{
				// Verify if there are banks in there
				var di = new System.IO.DirectoryInfo(SoundBankDest);
				var foundBanks = di.GetFiles("*.bnk", System.IO.SearchOption.AllDirectories);
				if (foundBanks.Length == 0)
				{
					return null;
				}

				if (!SoundBankDest.Contains(platformName))
				{
					if (LogWarnings)
						UnityEngine.Debug.LogWarning("WwiseUnity: The platform SoundBank subfolder does not match your platform name. You will need to create a custom platform name getter for your game. See section \"Using Wwise Custom Platforms in Unity\" of the Wwise Unity integration documentation for more information");
				}

				return SoundBankDest;
			}
			catch
			{
				return null;
			}
		}
		else
		{
			return SoundBankDest;
		}
	}
#endif

	private static AkBasePathGetter Instance;
	public static AkBasePathGetter Get()
	{
		if (Instance == null)
		{
			Instance = new AkBasePathGetter();
			Instance.EvaluateGamePaths();
		}
		return Instance;
	}

	public void EvaluateGamePaths()
	{
		string tempPersistentDataPath = GetPersistentDataPath();

		PersistentDataPath = tempPersistentDataPath;

		var persistentDataSubfolder = AkWwiseInitializationSettings.ActivePlatformSettings.SoundBankPersistentDataPath;

		string tempSoundBankBasePath = null;
		if (!string.IsNullOrEmpty(tempPersistentDataPath) && !string.IsNullOrEmpty(persistentDataSubfolder))
		{
			tempSoundBankBasePath = System.IO.Path.GetFullPath(System.IO.Path.Combine(tempPersistentDataPath, persistentDataSubfolder));
			if (LogWarnings)
				UnityEngine.Debug.LogFormat("WwiseUnity: Using persistentDataPath. SoundBanks base path set to <{0}>.", tempSoundBankBasePath);
		}
		else
		{
			tempSoundBankBasePath = GetPlatformBasePath();

#if !AK_WWISE_ADDRESSABLES //Don't log this if we're using addressables
			if (string.IsNullOrEmpty(tempSoundBankBasePath) || !InitBankExists(tempSoundBankBasePath))
			{
				if (LogWarnings)
				{
#if UNITY_EDITOR
					var format = "WwiseUnity: Could not locate the SoundBanks in {0}. Did you make sure to generate them?";
#else
					var format = "WwiseUnity: Could not locate the SoundBanks in {0}. Did you make sure to copy them to the StreamingAssets folder?";
#endif
					UnityEngine.Debug.LogErrorFormat(format, tempSoundBankBasePath);
				}
			}
#endif
		}

		SoundBankBasePath = tempSoundBankBasePath;
		DecodedBankFullPath = GetDecodedBankPath();
	}

	public string SoundBankBasePath { get; private set; }

	public string PersistentDataPath { get; private set; }

	public string DecodedBankFullPath { get; private set; }
}

#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.

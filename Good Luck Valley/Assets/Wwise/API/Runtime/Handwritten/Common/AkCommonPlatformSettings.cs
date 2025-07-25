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

public class AkBasePlatformSettings : UnityEngine.ScriptableObject
{
	public virtual AkInitializationSettings AkInitializationSettings
	{
		get { return new AkInitializationSettings(); }
	}

	public virtual AkSpatialAudioInitSettings AkSpatialAudioInitSettings
	{
		get { return new AkSpatialAudioInitSettings(); }
	}

	public virtual AkCallbackManager.InitializationSettings CallbackManagerInitializationSettings
	{
		get { return new AkCallbackManager.InitializationSettings(); }
	}

	public virtual string SoundBankPersistentDataPath
	{
		get { return null; }
	}

	public virtual string InitialLanguage
	{
		get { return "English(US)"; }
	}


	public virtual bool LoadBanksAsynchronously
    {
		get 
		{
#if AK_WWISE_ADDRESSABLES && UNITY_ADDRESSABLES
			return true;
#else
			return false;
#endif
		}
	}

	public virtual bool SuspendAudioDuringFocusLoss
	{
		get { return true; }
	}

	public virtual bool RenderDuringFocusLoss
	{
		get { return false; }
	}

	public virtual string SoundbankPath
	{
		get { return AkBasePathGetter.DefaultBasePath; }
	}

	public virtual AkCommunicationSettings AkCommunicationSettings
	{
		get { return new AkCommunicationSettings(); }
	}
	
	public virtual bool UseSubFoldersForGeneratedFiles
	{
		get { return false; }
	}

	public virtual uint MemoryDebugLevel
	{
		get { return 0; }
	}
	
	public virtual bool IsDecodedBankEnabled
	{
		get { return false; }
	}
	
	public virtual bool IsAutoBankEnabled
	{
		get { return true; }
		set { }
	}

	public virtual float defaultListenerScalingFactor
	{
		get { return 1f; }
	}
}

[System.Serializable]
public class AkCommonOutputSettings
{
	[UnityEngine.Tooltip("The name of a custom audio device to use. Custom audio devices are defined in the Audio Device Shareset section of the Wwise project. Leave this empty to output normally through the default audio device.")]
	public string m_AudioDeviceShareset = string.Empty;

	[UnityEngine.Tooltip("Device specific identifier, when multiple devices of the same type are possible.  If only one device is possible, leave to 0.")]
	public uint m_DeviceID = AkUnitySoundEngine.AK_INVALID_UNIQUE_ID;

	public enum PanningRule
	{
		Speakers = 0,
		Headphones = 1
	}

	[UnityEngine.Tooltip("Rule for 3D panning of signals routed to a stereo bus. In \"Speakers\" mode, the angle of the front loudspeakers is used. In \"Headphones\" mode, the speaker angles are superseded by constant power panning between two virtual microphones spaced 180 degrees apart.")]
	public PanningRule m_PanningRule = PanningRule.Speakers;

	[System.Serializable]
	public class ChannelConfiguration
	{
		public enum ChannelConfigType
		{
			Anonymous = 0x0,
			Standard = 0x1,
			Ambisonic = 0x2
		}

		[UnityEngine.Tooltip("A code that completes the identification of channels by uChannelMask. Anonymous: Channel mask == 0 and channels. Standard: Channels must be identified with standard defines in AkSpeakerConfigs. Ambisonic: Channel mask == 0 and channels follow standard ambisonic order.")]
		public ChannelConfigType m_ChannelConfigType = ChannelConfigType.Anonymous;

		public enum ChannelMask
		{
			NONE = 0x0,

			/// Standard speakers (channel mask):
			FRONT_LEFT = 0x1,        ///< Front left speaker bit mask
			FRONT_RIGHT = 0x2,       ///< Front right speaker bit mask
			FRONT_CENTER = 0x4,      ///< Front center speaker bit mask
			LOW_FREQUENCY = 0x8,     ///< Low-frequency speaker bit mask
			BACK_LEFT = 0x10,        ///< Rear left speaker bit mask
			BACK_RIGHT = 0x20,       ///< Rear right speaker bit mask
			BACK_CENTER = 0x100, ///< Rear center speaker ("surround speaker") bit mask
			SIDE_LEFT = 0x200,   ///< Side left speaker bit mask
			SIDE_RIGHT = 0x400,  ///< Side right speaker bit mask

			/// "Height" speakers.
			TOP = 0x800,     ///< Top speaker bit mask
			HEIGHT_FRONT_LEFT = 0x1000,  ///< Front left speaker bit mask
			HEIGHT_FRONT_CENTER = 0x2000,    ///< Front center speaker bit mask
			HEIGHT_FRONT_RIGHT = 0x4000, ///< Front right speaker bit mask
			HEIGHT_BACK_LEFT = 0x8000,   ///< Rear left speaker bit mask
			HEIGHT_BACK_CENTER = 0x10000,    ///< Rear center speaker bit mask
			HEIGHT_BACK_RIGHT = 0x20000, ///< Rear right speaker bit mask

			//
			// Supported speaker setups. Those are the ones that can be used in the Wwise Sound Engine audio pipeline.
			//
			SETUP_MONO = FRONT_CENTER,        ///< 1.0 setup channel mask
			SETUP_0POINT1 = LOW_FREQUENCY,    ///< 0.1 setup channel mask
			SETUP_1POINT1 = (FRONT_CENTER | LOW_FREQUENCY),    ///< 1.1 setup channel mask
			SETUP_STEREO = (FRONT_LEFT | FRONT_RIGHT), ///< 2.0 setup channel mask
			SETUP_2POINT1 = (SETUP_STEREO | LOW_FREQUENCY),    ///< 2.1 setup channel mask
			SETUP_3STEREO = (SETUP_STEREO | FRONT_CENTER), ///< 3.0 setup channel mask
			SETUP_3POINT1 = (SETUP_3STEREO | LOW_FREQUENCY),   ///< 3.1 setup channel mask
			SETUP_4 = (SETUP_STEREO | SIDE_LEFT | SIDE_RIGHT),  ///< 4.0 setup channel mask
			SETUP_4POINT1 = (SETUP_4 | LOW_FREQUENCY), ///< 4.1 setup channel mask
			SETUP_5 = (SETUP_4 | FRONT_CENTER),    ///< 5.0 setup channel mask
			SETUP_5POINT1 = (SETUP_5 | LOW_FREQUENCY), ///< 5.1 setup channel mask
			SETUP_6 = (SETUP_4 | BACK_LEFT | BACK_RIGHT),   ///< 6.0 setup channel mask
			SETUP_6POINT1 = (SETUP_6 | LOW_FREQUENCY), ///< 6.1 setup channel mask
			SETUP_7 = (SETUP_6 | FRONT_CENTER),    ///< 7.0 setup channel mask
			SETUP_7POINT1 = (SETUP_7 | LOW_FREQUENCY), ///< 7.1 setup channel mask
			SETUP_SURROUND = (SETUP_STEREO | BACK_CENTER), ///< Legacy surround setup channel mask

			// Note. DPL2 does not really have 4 channels, but it is used by plugins to differentiate from stereo setup.
			SETUP_DPL2 = (SETUP_4),       ///< Legacy DPL2 setup channel mask

			SETUP_HEIGHT_4 = (HEIGHT_FRONT_LEFT | HEIGHT_FRONT_RIGHT | HEIGHT_BACK_LEFT | HEIGHT_BACK_RIGHT),    ///< 4 speaker height layer.
			SETUP_HEIGHT_5 = (SETUP_HEIGHT_4 | HEIGHT_FRONT_CENTER),                                                                   ///< 5 speaker height layer.
			SETUP_HEIGHT_ALL = (SETUP_HEIGHT_5 | HEIGHT_BACK_CENTER),                                                                      ///< All height speaker layer.

			// Auro speaker setups
			SETUP_AURO_222 = (SETUP_4 | HEIGHT_FRONT_LEFT | HEIGHT_FRONT_RIGHT),    ///< Auro-222 setup channel mask
			SETUP_AURO_8 = (SETUP_AURO_222 | HEIGHT_BACK_LEFT | HEIGHT_BACK_RIGHT),     ///< Auro-8 setup channel mask
			SETUP_AURO_9 = (SETUP_AURO_8 | FRONT_CENTER),                                          ///< Auro-9.0 setup channel mask
			SETUP_AURO_9POINT1 = (SETUP_AURO_9 | LOW_FREQUENCY),                                           ///< Auro-9.1 setup channel mask
			SETUP_AURO_10 = (SETUP_AURO_9 | TOP),                                                  ///< Auro-10.0 setup channel mask		
			SETUP_AURO_10POINT1 = (SETUP_AURO_10 | LOW_FREQUENCY),                                         ///< Auro-10.1 setup channel mask	
			SETUP_AURO_11 = (SETUP_AURO_10 | HEIGHT_FRONT_CENTER),                                 ///< Auro-11.0 setup channel mask
			SETUP_AURO_11POINT1 = (SETUP_AURO_11 | LOW_FREQUENCY),                                         ///< Auro-11.1 setup channel mask	
			SETUP_AURO_11_740 = (SETUP_7 | SETUP_HEIGHT_4),                                        ///< Auro-11.0 (7+4) setup channel mask
			SETUP_AURO_11POINT1_740 = (SETUP_AURO_11_740 | LOW_FREQUENCY),                                     ///< Auro-11.1 (7+4) setup channel mask
			SETUP_AURO_13_751 = (SETUP_7 | SETUP_HEIGHT_5 | TOP),                       ///< Auro-13.0 setup channel mask
			SETUP_AURO_13POINT1_751 = (SETUP_AURO_13_751 | LOW_FREQUENCY),                                     ///< Auro-13.1 setup channel mask

			// Dolby speaker setups: in Dolby nomenclature, [#plane].[lfe].[#height]
			SETUP_DOLBY_5_0_2 = (SETUP_5 | HEIGHT_FRONT_LEFT | HEIGHT_FRONT_RIGHT), ///< Dolby 5.0.2 setup channel mask
			SETUP_DOLBY_5_1_2 = (SETUP_DOLBY_5_0_2 | LOW_FREQUENCY),                                   ///< Dolby 5.1.2 setup channel mask
			SETUP_DOLBY_6_0_2 = (SETUP_6 | HEIGHT_FRONT_LEFT | HEIGHT_FRONT_RIGHT), ///< Dolby 6.0.2 setup channel mask
			SETUP_DOLBY_6_1_2 = (SETUP_DOLBY_6_0_2 | LOW_FREQUENCY),                                   ///< Dolby 6.1.2 setup channel mask
			SETUP_DOLBY_6_0_4 = (SETUP_DOLBY_6_0_2 | HEIGHT_BACK_LEFT | HEIGHT_BACK_RIGHT), ///< Dolby 6.0.4 setup channel mask
			SETUP_DOLBY_6_1_4 = (SETUP_DOLBY_6_0_4 | LOW_FREQUENCY),                                   ///< Dolby 6.1.4 setup channel mask
			SETUP_DOLBY_7_0_2 = (SETUP_7 | HEIGHT_FRONT_LEFT | HEIGHT_FRONT_RIGHT), ///< Dolby 7.0.2 setup channel mask
			SETUP_DOLBY_7_1_2 = (SETUP_DOLBY_7_0_2 | LOW_FREQUENCY),                                   ///< Dolby 7.1.2 setup channel mask
			SETUP_DOLBY_7_0_4 = (SETUP_DOLBY_7_0_2 | HEIGHT_BACK_LEFT | HEIGHT_BACK_RIGHT), ///< Dolby 7.0.4 setup channel mask
			SETUP_DOLBY_7_1_4 = (SETUP_DOLBY_7_0_4 | LOW_FREQUENCY),                                   ///< Dolby 7.1.4 setup channel mask

			SETUP_ALL_SPEAKERS = (SETUP_7POINT1 | BACK_CENTER | SETUP_HEIGHT_ALL | TOP), ///< All speakers.
		};

		[UnityEngine.Tooltip("A bit field, whose channel identifiers depend on AkChannelConfigType (up to 20).")]
		[AkEnumFlag(typeof(ChannelMask))]
		public ChannelMask m_ChannelMask = ChannelMask.NONE;

		[UnityEngine.Tooltip("The number of channels, identified (deduced from channel mask) or anonymous (set directly).")]
		public uint m_NumberOfChannels = 0;

		public void CopyTo(AkChannelConfig config)
		{
			switch (m_ChannelConfigType)
			{
				case ChannelConfigType.Anonymous:
					config.SetAnonymous(m_NumberOfChannels);
					break;

				case ChannelConfigType.Standard:
					config.SetStandard((uint)m_ChannelMask);
					break;

				case ChannelConfigType.Ambisonic:
					config.SetAmbisonic(m_NumberOfChannels);
					break;
			}
		}
	}

	[UnityEngine.Tooltip("Channel configuration for this output. Hardware might not support the selected configuration.")]
	public ChannelConfiguration m_ChannelConfig = new ChannelConfiguration();

	public void CopyTo(AkOutputSettings settings)
	{
		settings.audioDeviceShareset = string.IsNullOrEmpty(m_AudioDeviceShareset) ? AkUnitySoundEngine.AK_INVALID_UNIQUE_ID : AkUtilities.ShortIDGenerator.Compute(m_AudioDeviceShareset);
		settings.idDevice = m_DeviceID;
		settings.ePanningRule = (AkPanningRule)m_PanningRule;
		m_ChannelConfig.CopyTo(settings.channelConfig);
	}
}

[System.Serializable]
public partial class AkCommonUserSettings
{
	[UnityEngine.Tooltip("Path for the SoundBanks. This must contain one sub folder per platform, with the same as in the Wwise project.")]
	public string m_BasePath = AkBasePathGetter.DefaultBasePath;

	[UnityEngine.Tooltip("Language sub-folder used at startup.")]
	public string m_StartupLanguage = "English(US)";

	[UnityEngine.Tooltip("Enable Wwise engine logging. This is used to turn on/off the logging of the Wwise engine.")]
	public bool m_EngineLogging = AkCallbackManager.InitializationSettings.DefaultIsLoggingEnabled;
	
	[UnityEngine.Tooltip("Whether Decoded Banks are enabled.")]
	public bool m_IsDecodedBankEnabled = false;
	
	[UnityEngine.Tooltip("Whether Auto Bank is enabled in Wwise.")]
	public bool m_IsAutoBankEnabled = true;

	[UnityEngine.Tooltip("The default value of the \"Attenuation Scaling Factor\" when an AkAudioListener is created.")]
	public float m_DefaultListenerScalingFactor = 1.0f;

	[UnityEngine.Tooltip("Maximum number of automation paths for positioning sounds.")]
	public uint m_MaximumNumberOfPositioningPaths = 255;

	[UnityEngine.Tooltip("Size of the command queue.")]
	public uint m_CommandQueueSize = 256 * 1024;

	[UnityEngine.Tooltip("Number of samples per audio frame (256, 512, 1024, or 2048).")]
	public uint m_SamplesPerFrame = 512;

	[UnityEngine.Tooltip("Main output device settings.")]
	public AkCommonOutputSettings m_MainOutputSettings;

	protected partial string GetPluginPath();

	public virtual void CopyTo(AkInitSettings settings)
	{
		settings.uMaxNumPaths = m_MaximumNumberOfPositioningPaths;
		settings.uCommandQueueSize = m_CommandQueueSize;
		settings.uNumSamplesPerFrame = m_SamplesPerFrame;
		settings.fStreamingLookAheadRatio = m_StreamingLookAheadRatio;
		m_MainOutputSettings.CopyTo(settings.settingsMainOutput);
		settings.szPluginDLLPath = GetPluginPath();
		UnityEngine.Debug.Log("WwiseUnity: Setting Plugin DLL path to: " + (settings.szPluginDLLPath == null ? "NULL" : settings.szPluginDLLPath));
	}

	[UnityEngine.Tooltip("Multiplication factor for all streaming look-ahead heuristic values.")]
	[UnityEngine.Range(0, 1)]
	public float m_StreamingLookAheadRatio = 1.0f;
	
	public void CopyTo(AkStreamMgrSettings settings)
	{
	}

	public virtual void CopyTo(AkDeviceSettings settings) { }

	[UnityEngine.Tooltip("Sampling rate in Hz. The default value is 48000. Use 24000 for low quality audio. Any positive, reasonable sample rate is supported. However, very low sample rates might cause the sound engine to malfunction.")]
	public uint m_SampleRate = 48000;

	[UnityEngine.Tooltip("Number of refill buffers in voice buffer. Set to 2 for double-buffered. The default value is to 4.")]
	public ushort m_NumberOfRefillsInVoice = 4;

	partial void SetSampleRate(AkPlatformInitSettings settings);

	public virtual void CopyTo(AkPlatformInitSettings settings)
	{
		SetSampleRate(settings);
		settings.uNumRefillsInVoice = m_NumberOfRefillsInVoice;
	}

	[System.Serializable]
	public class SpatialAudioSettings
	{
		public enum TransmissionOperation
		{
			Add,
			Multiply,
			Max
		};

		[UnityEngine.Tooltip("Maximum number of portals that sound can propagate through. The default value is 8.")]
		[UnityEngine.Range(0, AkUnitySoundEngine.AK_MAX_SOUND_PROPAGATION_DEPTH)]
		public uint m_MaxSoundPropagationDepth = AkUnitySoundEngine.AK_MAX_SOUND_PROPAGATION_DEPTH;

		[UnityEngine.Tooltip("Distance (in game units) that an emitter or listener has to move to trigger a recalculation of reflections and diffraction. A high distance value has a lower CPU load than a low distance value, but the accuracy is also lower. Note that this value does not affect the ray tracing itself. Rays are cast each time a Spatial Audio update is executed. The default value is 0.25.")]
		/// Distance (in game units) that an emitter or listener has to move to trigger a recalculation of reflections and diffraction.
		/// A high distance value has a lower CPU load than a low distance value, but the accuracy is also lower. Note that this value does not affect the ray tracing itself. Rays are cast each time a Spatial Audio update is executed.
		/// The default value is 0.25.
		public float m_MovementThreshold = 0.25f;

		[UnityEngine.Tooltip("The number of primary rays used in the ray tracing engine. A larger value increases the chances of finding reflection and diffraction paths but results in higher CPU usage. The default value is 35.")]
		/// The number of primary rays used in the ray tracing engine. A larger value increases the chances of finding reflection and diffraction paths but results in higher CPU usage.
		/// The default value is 35.
		public uint m_NumberOfPrimaryRays = 35;

		[UnityEngine.Range(0, 4)]
		[UnityEngine.Tooltip("The maximum reflection order: the number of \"bounces\" in a reflection path. A higher reflection order renders more detail at the expense of higher CPU usage. The default value is 2.")]
		[UnityEngine.Serialization.FormerlySerializedAs("m_ReflectionsOrder")]
		/// The maximum reflection order: the number of "bounces" in a reflection path. A higher reflection order renders more detail at the expense of higher CPU usage.
		/// Valid range: 1-4. The default value is 2.
		public uint m_MaxReflectionOrder = 2;

		[UnityEngine.Range(0, 8)]
		[UnityEngine.Tooltip("Maximum diffraction order: the number of \"bends\" in a diffraction path. A high diffraction order accommodates more complex geometry at the expense of higher CPU usage. Diffraction must be enabled on the geometry to find diffraction paths. Set to 0 to disable diffraction on all geometry. This parameter limits the recursion depth of diffraction rays cast from the listener to scan the environment and also the depth of the diffraction search to find paths between emitter and listener. To optimize CPU usage, set it to the maximum number of edges you expect the obstructing geometry to traverse. The default value is 4.")]
		/// Maximum diffraction order: the number of "bends" in a diffraction path. A high diffraction order accommodates more complex geometry at the expense of higher CPU usage.
		/// Diffraction must be enabled on the geometry to find diffraction paths. Set to 0 to disable diffraction on all geometry.
		/// This parameter limits the recursion depth of diffraction rays cast from the listener to scan the environment and also the depth of the diffraction search to find paths between emitter and listener.
		/// To optimize CPU usage, set it to the maximum number of edges you expect the obstructing geometry to traverse.
		/// Valid range: 1-4. The default value is 4.
		public uint m_MaxDiffractionOrder = 4;

		[UnityEngine.Range(0, 4)]
		[UnityEngine.Tooltip("The maximum possible number of diffraction points at each end of a reflection path. Diffraction on reflection allows reflections to fade in and out smoothly as the listener or emitter moves in and out of the reflection's shadow zone. When greater than zero, diffraction rays are sent from the listener to search for reflections around one or more corners from the listener. Diffraction must be enabled on the geometry to find diffracted reflections. Set to 0 to disable diffraction on reflections. Set to 2 or greater to allow Reflection paths to travel through Portals. The default value is 2.")]
		/// The maximum possible number of diffraction points at each end of a reflection path.
		/// Diffraction on reflection allows reflections to fade in and out smoothly as the listener or emitter moves in and out of the reflection's shadow zone.
		/// When greater than zero, diffraction rays are sent from the listener to search for reflections around one or more corners from the listener.
		/// Diffraction must be enabled on the geometry to find diffracted reflections.
		/// Set to 0 to disable diffraction on reflections. Set to 2 or greater to allow Reflection paths to travel through Portals. The default value is 2.
		public uint m_DiffractionOnReflectionsOrder = 2;

		[UnityEngine.Tooltip("The maximum number of game-defined auxiliary sends that can originate from a single emitter. An emitter can send to its own Room and to all adjacent Rooms if the emitter and listener are in the same Room. If a limit is set, the most prominent sends are kept, based on spread to the adjacent portal from the emitter's perspective. Set to 1 to only allow emitters to send directly to their current Room, and to the Room a listener is transitioning to if inside a portal. Set to 0 to disable the limit. The default value is 3.")]
		/// The maximum number of game-defined auxiliary sends that can originate from a single emitter.
		/// An emitter can send to its own Room and to all adjacent Rooms if the emitter and listener are in the same Room.
		/// If a limit is set, the most prominent sends are kept, based on spread to the adjacent portal from the emitter's perspective.
		/// Set to 1 to only allow emitters to send directly to their current Room, and to the Room a listener is transitioning to if inside a portal.
		/// Set to 0 to disable the limit. The default value is 3.
		public uint m_MaxEmitterRoomAuxSends = 3;

		[UnityEngine.Tooltip("Length of the rays that are cast inside Spatial Audio. Effectively caps the maximum length of an individual segment in a reflection or diffraction path. The default value is 1000.")]
		/// Length of the rays that are cast inside Spatial Audio. Effectively caps the maximum length of an individual segment in a reflection or diffraction path. The default value is 1000.
		public float m_MaxPathLength = 1000.0f;

		[UnityEngine.Tooltip("Defines the targeted maximum computation time allocated for Spatial Audio as a percentage [0, 100] of the current audio frame. When the value is greater than 0, Spatial Audio dynamically adapts the load-balancing based on the current CPU usage and the specified CPU limit. Set to 0 to disable the dynamic load balancing spread computation. The default value is 0.")]
		/// Defines the targeted maximum computation time allocated for Spatial Audio as a percentage [0, 100] of the current audio frame.
		/// When the value is greater than 0, Spatial Audio dynamically adapts the load-balancing spread based on the current CPU usage and the specified CPU limit.
		/// Set to 0 to disable the dynamic load-balancing spread computation.
		/// The default value is 0.
		public float m_CPULimitPercentage = 0.0f;

		[UnityEngine.Tooltip("Enable computation of geometric diffraction and transmission paths for all sources that have the \"Diffraction and Transmission\" option selected in the Positioning tab of the Wwise Property Editor. This flag enables sound paths around (diffraction) and through (transmission) geometry. Setting EnableGeometricDiffractionAndTransmission to false implies that geometry is only to be used for reflection calculation. Diffraction edges must be enabled on geometry for diffraction calculation. If EnableGeometricDiffractionAndTransmission is false but a sound has \"Diffraction and Transmission\" selected in the Positioning tab of Wwise Authoring, the sound will diffract through portals but pass through geometry as if it isn't there. Typically, we recommend you disable this setting if the game will perform obstruction calculations, but geometry is still passed to Spatial Audio for reflection calculations. The default value is true.")]
		[UnityEngine.Serialization.FormerlySerializedAs("m_EnableDirectPathDiffraction")]
		/// Enable computation of geometric diffraction and transmission paths for all sources that have the \"Diffraction and Transmission\" option selected in the Positioning tab of the Wwise Property Editor.
		/// This flag enables sound paths around (diffraction) and through (transmission) geometry. Setting EnableGeometricDiffractionAndTransmission to false implies that geometry is only to be used for reflection calculation.
		/// Diffraction edges must be enabled on geometry for diffraction calculation.
		/// If EnableGeometricDiffractionAndTransmission is false but a sound has \"Diffraction and Transmission\" selected in the Positioning tab of Wwise Authoring, the sound will diffract through portals but pass through geometry as if it isn't there.
		/// Typically, we recommend you disable this setting if the game will perform obstruction calculations, but geometry is still passed to Spatial Audio for reflection calculations.
		/// The default value is true.
		public bool m_EnableGeometricDiffractionAndTransmission = true;

		[UnityEngine.Tooltip("An emitter that is diffracted through a portal or around geometry will have its apparent or virtual position calculated by Wwise Spatial Audio and passed on to the sound engine. The default value is true.")]
		/// An emitter that is diffracted through a portal or around geometry will have its apparent or virtual position calculated by Wwise Spatial Audio and passed on to the sound engine. The default value is true.
		public bool m_CalcEmitterVirtualPosition = true;

		[UnityEngine.MinAttribute(1)]
		[UnityEngine.Tooltip("The computation of spatial audio paths is spread on LoadBalancingSpread frames. Spreading the computation of paths over several frames can prevent CPU peaks. The spread introduces a delay in path computation. The default value is 1.")]
		/// The computation of spatial audio paths is spread on LoadBalancingSpread frames.
		/// Spreading the computation of paths over several frames can prevent CPU peaks. The spread introduces a delay in path computation. The default value is 1.
		public uint m_LoadBalancingSpread = 1;

		[UnityEngine.MinAttribute(0)]
		[UnityEngine.Tooltip("Limit the maximum number of diffraction paths computed per emitter, excluding the direct/transmission path. The acoustics engine searches for up to uMaxDiffractionPaths paths and stops searching when this limit is reached. Setting a low number for uMaxDiffractionPaths (1-4) uses fewer CPU resources, but is more likely to cause discontinuities in the resulting audio. This can occur, for example, when a more prominent path is discovered, displacing a less prominent one. Conversely, a larger number (8 or more) produces higher quality output but requires more CPU resources. The recommended range is 2-8.")]
		/// Limit the maximum number of diffraction paths computed per emitter, excluding the direct/transmission path. The acoustics engine searches for up to uMaxDiffractionPaths paths and stops searching when this limit is reached.
		/// Setting a low number for uMaxDiffractionPaths (1-4) uses fewer CPU resources, but is more likely to cause discontinuities in the resulting audio. This can occur, for example, when a more prominent path is discovered, displacing a less prominent one.
		/// Conversely, a larger number (8 or more) produces higher quality output but requires more CPU resources. The recommended range is 2-8.
		public uint m_MaxDiffractionPaths = 8;
		
		[UnityEngine.MinAttribute(0)]
		[UnityEngine.Tooltip("[\"Experimental\"] Set a global reflection path limit among all sound emitters with early reflections enabled. Potential reflection paths, discovered by raycasting, are first sorted according to a heuristic to determine which paths are the most prominent. Afterwards, the full reflection path calculation is performed on only the uMaxReflectionPaths, most prominent paths. Limiting the total number of reflection path calculations can significantly reduce CPU usage. Recommended range: 10-50. Set to 0 to disable the limit. In this case, the number of paths computed is unbounded and depends on how many are discovered by raycasting.")]
		/// [\ref spatial_audio_experimental "Experimental"] Set a global reflection path limit among all sound emitters with early reflections enabled. Potential reflection paths, discovered by raycasting, are first sorted according to a heuristic to determine which paths are the most prominent. 
		/// Afterwards, the full reflection path calculation is performed on only the uMaxGlobalReflectionPaths, most prominent paths. Limiting the total number of reflection path calculations can significantly reduce CPU usage. Recommended range: 10-50. 
		/// Set to 0 to disable the limit. In this case, the number of paths computed is unbounded and depends on how many are discovered by raycasting.
		public uint m_MaxGlobalReflectionPaths = 0;

		[UnityEngine.MinAttribute(0.0f)]
		[UnityEngine.Tooltip("The largest possible diffraction value, in degrees, beyond which paths are not computed and are inaudible. Must be greater than zero. Default value: 180 degrees. A large value (for example, 360 degrees) allows paths to propagate further around corners and obstacles, but takes more CPU time to compute. A gain is applied to each diffraction path to taper the volume of the path to zero as the diffraction angle approaches fMaxDiffractionAngleDegrees, and appears in the Voice Inspector as 'Propagation Path Gain'. This tapering gain is applied in addition to the diffraction curves, and prevents paths from popping in or out suddenly when the maximum diffraction angle is exceeded. In Wwise Authoring, the horizontal axis of a diffraction curve in the attenuation editor is defined over the range 0-100%, corresponding to angles 0-180 degrees.  If fMaxDiffractionAngleDegrees is greater than 180 degrees, diffraction coefficients over 100% are clamped and the curve is evaluated at the rightmost point.")]
		/// The largest possible diffraction value, in degrees, beyond which paths are not computed and are inaudible. Must be greater than zero. Default value: 180 degrees.
		/// A large value (for example, 360 degrees) allows paths to propagate further around corners and obstacles, but takes more CPU time to compute.
		/// A gain is applied to each diffraction path to taper the volume of the path to zero as the diffraction angle approaches fMaxDiffractionAngleDegrees,
		/// and appears in the Voice Inspector as "Propagation Path Gain". This tapering gain is applied in addition to the diffraction curves, and prevents paths from popping in or out suddenly when the maximum diffraction angle is exceeded.
		/// In Wwise Authoring, the horizontal axis of a diffraction curve in the attenuation editor is defined over the range 0-100%, corresponding to angles 0-180 degrees.
		/// If fMaxDiffractionAngleDegrees is greater than 180 degrees, diffraction coefficients over 100% are clamped and the curve is evaluated at the rightmost point.
		public float m_MaxDiffractionAngleDegrees = 180.0f;

		[UnityEngine.Tooltip("[\"Experimental\"]  Enable parameter smoothing on the diffraction paths output from the Acoustics Engine. Set fSmoothingConstantMs to a value greater than 0 to define the time constant (in milliseconds) for parameter smoothing. The time constant of an exponential moving average is the amount of time for the smoothed response of a unit step function to reach 1 - 1/e ~= 63.2% of the original signal. A large value (eg. 500-1000 ms) results in less variance but introduces lag, which is a good choice when using conservative values for uNumberOfPrimaryRays (eg. 5-10), uMaxDiffractionPaths (eg. 1-3) or fMovementThreshold ( > 1m ), in order to reduce overall CPU cost. A small value (eg. 10-100 ms) results in greater accuracy and faster convergence of rendering parameters. Set to 0 to disable path smoothing.")]
		/// [\ref spatial_audio_experimental "Experimental"]  Enable parameter smoothing on the diffraction paths output from the Acoustics Engine. Set fSmoothingConstantMs to a value greater than 0 to define the time constant (in milliseconds) for parameter smoothing.
		/// The time constant of an exponential moving average is the amount of time for the smoothed response of a unit step function to reach 1 - 1/e ~= 63.2% of the original signal.
		/// A large value (eg. 500-1000 ms) results in less variance but introduces lag, which is a good choice when using conservative values for uNumberOfPrimaryRays (eg. 5-10), uMaxDiffractionPaths (eg. 1-3) or fMovementThreshold ( > 1m ), in order to reduce overall CPU cost.
		/// A small value (eg. 10-100 ms) results in greater accuracy and faster convergence of rendering parameters. Set to 0 to disable path smoothing.
		public float m_SmoothingConstantMs = 0.0f;
		
		[UnityEngine.Tooltip("The operation used to determine transmission loss on direct paths.")]
		public TransmissionOperation m_TransmissionOperation = TransmissionOperation.Max;

		[UnityEngine.Tooltip("[\"Experimental\"] Enable/Disable emitter clustering by defining the minimum number of emitters in a cluster. Default value is 0. Values less than 2 disable the clustering.")]
		///< Note: Emitters with multi-positions are not clustered and are treated as independant emitters.
		///< Note: Changing an emitter from single to multi-positions with load balancing enabled (see \ref AkSpatialAudioInitSettings::uLoadBalancingSpread) might lead to unknown behaviors for a few frames.)
		public uint m_ClusteringMinPoints = 0;

		[UnityEngine.MinAttribute(0)]
		[UnityEngine.Tooltip("Max distance between emitters to be considered as neighbors. This distance is specified for the reference distance defined by m_ClusteringDeadZoneDistance. Default value is 5.0.")]
		public float m_ClusteringMaxDistance = 5.0f;

		[UnityEngine.MinAttribute(0)]
		[UnityEngine.Tooltip("Defines a dead zone around the listener where no emitters are clusters. Default value is 10.0.")]
		public float m_ClusteringDeadZoneDistance = 10.0f;
	}

	[UnityEngine.Tooltip("Spatial audio common settings.")]
	public SpatialAudioSettings m_SpatialAudioSettings;

	public virtual void CopyTo(AkSpatialAudioInitSettings settings)
	{
		settings.uMaxSoundPropagationDepth = m_SpatialAudioSettings.m_MaxSoundPropagationDepth;
		settings.fMovementThreshold = m_SpatialAudioSettings.m_MovementThreshold;
		settings.uNumberOfPrimaryRays = m_SpatialAudioSettings.m_NumberOfPrimaryRays;
		settings.uMaxReflectionOrder = m_SpatialAudioSettings.m_MaxReflectionOrder;
		settings.uMaxDiffractionOrder = m_SpatialAudioSettings.m_MaxDiffractionOrder;
		settings.uMaxDiffractionPaths = m_SpatialAudioSettings.m_MaxDiffractionPaths;
		settings.uMaxGlobalReflectionPaths = m_SpatialAudioSettings.m_MaxGlobalReflectionPaths;
		settings.uMaxEmitterRoomAuxSends = m_SpatialAudioSettings.m_MaxEmitterRoomAuxSends;
		settings.uDiffractionOnReflectionsOrder = m_SpatialAudioSettings.m_DiffractionOnReflectionsOrder;
		settings.fMaxDiffractionAngleDegrees = m_SpatialAudioSettings.m_MaxDiffractionAngleDegrees;
		settings.fMaxPathLength = m_SpatialAudioSettings.m_MaxPathLength;
		settings.fCPULimitPercentage = m_SpatialAudioSettings.m_CPULimitPercentage;
		settings.fSmoothingConstantMs = m_SpatialAudioSettings.m_SmoothingConstantMs;
		settings.uLoadBalancingSpread = m_SpatialAudioSettings.m_LoadBalancingSpread;
		settings.bEnableGeometricDiffractionAndTransmission = m_SpatialAudioSettings.m_EnableGeometricDiffractionAndTransmission;
		settings.bCalcEmitterVirtualPosition = m_SpatialAudioSettings.m_CalcEmitterVirtualPosition;
		settings.eTransmissionOperation = (AkTransmissionOperation)m_SpatialAudioSettings.m_TransmissionOperation;
		settings.uClusteringMinPoints = m_SpatialAudioSettings.m_ClusteringMinPoints;
		settings.fClusteringMaxDistance = m_SpatialAudioSettings.m_ClusteringMaxDistance;
		settings.fClusteringDeadZoneDistance = m_SpatialAudioSettings.m_ClusteringDeadZoneDistance;
	}

	public virtual void Validate()
	{
		if (m_SpatialAudioSettings.m_MovementThreshold < 0.0f)
		{
			m_SpatialAudioSettings.m_MovementThreshold = 0.0f;
		}

		if (m_SpatialAudioSettings.m_MaxPathLength < 0.0f)
		{
			m_SpatialAudioSettings.m_MaxPathLength = 0.0f;
		}

		if (m_SpatialAudioSettings.m_CPULimitPercentage < 0.0f)
		{
			m_SpatialAudioSettings.m_CPULimitPercentage = 0.0f;
		}
		else if (m_SpatialAudioSettings.m_CPULimitPercentage > 100.0f)
		{
			m_SpatialAudioSettings.m_CPULimitPercentage = 100.0f;
		}
	}
}

[System.Serializable]
public class AkCommonAdvancedSettings
{
	[UnityEngine.Tooltip("Size of memory pool for I/O (for automatic streams). It is rounded down to a multiple of uGranularity and then passed directly to AK::MemoryMgr::CreatePool().")]
	public uint m_IOMemorySize = 2 * 1024 * 1024;

	[UnityEngine.Tooltip("Targeted automatic stream buffer length (ms). When a stream reaches that buffering, it stops being scheduled for I/O except if the scheduler is idle.")]
	public float m_TargetAutoStreamBufferLengthMs = 380.0f;

	[UnityEngine.Tooltip("If true the device attempts to reuse IO buffers that have already been streamed from disk. This is particularly useful when streaming small looping sounds. The drawback is a small CPU hit when allocating memory, and a slightly larger memory footprint in the StreamManager pool.")]
	public bool m_UseStreamCache = false;

	[UnityEngine.Tooltip("Default settings for loading banks.This setting can be overriden by each bank.")]
	public bool m_LoadBankAsynchronously = false;

	[UnityEngine.Tooltip("Maximum number of bytes that can be \"pinned\" using AK::SoundEngine::PinEventInStreamCache() or AK::IAkStreamMgr::PinFileInCache()")]
	public uint m_MaximumPinnedBytesInCache = unchecked((uint)(-1));

	public virtual void CopyTo(AkDeviceSettings settings)
	{
		settings.uIOMemorySize = m_IOMemorySize;
		settings.fTargetAutoStmBufferLength = m_TargetAutoStreamBufferLengthMs;
		settings.bUseStreamCache = m_UseStreamCache;
		settings.uMaxCachePinnedBytes = m_MaximumPinnedBytesInCache;
	}

	[UnityEngine.Tooltip("Set to true to enable AK::SoundEngine::PrepareGameSync usage.")]
	public bool m_EnableGameSyncPreparation = false;

	[UnityEngine.Tooltip("Number of quanta ahead when continuous containers instantiate a new voice before the following sounds start playing. This look-ahead time allows I/O to occur, and is especially useful to reduce the latency of continuous containers with trigger rate or sample-accurate transitions.")]
	public uint m_ContinuousPlaybackLookAhead = 1;

	[UnityEngine.Tooltip("Size of the monitoring queue pool. This parameter is ignored in Release build.")]
	public uint m_MonitorQueuePoolSize = 1024 * 1024;

	[UnityEngine.Tooltip("Time (in milliseconds) to wait to wait for hardware devices to trigger an audio interrupt. If there is no interrupt after that time, the sound engine reverts to silent mode and continues operating until the hardware responds.")]
	public uint m_MaximumHardwareTimeoutMs = 1000;

	[UnityEngine.Tooltip("Debug setting: Enable checks for out-of-range (and NAN) floats in the processing code. Do not enable in any normal usage because this setting uses a lot of CPU. It prints error messages in the log if invalid values are found at various points in the pipeline. Contact AK Support with the new error messages for more information.")]
	public bool m_DebugOutOfRangeCheckEnabled = false;

	[UnityEngine.Tooltip("Debug setting: Only used when bDebugOutOfRangeCheckEnabled is true. This defines the maximum values samples can have. Normal audio must be contained within +1/-1. Set this limit to a value greater than 1 to allow temporary or short excursions out of range. The default value is 16.")]
	public float m_DebugOutOfRangeLimit = 16.0f;

	public virtual void CopyTo(AkInitSettings settings)
	{
		settings.bEnableGameSyncPreparation = m_EnableGameSyncPreparation;
		settings.uContinuousPlaybackLookAhead = m_ContinuousPlaybackLookAhead;
		settings.uMonitorQueuePoolSize = m_MonitorQueuePoolSize;
		settings.uMaxHardwareTimeoutMs = m_MaximumHardwareTimeoutMs;
		settings.bDebugOutOfRangeCheckEnabled = m_DebugOutOfRangeCheckEnabled;
		settings.fDebugOutOfRangeLimit = m_DebugOutOfRangeLimit;		
	}

	public virtual void CopyTo(AkPlatformInitSettings settings) { }

	[UnityEngine.Tooltip("Whether to suspend the Wwise SoundEngine when the application loses focus.")]
	public bool m_SuspendAudioDuringFocusLoss = true;

	[UnityEngine.Tooltip("Only used when \"Suspend Audio During Focus Loss\" is enabled. The state of the \"in_bRenderAnyway\" argument passed to the AkUnitySoundEngine.Suspend() function when the \"OnApplicationFocus\" Unity callback is received with \"false\" as its argument.")]
	public bool m_RenderDuringFocusLoss;

	[UnityEngine.Tooltip("Sets the sub-folder underneath UnityEngine.Application.persistentDataPath that will be used as the SoundBank base path. This is useful when the Init.bnk needs to be downloaded. Setting this to an empty string uses the typical SoundBank base path resolution. Setting this to \".\" uses UnityEngine.Application.persistentDataPath.")]
	public string m_SoundBankPersistentDataPath;
	
	[UnityEngine.Tooltip("Configures whether sub-folders are created in output folders. This needs to match the \"Create sub-folders for generated files\" SoundBank setting in Wwise Authoring.")]
	public bool m_UseSubFoldersForGeneratedFiles;

	[UnityEngine.Tooltip("Initial size of SBA portion of the Primary Memory Arena.")]
	public uint m_MemoryPrimarySbaInitSize = 2097152;
	[UnityEngine.Tooltip("Initial size of TLSF portion of the Primary Memory Arena.")]
	public uint m_MemoryPrimaryTlsfInitSize = 2097152;
	[UnityEngine.Tooltip("Size of each secondary span initialized for TLSF portion of the Primary Memory Arena.")]
	public uint m_MemoryPrimaryTlsfSpanSize = 2097152;
	[UnityEngine.Tooltip("Maximum amount of memory will be reserved for the Primary Memory Arena. A value of 0 will indicate no limit.")]
	public uint m_MemoryPrimaryReservedLimit = 0;
	[UnityEngine.Tooltip("Minimum size of allocations to be considered 'Huge' for the Primary Memory Arena. Huge allocations are put into standalone spans, separate from the TLSF spans")]
	public uint m_MemoryPrimaryAllocSizeHuge = 2097152;

	[UnityEngine.Tooltip("Initial size of TLSF portion of the Media Memory Arena.")]
	public uint m_MemoryMediaTlsfInitSize = 2097152;
	[UnityEngine.Tooltip("Size of each secondary span initialized for TLSF portion of the Media Memory Arena.")]
	public uint m_MemoryMediaTlsfSpanSize = 2097152;
	[UnityEngine.Tooltip("Maximum amount of memory will be reserved for the Media Memory Arena. A value of 0 will indicate no limit.")]
	public uint m_MemoryMediaReservedLimit = 0;
	[UnityEngine.Tooltip("Minimum size of allocations to be considered 'Huge' for the Media Memory Arena. Huge allocations are put into standalone spans, separate from the TLSF spans")]
	public uint m_MemoryMediaAllocSizeHuge = 2097152;

	[UnityEngine.Tooltip("Memory allocator debug level. For use under Audiokinetic Support supervision.")]
	public uint m_MemoryDebugLevel = 0;
}

[System.Serializable]
public class AkCommonCommSettings
{
	[UnityEngine.Tooltip("Size of the communication pool.")]
	public uint m_PoolSize = 256 * 1024;

	public static ushort DefaultDiscoveryBroadcastPort = 24024;

	[UnityEngine.Tooltip("The port where the authoring application broadcasts \"Game Discovery\" requests to discover games running on the network. Default value: 24024. (Cannot be set to 0)")]
	public ushort m_DiscoveryBroadcastPort = DefaultDiscoveryBroadcastPort;

	[UnityEngine.Tooltip("The \"command\" channel port. Set to 0 to request a dynamic/ephemeral port.")]
	public ushort m_CommandPort;

	[UnityEngine.Tooltip("The \"notification\" channel port. Set to 0 to request a dynamic/ephemeral port.")]
	public ushort m_NotificationPort;

	[UnityEngine.Tooltip("Indicates whether or not to initialize the communication system. Some consoles have critical requirements for initialization of their communications system. Set to false only if your game already uses sockets before sound engine initialization.")]
	public bool m_InitializeSystemComms = true;

	[UnityEngine.Tooltip("The name used to identify this game within the authoring application. Leave empty to use \"UnityEngine.Application.productName\".")]
	public string m_NetworkName;

	[UnityEngine.Tooltip("HTCS communication can only be used with a Nintendo Switch Development Build")]
	public AkCommunicationSettings.AkCommSystem m_commSystem = AkCommunicationSettings.AkCommSystem.AkCommSystem_Socket;

	public virtual void CopyTo(AkCommunicationSettings settings)
	{
		settings.uPoolSize = m_PoolSize;
		settings.uDiscoveryBroadcastPort = m_DiscoveryBroadcastPort;
		settings.uCommandPort = m_CommandPort;
		settings.bInitSystemLib = m_InitializeSystemComms;
		settings.commSystem = m_commSystem;

		string networkName = m_NetworkName;
		if (string.IsNullOrEmpty(networkName))
			networkName = UnityEngine.Application.productName;

#if UNITY_EDITOR
		networkName += " (Editor)";
#endif

		settings.szAppNetworkName = networkName;
	}

	public virtual void Validate() { }
}

public abstract class AkCommonPlatformSettings : AkBasePlatformSettings
{
	protected abstract AkCommonUserSettings GetUserSettings();

	protected abstract AkCommonAdvancedSettings GetAdvancedSettings();

	protected abstract AkCommonCommSettings GetCommsSettings();

	public override AkInitializationSettings AkInitializationSettings
	{
		get
		{
			var settings = base.AkInitializationSettings;
			var userSettings = GetUserSettings();
			userSettings.CopyTo(settings.deviceSettings);
			userSettings.CopyTo(settings.streamMgrSettings);
			userSettings.CopyTo(settings.initSettings);
			userSettings.CopyTo(settings.platformSettings);

			var advancedSettings = GetAdvancedSettings();
			advancedSettings.CopyTo(settings.deviceSettings);
			advancedSettings.CopyTo(settings.initSettings);
			advancedSettings.CopyTo(settings.platformSettings);
			
			settings.bUseSubFoldersForGeneratedFiles = advancedSettings.m_UseSubFoldersForGeneratedFiles;

			settings.uMemoryPrimarySbaInitSize   = advancedSettings.m_MemoryPrimarySbaInitSize;
			settings.uMemoryPrimaryTlsfInitSize  = advancedSettings.m_MemoryPrimaryTlsfInitSize;
			settings.uMemoryPrimaryTlsfSpanSize  = advancedSettings.m_MemoryPrimaryTlsfSpanSize;
			settings.uMemoryPrimaryAllocSizeHuge = advancedSettings.m_MemoryPrimaryAllocSizeHuge;
			settings.uMemoryPrimaryReservedLimit = advancedSettings.m_MemoryPrimaryReservedLimit;

			settings.uMemoryMediaTlsfInitSize    = advancedSettings.m_MemoryMediaTlsfInitSize;
			settings.uMemoryMediaTlsfSpanSize    = advancedSettings.m_MemoryMediaTlsfSpanSize;
			settings.uMemoryMediaAllocSizeHuge   = advancedSettings.m_MemoryMediaAllocSizeHuge;
			settings.uMemoryMediaReservedLimit   = advancedSettings.m_MemoryMediaReservedLimit;
			
			settings.uMemDebugLevel = advancedSettings.m_MemoryDebugLevel;
			return settings;
		}
	}

	public override AkSpatialAudioInitSettings AkSpatialAudioInitSettings
	{
		get
		{
			var settings = base.AkSpatialAudioInitSettings;
			GetUserSettings().CopyTo(settings);
			return settings;
		}
	}

	public override AkCallbackManager.InitializationSettings CallbackManagerInitializationSettings
	{
		get
		{
			return new AkCallbackManager.InitializationSettings { IsLoggingEnabled = GetUserSettings().m_EngineLogging };
		}
	}

	public override string InitialLanguage
	{
		get { return GetUserSettings().m_StartupLanguage; }
	}
	
	public override bool IsDecodedBankEnabled
	{
		get { return GetUserSettings().m_IsDecodedBankEnabled; }
	}
	
	public override bool IsAutoBankEnabled
	{
		get { return GetUserSettings().m_IsAutoBankEnabled; }
		set { GetUserSettings().m_IsAutoBankEnabled = value; }
	}

	public override bool LoadBanksAsynchronously
	{
		get { return GetAdvancedSettings().m_LoadBankAsynchronously; }	
	}

	public override string SoundBankPersistentDataPath
	{
		get { return GetAdvancedSettings().m_SoundBankPersistentDataPath; }
	}

	public override bool SuspendAudioDuringFocusLoss
	{
		get { return GetAdvancedSettings().m_SuspendAudioDuringFocusLoss; }
	}

	public override bool RenderDuringFocusLoss
	{
		get { return GetAdvancedSettings().m_RenderDuringFocusLoss; }
	}

	public override string SoundbankPath
	{
		get { return GetUserSettings().m_BasePath; }
	}
	
	public override bool UseSubFoldersForGeneratedFiles
	{
		get { return GetAdvancedSettings().m_UseSubFoldersForGeneratedFiles; }
	}

	public override uint MemoryDebugLevel
	{
		get { return GetAdvancedSettings().m_MemoryDebugLevel; }
	}

	public override float defaultListenerScalingFactor
	{
		get { return GetUserSettings().m_DefaultListenerScalingFactor; }
	}

	public override AkCommunicationSettings AkCommunicationSettings
	{
		get
		{
			var settings = base.AkCommunicationSettings;
			GetCommsSettings().CopyTo(settings);
			return settings;
		}
	}

#region parameter validation
#if UNITY_EDITOR
	void OnValidate()
	{
		GetUserSettings().Validate();
		GetCommsSettings().Validate();
	}
#endif
#endregion
}

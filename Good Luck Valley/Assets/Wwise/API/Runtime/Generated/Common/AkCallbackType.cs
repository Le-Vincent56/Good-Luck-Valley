#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (https://www.swig.org).
// Version 4.3.0
//
// Do not make changes to this file unless you know what you are doing - modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------


///  Type of callback. Used as a bitfield in methods AK::SoundEngine::PostEvent() and AK::SoundEngine::DynamicSequence::Open().
public enum AkCallbackType {
  /// Callback triggered when reaching the end of an event. No additional callback information.
  AK_EndOfEvent = 1,
  /// Callback triggered when reaching the end of a dynamic sequence item. Callback info can be cast to AkDynamicSequenceItemCallbackInfo.
  AK_EndOfDynamicSequenceItem = 2,
  /// Callback triggered when encountering a marker during playback. Callback info can be cast to AkMarkerCallbackInfo.
  AK_Marker = 4,
  /// Callback triggered when the duration of the sound is known by the sound engine. Callback info can be cast to AkDurationCallbackInfo.
  AK_Duration = 8,
  /// Callback triggered at each frame, letting the client modify the speaker volume matrix. Callback info can be cast to AkSpeakerVolumeMatrixCallbackInfo.
  AK_SpeakerVolumeMatrix = 16,
  /// Callback triggered when playback skips a frame due to stream starvation. No additional callback information.
  AK_Starvation = 32,
  /// Callback triggered when music playlist container must select the next item to play. Callback info can be cast to AkMusicPlaylistCallbackInfo.
  AK_MusicPlaylistSelect = 64,
  /// Callback triggered when a "Play" or "Seek" command has been executed ("Seek" commands are issued from AK::SoundEngine::SeekOnEvent()). Applies to objects of the Interactive-Music Hierarchy only. No additional callback information.
  AK_MusicPlayStarted = 128,
  /// Enable notifications on Music Beat. Callback info can be cast to AkMusicSyncCallbackInfo.
  AK_MusicSyncBeat = 256,
  /// Enable notifications on Music Bar. Callback info can be cast to AkMusicSyncCallbackInfo.
  AK_MusicSyncBar = 512,
  /// Enable notifications on Music Entry Cue. Callback info can be cast to AkMusicSyncCallbackInfo.
  AK_MusicSyncEntry = 1024,
  /// Enable notifications on Music Exit Cue. Callback info can be cast to AkMusicSyncCallbackInfo.
  AK_MusicSyncExit = 2048,
  /// Enable notifications on Music Grid. Callback info can be cast to AkMusicSyncCallbackInfo.
  AK_MusicSyncGrid = 4096,
  /// Enable notifications on Music Custom Cue. Callback info can be cast to AkMusicSyncCallbackInfo.
  AK_MusicSyncUserCue = 8192,
  /// Enable notifications on Music switch transition synchronization point. Callback info can be cast to AkMusicSyncCallbackInfo.
  AK_MusicSyncPoint = 16384,
  /// Enable notifications for MIDI events. Callback info can be cast to AkMIDIEventCallbackInfo.
  AK_MIDIEvent = 32768,
  /// Callback triggered when dynamic sequence must select the next item to play. Callback info can be cast to AkDynamicSequenceSelectCallbackInfo.
  AK_DynamicSequenceSelect = 65536,
  ///  Last callback unused bit, invalid value.
  AK_Callback_Last = 131072,
  /// Use this flag if you want to receive all notifications concerning AK_MusicSync registration.
  AK_MusicSyncAll = 32512,
  /// Bitmask for all callback types.
  AK_CallbackBits = 1048575,
  /// Enable play position information of music objects, queried via AK::MusicEngine::GetPlayingSegmentInfo().
  AK_EnableGetMusicPlayPosition = 2097152,
  /// Enable stream buffering information for use by AK::SoundEngine::GetSourceStreamBuffering().
  AK_EnableGetSourceStreamBuffering = 4194304,
  /// Last source info enable bit, invalid value.
  AK_SourceInfo_Last = 8388608
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
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


///  Metering flags. Used for specifying bus metering, through AK::SoundEngine::RegisterBusVolumeCallback() or AK::IAkMixerPluginContext::SetMeteringFlags().
public enum AkMeteringFlags {
  ///  No metering.
  AK_NoMetering = 0,
  ///  Enable computation of peak metering.
  AK_EnableBusMeter_Peak = 1 << 0,
  ///  Enable computation of true peak metering (most CPU and memory intensive).
  AK_EnableBusMeter_TruePeak = 1 << 1,
  ///  Enable computation of RMS metering.
  AK_EnableBusMeter_RMS = 1 << 2,
  ///  Enable computation of K-weighted power metering (used as a basis for computing loudness, as defined by ITU-R BS.1770).
  AK_EnableBusMeter_KPower = 1 << 4,
  ///  Enable computation of data necessary to render a 3D visualization of volume distribution over the surface of a sphere.
  AK_EnableBusMeter_3DMeter = 1 << 5,
  ///  End of enum, invalid value.
  AK_EnableBusMeter_Last
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
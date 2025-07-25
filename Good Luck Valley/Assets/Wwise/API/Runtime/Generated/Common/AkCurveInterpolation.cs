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


///  Curve interpolation types
public enum AkCurveInterpolation {
  ///  Log3
  AkCurveInterpolation_Log3 = 0,
  ///  Sine
  AkCurveInterpolation_Sine = 1,
  ///  Log1
  AkCurveInterpolation_Log1 = 2,
  ///  Inversed S Curve
  AkCurveInterpolation_InvSCurve = 3,
  ///  Linear (Default)
  AkCurveInterpolation_Linear = 4,
  ///  S Curve
  AkCurveInterpolation_SCurve = 5,
  ///  Exp1
  AkCurveInterpolation_Exp1 = 6,
  ///  Reciprocal of sine curve
  AkCurveInterpolation_SineRecip = 7,
  ///  Exp3
  AkCurveInterpolation_Exp3 = 8,
  ///  Update this value to reflect last curve available for fades
  AkCurveInterpolation_LastFadeCurve = 8,
  ///  Constant ( not valid for fading values )
  AkCurveInterpolation_Constant = 9,
  ///  End of enum, invalid value.
  AkCurveInterpolation_Last
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
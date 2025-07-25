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


public class AkDynamicSequenceItemCallbackInfo : AkCallbackInfo {
  private global::System.IntPtr swigCPtr;

  internal AkDynamicSequenceItemCallbackInfo(global::System.IntPtr cPtr, bool cMemoryOwn) : base(AkUnitySoundEnginePINVOKE.CSharp_AkDynamicSequenceItemCallbackInfo_SWIGUpcast(cPtr), cMemoryOwn) {
    swigCPtr = cPtr;
  }

  internal static global::System.IntPtr getCPtr(AkDynamicSequenceItemCallbackInfo obj) {
    return (obj == null) ? global::System.IntPtr.Zero : obj.swigCPtr;
  }

  internal override void setCPtr(global::System.IntPtr cPtr) {
    base.setCPtr(AkUnitySoundEnginePINVOKE.CSharp_AkDynamicSequenceItemCallbackInfo_SWIGUpcast(cPtr));
    swigCPtr = cPtr;
  }

  protected override void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          AkUnitySoundEnginePINVOKE.CSharp_delete_AkDynamicSequenceItemCallbackInfo(swigCPtr);
        }
        swigCPtr = global::System.IntPtr.Zero;
      }
      global::System.GC.SuppressFinalize(this);
      base.Dispose(disposing);
    }
  }

  ///  Playing ID of Dynamic Sequence, returned by AK::SoundEngine:DynamicSequence::Open()
  public uint playingID { get { return AkUnitySoundEnginePINVOKE.CSharp_AkDynamicSequenceItemCallbackInfo_playingID_get(swigCPtr); } 
  }

  ///  Audio Node ID of finished item
  public uint audioNodeID { get { return AkUnitySoundEnginePINVOKE.CSharp_AkDynamicSequenceItemCallbackInfo_audioNodeID_get(swigCPtr); } 
  }

  ///  Custom info passed to the DynamicSequence::Open function
  public global::System.IntPtr pCustomInfo { get { return AkUnitySoundEnginePINVOKE.CSharp_AkDynamicSequenceItemCallbackInfo_pCustomInfo_get(swigCPtr); }
  }

  public AkDynamicSequenceItemCallbackInfo() : this(AkUnitySoundEnginePINVOKE.CSharp_new_AkDynamicSequenceItemCallbackInfo(), true) {
  }

}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
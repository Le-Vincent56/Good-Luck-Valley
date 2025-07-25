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

/// Structure for retrieving information about paths for a given emitter.
/// The diffraction paths represent indirect sound paths from the emitter to the listener, whether they go through portals
/// (via the rooms and portals API) or are diffracted around edges (via the geometric diffraction API).
/// The direct path is included here and can be identified by checking ``nodeCount`` == 0. The direct path may have a non-zero transmission loss
/// if it passes through geometry or between rooms.

public class AkDiffractionPathInfo : global::System.IDisposable {
  private global::System.IntPtr swigCPtr;
  protected bool swigCMemOwn;

  internal AkDiffractionPathInfo(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = cPtr;
  }

  internal static global::System.IntPtr getCPtr(AkDiffractionPathInfo obj) {
    return (obj == null) ? global::System.IntPtr.Zero : obj.swigCPtr;
  }

  internal virtual void setCPtr(global::System.IntPtr cPtr) {
    Dispose();
    swigCPtr = cPtr;
  }

  ~AkDiffractionPathInfo() {
    Dispose(false);
  }

  public void Dispose() {
    Dispose(true);
    global::System.GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          AkUnitySoundEnginePINVOKE.CSharp_delete_AkDiffractionPathInfo(swigCPtr);
        }
        swigCPtr = global::System.IntPtr.Zero;
      }
      global::System.GC.SuppressFinalize(this);
    }
  }

  /// Emitter position. This is the source position for an emitter. In all cases, except for radial emitters, it is the same position as the game object position.
  /// For radial emitters, it is the calculated position at the edge of the volume.
  public AkVector64 emitterPos { set { AkUnitySoundEnginePINVOKE.CSharp_AkDiffractionPathInfo_emitterPos_set(swigCPtr, value); }  get { return AkUnitySoundEnginePINVOKE.CSharp_AkDiffractionPathInfo_emitterPos_get(swigCPtr); } 
  }

  ///  Virtual emitter position. This is the position that is passed to the sound engine to render the audio using multi-positioning, for this particular path.
  public AkWorldTransform virtualPos { set { AkUnitySoundEnginePINVOKE.CSharp_AkDiffractionPathInfo_virtualPos_set(swigCPtr, AkWorldTransform.getCPtr(value)); } 
    get {
      global::System.IntPtr cPtr = AkUnitySoundEnginePINVOKE.CSharp_AkDiffractionPathInfo_virtualPos_get(swigCPtr);
      AkWorldTransform ret = (cPtr == global::System.IntPtr.Zero) ? null : new AkWorldTransform(cPtr, false);
      return ret;
    } 
  }

  ///  Total number of nodes in the path.  Defines the number of valid entries in the ``nodes``, ``angles``, and ``portals`` arrays. The ``rooms`` array has one extra slot to fit the emitter's room.
  public uint nodeCount { set { AkUnitySoundEnginePINVOKE.CSharp_AkDiffractionPathInfo_nodeCount_set(swigCPtr, value); }  get { return AkUnitySoundEnginePINVOKE.CSharp_AkDiffractionPathInfo_nodeCount_get(swigCPtr); } 
  }

  ///  Calculated total diffraction from this path, normalized to the range [0,1]
  ///  The diffraction amount is calculated from the sum of the deviation angles from a straight line, of all angles at each nodePoint.
  ///  This value is applied internally, by spatial audio, as the Diffraction value and built-in parameter of the emitter game object.
  /// <seealso cref="
  ///  - \ref AkSpatialAudioInitSettings"/>
  public float diffraction { set { AkUnitySoundEnginePINVOKE.CSharp_AkDiffractionPathInfo_diffraction_set(swigCPtr, value); }  get { return AkUnitySoundEnginePINVOKE.CSharp_AkDiffractionPathInfo_diffraction_get(swigCPtr); } 
  }

  ///  Calculated total transmission loss from this path, normalized to the range [0,1]
  ///  This field will be 0 for diffraction paths where ``nodeCount`` &gt; 0. It may be non-zero for the direct path where ``nodeCount`` == 0.
  ///  The path's transmission loss value might be geometric transmission loss, if geometry was intersected in the path,
  ///  or room transmission loss, if no geometry was available.
  ///  The geometric transmission loss is calculated from the transmission loss values assigned to the geometry that this path transmits through.
  ///  If a path transmits through multiple geometries with different transmission loss values, the largest value is taken.
  ///  The room transmission loss is taken from the emitter and listener rooms' transmission loss values, and likewise,
  ///  if the listener's room and the emitter's room have different transmission loss values, the greater of the two is used.
  ///  This value is applied internally, by spatial audio, as the Transmission Loss value and built-in parameter of the emitter game object.
  /// <seealso cref="
  ///  - \ref AkSpatialAudioInitSettings
  ///  - \ref AkRoomParams
  ///  - \ref AkAcousticSurface"/>
  public float transmissionLoss { set { AkUnitySoundEnginePINVOKE.CSharp_AkDiffractionPathInfo_transmissionLoss_set(swigCPtr, value); }  get { return AkUnitySoundEnginePINVOKE.CSharp_AkDiffractionPathInfo_transmissionLoss_get(swigCPtr); } 
  }

  ///  Total path length
  ///  Represents the sum of the length of the individual segments between nodes, with a correction factor applied for diffraction.
  ///  The correction factor simulates the phenomenon where by diffracted sound waves decay faster than incident sound waves and can be customized in the spatial audio init settings.
  /// <seealso cref="
  ///  - \ref AkSpatialAudioInitSettings"/>
  public float totLength { set { AkUnitySoundEnginePINVOKE.CSharp_AkDiffractionPathInfo_totLength_set(swigCPtr, value); }  get { return AkUnitySoundEnginePINVOKE.CSharp_AkDiffractionPathInfo_totLength_get(swigCPtr); } 
  }

  /// Obstruction value for this path
  /// This value includes the accumulated portal obstruction for all portals along the path.
  public float obstructionValue { set { AkUnitySoundEnginePINVOKE.CSharp_AkDiffractionPathInfo_obstructionValue_set(swigCPtr, value); }  get { return AkUnitySoundEnginePINVOKE.CSharp_AkDiffractionPathInfo_obstructionValue_get(swigCPtr); } 
  }

  /// Occlusion value for this path
  /// This value includes the accumulated portal occlusion for all portals along the path.
  public float occlusionValue { set { AkUnitySoundEnginePINVOKE.CSharp_AkDiffractionPathInfo_occlusionValue_set(swigCPtr, value); }  get { return AkUnitySoundEnginePINVOKE.CSharp_AkDiffractionPathInfo_occlusionValue_get(swigCPtr); } 
  }

  /// Propagation path gain.
  /// Includes volume tapering gain to ensure that diffraction paths do not cut in or out when the maximum diffraction angle is exceeded.
  public float gain { set { AkUnitySoundEnginePINVOKE.CSharp_AkDiffractionPathInfo_gain_set(swigCPtr, value); }  get { return AkUnitySoundEnginePINVOKE.CSharp_AkDiffractionPathInfo_gain_get(swigCPtr); } 
  }

  public static int GetSizeOf() { return AkUnitySoundEnginePINVOKE.CSharp_AkDiffractionPathInfo_GetSizeOf(); }

  public UnityEngine.Vector3 GetNodes(uint idx) { return AkUnitySoundEnginePINVOKE.CSharp_AkDiffractionPathInfo_GetNodes(swigCPtr, idx); }

  public float GetAngles(uint idx) { return AkUnitySoundEnginePINVOKE.CSharp_AkDiffractionPathInfo_GetAngles(swigCPtr, idx); }

  public ulong GetPortals(uint idx) { return AkUnitySoundEnginePINVOKE.CSharp_AkDiffractionPathInfo_GetPortals(swigCPtr, idx); }

  public ulong GetRooms(uint idx) { return AkUnitySoundEnginePINVOKE.CSharp_AkDiffractionPathInfo_GetRooms(swigCPtr, idx); }

  public void Clone(AkDiffractionPathInfo other) { AkUnitySoundEnginePINVOKE.CSharp_AkDiffractionPathInfo_Clone(swigCPtr, AkDiffractionPathInfo.getCPtr(other)); }

  public AkDiffractionPathInfo() : this(AkUnitySoundEnginePINVOKE.CSharp_new_AkDiffractionPathInfo(), true) {
  }

  ///  Defines the maximum number of nodes that a user can retrieve information about.  Longer paths will be truncated.
  public const uint kMaxNodes = 8;
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
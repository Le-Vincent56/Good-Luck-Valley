using Cinemachine;
using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.Persistence;
using GoodLuckValley.Timers;
using UnityEngine;

namespace GoodLuckValley.Cameras.Persistence
{
    public class CameraSaveHandler : MonoBehaviour, IBind<CameraData>
    {
        private SaveLoadSystem saveLoadSystem;
        private CinemachineVirtualCamera virtualCamera;
        private CinemachineComponentBase component;
        private bool dampingCorrected = true;
        [SerializeField] private CameraData data;

        private CountdownTimer correctDampTimer;

        [field: SerializeField] public SerializableGuid ID { get; set; } = SerializableGuid.NewGuid();

        private void Awake()
        {
            // Get services
            saveLoadSystem = ServiceLocator.Global.Get<SaveLoadSystem>();
        }

        private void OnDestroy()
        {
            // Dispose of the correct damp timer
            correctDampTimer?.Dispose();
        }

        private void LateUpdate()
        {
            // Exit case - the Camera Data or the virtual Camera have not been set
            if (data == null || virtualCamera == null) return;

            // Update the data
            data.Position = transform.position;
            data.Priority = virtualCamera.Priority;
            data.OrthographicSize = virtualCamera.m_Lens.OrthographicSize;

            // Exit case - there's no Component Base
            if (component == null) return;

            // Check if the component is a Framing Transposer
            if (component is CinemachineFramingTransposer)
            {
                // Cast the component
                CinemachineFramingTransposer transposer = component as CinemachineFramingTransposer;

                // Update the data
                data.Offset = transposer.m_TrackedObjectOffset;

                // Exit case - the damping has not been corrected yet
                if (!dampingCorrected) return;

                // Update the damping
                data.Damping = new Vector3(transposer.m_XDamping, transposer.m_YDamping, transposer.m_ZDamping);
            }
            // Otherwise, check if the component is a Tracked Dolly
            else if (component is CinemachineTrackedDolly)
            {
                // Cast the component
                CinemachineTrackedDolly dolly = component as CinemachineTrackedDolly;

                // Update the data
                data.Offset = dolly.m_PathOffset;
                data.PathPosition = dolly.m_PathPosition;

                // Exit case - the damping has not been corrected yet
                if (!dampingCorrected) return;

                // Update the damping
                data.Damping = new Vector3(dolly.m_XDamping, dolly.m_YDamping, dolly.m_ZDamping);
            }
        }

        /// <summary>
        /// Create a damp correction timer to allow for instant position setting
        /// </summary>
        private void CreateDampCorrectionTimer()
        {
            // Set damping corrected to false
            dampingCorrected = false;

            // Initialize the timer
            correctDampTimer = new CountdownTimer(1f);
            correctDampTimer.OnTimerStop += () =>
            {
                // Exit case - there's no Component Base
                if (component == null) return;

                // Check if the component is a Framing Transposer
                if (component is CinemachineFramingTransposer)
                {
                    // Cast the component
                    CinemachineFramingTransposer transposer = component as CinemachineFramingTransposer;

                    // Set the damping back to their values
                    transposer.m_XDamping = data.Damping.x;
                    transposer.m_YDamping = data.Damping.y;
                    transposer.m_ZDamping = data.Damping.z;
                }
                // Otherwise, check if the component is a Tracked Dolly
                else if (component is CinemachineTrackedDolly)
                {
                    // Cast the component
                    CinemachineTrackedDolly dolly = component as CinemachineTrackedDolly;

                    // Set the damping back to their values
                    dolly.m_XDamping = data.Damping.x;
                    dolly.m_YDamping = data.Damping.y;
                    dolly.m_ZDamping = data.Damping.z;
                }

                // Set damping to corrected
                dampingCorrected = true;
            };

            // Start the timer
            correctDampTimer.Start();
        }

        /// <summary>
        /// Bind the Camera Data
        /// </summary>
        public void Bind(CameraData data)
        {
            // Bind the data
            this.data = data;
            this.data.ID = data.ID;

            // Get the Virtual Camera
            virtualCamera = GetComponent<CinemachineVirtualCamera>();

            // Attempt to get a Component Base from the Virtual Camera
            component = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);

            // Exit case - if debugging
            if (saveLoadSystem.Debug) return;

            // Exit case - no data has been saved yet
            if (data.Priority == 0 || data.OrthographicSize == 0.0f) return;

            // Set the data
            transform.position = data.Position;
            virtualCamera.Priority = data.Priority;
            virtualCamera.m_Lens.OrthographicSize = data.OrthographicSize;

            // Exit case - there's no Component Base
            if (component == null) return;

            // Check if the component is a Framing Transposer
            if (component is CinemachineFramingTransposer)
            {
                // Cast the component and set the tracked object offset
                CinemachineFramingTransposer transposer = component as CinemachineFramingTransposer;

                // Nullify damping for instant movement
                transposer.m_XDamping = 0.0f;
                transposer.m_YDamping = 0.0f;
                transposer.m_ZDamping = 0.0f;

                // Set the tracked object offset
                transposer.m_TrackedObjectOffset = data.Offset;

                // Set to correct damping
                CreateDampCorrectionTimer();
            }
            // Otherwise, check if the component is a Tracked Dolly
            else if (component is CinemachineTrackedDolly)
            {
                // Cast the component and set the position
                CinemachineTrackedDolly dolly = component as CinemachineTrackedDolly;

                // Nullify damping for instant movement
                dolly.m_XDamping = 0.0f;
                dolly.m_YawDamping = 0.0f;
                dolly.m_ZDamping = 0.0f;

                // Set the path offset and position
                dolly.m_PathOffset = data.Offset;
                dolly.m_PathPosition = data.PathPosition;

                // Set to correct damping
                CreateDampCorrectionTimer();
            }
        }
    }
}

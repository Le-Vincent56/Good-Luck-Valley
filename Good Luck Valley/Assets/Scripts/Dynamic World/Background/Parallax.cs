using UnityEngine;
using HiveMind.Events;

namespace HiveMind.Background
{
    public class Parallax : MonoBehaviour
    {
        #region REFERENCES
        [SerializeField] private CameraScriptableObj cameraEvent;
        [SerializeField] private MovementScriptableObj movementEvent;
        [SerializeField] private DisableScriptableObj disableEvent;
        #endregion

        #region FIELDS
        [SerializeField] private float parallaxSpeed;
        [Header("-1 = left scrolling parallax | 1 = right scrolling parallax")]
        [Range(-1f, 1f)]
        [SerializeField] private int direction;
        [Header("0: no parallax | 1: parallax speed = anari speed")]
        [Range(0f, 1f)]
        [SerializeField] private float parallaxMultiplyValue;
        #endregion

        private void OnEnable()
        {
            cameraEvent.moveEvent.AddListener(UpdateParallax);
        }

        private void OnDisable()
        {
            cameraEvent.moveEvent.RemoveListener(UpdateParallax);
        }

        private void UpdateParallax()
        {
            // Set the parallax scrolling
            parallaxSpeed = (movementEvent.GetMovementVector().x * parallaxMultiplyValue * direction);

            if (disableEvent.GetDisableParallax() == false)
            {
                transform.position = new Vector3(transform.position.x + (parallaxSpeed * Time.deltaTime), transform.position.y, transform.position.z);
            }
        }
    }
}
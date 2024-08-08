using GoodLuckValley.Events;
using System.Linq;
using UnityEngine;

namespace GoodLuckValley.Player.Control
{
    public class PlayerHardpointsContainer : MonoBehaviour
    {
        [SerializeField] private GameEvent onSendHardpoints;

        public struct Hardpoints
        {
            public Transform Run;
            public Transform Bounce;

            public override string ToString()
            {
                return $"Run: {Run.position}\n Bounce: {Bounce.position}";
            }
        }

        [SerializeField] private Transform[] hardpoints;
        private Hardpoints container;

        private void Awake()
        {
            // Get all hardpoints
            Transform[] allHardpoints = GetComponentsInChildren<Transform>();

            // Skip the first (this transform)  
            hardpoints = allHardpoints.Skip(1).ToArray();

            // Set hardpoints
            container.Run = hardpoints[0];
            container.Bounce = hardpoints[1];

            // Set out the hardpoints
            onSendHardpoints.Raise(this, container);
        }

        public void SendHardpoints(Component sender, object data) => onSendHardpoints.Raise(this, container);
    }
}
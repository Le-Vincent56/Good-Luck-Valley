using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;
using GoodLuckValley.Audio.Music;

namespace GoodLuckValley.World.AreaTriggers
{
    [RequireComponent(typeof(AreaCollider))]
    public class MusicTrigger : MonoBehaviour
    {
        public enum Type
        {
            Play,
            Stop,
            Pause,
            Resume,
            StateChange
        }

        [SerializeField] private Type type;
        private AreaCollider areaCollider;

        [SerializeField] private bool permanentChange;
        [SerializeField] private bool disableStates;

        [SerializeField] private List<State> statesToSet;
        [SerializeField] private List<State> statesToDisable;

        public Type TriggerType
        {
            get => type; 
            set => type = value;
        }

        public bool PermanentChange
        {
            get => permanentChange;
            set => permanentChange = value;
        }

        private void Awake()
        {
            areaCollider = GetComponent<AreaCollider>();
        }

        private void OnEnable()
        {
            areaCollider.OnTriggerEnter += TriggerEnter;
        }

        private void OnDisable()
        {
            areaCollider.OnTriggerEnter -= TriggerEnter;
        }

        private void TriggerEnter(GameObject other)
        {
            switch (type)
            {
                case Type.Play:
                    break;

                case Type.Pause:
                    break;

                case Type.Stop:
                    break;

                case Type.StateChange:
                    // Set each state
                    foreach(State state in statesToSet)
                    {
                        MusicManager.Instance.SetState(state, permanentChange);
                    }

                    // If not disabling states, return
                    if (!disableStates) return;

                    // Disable states
                    foreach(State state in statesToDisable)
                    {
                        MusicManager.Instance.DisableState(state);
                    }
                    break;
            }
        }
    }
}
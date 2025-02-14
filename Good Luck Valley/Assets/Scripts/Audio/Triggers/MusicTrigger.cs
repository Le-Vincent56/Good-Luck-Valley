using GoodLuckValley.Player.Movement;
using GoodLuckValley.World.Triggers;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Audio.Triggers
{
    public class MusicTrigger : BaseTrigger
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

        [SerializeField] private bool permanentChange;
        [SerializeField] private bool disableStates;

        [SerializeField] private List<AK.Wwise.State> statesToSet;
        [SerializeField] private List<AK.Wwise.State> statesToDisable;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Exit case - there's no Player Controller on the collider
            if (!collision.TryGetComponent(out PlayerController controller)) return;

            switch (type)
            {
                case Type.Play:
                    // Set each state
                    foreach (AK.Wwise.State state in statesToSet)
                    {
                        MusicManager.Instance.SetState(state, permanentChange);
                    }

                    // Play the music
                    MusicManager.Instance.Play();
                    break;

                case Type.Pause:
                    break;

                case Type.Stop:
                    break;

                case Type.StateChange:
                    // Set each state
                    foreach (AK.Wwise.State state in statesToSet)
                    {
                        MusicManager.Instance.SetState(state, permanentChange);
                    }

                    // If not disabling states, return
                    if (!disableStates) return;

                    // Disable states
                    foreach (AK.Wwise.State state in statesToDisable)
                    {
                        MusicManager.Instance.DisableState(state);
                    }
                    break;
            }
        }
    }
}

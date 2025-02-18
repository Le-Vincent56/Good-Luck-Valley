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
        [SerializeField] private List<AK.Wwise.State> statesToSet;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Exit case - there's no Player Controller on the collider
            if (!collision.TryGetComponent(out PlayerController controller)) return;

            switch (type)
            {
                case Type.Play:
                    // Set each state
                    MusicManager.Instance.SetStates(statesToSet);

                    // Play the music
                    MusicManager.Instance.Play();
                    break;

                case Type.Pause:
                    break;

                case Type.Stop:
                    break;

                case Type.StateChange:
                    // Set each state
                    MusicManager.Instance.SetStates(statesToSet);
                    break;
            }
        }
    }
}

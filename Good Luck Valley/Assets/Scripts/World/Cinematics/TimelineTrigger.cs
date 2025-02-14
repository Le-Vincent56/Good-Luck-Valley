using GoodLuckValley.Events;
using GoodLuckValley.Events.Cinematics;
using GoodLuckValley.World.Triggers;
using UnityEngine;
using UnityEngine.Playables;

namespace GoodLuckValley.World.Episodes
{
    public class TimelineTrigger : BaseTrigger
    {
        private bool played;
        [SerializeField] private PlayableDirector director;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Exit case - already played the timeline
            if (played) return;

            // Play the timeline
            director.Play();

            // Set to played
            played = true;
        }

        public void ActivatePlayer() => EventBus<EndCinematic>.Raise(new EndCinematic());

        public void DeactivatePlayer() => EventBus<StartCinematic>.Raise(new StartCinematic());
    }
}

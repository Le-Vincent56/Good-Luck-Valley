using GoodLuckValley.Events;
using GoodLuckValley.Events.Cinematics;
using UnityEngine;
using UnityEngine.Playables;

namespace GoodLuckValley
{
    public class CinematicPlayer : MonoBehaviour
    {
        private PlayableDirector director;

        [Header("Fields")]
        [SerializeField] private int id;
        [SerializeField] private bool played;

        private EventBinding<PlayTimeline> onPlayTimeline;

        private void Awake()
        {
            director = GetComponent<PlayableDirector>();

            played = false;
        }

        private void OnEnable()
        {
            onPlayTimeline = new EventBinding<PlayTimeline>(PlayTimeline);
            EventBus<PlayTimeline>.Register(onPlayTimeline);
        }

        private void OnDisable()
        {
            EventBus<PlayTimeline>.Deregister(onPlayTimeline);
        }

        /// <summary>
        /// Play the timeline
        /// </summary>
        private void PlayTimeline(PlayTimeline eventData)
        {
            // Exit case - the timeline has already been played
            if (played) return;

            // Exit case - the ID does not match
            if (eventData.ID != id) return;

            // Play the timeline
            director.Play();

            // Set played to true
            played = true;
        }
    }
}

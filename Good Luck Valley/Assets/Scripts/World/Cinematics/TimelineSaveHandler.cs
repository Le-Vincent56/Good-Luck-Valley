using GoodLuckValley.Persistence;
using UnityEngine;
using UnityEngine.Playables;

namespace GoodLuckValley.World.Cinematics.Persistence
{
    public class TimelineSaveHandler : MonoBehaviour, IBind<TimelineData>
    {
        [Header("References")]
        [SerializeField] private PlayableDirector director;

        [Header("Data")]
        [SerializeField] private TimelineData data;
        [SerializeField] private bool played;
        [field: SerializeField] public SerializableGuid ID { get; set; } = SerializableGuid.NewGuid();

        public void UpdateSaveData()
        {
            // Update the data
            played = true;
            data.Played = played;
        }

        public void Bind(TimelineData data)
        {
            // Set the data
            this.data = data;
            this.data.ID = data.ID;
            played = data.Played;

            // Get the Playable Director
            director = GetComponent<PlayableDirector>();

            // Exit case - the Timeline has not been played
            if (!data.Played) return;

            // Set the Timeline to its end state
            director.time = director.duration;

            // Disable the director
            director.enabled = false;
        }
    }
}

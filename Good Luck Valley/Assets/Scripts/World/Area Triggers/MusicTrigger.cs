using GoodLuckValley.Audio.Music;
using GoodLuckValley.World.AreaTriggers;
using UnityEngine;

namespace GoodLuckValley.AreaTriggers.Music
{
    public class MusicTrigger : MonoBehaviour
    {
        private AreaCollider areaCollider;

        [Header("Fields")]
        [SerializeField] private AudioClip transition;
        [SerializeField] private AudioClip newLoop;

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
            // If the music is currently looping, stop looping
            if (MusicManager.Instance.current.loop && MusicManager.Instance.canEnqueue)
                MusicManager.Instance.current.loop = false;

            MusicManager.Instance.AddToPlaylist(transition, true);
            MusicManager.Instance.AddToPlaylist(newLoop);
            MusicManager.Instance.canEnqueue = false;
        }
    }
}
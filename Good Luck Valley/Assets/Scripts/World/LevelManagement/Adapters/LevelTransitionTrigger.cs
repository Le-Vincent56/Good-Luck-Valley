using GoodLuckValley.Core.DI.Attributes;
using GoodLuckValley.World.LevelManagement.Data;
using GoodLuckValley.World.LevelManagement.Interfaces;
using UnityEngine;

namespace GoodLuckValley.World.LevelManagement.Adapters
{
    /// <summary>
    /// Thin MonoBehaviour adapter placed on trigger zones in level scenes.
    /// When the player enters the trigger, initiates a level transition via
    /// <see cref="ILevelManager"/>. The Collider2D must be set to Is Trigger.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class LevelTransitionTrigger : MonoBehaviour
    {
        [Header("Target")] 
        [SerializeField] private LevelData targetLevel;
        [SerializeField] private string targetSpawnPointID;

        [Header("Transition (optional override)")] 
        [SerializeField] private TransitionConfig transitionConfig;

        [Inject] private ILevelManager _levelManager;

        private bool _triggered;

        /// <summary>
        /// The target level this trigger transitions to.
        /// </summary>
        public LevelData TargetLevel => targetLevel;

        /// <summary>
        /// The spawn point ID in the target scene.
        /// </summary>
        public string TargetSpawnPointID => targetSpawnPointID;

        private async void OnTriggerEnter2D(Collider2D other)
        {
            if (_triggered) return;
            
            // TODO: Replace tag-based player detection with layer mask check
            // or component-based detection for performance
            if (!other.CompareTag("Player")) return;

            _triggered = true;

            await _levelManager.TransitionToLevelAsync(
                targetLevel, 
                targetSpawnPointID,
                transitionConfig
            );
        }
    }
}
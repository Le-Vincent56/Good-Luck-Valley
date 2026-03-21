using UnityEngine;

namespace GoodLuckValley.World.LevelManagement.Adapters
{
    /// <summary>
    /// Marks a spawn position in a level scene. Sole source of truth for
    /// spawn positions — <see cref="Data.LevelData"/> stores IDs for editor
    /// validation only, never positions. Place on an empty GameObject at the
    /// desired spawn location.
    /// </summary>
    public class SpawnPointMarker : MonoBehaviour
    {
        [SerializeField] private string spawnPointID;
        [SerializeField] private bool faceRight = true;

        /// <summary>
        /// Unique identifier for this spawn point within the level.
        /// Referenced by <see cref="LevelTransitionTrigger"/> and
        /// <see cref="Data.LevelRegistry.StartingSpawnPointID"/>.
        /// </summary>
        public string SpawnPointID => spawnPointID;

        /// <summary>
        /// Whether the player should face right when spawning here.
        /// </summary>
        public bool FaceRight => faceRight;

        /// <summary>
        /// The world-space spawn position, derived from this GameObject's transform.
        /// </summary>
        public Vector2 Position => (Vector2)transform.position;
    }
}
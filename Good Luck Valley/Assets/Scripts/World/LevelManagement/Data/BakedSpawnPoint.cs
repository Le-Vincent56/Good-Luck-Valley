using System;
using UnityEngine;

namespace GoodLuckValley.World.LevelManagement.Data
{
    /// <summary>
    /// Immutable snapshot of a spawn point baked from a <see cref="Adapters.SpawnPointMarker"/>
    /// in the scene. Stored in <see cref="LevelData"/> at edit time so that spawn metadata
    /// is available without loading the scene (for editor dropdowns, validation, etc.).
    /// Runtime positioning still uses <see cref="Adapters.SpawnPointMarker"/> as the source of truth.
    /// </summary>
    [Serializable]
    public struct BakedSpawnPoint
    {
        [SerializeField] private string id;
        [SerializeField] private int stableID;
        [SerializeField] private Vector2 position;
        [SerializeField] private bool faceRight;

        /// <summary>
        /// The spawn point's string identifier, matching <see cref="Adapters.SpawnPointMarker.SpawnPointID"/>.
        /// </summary>
        public string ID => id;

        /// <summary>
        /// Deterministic integer hash of <see cref="ID"/>, computed via
        /// <see cref="Core.Utilities.HashUtility.ComputeStableHash"/> at bake time.
        /// </summary>
        public int StableID => stableID;

        /// <summary>
        /// World-space position of the spawn point at bake time.
        /// </summary>
        public Vector2 Position => position;

        /// <summary>
        /// Whether the player should face right when spawning here.
        /// </summary>
        public bool FaceRight => faceRight;

        /// <summary>
        /// Creates a baked spawn point. Used by editor baking tooling.
        /// </summary>
        internal BakedSpawnPoint(string id, int stableID, Vector2 position, bool faceRight)
        {
            this.id = id;
            this.stableID = stableID;
            this.position = position;
            this.faceRight = faceRight;
        }
    }
}
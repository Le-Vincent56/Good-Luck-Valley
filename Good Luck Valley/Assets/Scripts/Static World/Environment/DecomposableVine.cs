using UnityEngine;

namespace HiveMind.Environment
{
    public class DecomposableVine : AnguishVine
    {
        #region FIELDS
        [SerializeField] private float timeToDecompose = 1f;
        #endregion

        #region PROPERTIES
        public float DecomposeTime { get { return timeToDecompose; } set { timeToDecompose = value; } }
        #endregion
    }
}

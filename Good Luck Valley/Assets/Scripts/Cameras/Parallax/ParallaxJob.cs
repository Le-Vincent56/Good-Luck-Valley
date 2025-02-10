using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace GoodLuckValley.Cameras.Parallax
{
    [BurstCompile]
    [ExecuteInEditMode]
    public struct ParallaxJob : IJobParallelFor
    {
        public NativeArray<ParallaxData> ParallaxData;
        public float3 CameraStartPos;
        public float3 CameraCurrentPos;

        /// <summary>
        /// Execute Parallax movement
        /// </summary>
        public void Execute(int index)
        {
            // Retrieve the data
            ParallaxData data = ParallaxData[index];

            // Set the start position
            float3 position = data.StartPosition;

            // Check if only moving horizontally
            if (data.HorizontalOnly)
                // Only change the x-position
                position.x += data.Multiplier * (CameraCurrentPos.x - CameraStartPos.x);
            else
                // Otherwise, change all dimensions
                position += data.Multiplier * (CameraCurrentPos - CameraStartPos);

            // Set the current position
            data.CurrentPosition = position;

            // Set the data
            ParallaxData[index] = data;
        }
    }
}

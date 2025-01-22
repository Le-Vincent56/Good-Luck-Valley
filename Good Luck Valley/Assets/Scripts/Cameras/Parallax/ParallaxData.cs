using Unity.Burst;
using Unity.Mathematics;

namespace GoodLuckValley.Cameras.Parallax
{
    [BurstCompile]
    public struct ParallaxData
    {
        public float3 CurrentPosition;
        public float3 StartPosition;
        public float Multiplier;
        public bool HorizontalOnly;
    }
}

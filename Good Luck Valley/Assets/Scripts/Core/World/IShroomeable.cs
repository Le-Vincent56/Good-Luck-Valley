using HiveMind.Events;

namespace HiveMind.Core
{
    public interface IShroomeable
    {
        /// <summary>
        /// Get the type of shroom that will spawn on the tile
        /// </summary>
        /// <returns>The type of shroom that will spawn on the tile</returns>
        ShroomType GetType();
    }
}
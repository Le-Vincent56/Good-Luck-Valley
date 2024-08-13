using GoodLuckValley.Entities.Fireflies;
using GoodLuckValley.Events;
using UnityEngine;

namespace GoodLuckValley.World.Interactables
{
    public class FinalLantern : Lantern
    {
        [Header("Events")]
        [SerializeField] private GameEvent onFinalLanternLit;

        /// <summary>
        /// Accept the fireflies
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="visitor"></param>
        public override void Accept<T>(T visitor)
        {
            // Verify the correct visitor
            if (visitor is FireflyController controller)
            {
                // Set the fireflies
                fireflies = controller.gameObject.transform;

                // Raise the world trigger event based on the channel
                onFinalLanternLit.Raise(this, channel);
            }
        }
    }
}
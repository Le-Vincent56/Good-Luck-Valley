using UnityEngine;

namespace GoodLuckValley.Core.Input.Interfaces
{
    public interface IPlayerInput
    {
        /// <summary>
        /// Gets the directional input for player movement as a two-dimensional vector.
        /// This property represents the axis values typically used for horizontal and
        /// vertical player navigation within the game.
        /// </summary>
        Vector2 Move { get; }

        /// <summary>
        /// Gets a value indicating whether the jump action has been triggered.
        /// This property is used to determine if the player has pressed the input associated with the jump mechanic,
        /// enabling in-game functionality related to jumping to occur.
        /// </summary>
        bool JumpPressed { get; }

        /// <summary>
        /// Indicates whether the jump action is being continuously held down by the player.
        /// This property is typically used to track sustained input for jump mechanics.
        /// </summary>
        bool JumpHeld { get; }

        /// <summary>
        /// Gets a value indicating whether the bounce action has been triggered.
        /// This property is used to check if the player has pressed the input associated with the bounce mechanic,
        /// allowing specific in-game behaviors tied to bouncing to be executed.
        /// </summary>
        bool BouncePressed { get; }

        /// <summary>
        /// Gets a value indicating whether the interact action has been triggered.
        /// This property is used to determine if the player has pressed the input associated with interaction,
        /// such as interacting with an object or entity.
        /// </summary>
        bool InteractPressed { get; }

        /// <summary>
        /// Gets a value indicating whether the crouch action is currently being held down.
        /// This property is typically used to determine if the player is actively maintaining a crouching state.
        /// </summary>
        bool CrouchHeld { get; }

        /// <summary>
        /// Gets a value indicating whether the "Previous" action has been triggered.
        /// This property is typically used to detect input for cycling to the previous item or action
        /// in a sequential interface, such as moving to the previous element in a menu or inventory.
        /// </summary>
        bool PreviousPressed { get; }

        /// <summary>
        /// Gets a value indicating whether the "Next" action has been triggered.
        /// This property is typically used to detect input for cycling to the next item or action
        /// in a sequential interface, such as moving to the next element in a menu or inventory.
        /// </summary>
        bool NextPressed { get; }
    }
}
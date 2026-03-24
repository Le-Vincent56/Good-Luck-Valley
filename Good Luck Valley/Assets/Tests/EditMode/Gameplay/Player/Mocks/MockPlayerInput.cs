using UnityEngine;
using GoodLuckValley.Core.Input.Interfaces;

namespace GoodLuckValley.Tests.EditMode.Gameplay.Player.Mocks
{
    public class MockPlayerInput : IPlayerInput
    {
        public Vector2 Move { get; set; }
        public bool JumpPressed { get; set; }
        public bool JumpHeld { get; set; }
        public bool BouncePressed { get; set; }
        public bool InteractPressed { get; set; }
        public bool CrouchHeld { get; set; }
        public bool PreviousPressed { get; set; }
        public bool NextPressed { get; set; }
    }
}
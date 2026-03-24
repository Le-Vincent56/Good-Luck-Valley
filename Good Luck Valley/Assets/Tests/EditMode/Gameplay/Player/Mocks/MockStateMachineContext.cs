using System;
using GoodLuckValley.Core.StateMachine.Interfaces;
using GoodLuckValley.Gameplay.Player.States;

namespace GoodLuckValley.Tests.EditMode.Gameplay.Player.Mocks
{
    public class MockStateMachineContext : IStateMachineContext<IPlayerState>
    {
        public Type LastTransitionTarget { get; private set; }
        public int TransitionCount { get; private set; }

        public void ChangeState<T>() where T : IPlayerState
        {
            LastTransitionTarget = typeof(T);
            TransitionCount++;
        }

        public void Reset()
        {
            LastTransitionTarget = null;
            TransitionCount = 0;
        }
    }
}
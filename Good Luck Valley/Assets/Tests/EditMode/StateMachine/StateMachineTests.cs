using System;
using NUnit.Framework;
using GoodLuckValley.Core.StateMachine.Exceptions;
using GoodLuckValley.Core.StateMachine.Interfaces;
using GoodLuckValley.Core.StateMachine.Services;

namespace GoodLuckValley.Tests.EditMode.StateMachine
{
    [TestFixture]
    public class StateMachineTests
    {
        private StateMachine<ITestState> _fsm;
        private StateA _stateA;
        private StateB _stateB;

        [SetUp]
        public void SetUp()
        {
            _fsm = new StateMachine<ITestState>();
            _stateA = new StateA(_fsm);
            _stateB = new StateB(_fsm);
        }

        // --- Start Lifecycle ---

        [Test]
        public void Start_SetsCurrentState()
        {
            _fsm.AddState(_stateA);
            _fsm.AddState(_stateB);

            _fsm.Start<StateA>();

            Assert.AreSame(_stateA, _fsm.CurrentState);
        }

        [Test]
        public void Start_CallsEnterOnInitialState()
        {
            _fsm.AddState(_stateA);

            _fsm.Start<StateA>();

            Assert.AreEqual(1, _stateA.EnterCount);
        }

        [Test]
        public void Start_DoesNotCallExitOnInitialState()
        {
            _fsm.AddState(_stateA);

            _fsm.Start<StateA>();

            Assert.AreEqual(0, _stateA.ExitCount);
        }

        [Test]
        public void Start_ThrowsIfTypeNotRegistered()
        {
            Assert.Throws<StateMachineException>(() => _fsm.Start<StateA>());
        }

        [Test]
        public void Start_ThrowsIfCalledTwice()
        {
            _fsm.AddState(_stateA);
            _fsm.Start<StateA>();

            Assert.Throws<StateMachineException>(() => _fsm.Start<StateA>());
        }

        [Test]
        public void CurrentState_IsNullBeforeStart()
        {
            _fsm.AddState(_stateA);

            Assert.IsNull(_fsm.CurrentState);
        }

        // --- Update / FixedUpdate Forwarding ---                                                   

        [Test]
        public void Update_ForwardsToCurrentState()
        {
            _fsm.AddState(_stateA);
            _fsm.Start<StateA>();

            _fsm.Update(0.16f);

            Assert.AreEqual(1, _stateA.UpdateCount);
            Assert.AreEqual(0.16f, _stateA.LastDeltaTime, 0.0001f);
        }

        [Test]
        public void FixedUpdate_ForwardsToCurrentState()
        {
            _fsm.AddState(_stateA);
            _fsm.Start<StateA>();

            _fsm.FixedUpdate(0.02f);

            Assert.AreEqual(1, _stateA.FixedUpdateCount);
            Assert.AreEqual(0.02f, _stateA.LastFixedDeltaTime, 0.0001f);
        }

        [Test]
        public void Update_BeforeStart_IsNoOp()
        {
            _fsm.AddState(_stateA);

            Assert.DoesNotThrow(() => _fsm.Update(0.16f));
            Assert.AreEqual(0, _stateA.UpdateCount);
        }

        [Test]
        public void FixedUpdate_BeforeStart_IsNoOp()
        {
            _fsm.AddState(_stateA);

            Assert.DoesNotThrow(() => _fsm.FixedUpdate(0.02f));
            Assert.AreEqual(0, _stateA.FixedUpdateCount);
        }

        // --- ChangeState Transitions ---                

        [Test]
        public void ChangeState_TransitionsCorrectly()
        {
            _fsm.AddState(_stateA);
            _fsm.AddState(_stateB);
            _stateA.SetOnUpdateAction(() => _fsm.ChangeState<StateB>());
            _fsm.Start<StateA>();

            _fsm.Update(0.16f);

            Assert.AreEqual(1, _stateA.ExitCount);
            Assert.AreEqual(1, _stateB.EnterCount);
            Assert.AreSame(_stateB, _fsm.CurrentState);
        }

        [Test]
        public void ChangeState_FiresOnStateChangedEvent()
        {
            _fsm.AddState(_stateA);
            _fsm.AddState(_stateB);
            _stateA.SetOnUpdateAction(() => _fsm.ChangeState<StateB>());

            StateChangeTracker tracker = new StateChangeTracker();
            _fsm.OnStateChanged += tracker.OnStateChanged;
            _fsm.Start<StateA>();

            _fsm.Update(0.16f);

            Assert.AreEqual(1, tracker.Changes.Count);
            Assert.AreSame(_stateA, tracker.Changes[0].Previous);
            Assert.AreSame(_stateB, tracker.Changes[0].Current);
        }

        [Test]
        public void ChangeState_EventOrder_ExitBeforeEnterBeforeEvent()
        {
            _fsm.AddState(_stateA);
            _fsm.AddState(_stateB);

            _stateA.SetOnUpdateAction(() => _fsm.ChangeState<StateB>());

            // Capture state call counts at the moment the event fires
            int aExitCountAtEvent = 0;
            int bEnterCountAtEvent = 0;
            _fsm.OnStateChanged += (previous, current) =>
            {
                aExitCountAtEvent = _stateA.ExitCount;
                bEnterCountAtEvent = _stateB.EnterCount;
            };

            _fsm.Start<StateA>();
            _fsm.Update(0.16f);

            // At event fire time, both Exit and Enter should have already happened
            Assert.AreEqual(1, aExitCountAtEvent, "A.Exit should have been called before event");
            Assert.AreEqual(1, bEnterCountAtEvent, "B.Enter should have been called before event");
        }

        [Test]
        public void ChangeState_ToSameState_IsNoOp()
        {
            _fsm.AddState(_stateA);
            _stateA.SetOnUpdateAction(() => _fsm.ChangeState<StateA>());

            StateChangeTracker tracker = new StateChangeTracker();
            _fsm.OnStateChanged += tracker.OnStateChanged;
            _fsm.Start<StateA>();

            _fsm.Update(0.16f);

            // No Exit, no additional Enter, no event
            Assert.AreEqual(0, _stateA.ExitCount);
            Assert.AreEqual(1, _stateA.EnterCount); // Only the initial Start Enter
            Assert.AreEqual(0, tracker.Changes.Count);
        }

        [Test]
        public void ChangeState_ThrowsIfNotRegistered()
        {
            _fsm.AddState(_stateA);
            _stateA.SetOnUpdateAction(() => _fsm.ChangeState<UnregisteredState>());
            _fsm.Start<StateA>();

            Assert.Throws<StateMachineException>(() => _fsm.Update(0.16f));
        }

        // --- Transition Chains and Safety ---

        [Test]
        public void ChangeState_TransitionChainWorks()
        {
            StateC stateC = new StateC(_fsm);
            _fsm.AddState(_stateA);
            _fsm.AddState(_stateB);
            _fsm.AddState(stateC);

            // A.Update → transition to B, B.Enter → transition to C
            _stateA.SetOnUpdateAction(() => _fsm.ChangeState<StateB>());
            _stateB.SetOnEnterAction(() => _fsm.ChangeState<StateC>());

            _fsm.Start<StateA>();
            _fsm.Update(0.16f);

            // Verify full chain executed
            Assert.AreEqual(1, _stateA.ExitCount, "A.Exit should be called once");
            Assert.AreEqual(1, _stateB.EnterCount, "B.Enter should be called once");
            Assert.AreEqual(1, _stateB.ExitCount, "B.Exit should be called once");
            Assert.AreEqual(1, stateC.EnterCount, "C.Enter should be called once");
            Assert.AreSame(stateC, _fsm.CurrentState, "CurrentState should be C");
        }

        [Test]
        public void ChangeState_TransitionChain_EventsFire()
        {
            StateC stateC = new StateC(_fsm);
            _fsm.AddState(_stateA);
            _fsm.AddState(_stateB);
            _fsm.AddState(stateC);

            _stateA.SetOnUpdateAction(() => _fsm.ChangeState<StateB>());
            _stateB.SetOnEnterAction(() => _fsm.ChangeState<StateC>());

            StateChangeTracker tracker = new StateChangeTracker();
            _fsm.OnStateChanged += tracker.OnStateChanged;

            _fsm.Start<StateA>();
            _fsm.Update(0.16f);

            // Events fire depth-first: B -> C event fires before A→B event.
            // The outer event's current parameter is _currentState at fire time,
            // which is C (set by the inner chain) — not B.
            Assert.AreEqual(2, tracker.Changes.Count);
            Assert.AreSame(_stateB, tracker.Changes[0].Previous, "First event: B→C (inner)");
            Assert.AreSame(stateC, tracker.Changes[0].Current, "First event current: C");
            Assert.AreSame(_stateA, tracker.Changes[1].Previous, "Second event: A→B (outer)");
            Assert.AreSame(stateC, tracker.Changes[1].Current, "Second event current: C (chain already completed)");
        }

        [Test]
        public void ChangeState_MaxTransitionsGuard_ThrowsOnInfiniteLoop()
        {
            _fsm.AddState(_stateA);
            _fsm.AddState(_stateB);

            // Create an infinite loop: A.Update transitions to B, B.Enter transitions to A,
            // A.Enter transitions to B, etc.
            _stateA.SetOnUpdateAction(() => _fsm.ChangeState<StateB>());
            _stateB.SetOnEnterAction(() => _fsm.ChangeState<StateA>());

            _fsm.Start<StateA>();

            // Set A's Enter action AFTER Start so the initial Enter doesn't trigger the loop
            _stateA.SetOnEnterAction(() => _fsm.ChangeState<StateB>());

            Assert.Throws<StateMachineException>(() => _fsm.Update(0.16f));
        }

        [Test]
        public void ChangeState_TransitionCounterResetsPerUpdate()
        {
            _fsm.AddState(_stateA);
            _fsm.AddState(_stateB);

            bool shouldTransition = true;
            _stateA.SetOnUpdateAction(() =>
            {
                if (shouldTransition)
                {
                    _fsm.ChangeState<StateB>();
                }
            });

            _stateB.SetOnUpdateAction(() =>
            {
                if (shouldTransition)
                {
                    _fsm.ChangeState<StateA>();
                }
            });

            _fsm.Start<StateA>();

            // First Update: A -> B (1 transition, under limit)
            shouldTransition = true;
            _fsm.Update(0.16f);

            // Now in B. Disable transitions for clean state.
            shouldTransition = false;

            // Second Update: should not throw — counter was reset
            shouldTransition = true;
            Assert.DoesNotThrow(() => _fsm.Update(0.16f));
        }

        // --- AddState Guards ---                        

        [Test]
        public void AddState_ThrowsOnDuplicateType()
        {
            _fsm.AddState(_stateA);
            StateA duplicateA = new StateA(_fsm);

            Assert.Throws<StateMachineException>(() => _fsm.AddState(duplicateA));
        }

        [Test]
        public void AddState_ThrowsAfterStart()
        {
            _fsm.AddState(_stateA);
            _fsm.Start<StateA>();

            Assert.Throws<StateMachineException>(() => _fsm.AddState(_stateB));
        }

        [Test]
        public void AddState_ThrowsOnNull()
        {
            Assert.Throws<StateMachineException>(() => _fsm.AddState<StateA>(null));
        }
    }
}
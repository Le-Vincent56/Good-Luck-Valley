using NUnit.Framework;
using UnityEngine;
using GoodLuckValley.Core.Input.Enums;
using GoodLuckValley.Core.Input.Interfaces;
using GoodLuckValley.Core.Input.Services;

namespace GoodLuckValley.Tests.EditMode.Input
{
    [TestFixture]
    public class InputServiceTests
    {
        private InputService _service;
        private ManualClock _clock;
        private StubInputMapSwitcher _mapSwitcher;

        [SetUp]
        public void SetUp()
        {
            _clock = new ManualClock();
            _mapSwitcher = new StubInputMapSwitcher();
            _service = new InputService(_clock.TimeProvider);
        }

        // --- Continuous Values ---

        [Test]
        public void Move_DefaultsToZero()
        {
            IPlayerInput input = _service;

            Assert.AreEqual(Vector2.zero, input.Move);
        }

        [Test]
        public void Move_ReturnsSetValue()
        {
            IPlayerInput input = _service;
            Vector2 expected = new Vector2(0.5f, -0.3f);

            _service.SetMove(expected);

            Assert.AreEqual(expected, input.Move);
        }

        [Test]
        public void Navigate_ReturnsSetValue()
        {
            IUIInput input = _service;
            Vector2 expected = new Vector2(0f, 1f);

            _service.SetNavigate(expected);

            Assert.AreEqual(expected, input.Navigate);
        }

        [Test]
        public void CrouchHeld_DefaultsToFalse()
        {
            IPlayerInput input = _service;

            Assert.IsFalse(input.CrouchHeld);
        }

        [Test]
        public void CrouchHeld_TrueWhilePerformed()
        {
            IPlayerInput input = _service;

            _service.OnCrouchPerformed();

            Assert.IsTrue(input.CrouchHeld);
        }

        [Test]
        public void CrouchHeld_FalseAfterCanceled()
        {
            IPlayerInput input = _service;

            _service.OnCrouchPerformed();
            _service.OnCrouchCanceled();

            Assert.IsFalse(input.CrouchHeld);
        }

        // --- Buffered Press ---
        
        [Test]
        public void JumpPressed_FalseByDefault()
        {
            IPlayerInput input = _service;

            Assert.IsFalse(input.JumpPressed);
        }

        [Test]
        public void JumpPressed_TrueWithinBufferWindow()
        {
            IPlayerInput input = _service;

            _service.OnJumpPerformed();

            Assert.IsTrue(input.JumpPressed);
        }

        [Test]
        public void JumpPressed_ConsumedOnRead()
        {
            IPlayerInput input = _service;

            _service.OnJumpPerformed();
            bool firstRead = input.JumpPressed;
            bool secondRead = input.JumpPressed;

            Assert.IsTrue(firstRead);
            Assert.IsFalse(secondRead);
        }

        [Test]
        public void JumpPressed_FalseAfterBufferExpires()
        {
            IPlayerInput input = _service;

            _service.OnJumpPerformed();
            _clock.Advance(0.2f); // Past the 0.15s buffer window

            Assert.IsFalse(input.JumpPressed);
        }

        [Test]
        public void JumpPressed_TrueAtEdgeOfBufferWindow()
        {
            IPlayerInput input = _service;

            _service.OnJumpPerformed();
            _clock.Advance(0.14f); // Just inside the 0.15s window

            Assert.IsTrue(input.JumpPressed);
        }

        [Test]
        public void JumpPressed_NewPressResetsAfterConsume()
        {
            IPlayerInput input = _service;

            _service.OnJumpPerformed();
            bool consumed = input.JumpPressed; // Consume first press
            _clock.Advance(0.05f);
            _service.OnJumpPerformed(); // New press

            Assert.IsTrue(input.JumpPressed);
        }

        [Test]
        public void BouncePressed_FollowsBufferSemantics()
        {
            IPlayerInput input = _service;

            _service.OnBouncePerformed();

            Assert.IsTrue(input.BouncePressed);
            Assert.IsFalse(input.BouncePressed); // Consumed
        }

        [Test]
        public void InteractPressed_FollowsBufferSemantics()
        {
            IPlayerInput input = _service;

            _service.OnInteractPerformed();

            Assert.IsTrue(input.InteractPressed);
            Assert.IsFalse(input.InteractPressed); // Consumed
        }

        [Test]
        public void PreviousPressed_FollowsBufferSemantics()
        {
            IPlayerInput input = _service;

            _service.OnPreviousPerformed();

            Assert.IsTrue(input.PreviousPressed);
            Assert.IsFalse(input.PreviousPressed); // Consumed
        }

        [Test]
        public void NextPressed_FollowsBufferSemantics()
        {
            IPlayerInput input = _service;

            _service.OnNextPerformed();

            Assert.IsTrue(input.NextPressed);
            Assert.IsFalse(input.NextPressed); // Consumed
        }

        // --- UI Buffered Press ---

        [Test]
        public void SubmitPressed_FollowsBufferSemantics()
        {
            IUIInput input = _service;

            _service.OnSubmitPerformed();

            Assert.IsTrue(input.SubmitPressed);
            Assert.IsFalse(input.SubmitPressed); // Consumed
        }

        [Test]
        public void CancelPressed_FollowsBufferSemantics()
        {
            IUIInput input = _service;

            _service.OnCancelPerformed();

            Assert.IsTrue(input.CancelPressed);
            Assert.IsFalse(input.CancelPressed); // Consumed
        }

        // --- Context Switching ---                                                             

        [Test]
        public void CurrentContext_DefaultsToPlayer()
        {
            IInputContextService contextService = _service;

            Assert.AreEqual(InputContext.Player, contextService.CurrentContext);
        }

        [Test]
        public void SetContext_ChangesCurrentContext()
        {
            IInputContextService contextService = _service;

            contextService.SetContext(InputContext.UI);

            Assert.AreEqual(InputContext.UI, contextService.CurrentContext);
        }

        [Test]
        public void SetContext_FiresOnContextChanged()
        {
            IInputContextService contextService = _service;
            InputContext receivedContext = InputContext.Player;
            contextService.OnContextChanged += (ctx) => { receivedContext = ctx; };

            contextService.SetContext(InputContext.UI);

            Assert.AreEqual(InputContext.UI, receivedContext);
        }

        [Test]
        public void SetContext_SameContext_DoesNotFireEvent()
        {
            IInputContextService contextService = _service;
            int fireCount = 0;
            contextService.OnContextChanged += (ctx) => { fireCount++; };

            contextService.SetContext(InputContext.Player); // Already Player

            Assert.AreEqual(0, fireCount);
        }

        [Test]
        public void SetContext_ClearsContinuousValues()
        {
            IPlayerInput input = _service;
            _service.SetMove(new Vector2(1f, 0f));

            _service.SetContext(InputContext.UI);

            Assert.AreEqual(Vector2.zero, input.Move);
        }

        [Test]
        public void SetContext_ClearsBufferedPresses()
        {
            IPlayerInput input = _service;
            _service.OnJumpPerformed();
            _service.OnBouncePerformed();

            _service.SetContext(InputContext.UI);

            Assert.IsFalse(input.JumpPressed);
            Assert.IsFalse(input.BouncePressed);
        }

        [Test]
        public void SetContext_ClearsHeldState()
        {
            IPlayerInput input = _service;
            _service.OnCrouchPerformed();

            _service.SetContext(InputContext.UI);

            Assert.IsFalse(input.CrouchHeld);
        }

        [Test]
        public void SetContext_ClearsUIValues()
        {
            IUIInput input = _service;

            // Switch to UI first so we can set UI values in the "active" context
            _service.SetContext(InputContext.UI);
            _service.SetNavigate(new Vector2(1f, 0f));
            _service.OnSubmitPerformed();

            _service.SetContext(InputContext.Player);

            Assert.AreEqual(Vector2.zero, input.Navigate);
            Assert.IsFalse(input.SubmitPressed);
        }

        [Test]
        public void SetContext_ToUI_CallsEnableUIMapOnSwitcher()
        {
            _service.SetMapSwitcher(_mapSwitcher);

            _service.SetContext(InputContext.UI);

            Assert.AreEqual("EnableUIMap", _mapSwitcher.LastCall);
        }

        [Test]
        public void SetContext_ToPlayer_CallsEnablePlayerMapOnSwitcher()
        {
            _service.SetMapSwitcher(_mapSwitcher);
            _service.SetContext(InputContext.UI); // Switch away first
            _mapSwitcher.Reset();

            _service.SetContext(InputContext.Player);

            Assert.AreEqual("EnablePlayerMap", _mapSwitcher.LastCall);
        }

        [Test]
        public void SetContext_NoSwitcherWired_DoesNotThrow()
        {
            // No SetMapSwitcher called — should not throw, just skip adapter notification
            Assert.DoesNotThrow(() => _service.SetContext(InputContext.UI));
        }
    }
}
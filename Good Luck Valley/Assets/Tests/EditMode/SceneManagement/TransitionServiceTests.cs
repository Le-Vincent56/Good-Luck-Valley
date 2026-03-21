using NUnit.Framework;
using GoodLuckValley.Core.SceneManagement.Enums;
using GoodLuckValley.Core.SceneManagement.Exceptions;
using GoodLuckValley.Core.SceneManagement.Services;

namespace GoodLuckValley.Tests.EditMode.SceneManagement
{
    [TestFixture]
    public class TransitionServiceTests
    {
        private TransitionService _service;
        private MockTransitionEffect _mockEffect;

        [SetUp]
        public void SetUp()
        {
            _service = new TransitionService();
            _mockEffect = new MockTransitionEffect();
        }
        
        // --- Initial State ---

        [Test]
        public void InitialState_IsIdle()
        {
            Assert.AreEqual(TransitionState.Idle, _service.State);
        }

        [Test]
        public void IsTransitioning_WhenIdle_ReturnsFalse()
        {
            Assert.IsFalse(_service.IsTransitioning);
        }
        
        // --- Cover Phase ---

        [Test]
        public void TryBeginCover_WhenIdle_TransitionsToCovering()
        {
            bool result = _service.TryBeginCover(_mockEffect);
            
            Assert.IsTrue(result);
            Assert.AreEqual(TransitionState.Covering, _service.State);
        }

        [Test]
        public void TryBeginCover_WhenIdle_IsTransitioningReturnsTrue()
        {
            _service.TryBeginCover(_mockEffect);
            
            Assert.IsTrue(_service.IsTransitioning);
        }

        [Test]
        public void TryBeginCover_WhenIdle_FiresCoverStartedEvent()
        {
            bool eventFired = false;
            _service.OnTransitionCoverStarted += () => eventFired = true;
            _service.TryBeginCover(_mockEffect);

            Assert.IsTrue(eventFired);
        }

        [Test]
        public void TryBeginCover_WhenAlreadyTransitioning_ReturnsFalse()
        {
            _service.TryBeginCover(_mockEffect);

            bool result = _service.TryBeginCover(_mockEffect);
            
            Assert.IsFalse(result);
            Assert.AreEqual(TransitionState.Covering, _service.State);
        }

        [Test]
        public void TryBeginCover_WithNullEffect_ThrowsSceneManagementException()
        {
            Assert.Throws<SceneManagementException>(() => _service.TryBeginCover(null));
        }
        
        // --- Covered Phase ---

        [Test]
        public void CompleteCover_TransitionsToCovered()
        {
            _service.TryBeginCover(_mockEffect);
            _service.CompleteCover();
            
            Assert.AreEqual(TransitionState.Covered, _service.State);
        }

        [Test]
        public void CompleteCover_FiresCoveredEvent()
        {
            bool eventFired = false;
            _service.OnTransitionCovered += () => eventFired = true;
            _service.TryBeginCover(_mockEffect);
            _service.CompleteCover();
            
            Assert.IsTrue(eventFired);
        }
        
        // --- Reveal Phase ---
        
        [Test]
        public void TryBeginReveal_WhenCovered_TransitionsToRevealing()
        {
            _service.TryBeginCover(_mockEffect);
            _service.CompleteCover();

            bool result = _service.TryBeginReveal();
            
            Assert.IsTrue(result);
            Assert.AreEqual(TransitionState.Revealing, _service.State);
        }

        [Test]
        public void TryBeginReveal_WhenNotCovered_ReturnsFalse()
        {
            bool result = _service.TryBeginReveal();
            
            Assert.IsFalse(result);
            Assert.AreEqual(TransitionState.Idle, _service.State);
        }

        [Test]
        public void TryBeginReveal_WhenCovering_ReturnsFalse()
        {
            _service.TryBeginCover(_mockEffect);
            
            bool result = _service.TryBeginReveal();
            
            Assert.IsFalse(result);
        }
        
        // --- Complete Reveal ---

        [Test]
        public void CompleteReveal_TransitionsToIdle()
        {
            _service.TryBeginCover(_mockEffect);
            _service.CompleteCover();
            _service.TryBeginReveal();
            _service.CompleteReveal();

            Assert.AreEqual(TransitionState.Idle, _service.State);
        }
        
        [Test]
        public void CompleteReveal_FiresCompletedEvent()
        {
            bool eventFired = false;
            _service.OnTransitionCompleted += () => eventFired = true;
            _service.TryBeginCover(_mockEffect);
            _service.CompleteCover();
            _service.TryBeginReveal();

            _service.CompleteReveal();

            Assert.IsTrue(eventFired);
        }

        [Test]
        public void CompleteReveal_IsTransitioningReturnsFalse()
        {
            _service.TryBeginCover(_mockEffect);
            _service.CompleteCover();
            _service.TryBeginReveal();

            _service.CompleteReveal();

            Assert.IsFalse(_service.IsTransitioning);
        }

        // --- Full Cycle ---

        [Test]
        public void FullTransitionCycle_ReturnsToIdle()
        {
            _service.TryBeginCover(_mockEffect);
            Assert.IsTrue(_service.IsTransitioning);

            _service.CompleteCover();
            Assert.IsTrue(_service.IsTransitioning);

            _service.TryBeginReveal();
            Assert.IsTrue(_service.IsTransitioning);

            _service.CompleteReveal();
            Assert.IsFalse(_service.IsTransitioning);
            Assert.AreEqual(TransitionState.Idle, _service.State);
        }
    }
}
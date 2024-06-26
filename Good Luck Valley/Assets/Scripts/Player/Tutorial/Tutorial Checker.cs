using GoodLuckValley.Patterns.Blackboard;
using GoodLuckValley.Patterns.ServiceLocator;
using GoodLuckValley.Player.Input;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace GoodLuckValley.Player.Tutorial
{
    public class TutorialChecker : MonoBehaviour
    {
        [SerializeField] private InputReader input;

        Blackboard unlockBlackboard;
        BlackboardKey unlockedSpiritPower;

        Blackboard tutorialBlackboard;
        BlackboardKey seenMoveTrigger;
        BlackboardKey hasMoved;
        BlackboardKey seenJumpTrigger;
        BlackboardKey hasJumped;
        BlackboardKey seenSlideTrigger;
        BlackboardKey hasSlid;
        BlackboardKey seenFastFallTrigger;
        BlackboardKey hasFastFallen;
        BlackboardKey seenCrawlTrigger;
        BlackboardKey hasCrawled;
        BlackboardKey seenInteractTrigger;
        BlackboardKey hasInteracted;
        BlackboardKey seenAimTrigger;
        BlackboardKey hasAimed;
        BlackboardKey seenThrowTrigger;
        BlackboardKey hasThrown;
        BlackboardKey seenPeekTrigger;
        BlackboardKey hasPeeked;
        BlackboardKey seenTotalRecallTrigger;
        BlackboardKey hasTotalRecall;
        BlackboardKey seenSingleRecallTrigger;
        BlackboardKey hasSingleRecall;
        BlackboardKey seenQuickBounceTrigger;
        BlackboardKey hasQuickBounced;
        BlackboardKey seenChainBounceTrigger;
        BlackboardKey hasChainBounced;

        private void OnEnable()
        {
            input.Move += TeachMovement;
            input.Jump += TeachJump;
            input.FastSlide += TeachSlide;
            input.FastFall += TeachFastFall;
            input.Crawl += TeachCrawl;
            input.Throw += TeachThrow;
            input.RecallAll += TeachTotalRecall;
            input.RecallLast += TeachSingleRecall;
            input.QuickBounce += TeachQuickBounce;
        }

        private void OnDisable()
        {
            input.Move -= TeachMovement;
            input.Jump -= TeachJump;
            input.FastSlide -= TeachSlide;
            input.FastFall -= TeachFastFall;
            input.Crawl -= TeachCrawl;
            input.Throw -= TeachThrow;
            input.RecallAll -= TeachTotalRecall;
            input.RecallLast -= TeachSingleRecall;
            input.QuickBounce -= TeachQuickBounce;
        }

        // Start is called before the first frame update
        void Start()
        {
            // Get blackboards
            tutorialBlackboard = ServiceLocator.For(this).Get<BlackboardController>().GetBlackboard("Tutorial");
            unlockBlackboard = ServiceLocator.For(this).Get<BlackboardController>().GetBlackboard("Unlocks");

            // Register keys
            unlockedSpiritPower = unlockBlackboard.GetOrRegisterKey("UnlockedSpiritPower");
            seenMoveTrigger = tutorialBlackboard.GetOrRegisterKey("SeenMoveTrigger");
            hasMoved = tutorialBlackboard.GetOrRegisterKey("HasMoved");
            seenJumpTrigger = tutorialBlackboard.GetOrRegisterKey("SeenJumpTrigger");
            hasJumped = tutorialBlackboard.GetOrRegisterKey("HasJumped");
            seenSlideTrigger = tutorialBlackboard.GetOrRegisterKey("SeenSlideTrigger");
            hasSlid = tutorialBlackboard.GetOrRegisterKey("HasSlid");
            seenFastFallTrigger = tutorialBlackboard.GetOrRegisterKey("SeenFastFallTrigger");
            hasFastFallen = tutorialBlackboard.GetOrRegisterKey("HasFastFallen");
            seenCrawlTrigger = tutorialBlackboard.GetOrRegisterKey("SeenCrawlTrigger");
            hasCrawled = tutorialBlackboard.GetOrRegisterKey("HasCrawled");
            seenInteractTrigger = tutorialBlackboard.GetOrRegisterKey("SeenInteractTrigger");
            hasInteracted = tutorialBlackboard.GetOrRegisterKey("HasInteracted");
            seenAimTrigger = tutorialBlackboard.GetOrRegisterKey("SeenAimTrigger");
            hasAimed = tutorialBlackboard.GetOrRegisterKey("HasAimed");
            seenThrowTrigger = tutorialBlackboard.GetOrRegisterKey("SeenThrowTrigger");
            hasThrown = tutorialBlackboard.GetOrRegisterKey("HasThrown");
            seenPeekTrigger = tutorialBlackboard.GetOrRegisterKey("SeenPeekTrigger");
            hasPeeked = tutorialBlackboard.GetOrRegisterKey("HasPeeked");
            seenTotalRecallTrigger = tutorialBlackboard.GetOrRegisterKey("SeenTotalRecallTrigger");
            hasTotalRecall = tutorialBlackboard.GetOrRegisterKey("HasTotalRecall");
            seenSingleRecallTrigger = tutorialBlackboard.GetOrRegisterKey("SeenSingleRecallTrigger");
            hasSingleRecall = tutorialBlackboard.GetOrRegisterKey("HasSingleRecall");
            seenQuickBounceTrigger = tutorialBlackboard.GetOrRegisterKey("SeenQuickBounceTrigger");
            hasQuickBounced = tutorialBlackboard.GetOrRegisterKey("HasQuickBounced");
            seenChainBounceTrigger = tutorialBlackboard.GetOrRegisterKey("SeenChainBounceTrigger");
            hasChainBounced = tutorialBlackboard.GetOrRegisterKey("HasChainBounced");

            // Set default values, for now
            SetDefaultBlackboardValues(false);
        }

        private void SetDefaultBlackboardValues(bool defaultValue)
        {
            ChangeBlackboardValue(seenMoveTrigger, defaultValue);
            ChangeBlackboardValue(hasMoved, defaultValue);
            ChangeBlackboardValue(seenJumpTrigger, defaultValue);
            ChangeBlackboardValue(hasJumped, defaultValue);
            ChangeBlackboardValue(seenSlideTrigger, defaultValue);
            ChangeBlackboardValue(hasSlid, defaultValue);
            ChangeBlackboardValue(seenFastFallTrigger, defaultValue);
            ChangeBlackboardValue(hasFastFallen, defaultValue);
            ChangeBlackboardValue(seenCrawlTrigger, defaultValue);
            ChangeBlackboardValue(hasCrawled, defaultValue);
            ChangeBlackboardValue(seenInteractTrigger, defaultValue);
            ChangeBlackboardValue(hasInteracted, defaultValue);
            ChangeBlackboardValue(seenAimTrigger, defaultValue);
            ChangeBlackboardValue(hasAimed, defaultValue);
            ChangeBlackboardValue(seenThrowTrigger, defaultValue);
            ChangeBlackboardValue(hasThrown, defaultValue);
            ChangeBlackboardValue(seenPeekTrigger, defaultValue);
            ChangeBlackboardValue(hasPeeked, defaultValue);
            ChangeBlackboardValue(seenTotalRecallTrigger, defaultValue);
            ChangeBlackboardValue(hasTotalRecall, defaultValue);
            ChangeBlackboardValue(seenSingleRecallTrigger, defaultValue);
            ChangeBlackboardValue(hasSingleRecall, defaultValue);
            ChangeBlackboardValue(seenQuickBounceTrigger, defaultValue);
            ChangeBlackboardValue(hasQuickBounced, defaultValue);
            ChangeBlackboardValue(seenChainBounceTrigger, defaultValue);
            ChangeBlackboardValue(hasChainBounced, defaultValue);
        }

        public void TeachMovement(Vector2 moveVec) => TeachDefaultControl(hasMoved);
        public void TeachJump(bool started) => TeachDefaultControl(hasJumped);
        public void TeachSlide(bool started) => TeachDefaultControl(hasSlid);
        public void TeachFastFall(bool started) => TeachDefaultControl(hasFastFallen);
        public void TeachCrawl(bool started) => TeachDefaultControl(hasCrawled);
        public void TeachInteract(Component sender, object data) => TeachDefaultControl(hasInteracted);
        public void TeachThrow(bool started, bool canceled)
        {
            if(started)
            {
                TeachMushroomControl(hasAimed);
            }

            if(canceled)
            {
                TeachMushroomControl(hasThrown);
            }
        }
        public void TeachPeek(Component sender, object data) => TeachDefaultControl(hasPeeked);
        public void TeachTotalRecall(bool started) => TeachMushroomControl(hasTotalRecall);
        public void TeachSingleRecall(bool started) => TeachMushroomControl(hasSingleRecall);
        public void TeachQuickBounce(bool started) => TeachMushroomControl(hasQuickBounced);


        private void TeachDefaultControl(BlackboardKey key)
        {
            // Try to get the blackboard value
            if(tutorialBlackboard.TryGetValue(key, out bool blackboardValue))
            {
                // If the control has been taught, return
                if (blackboardValue) return;

                // Teach the control
                tutorialBlackboard.SetValue(key, true);
            }
        }

        private void TeachMushroomControl(BlackboardKey key)
        {
            // Try to get the unlock blackboard value
            if(unlockBlackboard.TryGetValue(unlockedSpiritPower, out bool unlockValue))
            {
                // If the spirit power has not been unlocked, return
                if (!unlockValue) return;
            }

            if(tutorialBlackboard.TryGetValue(key, out bool controlValue))
            {
                // If the control has been taught, return
                if (controlValue) return;

                // Teach the control
                tutorialBlackboard.SetValue(key, true);
            }
        }

        private void ChangeBlackboardValue(BlackboardKey key, bool value)
        {
            if (tutorialBlackboard.TryGetValue(key, out bool blackboardValue))
                tutorialBlackboard.SetValue(key, value);
        }
    }
}
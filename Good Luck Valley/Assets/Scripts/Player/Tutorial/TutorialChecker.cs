using GoodLuckValley.Patterns.Blackboard;
using GoodLuckValley.Patterns.ServiceLocator;
using GoodLuckValley.Player.Input;
using GoodLuckValley.UI.Tutorial;
using UnityEngine;

namespace GoodLuckValley.Player.Tutorial
{
    public class TutorialChecker : MonoBehaviour
    {
        private TutorialUIHandler UIHandler;

        [SerializeField] private bool bypassTutorials;
        [SerializeField] private InputReader input;

        Blackboard playerBlackboard;
        BlackboardKey unlockedThrow;

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

        // Start is called before the first frame update
        void Start()
        {
            // Get references
            UIHandler = GetComponent<TutorialUIHandler>();

            // Get blackboards
            tutorialBlackboard = ServiceLocator.For(this).Get<BlackboardController>().GetBlackboard("Tutorial");
            playerBlackboard = ServiceLocator.For(this).Get<BlackboardController>().GetBlackboard("Player");

            // Register keys
            unlockedThrow = playerBlackboard.GetOrRegisterKey("UnlockedThrow");
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

        private void OnEnable()
        {
            input.Throw += TeachThrow;
            input.RecallAll += TeachTotalRecall;
            input.RecallLast += TeachSingleRecall;
            input.QuickBounce += TeachQuickBounce;
        }

        private void OnDisable()
        {
            input.Throw -= TeachThrow;
            input.RecallAll -= TeachTotalRecall;
            input.RecallLast -= TeachSingleRecall;
            input.QuickBounce -= TeachQuickBounce;
        }

        /// <summary>
        /// Set all Tutorial Blackboard values to a default value
        /// </summary>
        /// <param name="defaultValue">The value to set</param>
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

        /// <summary>
        /// Learn a movement control based on a string
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void LearnControl(Component sender, object data)
        {
            // Verify that the correct data was sent
            if (data is not string) return;

            // Cast the data
            string controlName = (string)data;

            // Process further using the string name
            switch(controlName)
            {
                case "Move":
                    TeachDefaultControl(seenMoveTrigger, hasMoved, controlName);
                    break;

                case "Jump":
                    TeachDefaultControl(seenJumpTrigger, hasJumped, controlName);
                    break;

                case "Slide":
                    TeachDefaultControl(seenSlideTrigger, hasSlid, controlName);
                    break;

                case "Fast Fall":
                    TeachDefaultControl(seenFastFallTrigger, hasFastFallen, controlName);
                    break;

                case "Crawl":
                    TeachDefaultControl(seenCrawlTrigger, hasCrawled, controlName);
                    break;

                default:
                    return;
            }
        }

        public void TeachInteract(Component sender, object data) => TeachDefaultControl(seenInteractTrigger, hasInteracted, "Interact");
        public void TeachThrow(bool started, bool canceled)
        {
            if(started)
            {
                TeachMushroomControl(seenAimTrigger, hasAimed, "Aim");
            }

            if(canceled)
            {
                TeachMushroomControl(seenThrowTrigger, hasThrown, "Throw");
            }
        }
        public void TeachPeek(Component sender, object data) => TeachDefaultControl(seenPeekTrigger, hasPeeked, "Peek");
        public void TeachTotalRecall(bool started) => TeachMushroomControl(seenTotalRecallTrigger, hasTotalRecall, "Total Recall");
        public void TeachSingleRecall(bool started) => TeachMushroomControl(seenSingleRecallTrigger, hasSingleRecall, "Single Recall");
        public void TeachQuickBounce(bool started) => TeachMushroomControl(seenQuickBounceTrigger, hasQuickBounced, "Quick Bounce");
        public void TeachChainBounce(Component sender, object data) => TeachMushroomControl(seenChainBounceTrigger, hasChainBounced, "Chain Bounce");

        /// <summary>
        /// Teach a default control (not locked behind something)
        /// </summary>
        /// <param name="prereqKey">The prerequisite key</param>
        /// <param name="controlKey">The control key</param>
        private void TeachDefaultControl(BlackboardKey prereqKey, BlackboardKey controlKey, string name)
        {
            // Check if the trigger has been seen
            if (tutorialBlackboard.TryGetValue(prereqKey, out bool prereqValue))
            {
                // If the trigger hasn't been seen, return
                if (!prereqValue & !bypassTutorials) return;
            }
            else return; // If the trigger key doesn't exist, return

            // Try to get the blackboard value
            if(tutorialBlackboard.TryGetValue(controlKey, out bool blackboardValue))
            {
                // If the control has been taught, return
                if (blackboardValue) return;

                // Teach the control
                tutorialBlackboard.SetValue(controlKey, true);

                UIHandler.Hide(name);
            }
        }

        /// <summary>
        /// Teach a Mushroom control (need to unlock the Spirit Power)
        /// </summary>
        /// <param name="prereqKey">The prerequisite key</param>
        /// <param name="key">The control key</param>
        private void TeachMushroomControl(BlackboardKey prereqKey, BlackboardKey key, string name)
        {
            // Check if the trigger has been seen
            if (tutorialBlackboard.TryGetValue(prereqKey, out bool prereqValue))
            {
                // If the trigger hasn't been seen, return
                if (!prereqValue && !bypassTutorials) return;
            }
            else return; // If the trigger key doesn't exist, return

            // Try to get the unlock blackboard value
            if (playerBlackboard.TryGetValue(unlockedThrow, out bool unlockValue))
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

                // Show the UI
                UIHandler.Hide(name);

                // After aiming, show the throw tutorial
                if (key == hasAimed)
                    UIHandler.Show(this, "Throw");
            }
        }

        /// <summary>
        /// Change a Blackboard value
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        private void ChangeBlackboardValue(BlackboardKey key, bool value)
        {
            if (tutorialBlackboard.TryGetValue(key, out bool blackboardValue))
                tutorialBlackboard.SetValue(key, value);
        }
    }
}
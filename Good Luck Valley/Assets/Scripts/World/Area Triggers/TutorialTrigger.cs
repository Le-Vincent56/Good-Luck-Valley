using GoodLuckValley.World.AreaTriggers;
using GoodLuckValley.Events;
using GoodLuckValley.Patterns.Blackboard;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AreaCollider))]
public class TutorialTrigger : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private GameEvent onShowTutorialUI;

    [Header("Fields")]
    [SerializeField] private string tutorialKeyName;
    [SerializeField] private string tutorialActionName;
    Dictionary<string, string> blackboardToName;
    protected Blackboard tutorialBlackboard;
    protected BlackboardKey seenTutorialKey;
    protected BlackboardKey performedTutorialKey;
    private AreaCollider areaCollider;

    private void Awake()
    {
        // Get components
        areaCollider = GetComponent<AreaCollider>();

        // Initialize dictionary
        blackboardToName = new Dictionary<string, string>()
        {
            { "SeenMoveTrigger", "Move"},
            { "SeenJumpTrigger", "Jump" },
            { "SeenSlideTrigger", "Slide" },
            { "SeenFastFallTrigger", "Fast Fall" },
            { "SeenCrawlTrigger", "Crawl" },
            { "SeenInteractTrigger", "Interact" },
            { "SeenAimTrigger", "Aim" },
            { "SeenThrowTrigger", "Throw" },
            { "SeenPeekTrigger", "Peek" },
            { "SeenTotalRecallTrigger", "Total Recall" },
            { "SeenSingleRecallTrigger", "Single Recall" },
            { "SeenQuickBounceTrigger", "Quick Bounce" },
            { "SeenChainBounceTrigger", "Chain Bounce" }
        };
    }

    private void OnEnable()
    {
        // Subscribe to events
        areaCollider.OnTriggerEnter += ShowTutorial;
    }

    private void OnDisable()
    {
        // Unsubscribe to events
        areaCollider.OnTriggerEnter -= ShowTutorial;
    }

    protected virtual void Start()
    {
        tutorialBlackboard = BlackboardController.Instance.GetBlackboard("Tutorial");

        // Get/Register key
        seenTutorialKey = tutorialBlackboard.GetOrRegisterKey(tutorialKeyName);
        performedTutorialKey = tutorialBlackboard.GetOrRegisterKey(tutorialActionName);
    }

    /// <summary>
    /// Show tutorial UI and update whether or not the player has seen the tutorial
    /// </summary>
    /// <param name="gameObject"></param>
    protected void ShowTutorial(GameObject gameObject)
    {
        if(tutorialBlackboard.TryGetValue(performedTutorialKey, out bool performedValue))
        {
            // Exit case - the value has already been performed
            if (performedValue) return;
        }

        if (tutorialBlackboard.TryGetValue(seenTutorialKey, out bool blackboardValue))
        {
            // Exit case - already seen the trigger
            if (blackboardValue) return;

            // Set the trigger to have been seen
            tutorialBlackboard.SetValue(seenTutorialKey, true);

            // Special case for Aim/Throw
            if(tutorialKeyName == "SeenThrowTrigger")
            {
                BlackboardKey hasAimedKey = tutorialBlackboard.GetOrRegisterKey("HasAimed");
                if (tutorialBlackboard.TryGetValue(hasAimedKey, out bool seenAimedValue))
                    if (!seenAimedValue) return;
            }

            // Show the tutorial UI
            // Calls to:
            //  - TutorialUIHandler.Show()
            onShowTutorialUI.Raise(this, blackboardToName[tutorialKeyName]);
        }
    }
}

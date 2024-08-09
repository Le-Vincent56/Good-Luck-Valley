using GoodLuckValley.Patterns.Blackboard;
using GoodLuckValley.Persistence;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TutorialSaveHandler : MonoBehaviour, IBind<TutorialData>
{
    [SerializeField] private TutorialData data;
    [SerializeField] private bool registered;
    private Blackboard tutorialBlackboard;
    private BlackboardKey seenMoveTrigger;
    private BlackboardKey seenJumpTrigger;
    private BlackboardKey seenSlideTrigger;
    private BlackboardKey seenFastFallTrigger;
    private BlackboardKey seenCrawlTrigger;
    private BlackboardKey seenInteractTrigger;
    private BlackboardKey seenAimTrigger;
    private BlackboardKey seenThrowTrigger;
    private BlackboardKey seenPeekTrigger;
    private BlackboardKey seenTotalRecallTrigger;
    private BlackboardKey seenSingleRecallTrigger;
    private BlackboardKey seenQuickBounceTrigger;
    private BlackboardKey seenChainBounceTrigger;

    private BlackboardKey hasMoved;
    private BlackboardKey hasJumped;
    private BlackboardKey hasSlid;
    private BlackboardKey hasFastFallen;
    private BlackboardKey hasCrawled;
    private BlackboardKey hasInteracted;
    private BlackboardKey hasAimed;
    private BlackboardKey hasThrown;
    private BlackboardKey hasPeeked;
    private BlackboardKey hasTotalRecall;
    private BlackboardKey hasSingleRecall;
    private BlackboardKey hasQuickBounced;
    private BlackboardKey hasChainBounced;

    public SerializableGuid ID { get; set; } = new SerializableGuid(18546940, 1107023770, 619373717, 789795764);

    private void Start()
    {
        if(!registered)
            RegisterKeys();
    }

    /// <summary>
    /// Save tutorial data
    /// </summary>
    public void SaveData()
    {
        if (tutorialBlackboard == null)
            tutorialBlackboard = BlackboardController.Instance.GetBlackboard("Tutorial");

        data.SeenMoveTrigger = GetData(seenMoveTrigger);
        data.SeenJumpTrigger = GetData(seenJumpTrigger);
        data.SeenSlideTrigger = GetData(seenSlideTrigger);
        data.SeenFastFallTrigger = GetData(seenFastFallTrigger);
        data.SeenCrawlTrigger = GetData(seenCrawlTrigger);
        data.SeenInteractTrigger = GetData(seenInteractTrigger);
        data.SeenAimTrigger = GetData(seenAimTrigger);
        data.SeenThrowTrigger = GetData(seenThrowTrigger);
        data.SeenPeekTrigger = GetData(seenPeekTrigger);
        data.SeenTotalRecallTrigger = GetData(seenTotalRecallTrigger);
        data.SeenSingleRecallTrigger = GetData(seenSingleRecallTrigger);
        data.SeenQuickBounceTrigger = GetData(seenQuickBounceTrigger);
        data.SeenChainBounceTrigger = GetData(seenChainBounceTrigger);

        data.Moved = GetData(hasMoved);
        data.Jumped = GetData(hasJumped);
        data.Slid = GetData(hasSlid);
        data.FastFallen = GetData(hasFastFallen);
        data.Crawled = GetData(hasCrawled);
        data.Interacted = GetData(hasInteracted);
        data.Aimed = GetData(hasAimed);
        data.Thrown = GetData(hasThrown);
        data.Peeked = GetData(hasPeeked);
        data.TotalRecall = GetData(hasTotalRecall);
        data.SingleRecall = GetData(hasSingleRecall);
        data.QuickBounced = GetData(hasQuickBounced);
        data.ChainBounced = GetData(hasChainBounced);
    }

    /// <summary>
    /// Retrieve data from a Blackboard using a Key
    /// </summary>
    /// <param name="key">The Key of the value to retrieve from the Blackboard</param>
    /// <returns>A boolean value</returns>
    private bool GetData(BlackboardKey key)
    {
        // Try to get the value out of the blackboard
        if(tutorialBlackboard.TryGetValue(key, out bool blackboardValue))
        {
            // Return the value
            return blackboardValue;
        }

        // Otherwise, return false
        return false;
    }

    /// <summary>
    /// Bind tutorial data
    /// </summary>
    /// <param name="data"></param>
    /// <param name="applyData"></param>
    public void Bind(TutorialData data, bool applyData = true)
    {
        this.data = data;
        this.data.ID = ID;

        // Register keys if they have not been registered already
        if (!registered)
            RegisterKeys();

        // Check whether or not to apply data
        if(applyData)
        {
            // Apply Blackboard values
            tutorialBlackboard.SetValue(seenMoveTrigger, data.SeenMoveTrigger);
            tutorialBlackboard.SetValue(seenJumpTrigger, data.SeenJumpTrigger);
            tutorialBlackboard.SetValue(seenSlideTrigger, data.SeenSlideTrigger);
            tutorialBlackboard.SetValue(seenFastFallTrigger, data.SeenFastFallTrigger);
            tutorialBlackboard.SetValue(seenCrawlTrigger, data.SeenCrawlTrigger);
            tutorialBlackboard.SetValue(seenInteractTrigger, data.SeenInteractTrigger);
            tutorialBlackboard.SetValue(seenAimTrigger, data.SeenAimTrigger);
            tutorialBlackboard.SetValue(seenThrowTrigger, data.SeenThrowTrigger);
            tutorialBlackboard.SetValue(seenPeekTrigger, data.SeenPeekTrigger);
            tutorialBlackboard.SetValue(seenTotalRecallTrigger, data.SeenTotalRecallTrigger);
            tutorialBlackboard.SetValue(seenSingleRecallTrigger, data.SeenSingleRecallTrigger);
            tutorialBlackboard.SetValue(seenQuickBounceTrigger, data.SeenQuickBounceTrigger);
            tutorialBlackboard.SetValue(seenChainBounceTrigger, data.SeenChainBounceTrigger);

            tutorialBlackboard.SetValue(hasMoved, data.Moved);
            tutorialBlackboard.SetValue(hasJumped, data.Jumped);
            tutorialBlackboard.SetValue(hasSlid, data.Slid);
            tutorialBlackboard.SetValue(hasFastFallen, data.FastFallen);
            tutorialBlackboard.SetValue(hasCrawled, data.Crawled);
            tutorialBlackboard.SetValue(hasInteracted, data.Interacted);
            tutorialBlackboard.SetValue(hasAimed, data.Aimed);
            tutorialBlackboard.SetValue(hasThrown, data.Thrown);
            tutorialBlackboard.SetValue(hasPeeked, data.Peeked);
            tutorialBlackboard.SetValue(hasTotalRecall, data.TotalRecall);
            tutorialBlackboard.SetValue(hasSingleRecall, data.SingleRecall);
            tutorialBlackboard.SetValue(hasQuickBounced, data.QuickBounced);
            tutorialBlackboard.SetValue(hasChainBounced, data.ChainBounced);
        }
    }

    private void RegisterKeys()
    {
        // Initialize the blackboard
        if (tutorialBlackboard == null)
            tutorialBlackboard = BlackboardController.Instance.GetBlackboard("Tutorial");

        // Register keys
        seenMoveTrigger = tutorialBlackboard.GetOrRegisterKey("SeenMoveTrigger");
        seenJumpTrigger = tutorialBlackboard.GetOrRegisterKey("SeenJumpTrigger");
        seenSlideTrigger = tutorialBlackboard.GetOrRegisterKey("SeenSlideTrigger");
        seenFastFallTrigger = tutorialBlackboard.GetOrRegisterKey("SeenFastFallTrigger");
        seenCrawlTrigger = tutorialBlackboard.GetOrRegisterKey("SeenCrawlTrigger");
        seenInteractTrigger = tutorialBlackboard.GetOrRegisterKey("SeenInteractTrigger");
        seenAimTrigger = tutorialBlackboard.GetOrRegisterKey("SeenAimTrigger");
        seenThrowTrigger = tutorialBlackboard.GetOrRegisterKey("SeenThrowTrigger");
        seenPeekTrigger = tutorialBlackboard.GetOrRegisterKey("SeenPeekTrigger");
        seenTotalRecallTrigger = tutorialBlackboard.GetOrRegisterKey("SeenTotalRecallTrigger");
        seenSingleRecallTrigger = tutorialBlackboard.GetOrRegisterKey("SeenSingleRecallTrigger");
        seenQuickBounceTrigger = tutorialBlackboard.GetOrRegisterKey("SeenQuickBounceTrigger");
        seenChainBounceTrigger = tutorialBlackboard.GetOrRegisterKey("SeenChainBounceTrigger");

        hasMoved = tutorialBlackboard.GetOrRegisterKey("HasMoved");
        hasJumped = tutorialBlackboard.GetOrRegisterKey("HasJumped");
        hasSlid = tutorialBlackboard.GetOrRegisterKey("HasSlid");
        hasFastFallen = tutorialBlackboard.GetOrRegisterKey("HasFastFallen");
        hasCrawled = tutorialBlackboard.GetOrRegisterKey("HasCrawled");
        hasInteracted = tutorialBlackboard.GetOrRegisterKey("HasInteracted");
        hasAimed = tutorialBlackboard.GetOrRegisterKey("HasAimed");
        hasThrown = tutorialBlackboard.GetOrRegisterKey("HasThrown");
        hasPeeked = tutorialBlackboard.GetOrRegisterKey("HasPeeked");
        hasTotalRecall = tutorialBlackboard.GetOrRegisterKey("HasTotalRecall");
        hasSingleRecall = tutorialBlackboard.GetOrRegisterKey("HasSingleRecall");
        hasQuickBounced = tutorialBlackboard.GetOrRegisterKey("HasQuickBounced");
        hasChainBounced = tutorialBlackboard.GetOrRegisterKey("HasChainBounced");

        // Set the save handler to registered
        registered = true;
    }
}

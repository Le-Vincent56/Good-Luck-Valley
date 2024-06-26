using GoodLuckValley.Entities;
using GoodLuckValley.Patterns.Blackboard;
using GoodLuckValley.Patterns.ServiceLocator;
using UnityEngine;

[RequireComponent(typeof(AreaCollider))]
public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private string tutorialKeyName;
    protected Blackboard tutorialBlackboard;
    protected BlackboardKey seenTutorialKey;
    private AreaCollider areaCollider;

    private void Awake()
    {
        // Get components
        areaCollider = GetComponent<AreaCollider>();
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
        tutorialBlackboard = ServiceLocator.For(this).Get<BlackboardController>().GetBlackboard("Tutorial");

        // Get/Register key
        seenTutorialKey = tutorialBlackboard.GetOrRegisterKey(tutorialKeyName);
    }

    protected void ShowTutorial(GameObject gameObject)
    {
        if (tutorialBlackboard.TryGetValue(seenTutorialKey, out bool blackboardValue))
        {
            // If already true, return
            if (blackboardValue) return;

            // Set the trigger to have been seen
            tutorialBlackboard.SetValue(seenTutorialKey, true);
        }
    }
}

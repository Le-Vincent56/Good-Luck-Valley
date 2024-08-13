using GoodLuckValley.Entities.Fireflies;
using GoodLuckValley.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireflyHandler : MonoBehaviour
{
    [Header("Fields")]
    [SerializeField] private bool hasFireflies;

    private void Start()
    {
        hasFireflies = false;
    }

    public void OnGetPlayerTransform(Component sender, object data)
    {
        // Verify the correct sender is being sent
        if (sender is not FireflyController) return;
        if (hasFireflies) return;

        // Set the follow target and set the check variable
        ((FireflyController)sender).SetFollowTarget(transform);
        hasFireflies = true;
    }

    public void OnSetPlayerFireflies(Component sender, object data)
    {
        // Verify that the correct data was sent
        if (data is not bool) return;
        if (sender is not FireflyController) return;

        // Cast the data
        bool hasFireflies = (bool)data;
        ((FireflyController)sender).PlayerHasFireflies(hasFireflies);

        // Set the data
        this.hasFireflies = hasFireflies;

        
    }
}

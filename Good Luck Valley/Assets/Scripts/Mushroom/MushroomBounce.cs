using GoodLuckValley.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomBounce : MonoBehaviour
{
    public struct BounceData
    {
        public Vector2 BounceVector;
        public ForceMode2D ForceMode;

        public BounceData(Vector2 bounceVector, ForceMode2D forceMode)
        {
            BounceVector = bounceVector;
            ForceMode = forceMode;
        }
    }

    #region EVENTS
    [SerializeField] private GameEvent onBounce;
    #endregion

    #region FIELDS
    [SerializeField] private bool canBounce;
    [SerializeField] private float bounceForce;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Bounce(Component sender, object data)
    {
        // Check if the correct data type was sent
        if (data is not GameObject) return;

        // Cast the data
        GameObject mushroom = (GameObject)data;

        // Create Bounce Data
        BounceData bounceData = new BounceData(mushroom.GetComponent<MushroomData>().GetBounceVector(), ForceMode2D.Impulse);

        // Raise the bounce event
        onBounce.Raise(this, bounceData);
    }
}

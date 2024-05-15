using GoodLuckValley.Mushroom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomData : MonoBehaviour
{
    #region FIELDS
    [SerializeField] private ShroomType shroomType;
    [SerializeField] private Vector2 bounceDirection;
    [SerializeField] private float bounceForce;
    #endregion

    public void InstantiateMushroomData(ShroomType shroomType)
    {
        // Set data
        this.shroomType = shroomType;
        bounceDirection = transform.up.normalized;

        // Decide bounce force based on shroom type
        switch (shroomType)
        {
            case ShroomType.Regular:
                bounceForce = 5f;
                break;

            case ShroomType.Wall:
                bounceForce = 5f;
                break;
        }
    }

    public Vector2 GetBounceVector()
    {
        return bounceDirection * bounceForce;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, bounceDirection);
    }
}

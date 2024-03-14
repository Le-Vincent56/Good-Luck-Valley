using GoodLuckValley.Mushroom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spore : MonoBehaviour
{
    #region FIELDS
    [SerializeField] private ShroomType shroomType;
    [SerializeField] private float rotation;
    #endregion

    public void CreateShroom()
    {
        switch(shroomType)
        {
            case ShroomType.Regular:
                break;

            case ShroomType.Wall:
                break;

            default:
                break;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider is BoxCollider2D)
        {
            // Set shroom type
            IShroomeable shroomeableTile = collision.gameObject.GetComponent<IShroomeable>();
            if (shroomeableTile != null)
            {
                shroomType = shroomeableTile.GetShroomType();
                rotation = shroomeableTile.GetCollisionAngle(collision);
            }
            else
            {
                shroomType = ShroomType.Regular;
            }

            CreateShroom();
        }
    }
}

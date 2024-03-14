using GoodLuckValley.Mushroom;
using GoodLuckValley.World.Tiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spore : MonoBehaviour
{
    #region REFERENCES
    [SerializeField] private GameObject regShroom;
    #endregion

    #region FIELDS
    [SerializeField] private ShroomType shroomType;
    [SerializeField] private CollisionData.CollisionDirection collisionDirection;
    [SerializeField] private float rotation;
    #endregion

    public void CreateShroom()
    {
        float shroomHeight = regShroom.GetComponent<SpriteRenderer>().bounds.size.y / 2;

        // The quaternion that will rotate the shroom
        Quaternion rotationQuat = Quaternion.AngleAxis(rotation, Vector3.forward);

        Vector2 spawnPosition = transform.position;

        switch(collisionDirection)
        {
            case CollisionData.CollisionDirection.Up:
                spawnPosition.y += shroomHeight;
                break;

            case CollisionData.CollisionDirection.Right:
                spawnPosition.x += shroomHeight;
                break;

            case CollisionData.CollisionDirection.Down:
                spawnPosition.y -= shroomHeight;
                break;

            case CollisionData.CollisionDirection.Left:
                spawnPosition.x -= shroomHeight;
                break;
        }

        GameObject shroom;

        switch (shroomType)
        {
            case ShroomType.Regular:
                shroom = Instantiate(regShroom, spawnPosition, rotationQuat);
                break;

            case ShroomType.Wall:
                break;

            default:
                shroom = Instantiate(regShroom, spawnPosition, rotationQuat);
                break;
        }
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider is not BoxCollider2D) return;

        // Set shroom type
        IShroomeable shroomeableTile = collider.GetComponent<IShroomeable>();
        if (shroomeableTile != null)
        {
            // Get the shroom type
            shroomType = shroomeableTile.GetShroomType();
            
            // Get and set collision data
            CollisionData collisionData = shroomeableTile.GetCollisionAngle(GetComponent<CircleCollider2D>());
            collisionDirection = collisionData.Direction;
            rotation = collisionData.Rotation;

            // Create the shroom
            CreateShroom();

            // Destroy the gameObject
            Destroy(gameObject);
        }
        else
        {
            shroomType = ShroomType.Regular;
        }
    }
}

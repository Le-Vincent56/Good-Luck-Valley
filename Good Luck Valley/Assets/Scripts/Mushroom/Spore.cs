using GoodLuckValley.Events;
using GoodLuckValley.Mushroom;
using GoodLuckValley.World.Tiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Spore : MonoBehaviour
{
    #region REFERENCES
    [Header("Events")]
    [SerializeField] private GameEvent onAddMushroom;

    [Header("Objects")]
    [SerializeField] private GameObject regShroom;
    #endregion

    #region FIELDS
    [SerializeField] private ShroomType shroomType;
    private CollisionData.CollisionDirection collisionDirection;
    private Vector2 contactPoint;
    [SerializeField] private List<Vector2> path;
    private int waypoint = 0;
    [SerializeField] private float speed;
    private float rotation;
    private bool spawnConfirmed = false;

    [Header("Physics")]
    [SerializeField] private Vector2 velocity;
    [SerializeField] private float initialUpwardVelocity = 10f;
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private float maxUpwardVelocity = 5f;
    [SerializeField] private float drag = 0.1f;
    #endregion

    private void Start()
    {
        velocity = Vector2.up * initialUpwardVelocity;
    }

    private void Update()
    {
        if(waypoint < path.Count)
        {
            // Get the current path point
            Vector2 targetPosition = path[waypoint];

            // Apply physics
            ApplyPhysics(targetPosition);

            // Move towards the target position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // Set the next position
            if((Vector2)transform.position == targetPosition)
            {
                waypoint++;
            }
        }
    }

    public void ApplyPhysics(Vector2 targetPosition)
    {
        // Get the direction of movemenbt
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        // Apply upward velocity if moving upward
        if (direction.y > 0)
            velocity.y = initialUpwardVelocity;
        else // Otherwise apply gravity
            velocity.y -= gravity * Time.deltaTime;

        // Apply drag
        velocity.y *= 1f - drag * Time.deltaTime;

        // Ensure velocity doesn't exceed the max velocity
        velocity.y = Mathf.Clamp(velocity.y, -maxUpwardVelocity, initialUpwardVelocity);

        // Move the spore
        transform.position += (Vector3)velocity * Time.deltaTime;
    }

    /// <summary>
    /// Create a mushroom
    /// </summary>
    /// <param name="spawnPoint">The spawn point to create the mushroom at</param>
    public void CreateShroom(Vector2 spawnPoint)
    {
        float shroomHeight = (regShroom.GetComponent<SpriteRenderer>().bounds.size.y / 2) - 0.035f;
        float shroomHeightDiag = shroomHeight * (3f / 4f);

        // The quaternion that will rotate the shroom
        Quaternion rotationQuat = Quaternion.AngleAxis(rotation, Vector3.forward);

        // Displace the shroom depending on collision direction
        switch (collisionDirection)
        {
            case CollisionData.CollisionDirection.Up:
                spawnPoint.y += shroomHeight;
                break;

            case CollisionData.CollisionDirection.Right:
                spawnPoint.x += shroomHeight;
                break;

            case CollisionData.CollisionDirection.Down:
                spawnPoint.y -= shroomHeight;
                break;

            case CollisionData.CollisionDirection.Left:
                spawnPoint.x -= shroomHeight;
                break;

            case CollisionData.CollisionDirection.TopRightDiag:
                spawnPoint.x += shroomHeightDiag;
                spawnPoint.y += shroomHeightDiag;
                break;

            case CollisionData.CollisionDirection.TopLeftDiag:
                spawnPoint.x -= shroomHeightDiag;
                spawnPoint.y += shroomHeightDiag;
                break;

            case CollisionData.CollisionDirection.BottomLeftDiag:
                spawnPoint.x -= shroomHeightDiag;
                spawnPoint.y -= shroomHeightDiag;
                break;

            case CollisionData.CollisionDirection.BottomRightDiag:
                spawnPoint.x += shroomHeightDiag;
                spawnPoint.y -= shroomHeightDiag;
                break;
        }

        // Create a shroom
        GameObject shroom = null;

        // Instantiate depending on the type
        switch (shroomType)
        {
            case ShroomType.Regular:
                shroom = Instantiate(regShroom, spawnPoint, rotationQuat);
                shroom.GetComponent<MushroomData>().InstantiateMushroomData(ShroomType.Regular);
                break;

            case ShroomType.Wall:
                break;

            default:
                shroom = Instantiate(regShroom, spawnPoint, rotationQuat);
                shroom.GetComponent<MushroomData>().InstantiateMushroomData(ShroomType.Regular);
                break;
        }

        onAddMushroom.Raise(this, shroom);
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        // Exit case - not the correct collider type
        if (collider is not BoxCollider2D && collider is not PolygonCollider2D) return;
        
        // Exit case - a spawn has already happened
        if (spawnConfirmed) return;

        // Set shroom type
        IShroomeable shroomeableTile = collider.GetComponent<IShroomeable>();

        // Exit case - no shroomeable tile found
        if (shroomeableTile == null) return;

        // Don't allow any other spawns to happen
        spawnConfirmed = true;

        // Find the contact point
        contactPoint = collider.ClosestPoint(transform.position);

        // Get the shroom type
        shroomType = shroomeableTile.GetShroomType();

        // Get and set collision data
        CollisionData collisionData = shroomeableTile.GetCollisionAngle(GetComponent<CircleCollider2D>(), contactPoint);
        collisionDirection = collisionData.Direction;
        rotation = collisionData.Rotation;

        // Create the shroom
        CreateShroom(collisionData.SpawnPoint);

        // Destroy the gameObject
        Destroy(gameObject);
    }

    public void SetPath(List<Vector2> path)
    {
        this.path = path;
    }
}

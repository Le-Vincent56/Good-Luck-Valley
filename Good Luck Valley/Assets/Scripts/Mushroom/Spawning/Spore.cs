using GoodLuckValley.Entity;
using GoodLuckValley.Events;
using GoodLuckValley.Mushroom;
using UnityEngine;

public class Spore : MonoBehaviour
{
    #region REFERENCES
    [Header("Events")]
    [SerializeField] private GameEvent onAddMushroom;
    

    [Header("References")]
    [SerializeField] private GameObject regShroom;
    [SerializeField] private DynamicCollisionHandler collisionHandler;
    [SerializeField] private SporeData sporeData;
    #endregion

    #region FIELDS
    [SerializeField] private ShroomType shroomType;
    [SerializeField] private Vector2 velocity;
    private FinalSpawnInfo spawnInfo;
    private bool spawnConfirmed = false;
    #endregion

    private void Start()
    {
        collisionHandler = GetComponent<DynamicCollisionHandler>();
    }

    private void FixedUpdate()
    {
        // Check for any collisions
        if (collisionHandler.collisions.Above || collisionHandler.collisions.Below ||
            collisionHandler.collisions.Left || collisionHandler.collisions.Right)
        {
            // Exit case - a spawn has already happened
            if (spawnConfirmed) return;

            // Don't allow any other spawns to happen
            spawnConfirmed = true;

            // Create the shroom
            CreateShroom();

            // Destroy the gameObject
            Destroy(gameObject);
        }

        // Apply gravity
        velocity.y += sporeData.gravity * Time.deltaTime;

        // Move the spore
        Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// Move the spore
    /// </summary>
    /// <param name="velocity"></param>
    private void Move(Vector2 velocity)
    {
        // Update raycasts
        collisionHandler.UpdateRaycastOrigins();

        // Reset collisions
        collisionHandler.collisions.ResetInfo();

        // Set the old velocity
        collisionHandler.collisions.PrevVelocity = velocity;

        // Set the facing direction
        if (velocity.x != 0f)
        {
            collisionHandler.collisions.FacingDirection = (int)Mathf.Sign(velocity.x);
        }

        // Handle horizontal collisions
        collisionHandler.HorizontalCollisions(ref velocity);

        // Handle vertical collisions
        if (velocity.y != 0f)
            collisionHandler.VerticalCollisions(ref velocity);

        // Move
        transform.Translate(velocity);
    }

    /// <summary>
    /// Create a mushroom
    /// </summary>
    /// <param name="spawnPoint">The spawn point to create the mushroom at</param>
    private void CreateShroom()
    {
        // Create a shroom
        GameObject shroom = null;

        // Instantiate depending on the type
        switch (shroomType)
        {
            case ShroomType.Regular:
                shroom = Instantiate(regShroom, spawnInfo.Position, spawnInfo.Rotation);
                shroom.GetComponent<MushroomInfo>().InstantiateMushroomData(ShroomType.Regular, spawnInfo.Angle);
                break;

            case ShroomType.Wall:
                shroom = Instantiate(regShroom, spawnInfo.Position, spawnInfo.Rotation);
                shroom.GetComponent<MushroomInfo>().InstantiateMushroomData(ShroomType.Regular, spawnInfo.Angle);
                break;

            default:
                shroom = Instantiate(regShroom, spawnInfo.Position, spawnInfo.Rotation);
                shroom.GetComponent<MushroomInfo>().InstantiateMushroomData(ShroomType.Regular, spawnInfo.Angle);
                break;
        }

        // Add the Mushroom to its respective list
        // Calls to:
        //  - MushroomTracker.AddMushroom();
        onAddMushroom.Raise(this, shroom);
    }

    /// <summary>
    /// Set the throw vector for the Spore
    /// </summary>
    /// <param name="throwDirection"></param>
    public void ThrowSpore(Vector2 throwDirection)
    {
        
        velocity.x = throwDirection.x * sporeData.throwSpeed;
        velocity.y = throwDirection.y * sporeData.throwSpeed;
    }

    /// <summary>
    /// Set the final spawn info for the Spore
    /// </summary>
    /// <param name="spawnInfo"></param>
    public void SetSpawnInfo(FinalSpawnInfo spawnInfo)
    {
        this.spawnInfo = spawnInfo;
    }
}

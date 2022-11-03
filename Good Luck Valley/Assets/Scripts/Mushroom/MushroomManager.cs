using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public enum ThrowState
{
    NotThrowing,
    Throwing
}

public class MushroomManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] GameObject player;
    [SerializeField] Rigidbody2D playerRB;             // The player's rigidbody used for spawning mushrooms
    private PlayerMovement playerMove;        // PlayerMovement checks which direction player is facing

    [Header("Camera")]
    [SerializeField] Camera cam;
    float camHeight;
    float camWidth;

    private bool canThrow;
    private float throwCooldown = 0.2f;

    [Header("Platform Interaction")]
    [SerializeField] string stuckSurfaceTag;  // Tag of object shroom will stick to
    [SerializeField] EnvironmentManager environmentManager;
    private ContactFilter2D layer;         // A contact filter to filter out ground layers
    Vector2 forceDirection;

    [Header("Cursor")]
    [SerializeField] GameCursor cursor;

    [Header("Mushroom")]
    [SerializeField] GameObject organicShroom;
    private List<GameObject> mushroomList;    // List of currently spawned shrooms
    private const int mushroomLimit = 3;      // Constant for max amount of shrooms

    [SerializeField] private float offset;      // Offset for spawning shrooms outside of player hitbox                                        
    private int mushroomCount;                // How many shrooms are currently spawned in
    public List<GameObject> MushroomList { get { return mushroomList; } }

    [Header("Throw")]
    [SerializeField] int throwMultiplier;

    public GameObject throwUI_Script;
    private ThrowState throwState;

    [SerializeField] GameObject shroomPoint;
    [SerializeField] GameObject tilemap;
    private float shiftAmount;

    // Start is called before the first frame update
    void Start()
    {
        // Grab components
        cam = Camera.main;
        camHeight = cam.orthographicSize;
        camWidth = camHeight * cam.aspect;
        player = GameObject.Find("Player");
        playerRB = player.GetComponent<Rigidbody2D>();
        playerMove = player.GetComponent<PlayerMovement>();
        mushroomList = new List<GameObject>();
        environmentManager = FindObjectOfType<EnvironmentManager>();
        cursor = FindObjectOfType<GameCursor>();

        // Instantiates layer field
        layer = new ContactFilter2D();
        // Allows the layer to use layer masks when filtering
        layer.useLayerMask = true;
        // Sets the layerMask property of layer to the ground layer 
        layer.layerMask = LayerMask.GetMask("Ground");
    }

    // Update is called once per frame
    void Update()
    {
        // Updates mushroom count               
        mushroomCount = mushroomList.Count;

        // Direction force is being applied to shroom
        forceDirection = cursor.transform.position - playerRB.transform.position;
        //forceDirection = cam.ScreenToWorldPoint(new Vector2(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y)) - playerRB.transform.position;
        //forceDirection = cam.ScreenToWorldPoint(Input.mousePosition) - playerRB.transform.position;
        //Debug.Log(forceDirection);
        //Debug.Log(playerRB.position);

        switch (throwState)
        {
            case ThrowState.NotThrowing:
                break;


            case ThrowState.Throwing:
                if (playerMove.IsFacingRight)
                {
                    throwUI_Script.GetComponent<ThrowUI>().PlotTrajectory(playerRB.position, 
                                                                          forceDirection.normalized * throwMultiplier, 
                                                                          offset, 
                                                                          playerMove.IsFacingRight);
                }
                else
                {
                    throwUI_Script.GetComponent<ThrowUI>().PlotTrajectory(playerRB.position, 
                                                                          forceDirection.normalized * throwMultiplier, 
                                                                          offset, 
                                                                          playerMove.IsFacingRight);
                }
                break;                
        }

        if (MushroomList.Count > 0)
        {
            StickShrooms();
            TriggerPlatforms();
        }

        CheckIfCanThrow();
    }

    /// <summary>
    /// Creates a shroom, adds it to mushroomList, launches shroom in the appropriate Direction
    /// </summary>
    /// <param name="type"> Which type of mushroom is being thrown</param>
    void ThrowMushroom()
    {        
        if (playerMove.IsFacingRight)
        {
            mushroomList.Add(Instantiate(organicShroom, new Vector2(playerRB.position.x + offset, playerRB.position.y), Quaternion.identity));
            mushroomList[mushroomCount].GetComponent<Rigidbody2D>().AddForce(forceDirection.normalized * throwMultiplier, ForceMode2D.Impulse);
        }
        else
        {   
            mushroomList.Add(Instantiate(organicShroom,new Vector2(playerRB.position.x - offset, playerRB.position.y), Quaternion.identity));
            mushroomList[mushroomCount].GetComponent<Rigidbody2D>().AddForce(forceDirection.normalized * throwMultiplier, ForceMode2D.Impulse);
        }
    }

    /// <summary>
    /// Checks if the shroom being thrown will cause there to be more than 3 shrooms
    /// spawned; if so, deletes the first shroom thrown and calls the ThrowShroom method
    /// if not, calls the ThrowShroom method
    /// </summary>
    /// <param name="type"> Which type of mushroom is being thrown </param>
    void CheckShroomCount()
    {
        // Checks if the current number of spawned mushrooms is lower than the max amount
        if (mushroomCount < mushroomLimit)
        {
            // If so, ThrowMushroom is called
            throwUI_Script.GetComponent<ThrowUI>().DeleteLine();
            ThrowMushroom();
        }
        else if (mushroomCount >= mushroomLimit)
        {
            // If not, ThrowMushroom is called and the first shroom thrown is destroyed and removed from mushroomList
            throwUI_Script.GetComponent<ThrowUI>().DeleteLine();
            ThrowMushroom();
            Destroy(mushroomList[0]);
            mushroomList.RemoveAt(0);
        }
    }

    /// <summary>
    /// Runs through shroom list and checks if it is colliding with a platform
    /// If so it freezes the shroom and rotates to match rotation of platform
    /// </summary>
    private void StickShrooms()
    {
        // loops for each object in the mushroomlist
        foreach (GameObject m in mushroomList)
        {

            // checks if the mushroom is touching the platform and hasn't rotated
            if (m.GetComponent<CircleCollider2D>().IsTouching(tilemap.GetComponent<TilemapCollider2D>()) &&
                !m.GetComponent<MushroomInfo>().hasRotated)
            {
                // If so, calls rotate shroom method to rotate and freeze the shroom properly
                RotateAndFreezeShroom(m);
            }
        }
    }

    private void TriggerPlatforms()
    {
        // loops for each object in the mushroomlist
        foreach (GameObject m in mushroomList)
        {
            foreach(GameObject p in environmentManager.weightedPlatforms)
            {
                // checks if the mushroom is touching the platform and hasn't rotated
                if (m.GetComponent<CircleCollider2D>().IsTouching(p.GetComponent<BoxCollider2D>()) &&
                    !m.GetComponent<MushroomInfo>().hasRotated)
                {
                    // If so, calls rotate shroom method to rotate and freeze the shroom properly
                    RotateAndFreezeShroom(m);

                    p.GetComponent<MoveablePlatform>().CheckWeight(m);
                }
            }
        }
    }


    /// <summary>
    /// Rotates and freezes a shroom to match the orientation of the platform it is colliding with
    /// </summary>
    /// <param name="platform"> The platform colliding with the shroom</param>
    /// <param name="mushroom"> The mushroom colliding with the platform</param>
    private void RotateAndFreezeShroom(GameObject mushroom)
    {
        // Saves the colliders of the platforms the shroom is coming into contact with into an array
        ContactPoint2D[] contacts = new ContactPoint2D[1];
        mushroom.GetComponent<CircleCollider2D>().GetContacts(contacts);
        Debug.Log(contacts[0].point);

        AdjustShroomAndPlayerPos(mushroom);

        // The direction vector that the mushroom needs to point towards,
        //      contacts[0].point is the point the shroom is touching the platform at
        //      mushroom.transform.position is the mushroom's position,
        //          casted to a vector 2 so it can be subtracted from the contact point
        Vector2 direction = contacts[0].point - (Vector2)mushroom.transform.position;

        // The angle that the shroom is going to rotate at
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // The quaternion that will rotate the s
        Quaternion rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
        mushroom.transform.rotation = rotation;


        // Freezes shroom movement and rotation, and sets hasRotated to true
        mushroom.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        mushroom.GetComponent<MushroomInfo>().hasRotated = true;
    }

    /// <summary>
    /// Check if the player can throw
    /// </summary>
    private void CheckIfCanThrow()
    {
        // Reduce time from the throw cooldown
        throwCooldown -= Time.deltaTime;

        // If enough time has passed, set canThrow to true, otherwise set it to false
        if(throwCooldown <= 0)
        {
            canThrow = true;
        } else
        {
            canThrow = false;
        }
    }

    private void AdjustShroomAndPlayerPos(GameObject mushroom)
    {
        float shroomShift;
        if (playerMove.IsFacingRight)
        {
            shroomShift = -shiftAmount;
        }
        else
        {
            shroomShift = shiftAmount;
        }
        playerRB.transform.position = new Vector2(playerRB.transform.position.x + shroomShift, playerRB.transform.position.y);
        mushroom.transform.position = new Vector2(mushroom.transform.position.x + shroomShift, mushroom.transform.position.y);

    }

    private void ShroomInWallCheck()
    {
        LayerMask mask = LayerMask.GetMask("Ground");
        float currentOffset;
        if (playerMove.IsFacingRight)
        {
            currentOffset = 0.1f;
        }
        else
        {
            currentOffset = -0.1f;
        }
        RaycastHit2D hitInfo = Physics2D.Linecast(playerRB.position, new Vector2(playerRB.position.x + currentOffset, playerRB.position.y), mask);

        if (hitInfo)
        {
            shiftAmount = offset;
        }
        else
        {
            shiftAmount = 0;
        }
    }

    #region INPUT HANDLER
    
    // If we want a separate fire and aim button
    public void OnTriggerAim(InputAction.CallbackContext context)
    {
        //if (context.started)
        //{
        //    switch (throwState)
        //    {
        //        case ThrowState.NotThrowing:
        //            throwState = ThrowState.Throwing;
        //            break;

        //        case ThrowState.Throwing:
        //            throwState = ThrowState.NotThrowing;
        //            break;
        //    }
        //}
    }
    
    public void OnFire(InputAction.CallbackContext context)
    {
        // If we want the same button for fire and aim - aim on press, fire on release
        if (context.started)
        {
            switch (throwState)
            {
                case ThrowState.NotThrowing:
                    throwState = ThrowState.Throwing;
                    break;
            }
        }

        if (context.canceled)
        {
            // Check if the shroom can be thrown
            if (canThrow)
            {
                // Throw the shroom
                switch (throwState)
                {
                    case ThrowState.Throwing:
                        CheckShroomCount();
                        throwState = ThrowState.NotThrowing;
                        break;
                }

                // Reset throw variables
                canThrow = false;
                throwCooldown = 0.2f;
            }
            switch (throwState)
            {
                case ThrowState.Throwing:
                    ShroomInWallCheck();
                    CheckShroomCount();
                    throwState = ThrowState.NotThrowing;
                    break;
            }
        }
    }

    public void OnRecallShrooms(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            foreach (GameObject m in mushroomList)
            {
                Destroy(m);
            }
            mushroomList.Clear();
        }
    }
    #endregion
}

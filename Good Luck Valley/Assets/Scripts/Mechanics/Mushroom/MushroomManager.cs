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
    #region REFERENCES
    private GameObject player;
    [SerializeField] private Rigidbody2D playerRB;             // The player's rigidbody used for spawning mushrooms
    private PlayerMovement playerMove;                         // PlayerMovement checks which direction player is facing
    private Animator playerAnim;
    private PauseMenu pauseMenu;
    [SerializeField] private Camera cam;
    [SerializeField] private PlatformsManager environmentManager;
    [SerializeField] private GameCursor cursor;
    [SerializeField] private GameObject tilemap;
    [SerializeField] private GameObject shroomPoint;
    private UIManager uiManager;
    [SerializeField] private GameObject spore;
    [SerializeField] private GameObject mushroom;
    private ThrowUI throwUI_Script;
    [SerializeField] private GameObject testObject;
    #endregion

    #region FIELDS
    private float camHeight;
    private float camWidth;
    private bool canThrow;
    private float throwCooldown = 0.2f;
    private float bounceCooldown = 0.2f;    
    [SerializeField] bool throwing = false;
    [SerializeField] float throwAnimTimer = 2f;
    [SerializeField] string stuckSurfaceTag;                    // Tag of object shroom will stick to
    private ContactFilter2D layer;                              // A contact filter to filter out ground layers
    private Vector2 forceDirection;    
    private List<GameObject> mushroomList;                      // List of currently spawned shrooms
    private int mushroomLimit = 3;                              // Max amount of shrooms
    private Stack<int> removeShroomIndexes;
    private Dictionary<int, GameObject> changeShroomIndexes;
    [SerializeField] private Vector2 offset;                    // Offset for spawning shrooms outside of player hitbox
    private int mushroomCount;                                   // How many shrooms are currently spawned in
    [SerializeField] bool throwUnlocked = false;
    [SerializeField] int throwMultiplier;
    [SerializeField] Vector3 fixPlayer;
    private ThrowState throwState;
    private float tempOffset;
    #endregion

    #region PROPERTIES
    public int MushroomCount { get { return mushroomCount; } set { mushroomCount = value; } }
    public List<GameObject> MushroomList { get { return mushroomList; } }
    public bool ThrowUnlocked { get { return throwUnlocked; } set { throwUnlocked = value; } }

    public int MushroomLimit { get { return mushroomLimit; } set { mushroomLimit = value; } }

    public int ThrowMultiplier { get { return throwMultiplier; } set { throwMultiplier = value; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Grab components

        // Camera
        cam = Camera.main;
        camHeight = cam.orthographicSize;
        camWidth = camHeight * cam.aspect;

        // Player
        player = GameObject.Find("Player");
        playerRB = player.GetComponent<Rigidbody2D>();
        playerMove = player.GetComponent<PlayerMovement>();
        playerAnim = GameObject.Find("PlayerSprite").GetComponent<Animator>();

        // Mushroom
        mushroomList = new List<GameObject>();
        removeShroomIndexes = new Stack<int>();
        changeShroomIndexes = new Dictionary<int, GameObject>();

        // Managers
        environmentManager = FindObjectOfType<PlatformsManager>();
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        // UI
        cursor = FindObjectOfType<GameCursor>();
        pauseMenu = GameObject.Find("PauseUI").GetComponent<PauseMenu>();
        throwUI_Script = GameObject.Find("Throw UI").GetComponent<ThrowUI>();

        // Instantiates layer field
        layer = new ContactFilter2D();
        // Allows the layer to use layer masks when filtering
        layer.useLayerMask = true;
        // Sets the layerMask property of layer to the ground layer 
        layer.layerMask = LayerMask.GetMask("Ground");
        tempOffset = offset.x;
    }

    // Update is called once per frame
    void Update()
    {
        // Animation updates
        if (throwing)
        {
            throwAnimTimer -= Time.deltaTime;
            if(throwAnimTimer <= 0)
            {
                throwing = false;
                playerAnim.SetBool("Throwing", false);
            }
        }

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
                throwUI_Script.PlotTrajectory(playerRB.position,
                                              forceDirection.normalized * throwMultiplier,
                                              playerMove.IsFacingRight);
                if (pauseMenu.Paused)
                {
                    throwUI_Script.DeleteLine();
                }
                break;                
        }

        if (MushroomList.Count > 0)
        {
            StickShrooms();
            // TriggerPlatforms(); moved to stick shrooms to save some memory
        }

        CheckIfCanThrow();
        CheckIfCanBounce();
    }

    /// <summary>
    /// Creates a shroom, adds it to mushroomList, launches shroom in the appropriate Direction
    /// </summary>
    /// <param name="type"> Which type of mushroom is being thrown</param>
    void ThrowMushroom()
    {
        mushroomCount = mushroomList.Count;
        mushroomList.Add(Instantiate(spore, playerRB.position, Quaternion.identity));

        mushroomList[mushroomCount].GetComponent<Rigidbody2D>().AddForce(forceDirection.normalized * throwMultiplier, ForceMode2D.Impulse);
        mushroomList[mushroomCount].GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
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
            throwUI_Script.DeleteLine();
            ThrowMushroom();
        }
        else if (mushroomCount >= mushroomLimit)
        {
            // If not, ThrowMushroom is called and the first shroom thrown is destroyed and removed from mushroomList
            throwUI_Script.DeleteLine();
            Destroy(mushroomList[0]);
            mushroomList.RemoveAt(0);
            ThrowMushroom();
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
            MushroomInfo mInfo = m.GetComponent<MushroomInfo>();

            if (!mInfo.HasRotated)
            {
                // checks if the mushroom is touching the platform and hasn't rotated
                if (m.GetComponent<CircleCollider2D>().IsTouching(tilemap.GetComponent<CompositeCollider2D>()))
                {
                    // If so, calls rotate shroom method to rotate and freeze the shroom properly
                    RotateAndFreezeShroom(m);
                }

                foreach (GameObject p in environmentManager.WeightedPlatforms)
                {
                    // checks if the mushroom is touching the platform and hasn't rotated
                    if (m.GetComponent<CircleCollider2D>().IsTouching(p.GetComponent<BoxCollider2D>()))
                    {
                        // If so, calls rotate shroom method to rotate and freeze the shroom properly
                        Debug.Log("1111");
                        RotateAndFreezeShroom(m);

                        p.GetComponent<MoveablePlatform>().CheckWeight(m);
                    }
                }

                foreach (GameObject d in environmentManager.DecomposableTiles)
                {
                    if (m.GetComponent<CircleCollider2D>().IsTouching(d.GetComponent<BoxCollider2D>()))
                    {
                        d.GetComponent<DecompasableTile>().IsDecomposed = true;
                        removeShroomIndexes.Push(mushroomList.IndexOf(m));
                    }
                }
            }
        }

        if (removeShroomIndexes.Count > 0)
        {
            RemoveShrooms();
        }
        if (changeShroomIndexes.Count > 0)
        {
            ChangeShrooms();
        }
    }
    
    /// <summary>
    /// Remove all Mushrooms
    /// </summary>
    private void RemoveShrooms()
    {
        for (int i = 0; i < removeShroomIndexes.Count; i++)
        {
            Destroy(MushroomList[removeShroomIndexes.Pop()]);
            mushroomList.RemoveAt(removeShroomIndexes.Pop());
        }
    }

    /// <summary>
    /// Change the Spores into Mushrooms
    /// </summary>
    private void ChangeShrooms()
    {
        for (int i = 0; i < mushroomCount; i++)
        {
            if (changeShroomIndexes.ContainsKey(i))
            {
                GameObject tShroom = mushroomList[i];
                mushroomList[i] = changeShroomIndexes[i];
                Destroy(tShroom);
            }
        }
        changeShroomIndexes.Clear();
    }

    /// <summary>
    /// Rotates and freezes a shroom to match the orientation of the platform it is colliding with
    /// </summary>
    /// <param name="platform"> The platform colliding with the shroom</param>
    /// <param name="mushroom"> The mushroom colliding with the platform</param>
    private void RotateAndFreezeShroom(GameObject mushroom)
    {
        // Saves the colliders of the platforms the shroom is coming into contact with into an array
        ContactPoint2D[] contacts = new ContactPoint2D[10];
        mushroom.GetComponent<CircleCollider2D>().GetContacts(contacts);

        // The direction vector that the mushroom needs to point towards,
        //      contacts[0].point is the point the shroom is touching the platform at
        //      mushroom.transform.position is the mushroom's position,
        //          casted to a vector 2 so it can be subtracted from the contact point
        ContactPoint2D contactPoint = contacts[0];

        foreach (ContactPoint2D cPoint in contacts)
        {
            if (cPoint.collider is CompositeCollider2D)
            {
                contactPoint = cPoint;
            }
        }
        Vector2 direction = contactPoint.normal;

        // The angle that the shroom is going to rotate at
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // The quaternion that will rotate the shroom
        Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        mushroom.transform.rotation = rotation;

        // Freezes shroom movement and rotation, and sets hasRotated to true
        mushroom.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        mushroom.GetComponent<MushroomInfo>().HasRotated = true;

        GameObject shroom = Instantiate(this.mushroom, mushroom.transform.position, rotation);
        shroom.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        shroom.GetComponent<MushroomInfo>().HasRotated = true;
        changeShroomIndexes[mushroomList.IndexOf(mushroom)] = shroom;

        // Set the MushroomInfo angle to the calculated angle
        shroom.GetComponent<MushroomInfo>().RotateAngle = angle;
    }

    /// <summary>
    /// Check if the player can throw
    /// </summary>
    private void CheckIfCanThrow()
    {
        // Reduce time from the throw cooldown
        throwCooldown -= Time.deltaTime;

        // If enough time has passed, set canThrow to true, otherwise set it to false
        if (throwCooldown <= 0)
        {
            canThrow = true;
        }
        else
        {
            canThrow = false;
        }
    }

    /// <summary>
    /// Check if the player can bounce
    /// </summary>
    private void CheckIfCanBounce()
    {
        // Reduce time from the bounce cooldown
        bounceCooldown -= Time.deltaTime;

        // If enough time has passed, set canBounce to true, otherwise set it to false
        if (bounceCooldown <= 0)
        {
            playerMove.GetComponent<BouncingEffect>().CanBounce = true;
        }
        else
        {
            playerMove.GetComponent<BouncingEffect>().CanBounce = false;
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
        if(!pauseMenu.Paused && throwUnlocked)
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
                    throwing = true;
                    throwAnimTimer = 0.01f;
                    playerAnim.SetBool("Throwing", true);

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
                    bounceCooldown = 0.2f;
                }
            }
        }
    }

    public void OnRecallShrooms(InputAction.CallbackContext context)
    {
        if(!pauseMenu.Paused && throwUnlocked)
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
    }

    public void OnRemoveLastShroom(InputAction.CallbackContext context)
    {
        if (!pauseMenu.Paused && throwUnlocked)
        {
            if (context.started)
            {
                if (mushroomCount != 0)
                {
                    Destroy(mushroomList[mushroomCount - 1]);
                    mushroomList.RemoveAt(mushroomCount - 1);
                    mushroomCount--;
                }
            }
        }
    }    
    #endregion
}

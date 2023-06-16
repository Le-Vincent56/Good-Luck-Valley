using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
using static UnityEngine.ParticleSystem;

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
    private Journal journal;
    private ShroomCounter shroomCounter;
    [SerializeField] private PlayableDirector director;
    private Tutorial tutorialEvent;
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
    [SerializeField] private List<GameObject> mushroomList;                      // List of currently spawned shrooms
    private int mushroomLimit = 3;                              // Max amount of shrooms
    private Stack<int> removeShroomIndexes;                     // Stack for tracking the indexes of shrooms that need to be removed
    private Dictionary<int, GameObject> changeShroomIndexes;    // Dictionary for tracking the indexes and objects of shrooms that need to be changed from spores.
    [SerializeField] private Vector2 offset;                    // Offset for spawning shrooms outside of player hitbox
    private bool throwUnlocked = false;
    [SerializeField] int throwMultiplier;
    [SerializeField] Vector3 fixPlayer;
    private ThrowState throwState;
    private bool throwPrepared = false;
    [SerializeField] private float shroomDuration;
    [SerializeField] private bool enableShroomTimers;
    private bool usingLotusCutscene;
    private bool throwLocked = false;
    [SerializeField] private bool usingTutorial = false;
    [SerializeField] private bool firstTimeHittingMax = true;
    [SerializeField] private bool firstTimeRecalling = true;
    #endregion

    #region PROPERTIES
    public List<GameObject> MushroomList { get { return mushroomList; } }
    public bool ThrowUnlocked { get { return throwUnlocked; } set { throwUnlocked = value; } }

    public int MushroomLimit { get { return mushroomLimit; } set { mushroomLimit = value; } }

    public int ThrowMultiplier { get { return throwMultiplier; } set { throwMultiplier = value; } }
    public float ShroomDuration { get { return shroomDuration; } set { shroomDuration = value; } }
    public bool EnableShroomTimers { get { return enableShroomTimers;} set { enableShroomTimers = value; } }
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
        shroomCounter = GameObject.Find("MushroomCountUI").GetComponent<ShroomCounter>();

        // Managers
        environmentManager = FindObjectOfType<PlatformsManager>();
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        // UI
        cursor = FindObjectOfType<GameCursor>();
        pauseMenu = GameObject.Find("PauseUI").GetComponent<PauseMenu>();
        throwUI_Script = GameObject.Find("Throw UI").GetComponent<ThrowUI>();
        journal = GameObject.Find("JournalUI").GetComponent<Journal>();

        // Instantiates layer field
        layer = new ContactFilter2D();
        // Allows the layer to use layer masks when filtering
        layer.useLayerMask = true;
        // Sets the layerMask property of layer to the ground layer 
        layer.layerMask = LayerMask.GetMask("Ground");

        // Set Cutscene Director
        if(director == null)
        {
            usingLotusCutscene = false;
        } else
        {
            usingLotusCutscene = true;
        }

        // Set Tutorial Event
        if(usingTutorial)
        {
            tutorialEvent = GameObject.Find("TutorialUI").GetComponent<Tutorial>();
        } else
        {
            tutorialEvent = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Direction force is being applied to shroom
        forceDirection = cursor.transform.position - playerRB.transform.position;
        //forceDirection = cam.ScreenToWorldPoint(new Vector2(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y)) - playerRB.transform.position;
        //forceDirection = cam.ScreenToWorldPoint(Input.mousePosition) - playerRB.transform.position;
        //Debug.Log(forceDirection);
        //Debug.Log(playerRB.position);

        // Check if a cutscene will be used at the beginning of the level
        if(usingLotusCutscene)
        {
            // If so, check the PlayableDirector state
            if(director.state == PlayState.Playing)
            {
                // If it's playing, lock the throw
                throwLocked = true;
            } else
            {
                // Otherwise, unlock the throw
                throwLocked = false;
            }
        }

        if(usingTutorial && firstTimeHittingMax && mushroomList.Count == 3)
        {
            tutorialEvent.ShowingRemoveText = true;
            firstTimeHittingMax = false;
        }

        
        if(playerAnim.GetBool("Throwing") == true)
        {
            AnimatorClipInfo[] animationClip = playerAnim.GetCurrentAnimatorClipInfo(0);
            AnimatorStateInfo animationInfo = playerAnim.GetCurrentAnimatorStateInfo(0);
            Debug.Log(animationInfo.normalizedTime);
            if(animationInfo.normalizedTime % 1 > 0.9)
            {
                playerAnim.SetBool("Throwing", false);
            }
        }

        // FOR WHEN THROW ANIMATIONS ARE FULLY IMPLEMENTED
        //if (throwPrepared)
        //{
        //    AnimatorClipInfo[] throwAnimationClip = playerAnim.GetCurrentAnimatorClipInfo(0);
        //    int currentFrame = (int)(throwAnimationClip[0].weight * (throwAnimationClip[0].clip.length * throwAnimationClip[0].clip.frameRate));
        //    if (currentFrame == 5)
        //    {
        //        throwing = true;

        //        // Throw the shroom
        //        //switch (throwState)
        //        //{
        //        //    case ThrowState.Throwing:
        //        //        CheckShroomCount();
        //        //        throwState = ThrowState.NotThrowing;
        //        //        break;
        //        //}

        //        if (throwState == ThrowState.Throwing)
        //        {
        //            CheckShroomCount();
        //            throwState = ThrowState.NotThrowing;
        //        }

        //        // Reset throw variables
        //        canThrow = false;
        //        throwCooldown = 0.2f;
        //        bounceCooldown = 0.2f;
        //        throwPrepared = false;
        //    }
        //}

        switch (throwState)
        {
            case ThrowState.NotThrowing:
                break;


            case ThrowState.Throwing:
                if (mushroomList.Count < mushroomLimit)
                {
                    throwUI_Script.PlotTrajectory(playerRB.position,
                                                  forceDirection.normalized * throwMultiplier,
                                                  playerMove.IsFacingRight);
                }
                if (pauseMenu.Paused)
                {
                    throwUI_Script.DeleteLine();
                }
                break;                
        }

        if (mushroomList.Count > 0)
        {
            StickShrooms();
            // TriggerPlatforms(); moved to stick shrooms to save some memory
        }

        CheckIfCanThrow();
        //CheckIfCanBounce();
        CheckShroomDuration();
        //UpdateShroomCooldowns();
    }

    void CheckShroomDuration()
    {
        foreach (GameObject m in mushroomList) 
        { 
            if (m.GetComponent<MushroomInfo>().DurationTimer <= 0)
            {
                removeShroomIndexes.Push(mushroomList.IndexOf(m));
            }
        }

        if (removeShroomIndexes.Count > 0)
        {
            RemoveShrooms();
        }
    }

    /// <summary>
    /// Creates a shroom, adds it to mushroomList, launches shroom in the appropriate Direction
    /// </summary>
    /// <param name="type"> Which type of mushroom is being thrown</param>
    void ThrowMushroom()
    {
        mushroomList.Add(Instantiate(spore, playerRB.position, Quaternion.identity));

        mushroomList[mushroomList.Count - 1].GetComponent<Rigidbody2D>().AddForce(forceDirection.normalized * throwMultiplier, ForceMode2D.Impulse);
        mushroomList[mushroomList.Count - 1].GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

        if (mushroomList.Count - 1 < mushroomLimit)
        {
            mushroomList[mushroomList.Count - 1].GetComponent<MushroomInfo>().ShroomIcon = shroomCounter.ShroomIconQueue[0];
            shroomCounter.ShroomIconQueue.RemoveAt(0);
        }
        Debug.Log("Shroom Thrown Icon: " + mushroomList[mushroomList.Count - 1].GetComponent<MushroomInfo>().ShroomIcon);
    }

    /// <summary>
    /// Checks if the shroom being thrown will cause there to be more than 3 shrooms
    /// spawned; if so, deletes the first shroom thrown and calls the ThrowShroom method
    /// if not, calls the ThrowShroom method
    /// </summary>
    /// <param name="type"> Which type of mushroom is being thrown </param>
    void CheckShroomCount()
    {
        // If so, ThrowMushroom is called
        throwUI_Script.DeleteLine();
        ThrowMushroom();


        //// Checks if the current number of spawned mushrooms is lower than the max amount
        //if (mushroomList.Count < mushroomLimit)
        //{
        //}
        //else if (mushroomList.Count >= mushroomLimit)
        //{
        //    // If not, ThrowMushroom is called and the first shroom thrown is destroyed and removed from mushroomList
        //    throwUI_Script.DeleteLine();
        //    //Debug.Log("Shroom Destroy Icon: " + mushroomList[0].GetComponent<MushroomInfo>().ShroomIcon);
        //    //shroomCounter.ShroomIconQueue.Add(mushroomList[0].GetComponent<MushroomInfo>().ShroomIcon);
        //    //mushroomList[0].GetComponent<MushroomInfo>().ResetCounter();
        //    //Destroy(mushroomList[0]);
        //    //mushroomList.RemoveAt(0);
        //    //ThrowMushroom();
        //}
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
                        RotateAndFreezeShroom(m);

                        p.GetComponent<MoveablePlatform>().CheckWeight(m);
                    }
                }

                // Loops through all decomposable tiles
                foreach (GameObject d in environmentManager.DecomposableTiles)
                {
                    // Checks if the tile is touching the shroom
                    if (m.GetComponent<CircleCollider2D>().IsTouching(d.GetComponent<BoxCollider2D>()))
                    {
                        // Sets the tile to decomposed
                        //if (d.GetComponent<DecompasableTile>().IsDecomposed == false)
                        //{
                        //    d.GetComponent<DecompasableTile>().IsDecomposed = true;
                        //}

                        // Pushes the index of shroom that is touching it to the stack of shroom removal indexes
                        // removeShroomIndexes.Push(mushroomList.IndexOf(m));
                        RotateAndFreezeShroom(m);
                    }
                }
            }
        }

        // Checks if there are any indexes in the removeShroomIndexes stack
        if (removeShroomIndexes.Count > 0)
        {
            // If so, calls removeShrooms
            RemoveShrooms();
        }
        // Checks if there are any indexes in the changeShroomIndexes stack
        if (changeShroomIndexes.Count > 0)
        {
            // If so, calles ChangeShrooms
            ChangeShrooms();
        }
    }
    
    /// <summary>
    /// Remove all Mushrooms
    /// </summary>
    private void RemoveShrooms()
    {
        int indexCount = removeShroomIndexes.Count;
        for (int i = 0; i < indexCount; i++)
        {
            int shroomIndex = removeShroomIndexes.Pop();
            Debug.Log("Shroom Index" + shroomIndex);
            shroomCounter.ShroomIconQueue.Add(mushroomList[shroomIndex].GetComponent<MushroomInfo>().ShroomIcon);
            mushroomList[shroomIndex].GetComponent<MushroomInfo>().ResetCounter();
            Destroy(mushroomList[shroomIndex]);
            mushroomList.RemoveAt(shroomIndex);
        }
    }

    /// <summary>
    /// Change the Spores into Mushrooms
    /// </summary>
    private void ChangeShrooms()
    {
        // Loops through the list of mushrooms
        for (int i = 0; i < mushroomList.Count; i++)
        {
            // Checks if the current index is contained in the shroomIndexes dictionary as a key
            if (changeShroomIndexes.ContainsKey(i))
            {
                // Saves a reference to the spore in the mushroom list at the current index
                GameObject tempShroom = mushroomList[i];

                // Sets the mushroom in the mushroomList at the current index to the shroom paired with the index key i
                mushroomList[i] = changeShroomIndexes[i];

                // Destroys the temporary shroom
                Destroy(tempShroom);
            }
        }
        // Clears the changeShroomIndexes stack
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
        shroom.GetComponent<MushroomInfo>().ShroomIcon = mushroom.GetComponent<MushroomInfo>().ShroomIcon;
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

    ///// <summary>
    ///// Check if the player can bounce
    ///// </summary>
    //private void CheckIfCanBounce()
    //{
    //    // Reduce time from the bounce cooldown
    //    bounceCooldown -= Time.deltaTime;

    //    // If enough time has passed, set canBounce to true, otherwise set it to false
    //    if (bounceCooldown <= 0)
    //    {
    //        playerMove.GetComponent<BouncingEffect>().CanBounce = true;
    //    }
    //    else
    //    {
    //        playerMove.GetComponent<BouncingEffect>().CanBounce = false;
    //    }
    //}
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
        if(!pauseMenu.Paused && throwUnlocked && !journal.MenuOpen && !throwLocked && mushroomList.Count < mushroomLimit)
        {
            // If we want the same button for fire and aim - aim on press, fire on release
            if (context.started)
            {
                //switch (throwState)
                //{
                //    case ThrowState.NotThrowing:
                //        throwState = ThrowState.Throwing;
                //        break;
                //}
                if (throwState == ThrowState.NotThrowing) 
                {
                    throwState = ThrowState.Throwing;
                }
            }

            if (context.canceled)
            {
                // Set animation
                playerAnim.SetBool("Throwing", true);

                // Check if the shroom can be thrown
                if (canThrow)
                {
                    throwing = true;

                    // Throw the shroom
                    //switch (throwState)
                    //{
                    //    case ThrowState.Throwing:
                    //        CheckShroomCount();
                    //        throwState = ThrowState.NotThrowing;
                    //        break;
                    //}

                    if (throwState == ThrowState.Throwing)
                    {
                        CheckShroomCount();
                        throwState = ThrowState.NotThrowing;
                    }

                    // Reset throw variables
                    canThrow = false;
                    throwCooldown = 0.2f;
                    bounceCooldown = 0.2f;

                    // Prepare the throw for Animation
                    throwPrepared = true;
                }
            }
        }
    }

    /// <summary>
    /// Recalls all thrown shrooms
    /// </summary>
    /// <param name="context"></param>
    public void OnRecallShrooms(InputAction.CallbackContext context)
    {
        // Checks if the game is paused
        if(!pauseMenu.Paused && throwUnlocked)
        {
            // On initial button press
            if (context.started)
            {
                // Loops through all mushrooms in the mushroomList
                foreach (GameObject m in mushroomList)
                {
                    // Destroys that mushroom

                    Destroy(m);
                }
                // Removes all mushrooms from the list
                shroomCounter.ResetQueue();
                mushroomList.Clear();

                // If it's the player's first time recalling, remove tutorial text
                if(usingTutorial && firstTimeRecalling)
                {
                    firstTimeRecalling = false;
                    tutorialEvent.ShowingRemoveText = false;
                }
            }
        }
    }

    /// <summary>
    /// Removes the most recently thrown shroom
    /// </summary>
    /// <param name="context"></param>
    public void OnRemoveLastShroom(InputAction.CallbackContext context)
    {
        // Checks if the game is paused
        if (!pauseMenu.Paused && throwUnlocked)
        {
            // On initial button press
            if (context.started)
            {
                // Checks if the mushroomCount isn't 0
                if (mushroomList.Count != 0)
                {
                    // Destroys the mushroom at the front of the list
                    mushroomList[mushroomList.Count - 1].GetComponent<MushroomInfo>().ResetCounter();
                    Destroy(mushroomList[mushroomList.Count - 1]);

                    shroomCounter.ShroomIconQueue.Add(mushroomList[mushroomList.Count - 1].GetComponent<MushroomInfo>().ShroomIcon);
                    //
                    mushroomList.RemoveAt(mushroomList.Count - 1);
                }

                // If it's the player's first time recalling, remove tutorial text
                if (usingTutorial && firstTimeRecalling)
                {
                    firstTimeRecalling = false;
                    tutorialEvent.ShowingRemoveText = false;
                }
            }
        }
    }    
    #endregion
}

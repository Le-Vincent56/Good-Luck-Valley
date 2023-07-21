using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
using static UnityEngine.ParticleSystem;
using UnityEngine.UI;

public enum ThrowState
{
    NotThrowing,
    Throwing
}

public class MushroomManager : MonoBehaviour, IData
{
    #region REFERENCES
    [SerializeField] private MushroomScriptableObj mushroomEvent;
    [SerializeField] private DisableScriptableObj disableEvent;
    [SerializeField] private CutsceneScriptableObj cutsceneEvent;
    [SerializeField] private PauseScriptableObj pauseEvent;
    [SerializeField] private LoadLevelScriptableObj loadLevelEvent;
    [SerializeField] private MovementScriptableObj movementEvent;
    private GameObject player;
    [SerializeField] private Rigidbody2D playerRB;             // The player's rigidbody used for spawning mushrooms
    private PlayerMovement playerMove;                         // PlayerMovement checks which direction player is facing
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
    private ShroomCounter shroomCounter;
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
    [SerializeField] private bool throwUnlocked = false;
    [SerializeField] int throwMultiplier;
    [SerializeField] Vector3 fixPlayer;
    private ThrowState throwState;
    private bool throwPrepared = false;
    [SerializeField] private float shroomDuration;
    [SerializeField] private bool enableShroomTimers;
    [SerializeField] private bool throwLocked = false;
    [SerializeField] private bool recallLocked = false;
    [SerializeField] private bool usingTutorial = false;
    [SerializeField] private bool firstTimeHittingMax = true;
    [SerializeField] private bool firstTimeRecalling = true;
    private bool throwLineOn;
    #endregion

    #region PROPERTIES
    public List<GameObject> MushroomList { get { return mushroomList; } }
    public bool ThrowUnlocked { get { return throwUnlocked; } set { throwUnlocked = value; } }
    public int MushroomLimit { get { return mushroomLimit; } set { mushroomLimit = value; } }
    public int ThrowMultiplier { get { return throwMultiplier; } set { throwMultiplier = value; } }
    public float ShroomDuration { get { return shroomDuration; } set { shroomDuration = value; } }
    public bool EnableShroomTimers { get { return enableShroomTimers;} set { enableShroomTimers = value; } }
    public ShroomCounter ShroomCounter { get { return shroomCounter; } }
    public bool ThrowLocked { get { return throwLocked; } set { throwLocked = value; } }
    public bool ThrowLineOn { get { return throwLineOn; } set {  throwLineOn = value; } }
    public Dictionary<int, GameObject> ChangeShroomIndexes { get { return changeShroomIndexes; } }
    #endregion

    private void OnEnable()
    {
        mushroomEvent.unlockThrowEvent.AddListener(GetThrow);
        mushroomEvent.endThrowEvent.AddListener(EndThrow);
        mushroomEvent.clearShroomsEvent.AddListener(ClearShrooms);
        pauseEvent.pauseEvent.AddListener(LockThrow);
        pauseEvent.unpauseEvent.AddListener(UnlockThrow);
        disableEvent.lockPlayerEvent.AddListener(LockThrow);
        disableEvent.unlockPlayerEvent.AddListener(UnlockThrow);
        disableEvent.disablePlayerInputEvent.AddListener(LockThrow);
        disableEvent.enablePlayerInputEvent.AddListener(UnlockThrow);
        loadLevelEvent.startLoad.AddListener(LockThrow);
        loadLevelEvent.endLoad.AddListener(UnlockThrow);
        cutsceneEvent.startLotusCutscene.AddListener(LockThrow);
        cutsceneEvent.endLotusCutscene.AddListener(UnlockThrow);
        cutsceneEvent.startLeaveCutscene.AddListener(DeleteThrowLine);
    }

    private void OnDisable()
    {
        mushroomEvent.unlockThrowEvent.RemoveListener(GetThrow);
        mushroomEvent.endThrowEvent.RemoveListener(EndThrow);
        mushroomEvent.clearShroomsEvent.RemoveListener(ClearShrooms);
        pauseEvent.pauseEvent.RemoveListener(LockThrow);
        pauseEvent.unpauseEvent.RemoveListener(UnlockThrow);
        disableEvent.lockPlayerEvent.RemoveListener(LockThrow);
        disableEvent.unlockPlayerEvent.RemoveListener(UnlockThrow);
        disableEvent.disablePlayerInputEvent.RemoveListener(LockThrow);
        disableEvent.enablePlayerInputEvent.RemoveListener(UnlockThrow);
        loadLevelEvent.startLoad.RemoveListener(LockThrow);
        loadLevelEvent.endLoad.RemoveListener(UnlockThrow);
        cutsceneEvent.startLotusCutscene.RemoveListener(LockThrow);
        cutsceneEvent.endLotusCutscene.RemoveListener(UnlockThrow);
    }

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
        throwUI_Script = GameObject.Find("Throw UI").GetComponent<ThrowUI>();

        // Instantiates layer field
        layer = new ContactFilter2D();
        // Allows the layer to use layer masks when filtering
        layer.useLayerMask = true;
        // Sets the layerMask property of layer to the ground layer 
        layer.layerMask = LayerMask.GetMask("Ground");

        // Set Tutorial Event
        if(usingTutorial)
        {
            tutorialEvent = GameObject.Find("TutorialUI").GetComponent<Tutorial>();
        } else
        {
            tutorialEvent = null;
        }

        tilemap = GameObject.Find("foreground");
        shroomCounter.ResetQueue();
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

        if(usingTutorial && firstTimeHittingMax && mushroomList.Count == 3)
        {
            tutorialEvent.ShowingRemoveText = true;
            firstTimeHittingMax = false;
        }

        // Trigger CheckThrow Event
        mushroomEvent.CheckThrow();

        switch (throwState)
        {
            case ThrowState.NotThrowing:
                break;


            case ThrowState.Throwing:
                if (throwLineOn && !throwLocked)
                {
                    throwUI_Script.PlotTrajectory(playerRB.position,
                                                      forceDirection.normalized * throwMultiplier,
                                                      playerMove.IsFacingRight);
                }
                if (!throwLineOn)
                {
                    DeleteThrowLine();
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

    private void DeleteThrowLine()
    {
        throwUI_Script.DeleteLine();
    }

    void CheckShroomDuration()
    {
        foreach (GameObject m in mushroomList) 
        { 
            if (m.GetComponent<Shroom>().DurationTimer <= 0)
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

        if (mushroomLimit == 3)
        {
            if (mushroomList.Count - 1 < mushroomLimit)
            {
                // mushroomList[mushroomList.Count - 1].GetComponent<MushroomInfo>().ShroomIcon = shroomCounter.ShroomIconQueue[0];
                // shroomCounter.ShroomIconQueue.RemoveAt(0);

                Shroom mInfo = mushroomList[mushroomList.Count - 1].GetComponent<Shroom>();

                mInfo.ShroomIcon = GetRightMostShroomIcon();
                shroomCounter.ShroomIconQueue.Remove(mInfo.ShroomIcon);
                mInfo.StartCounter();
            }
        }
    }

    private GameObject GetRightMostShroomIcon()
    {
        float rightMostXPos = int.MinValue;
        GameObject assignedShroomIcon = null;
        foreach (GameObject shroomIcon in shroomCounter.ShroomIconQueue)
        {
            if (shroomIcon.GetComponent<RectTransform>().position.x > rightMostXPos)
            {
                rightMostXPos = shroomIcon.GetComponent<RectTransform>().position.x;
                assignedShroomIcon = shroomIcon;
            }
        }

        return assignedShroomIcon;
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
        if (mushroomList.Count < mushroomLimit)
        {
            DeleteThrowLine();
            ThrowMushroom();
        }
        else if (mushroomList.Count >= mushroomLimit)
        {
            // If not, ThrowMushroom is called and the first shroom thrown is destroyed and removed from mushroomList
            Shroom mInfo = mushroomList[0].GetComponent<Shroom>();
            DeleteThrowLine();

            if (mushroomLimit == 3)
            {
                shroomCounter.ShroomIconQueue.Add(mInfo.ShroomIcon);
                mInfo.ResetCounter();
            }
            
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

            if (mushroomLimit == 3)
            shroomCounter.ShroomIconQueue.Add(mushroomList[shroomIndex].GetComponent<Shroom>().ShroomIcon);
            
            mushroomList[shroomIndex].GetComponent<Shroom>().ResetCounter();
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

    public void OnCheckWallShroom(InputAction.CallbackContext context) 
    {
        // Check if the player is touching a wall
        if (movementEvent.GetIsTouchingWall() && !playerMove.IsGrounded)
        {
            // Player is no longer touching the wall
            movementEvent.SetIsTouchingWall(false);

            // Only call when the button is pressed, not on release as well
            if (context.started)
            {
                if (mushroomList.Count >= mushroomLimit)
                {
                    // If not, ThrowMushroom is called and the first shroom thrown is destroyed and removed from mushroomList
                    Shroom mInfo = mushroomList[0].GetComponent<Shroom>();

                    if (mushroomLimit == 3)
                    {
                        shroomCounter.ShroomIconQueue.Add(mInfo.ShroomIcon);
                        mInfo.ResetCounter();
                    }

                    Destroy(mushroomList[0]);
                    mushroomList.RemoveAt(0);
                }

                // Set the default rotation for left side collision
                float rotation = -90;
                // Set the default difference in position for left side collision
                float differenceX = playerRB.GetComponent<BoxCollider2D>().size.x;
                float differenceY = playerRB.GetComponent<BoxCollider2D>().size.y;

                // Check if the wall is to the right of the player
                if (movementEvent.GetMushroomPosition().x > playerRB.transform.position.x)
                {
                    // Flip rotation and difference
                    rotation *= -1;
                    differenceX *= -1;
                }

                Vector3 shroomPos = new Vector3(playerRB.transform.position.x - (differenceX / 2), playerRB.transform.position.y, 0);
                // Create the shroom that bounces the player
                GameObject shroom = Instantiate(spore, movementEvent.GetMushroomPosition(), Quaternion.identity);

                mushroomList.Add(shroom);
                Vector2 direction = movementEvent.GetMushroomPosition() - playerRB.transform.position;
                //MushroomList[mushroomList.Count - 1].GetComponent<Rigidbody2D>().AddForce(direction.normalized * throwMultiplier, ForceMode2D.Impulse);
                shroom.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

                if (mushroomLimit == 3)
                {
                    if (mushroomList.Count - 1 < mushroomLimit)
                    {
                        // mushroomList[mushroomList.Count - 1].GetComponent<MushroomInfo>().ShroomIcon = shroomCounter.ShroomIconQueue[0];
                        // shroomCounter.ShroomIconQueue.RemoveAt(0);

                        Shroom mInfo = mushroomList[mushroomList.Count - 1].GetComponent<Shroom>();

                        mInfo.ShroomIcon = GetRightMostShroomIcon();
                        shroomCounter.ShroomIconQueue.Remove(mInfo.ShroomIcon);
                        mInfo.StartCounter();
                    }
                }

                // Set the shroom to not be an anari shroom so certain things wont happen to it
                shroom.GetComponent<Shroom>().NonAnariShroom = true;

                // Rotate the mushroom using the given rotation
                shroom.transform.Rotate(new Vector3(0, 0, rotation));

                // Check if the rotation is greater than 0 (right side collision)
                if (rotation >= 0)
                {
                    // Flip the rotation
                    shroom.GetComponent<Shroom>().FlipRotation = true;
                }
                // Otherwise dont flip the rotation
                else
                {
                    // Flip the rotation
                    shroom.GetComponent<Shroom>().FlipRotation = false;
                }
            }
        }
    }
    
    public void OnFire(InputAction.CallbackContext context)
    {
        if(throwUnlocked && !throwLocked)
        {
            // If we want the same button for fire and aim - aim on press, fire on release
            if (context.started)
            {
                if (canThrow)
                {
                    throwing = mushroomEvent.GetThrowing();

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
            }

            if (context.canceled)
            {
                // Set animation
                if (!throwing)
                {
                    mushroomEvent.SetThrowing(true);
                    mushroomEvent.SetThrowAnim();
                }

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

                    // Delete the throw line
                    DeleteThrowLine();
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
        if(!recallLocked && throwUnlocked)
        {
            // On initial button press
            if (context.started)
            {
                // Loops through all mushrooms in the mushroomList
                foreach (GameObject m in mushroomList)
                {
                    // Destroys that mushroom
                    if (mushroomLimit == 3)
                    m.GetComponent<Shroom>().ResetCounter();

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
        if (!recallLocked && throwUnlocked)
        {
            // On initial button press
            if (context.started)
            {
                // Checks if the mushroomCount isn't 0
                if (mushroomList.Count != 0)
                {
                    Debug.Log("RemoveLAstshroom");
                    // Destroys the mushroom at the front of the list   
                    if (mushroomLimit == 3)
                    {
                        mushroomList[mushroomList.Count - 1].GetComponent<Shroom>().ResetCounter();
                    }
                    Destroy(mushroomList[mushroomList.Count - 1]);

                    if (mushroomLimit == 3)
                    shroomCounter.ShroomIconQueue.Add(mushroomList[mushroomList.Count - 1].GetComponent<Shroom>().ShroomIcon);

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

    #region EVENT FUNCTIONS
    /// <summary>
    /// Get the mushroom throw ability
    /// </summary>
    public void GetThrow()
    {
        throwUnlocked = true;
        shroomCounter.ResetQueue();
    }

    /// <summary>
    /// Lock mushroom throw
    /// </summary>
    public void LockThrow()
    {
        recallLocked = true;
        throwLocked = true;
    }

    /// <summary>
    /// Unlock mushroom throw
    /// </summary>
    public void UnlockThrow()
    {
        recallLocked = false;
        throwLocked = false;
    }

    /// <summary>
    /// Forcibly end a throw
    /// </summary>
    public void EndThrow()
    {
        if(throwState == ThrowState.Throwing)
        {
            // Set animation
            if (!throwing)
            {
                mushroomEvent.SetThrowing(true);
                mushroomEvent.SetThrowAnim();
            }

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

                // Delete the throw line
                DeleteThrowLine();
            }
        }
    }

    /// <summary>
    /// Clear all shrooms
    /// </summary>
    public void ClearShrooms()
    {
        // Loops through all mushrooms in the mushroomList
        foreach (GameObject m in mushroomList)
        {
            // Destroys that mushroom
            if (mushroomLimit == 3)
                m.GetComponent<Shroom>().ResetCounter();

            Destroy(m);
        }

        // Removes all mushrooms from the list
        shroomCounter.ResetQueue();
        mushroomList.Clear();
    }
    #endregion

    #region DATA HANDLING
    public void LoadData(GameData data)
    {
        // Set whether or not the player has their throw unlocked depending on data
        throwUnlocked = data.throwUnlocked;
        mushroomEvent.SetThrowUnlocked(throwUnlocked);

        // Load throwing data - setting throwing to false for animations
        mushroomEvent.LoadData(data);
    }

    public void SaveData(GameData data)
    {
        data.throwUnlocked = throwUnlocked;
    }
    #endregion
}

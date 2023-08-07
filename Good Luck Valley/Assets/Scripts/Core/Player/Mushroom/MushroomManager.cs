using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using HiveMind.Movement;
using HiveMind.SaveData;
using HiveMind.Events;

namespace HiveMind.Mushroom
{
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
        [SerializeField] private UIScriptableObj UIEvent;
        [SerializeField] private SettingsScriptableObj settingsEvent;
        private GameObject player;
        [SerializeField] private Rigidbody2D playerRB;             // The player's rigidbody used for spawning mushrooms
        private PlayerMovement playerMove;                         // PlayerMovement checks which direction player is facing
        [SerializeField] private Camera cam;
        //[SerializeField] private PlatformsManager environmentManager;
        //[SerializeField] private GameCursor cursor;
        [SerializeField] private GameObject tilemap;
        [SerializeField] private GameObject shroomPoint;
        // private UIManager uiManager;
        [SerializeField] private GameObject spore;
        [SerializeField] private GameObject mushroom;
        [SerializeField] private GameObject testObject;
        // private ShroomCounter shroomCounter;
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
        private bool firstThrow;
        private bool firstBounce;
        private bool firstFull;
        private bool firstWallBounce;
        private bool showQuickBounceMessage;
        [SerializeField] private Vector3 wallShroomPosDifference;
        #endregion

        #region PROPERTIES
        public List<GameObject> MushroomList { get { return mushroomList; } }
        public bool ThrowUnlocked { get { return throwUnlocked; } set { throwUnlocked = value; } }
        public int MushroomLimit { get { return mushroomLimit; } set { mushroomLimit = value; } }
        public int ThrowMultiplier { get { return throwMultiplier; } set { throwMultiplier = value; } }
        public float ShroomDuration { get { return shroomDuration; } set { shroomDuration = value; } }
        public bool EnableShroomTimers { get { return enableShroomTimers; } set { enableShroomTimers = value; } }
        public bool ThrowLocked { get { return throwLocked; } set { throwLocked = value; } }
        public bool ThrowLineOn { get { return throwLineOn; } set { throwLineOn = value; } }
        public Dictionary<int, GameObject> ChangeShroomIndexes { get { return changeShroomIndexes; } }
        #endregion

        private void OnEnable()
        {
            mushroomEvent.unlockThrowEvent.AddListener(GetThrow);
            mushroomEvent.endThrowEvent.AddListener(EndThrow);
            mushroomEvent.clearShroomsEvent.AddListener(ClearShrooms);
            settingsEvent.updateSettings.AddListener(UpdateSettings);
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
            settingsEvent.updateSettings.RemoveListener(UpdateSettings);
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
            //shroomCounter = GameObject.Find("MushroomCountUI").GetComponent<ShroomCounter>();

            // Instantiates layer field
            layer = new ContactFilter2D();
            // Allows the layer to use layer masks when filtering
            layer.useLayerMask = true;
            // Sets the layerMask property of layer to the ground layer 
            layer.layerMask = LayerMask.GetMask("Ground");

            tilemap = GameObject.Find("foreground");
        }

        // Update is called once per frame
        void Update()
        {

            // Direction force is being applied to shroom
            forceDirection = UIEvent.GetCursorPosition() - playerRB.transform.position;
            //forceDirection = cam.ScreenToWorldPoint(new Vector2(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y)) - playerRB.transform.position;
            //forceDirection = cam.ScreenToWorldPoint(Input.mousePosition) - playerRB.transform.position;

            // Trigger CheckThrow Event
            mushroomEvent.CheckThrow();

            // Checks what state the playe is in relating to throwing
            switch (throwState)
            {
                // If they are not throwing, do nothing
                case ThrowState.NotThrowing:
                    break;

                // If they are throwing then show the throw line
                case ThrowState.Throwing:

                    // Check if throw isn't locked and the throw line is enabled
                    if (throwLineOn && !throwLocked)
                    {
                        // Plots the trajectory
                        UIEvent.PlotTrajectory(playerRB.position,
                                                          forceDirection.normalized * throwMultiplier,
                                                          playerMove.IsFacingRight);
                    }
                    // If the throw line isnt on then delete it in case it was ever enabled
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

            if (mushroomList.Count == mushroomLimit)
            {
                // Set if it's the first time the player has hit 3 mushrooms
                if (!mushroomEvent.GetFirstFull())
                {
                    // Set first full
                    firstFull = true;

                    // Show the tutorial message about removing mushrooms
                    mushroomEvent.ShowMaxMessage();
                }
            }


            CheckIfCanThrow();
            CheckShroomDuration();
        }

        // Deletes the throw line
        private void DeleteThrowLine()
        {
            UIEvent.DeleteLine();
        }

        /// <summary>
        /// Checks the shroom duration for all shrooms in the list and puts them in the remove shroom icon stack
        /// </summary>
        void CheckShroomDuration()
        {
            // Loops through the mushroom list
            foreach (GameObject m in mushroomList)
            {
                // Check if the duration timer is less than or equal to 0
                if (m.GetComponent<Shroom>().DurationTimer <= 0)
                {
                    // If so, pushes the shroom to the remove indexes stack
                    removeShroomIndexes.Push(mushroomList.IndexOf(m));
                }
            }

            // Checks if the remove shroom indexes count is greater then 0
            if (removeShroomIndexes.Count > 0)
            {
                // Removes those shrooms
                RemoveShrooms();
            }
        }

        /// <summary>
        /// Creates a shroom, adds it to mushroomList, launches shroom in the appropriate Direction
        /// </summary>
        /// <param name="type"> Which type of mushroom is being thrown</param>
        void ThrowMushroom()
        {
            // Adds a new spore to the mushroom list
            mushroomList.Add(Instantiate(spore, playerRB.position, Quaternion.identity));

            // Adds a force to the shroom at the end of the mushroom list and freezes its rotation
            mushroomList[mushroomList.Count - 1].GetComponent<Rigidbody2D>().AddForce(forceDirection.normalized * throwMultiplier, ForceMode2D.Impulse);
            mushroomList[mushroomList.Count - 1].GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            mushroomList[mushroomList.Count - 1].GetComponent<Shroom>().Rotation = -1;

            // Check if infinite shrooms is off
            if (mushroomLimit == 3)
            {
                // Check if the mushroom count is less than the mushroom limit
                if (mushroomList.Count - 1 < mushroomLimit)
                {
                    // Get a reference to the mushroom info of the mushroom at the end of the list
                    Shroom mInfo = mushroomList[mushroomList.Count - 1].GetComponent<Shroom>();

                    // Get the rightmost shroom icon and assign it to the mushroom's shroom icon
                    mInfo.ShroomIcon = GetRightMostShroomIcon();

                    // Remove the shroom icon from the icon queue and start the counter
                    UIEvent.RemoveFromShroomCounter(mInfo.ShroomIcon);
                    mInfo.StartCounter();
                }
            }
        }

        /// <summary>
        /// Gets the active shroom icon most to the right
        /// </summary>
        /// <returns> The rightmost shroom icon</returns>
        private GameObject GetRightMostShroomIcon()
        {
            // Get an initial x pos value using MinValue
            float rightMostXPos = int.MinValue;

            // Initializes a null game object for returning the rightmost shroom
            GameObject assignedShroomIcon = null;

            // Loops through the shroom icon queue
            foreach (GameObject shroomIcon in UIEvent.GetShroomCounter())
            {
                // Checks if the current shroom icon's x position is greater than the current right most x pos
                if (shroomIcon.GetComponent<RectTransform>().position.x > rightMostXPos)
                {
                    // If so, sets the right most x pos to the current shroom icon's x pos
                    rightMostXPos = shroomIcon.GetComponent<RectTransform>().position.x;
                    // Updates the assigned shroom game object
                    assignedShroomIcon = shroomIcon;
                }
            }

            // Returns the right most shroom icon
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
                // Delete the throw line and throw the mushroom
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
                    // Update counter
                    UIEvent.AddToShroomCounter(mInfo.ShroomIcon);
                    mInfo.ResetCounter();
                }

                // Destroy and remove the first mushroom 
                Destroy(mushroomList[0]);
                mushroomList.RemoveAt(0);

                // Throw the mushroom
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
        /// Remove all shrooms that have been queued to be removed
        /// </summary>
        private void RemoveShrooms()
        {
            // The number of mushrooms that need to be removed
            int indexCount = removeShroomIndexes.Count;

            // Loops through the mushroom indexes stack 
            for (int i = 0; i < indexCount; i++)
            {
                // Get the shroom index from the stack of indexes
                int shroomIndex = removeShroomIndexes.Pop();

                // Check if infinite shrooms is off
                if (mushroomLimit == 3)
                {
                    // Add this mushrooms shroom icon back to the icon queue
                    UIEvent.AddToShroomCounter(mushroomList[shroomIndex].GetComponent<Shroom>().ShroomIcon);
                }

                // Reset the shroom counter for this mushroom
                mushroomList[shroomIndex].GetComponent<Shroom>().ResetCounter();

                // Destroy and remove the mushroom
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

        /// <summary>
        /// Executes the wall jump feature
        /// </summary>
        /// <param name="context">Used to determine whether the button is pressed or released</param>
        public void OnCheckWallShroom(InputAction.CallbackContext context)
        {
            // Check if the player is touching a wall
            if (movementEvent.GetIsTouchingWall() && !movementEvent.GetIsGrounded())
            {
                // Only call when the button is pressed, not on release as well
                if (context.started && movementEvent.GetMushroomPosition().x != -1000)
                {
                    if (mushroomEvent.GetFirstWallBounce())
                    {
                        firstWallBounce = false;
                        mushroomEvent.SetFirstWallBounce(false);
                        mushroomEvent.HideWallBounceMessage();
                    }
                    // Player is no longer touching the wall
                    movementEvent.SetIsTouchingWall(false);

                    if (mushroomList.Count >= mushroomLimit)
                    {
                        // If not, ThrowMushroom is called and the first shroom thrown is destroyed and removed from mushroomList
                        Shroom mInfo = mushroomList[0].GetComponent<Shroom>();

                        if (mushroomLimit == 3)
                        {
                            UIEvent.AddToShroomCounter(mInfo.ShroomIcon);
                            mInfo.ResetCounter();
                        }

                        Destroy(mushroomList[0]);
                        mushroomList.RemoveAt(0);
                    }

                    // Set the default rotation for left side collision
                    float rotation = -90;
                    Vector3 difference = wallShroomPosDifference;

                    // Check if the wall is to the right of the player
                    if (movementEvent.GetMushroomPosition().x > playerRB.transform.position.x)
                    {
                        // Flip rotation and difference
                        rotation *= -1;
                        difference *= -1;
                    }
                    // Create the shroom that bounces the player
                    GameObject shroom = Instantiate(spore, movementEvent.GetMushroomPosition() + difference, Quaternion.identity);
                    shroom.GetComponent<Shroom>().Rotation = rotation;


                    mushroomList.Add(shroom);
                    shroom.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

                    if (mushroomLimit == 3)
                    {
                        if (mushroomList.Count - 1 < mushroomLimit)
                        {
                            // mushroomList[mushroomList.Count - 1].GetComponent<MushroomInfo>().ShroomIcon = shroomCounter.ShroomIconQueue[0];
                            // shroomCounter.ShroomIconQueue.RemoveAt(0);

                            Shroom mInfo = mushroomList[mushroomList.Count - 1].GetComponent<Shroom>();

                            mInfo.ShroomIcon = GetRightMostShroomIcon();
                            UIEvent.RemoveFromShroomCounter(mInfo.ShroomIcon);
                            mInfo.StartCounter();
                        }
                    }

                    // Rotate the mushroom using the given rotation
                    //shroom.transform.Rotate(new Vector3(0, 0, rotation));

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

                    movementEvent.SetMushroomPosition(new Vector3(-1000, -1000, -1000));
                }
            }
        }

        /// <summary>
        /// Throws the mushroom
        /// </summary>
        /// <param name="context">Used to determine whether the button was pressed or released</param>
        public void OnFire(InputAction.CallbackContext context)
        {
            if (throwUnlocked && !throwLocked)
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
                        // Remove tutorial message
                        if (mushroomEvent.GetFirstThrow())
                        {
                            firstThrow = false;
                            mushroomEvent.HideThrowMessage();
                            mushroomEvent.ShowBounceMessage();
                        }

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
        /// Executes the quick bounce feature, placing a shroom directly below the player
        /// </summary>
        /// <param name="context">Used to determine whether the button was pressed or released</param>
        public void OnQuickBounce(InputAction.CallbackContext context)
        {
            // Check if the button was pressed, so this only happens on initial press not on release
            // Also check if we have unlocked the throw and are grounded
            if (context.started && !throwLocked && throwUnlocked && movementEvent.GetIsGrounded())
            {
                // Check if we are within the bounce of the quick bounce tutorial message
                if (mushroomEvent.GetTouchingQuickBounceMessage())
                {
                    // If so, hide the quick bounce message
                    mushroomEvent.HideQuickBounceMessage();
                    // Set show quick bounce to false, so that we know not to show it in the future
                    showQuickBounceMessage = false;
                }

                // Check if the current count of mushrooms is greater than or equal to the liit
                if (mushroomList.Count >= mushroomLimit)
                {
                    // If not, get the mushroom info of the first mushroom in the list
                    Shroom mInfo = mushroomList[0].GetComponent<Shroom>();

                    // Check if the limit is 3 (infinite shrooms is off)
                    if (mushroomLimit == 3)
                    {
                        UIEvent.AddToShroomCounter(mInfo.ShroomIcon);

                        // Reset the shroom icon
                        mInfo.ResetCounter();
                    }

                    // Destroy the mushroom
                    Destroy(mushroomList[0]);
                    // Remove the mushroom from the list
                    mushroomList.RemoveAt(0);
                }

                // Small difference for spawning the mushroom below the player
                Vector3 difference = new Vector3(0, playerRB.GetComponent<BoxCollider2D>().size.y, 0);

                // Create the shroom that bounces the player
                GameObject shroom = Instantiate(spore, playerRB.transform.position - difference, Quaternion.identity);

                // Freeze its rotation
                shroom.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

                // Add the mushroom to the list
                mushroomList.Add(shroom);

                // Check if infinite shrooms is off
                if (mushroomLimit == 3)
                {
                    // Check if the mushroom count is less than the mushroom limit
                    if (mushroomList.Count - 1 < mushroomLimit)
                    {
                        // Get the mushroom info component for the newly created mushroom
                        Shroom mInfo = shroom.GetComponent<Shroom>();

                        // Set the icon for this new shroom to be the right most shroom
                        mInfo.ShroomIcon = GetRightMostShroomIcon();

                        // Remove the shroom icon from the icon queue
                        UIEvent.RemoveFromShroomCounter(mInfo.ShroomIcon);

                        // Start the counter
                        mInfo.StartCounter();
                    }
                }

                // Flip the rotation
                shroom.GetComponent<Shroom>().FlipRotation = false;
            }

        }

        /// <summary>
        /// Recalls all thrown shrooms
        /// </summary>
        /// <param name="context"></param>
        public void OnRecallShrooms(InputAction.CallbackContext context)
        {
            // Checks if the game is paused
            if (!recallLocked && throwUnlocked)
            {
                // On initial button press
                if (context.started)
                {
                    // Hide the mushroom max tutorial message if showing
                    if (mushroomEvent.GetShowingMaxMessage())
                    {
                        mushroomEvent.HideMaxMessage();
                    }

                    // Loops through all mushrooms in the mushroomList
                    foreach (GameObject m in mushroomList)
                    {
                        // Destroys that mushroom
                        if (mushroomLimit == 3)
                            m.GetComponent<Shroom>().ResetCounter();

                        Destroy(m);
                    }
                    // Removes all mushrooms from the list
                    UIEvent.ResetCounterQueue();
                    mushroomList.Clear();
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
                        // Hide the mushroom max tutorial message if showing
                        if (mushroomEvent.GetShowingMaxMessage())
                        {
                            mushroomEvent.HideMaxMessage();
                        }

                        // Destroys the mushroom at the front of the list   
                        if (mushroomLimit == 3)
                        {
                            mushroomList[mushroomList.Count - 1].GetComponent<Shroom>().ResetCounter();
                        }
                        Destroy(mushroomList[mushroomList.Count - 1]);

                        if (mushroomLimit == 3)
                        {
                            UIEvent.AddToShroomCounter(mushroomList[mushroomList.Count - 1].GetComponent<Shroom>().ShroomIcon);
                        }

                        mushroomList.RemoveAt(mushroomList.Count - 1);
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
            UIEvent.ResetCounterQueue();
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
            if (throwState == ThrowState.Throwing)
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
            UIEvent.ResetCounterQueue();
            mushroomList.Clear();
        }

        public void UpdateSettings()
        {
            throwMultiplier = settingsEvent.GetThrowMultiplier();
            mushroomLimit = settingsEvent.GetShroomLimit();
            enableShroomTimers = settingsEvent.GetShroomTimersActive();
            throwLineOn = settingsEvent.GetThrowLineActive();
        }
        #endregion

        #region DATA HANDLING
        public void LoadData(GameData data)
        {
            // Set whether or not the player has their throw unlocked depending on data
            firstThrow = data.firstThrow;
            firstBounce = data.firstBounce;
            firstFull = data.firstFull;
            firstWallBounce = data.firstWallBounce;
            throwUnlocked = data.throwUnlocked;
            showQuickBounceMessage = data.showQuickBounce;
            mushroomEvent.SetFirstThrow(firstThrow);
            mushroomEvent.SetFirstBounce(firstBounce);
            mushroomEvent.SetFirstFull(firstFull);
            mushroomEvent.SetThrowUnlocked(throwUnlocked);
            mushroomEvent.SetShowingQuickBounceMessage(showQuickBounceMessage);
            mushroomEvent.SetFirstWallBounce(firstWallBounce);
        }

        public void SaveData(GameData data)
        {
            data.firstThrow = firstThrow;
            data.firstBounce = firstBounce;
            data.firstFull = firstFull;
            data.throwUnlocked = throwUnlocked;
            data.showQuickBounce = showQuickBounceMessage;
            data.firstWallBounce = firstWallBounce;
        }
        #endregion
    }
}

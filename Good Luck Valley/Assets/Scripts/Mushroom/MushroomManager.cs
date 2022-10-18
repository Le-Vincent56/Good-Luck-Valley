using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public enum ThrowState
{
    NotThrowing,
    Throwing
}

public class MushroomManager : MonoBehaviour
{
    // MushroomManager PREFABS
    [SerializeField] GameObject organicShroom;
    [SerializeField] int throwMultiplier;
    [SerializeField] string stuckSurfaceTag;  // Tag of object shroom will stick to
    [SerializeField] WeightedPlatform weightedPlatformScript;
    Vector2 forceDirection;
    Camera cam;

    private List<GameObject> mushroomList;    // List of currently spawned shrooms

    private const int mushroomLimit = 3;      // Constant for max amount of shrooms

    [SerializeField] private int offset;      // Offset for spawning shrooms outside of player hitbox
                                              
    private int mushroomCount;                // How many shrooms are currently spawned in
                                              
    private Rigidbody2D playerRB;             // The player's rigidbody used for spawning mushrooms
                                              
    private PlayerMovement playerMove;        // PlayerMovement checks which direction player is facing

    public GameObject throwUI_Script;

    private ThrowState throwState;

    private ContactFilter2D layer;         // A contact filter to filter out ground layers

    [SerializeField] List<BoxCollider2D> platforms; // A List of all platforms in the levels

    public List<GameObject> MushroomList { get { return mushroomList; } }

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        playerRB = GetComponent<Rigidbody2D>();
        playerMove = GetComponent<PlayerMovement>();
        mushroomList = new List<GameObject>();


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

        // Update mouse position

        // Direction force is being applied to shroom
        forceDirection = cam.ScreenToWorldPoint(new Vector2(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y)) - playerRB.transform.position;
        // forceDirection = cam.ScreenToWorldPoint(Input.mousePosition) - playerRB.transform.position;
        //Debug.Log(forceDirection);
        //Debug.Log(playerRB.position);

        switch(throwState)
        {
            case ThrowState.NotThrowing:
                break;


            case ThrowState.Throwing:
                if (playerMove.IsFacingRight)
                {
                    throwUI_Script.GetComponent<ThrowUI>().PlotTrajectory(playerRB.position, forceDirection.normalized * throwMultiplier, offset, playerMove.IsFacingRight);
                }
                else
                {
                    throwUI_Script.GetComponent<ThrowUI>().PlotTrajectory(playerRB.position, forceDirection.normalized * throwMultiplier, offset, playerMove.IsFacingRight);
                }
                break;                
        }

        if (MushroomList.Count > 0)
        {
            StickShrooms();
        }
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
            ThrowMushroom();
        }
        else if (mushroomCount >= mushroomLimit)
        {
            // If not, ThrowMushroom is called and the first shroom thrown is destroyed and removed from mushroomList
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
            // loops for each platform's boxcollider in the platforms list
            foreach (BoxCollider2D p in platforms)
            {
                // checks if the mushroom is touching the platform and hasn't rotated
                if (m.GetComponent<CircleCollider2D>().IsTouching(p) &&
                    !m.GetComponent<MushroomInfo>().hasRotated)
                {
                    // If so, calls rotate shroom method to rotate and freeze the shroom properly
                    RotateAndFreezeShroom(p, m);

                    if (GameObject.FindGameObjectWithTag("weightablePlatform"))
                    {
                        weightedPlatformScript.CheckWeight(mushroomCount);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Rotates and freezes a shroom to match the orientation of the platform it is colliding with
    /// </summary>
    /// <param name="platform"> The platform colliding with the shroom</param>
    /// <param name="mushroom"> The mushroom colliding with the platform</param>
    private void RotateAndFreezeShroom(BoxCollider2D platform, GameObject mushroom)
    {
        // Loops 4 times to check the top, left, right, and bottom of the mushroom's circle
        //  collider 2d for which side is hitting the platform
        for(int i = 0; i < 4; i++)
        {
            // Determines the angle of the point to be checked by multipling the current iteration
            //  by 90 degrees then subtracting 90 degrees because it worked (idk why)
            float circumferenceAngle = i * 90 - 90;

            // Checks if the shroom has rotated (in order to stop the loop sooner if the proper side
            //  has already been determined) and checks if a point on the circle collider's
            //  circumference is within the bounds of the platform collider by calling the
            //  GetPointOnCircumference method (refer below)
            if (mushroom.GetComponent<MushroomInfo>().hasRotated == false && 
                platform.bounds.Contains(GetPointOnCircumference(mushroom, circumferenceAngle)))
            {
                // If so, rotates the mushroom 90 + angle for circumference point in degrees around the Z axis
                //  by calling unity's Rotate method
                mushroom.transform.Rotate(new Vector3(0f, 0f, circumferenceAngle + 90));

                // Also freezes all rotations and movements for the mushroom
                mushroom.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

                // Finally, sets the mushroom's 'HasRotated' method to true so the loop can stop sooner
                //  and the shroom will no longer attempt to be checked for rotations
                mushroom.GetComponent<MushroomInfo>().hasRotated = true;
            }
        }
    }

    /// <summary>
    /// Uses formula for finding point on circumference of a circle to determine a point on the 
    ///     circumference of the mushroom's circle collider (x = r * cos(angle) + x1) and (y = r * sin(angle) + y1)
    /// </summary>
    /// <param name="mushroom"> The mushroom being checked</param>
    /// <param name="angle"> The angle from the center of the shroom to the point in degrees</param>
    /// <returns> A point in Vector2 form on the circumference of the mushroom's Circle collider 2d</returns>
    private Vector2 GetPointOnCircumference(GameObject mushroom, float angle)
    {
        // Rounds because vector2's round when they are created and it was causing issues,
        //  rounding everything beforehand fixed it
        float x = Mathf.Round(mushroom.GetComponent<CircleCollider2D>().radius) * 
                  Mathf.Round(Mathf.Cos(angle)) + 
                  Mathf.Round(mushroom.GetComponent<CircleCollider2D>().bounds.center.x);

        float y = Mathf.Round(mushroom.GetComponent<CircleCollider2D>().radius) * 
                  Mathf.Round(Mathf.Sin(angle)) + 
                  Mathf.Round(mushroom.GetComponent<CircleCollider2D>().bounds.center.y);
        // Returns the newly calculated vector
        return new Vector2(x, y);
    }

    #region INPUT HANDLER
    public void OnTriggerAim(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                switch (throwState)
                {
                    case ThrowState.NotThrowing:
                        throwState = ThrowState.Throwing;
                        break;
    
                    case ThrowState.Throwing:
                        throwState = ThrowState.NotThrowing;
                        break;
                }
            }
        }
          
    
        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                CheckShroomCount();
                throwState = ThrowState.NotThrowing;
            }
        }
    
        public void OnAim(InputAction.CallbackContext context)
        {
            // Implement looking
        }
        #endregion
    }

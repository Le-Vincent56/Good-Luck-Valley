using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        forceDirection = cam.ScreenToWorldPoint(Input.mousePosition) - playerRB.transform.position;
        //Debug.Log(forceDirection);
        //Debug.Log(playerRB.position);

        switch(throwState)
        {
            case ThrowState.NotThrowing:

                // If Q is pressed, line trajectory is drawn
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    throwState = ThrowState.Throwing;
                }
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
                if(Input.GetKeyDown(KeyCode.Mouse0))
                {
                    CheckShroomCount();
                    throwState = ThrowState.NotThrowing;
                }
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    throwState = ThrowState.NotThrowing;
                }
                break;                
        }

        StickShrooms();
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
    /// If so it freezes the shroom
    /// </summary>
    private void StickShrooms()
    {
        // Loops for each object in the mushroomlist
        foreach (GameObject obj in mushroomList)
        {
            // An empty list to use in the OverlapCollider method
            List<Collider2D> list = new List<Collider2D>();
            
            // Calls OverlapCollider method on the shroom's BoxCollider2D component
            // OverlapCollider checks if any Collider in the scene is overlapping with
            //  collider it is cecking against (the mushroom's BoxCollider2D) and
            //  returns the number of colliders doing so as an int
            if (obj.GetComponent<BoxCollider2D>().OverlapCollider(layer, list) > 0)
            {
                // If the method returns a number greater than 0 the mushroom is frozen in that position
                obj.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            }
        }
    }




}

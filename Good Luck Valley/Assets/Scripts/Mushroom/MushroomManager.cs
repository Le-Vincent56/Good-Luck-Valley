using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomManager : MonoBehaviour
{
    // MushroomManager PREFABS
    [SerializeField] GameObject organicShroom;
    [SerializeField] int throwMultiplier;
    Vector2 forceDirection;
    Camera cam;

    //public float power = 10f;            // Power of the force applied to shroom

    private List<GameObject> mushroomList; // List of currently spawned shrooms

    private const int mushroomLimit = 3;   // Constant for max amount of shrooms

    [SerializeField] private int offset;   // Offset for spawning shrooms outside of player hitbox

    private int mushroomCount;             // How many shrooms are currently spawned in

    private Rigidbody2D playerRB;          // The player's rigidbody used for spawning mushrooms

    private Rigidbody2D mushroomRigidbody; // Mushrooms rigidbody used for adding force

    private PlayerMovement playerMove;     // PlayerMovement checks which direction player is facing
                                           

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        playerRB = GetComponent<Rigidbody2D>();
        mushroomRigidbody = organicShroom.GetComponent<Rigidbody2D>();
        playerMove = GetComponent<PlayerMovement>();
        mushroomList = new List<GameObject>();        
    }

    // Update is called once per frame
    void Update()
    {
        // Updates mushroom count               
        mushroomCount = mushroomList.Count;

        // Update mouse position
        forceDirection = cam.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log(forceDirection);


        // If E is pressed, CheckShroomCount is called
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            CheckShroomCount(); // Checks than throws            
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




}

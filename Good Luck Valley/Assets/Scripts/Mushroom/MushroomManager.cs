using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomManager : MonoBehaviour
{
    // Prefabs for shroom types
    [SerializeField] GameObject organicShroom;  
    [SerializeField] GameObject metallicShroom;

    [SerializeField] Vector2 velocity;   // Velocity for launching shroom
    private List<GameObject> mushrooms;  // List of currently spawned shrooms
    private const int mushroomLimit = 3; // Constant for max amount of shrooms
    [SerializeField] private int offset; // Offset for spawning shrooms outside of player hitbox
    private int mushroomCount;           // How many shrooms are currently spawned in
    private Rigidbody2D playerRB;        // The player's rigidbody used for spawning mushrooms
    private PlayerMovement playerMove;   // PlayerMovement object for checking which direction
                                         //   player is facing
    private Mushroom mushroom;           // Mushroom object for calling AddForce on mushrooms
    private int type;                    // Int to determine type of shroom to spawn and thron
                                         //     0 is organic ; 1 is metallic

    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        playerMove = GetComponent<PlayerMovement>();
        mushrooms = new List<GameObject>();
        mushroom = GetComponent<Mushroom>();
    }

    // Update is called once per frame
    void Update()
    {
        // Updates mushroom count
        mushroomCount = mushrooms.Count;

        // Checks to see which type of shroom is being thrown
        // If E is pressed, type is set to organic shroom (0) and CheckShroomCount is called,
        //      causing an organic shroom to be thrown
        if (Input.GetKeyDown(KeyCode.E))
        {
            type = 0;
            CheckShroomCount();
        }
        // If Q is pressed, type is set to metallic shroom (1) and CheckShroomCount is called,
        //      causing a metallic shroom to be thrown
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            type = 1;
            CheckShroomCount();
        }

    }

    /// <summary>
    /// Creates a new mushroom, adds it to the list, and launches it in the 
    ///     appropriate direction by calling Mushroom's AddForce method
    /// </summary>
    /// <param name="type"> Which type of mushroom is being thrown</param>
    void ThrowMushrom()
    {
        switch (type)
        {
            case 0:
                if (playerMove.IsFacingRight)
                {
                    // Adds a new mushroom to the list created using the organic shroom prefab,
                    //  player position + offset, and default rotation
                    mushrooms.Add(Instantiate(organicShroom, 
                                              new Vector2(
                                                   playerRB.position.x + offset, 
                                                   playerRB.position.y), 
                                              Quaternion.identity));
                    // Calls AddForce method to launch shroom with a right direction
                    mushroom.AddForce(mushrooms[mushroomCount], 1);
                }
                else
                {
                    // Adds a new mushroom to the list created using the organic shroom prefab,
                    //  player position + offset, and default rotation
                    mushrooms.Add(Instantiate(organicShroom, 
                                              new Vector2(playerRB.position.x - offset, 
                                                          playerRB.position.y),
                                              Quaternion.identity));
                    // Calls AddForce method to launch shroom with a left direction
                    mushroom.AddForce(mushrooms[mushroomCount], -1);
                }
                break;

            case 1:
                if (playerMove.IsFacingRight)
                {
                    // Adds a new mushroom to the list created using the organic shroom prefab,
                    //  player position + offset, and default rotation
                    mushrooms.Add(Instantiate(metallicShroom,
                                              new Vector2(playerRB.position.x + offset, 
                                                          playerRB.position.y), 
                                              Quaternion.identity));
                    // Calls AddForce method to launch shroom with a left direction
                    mushroom.AddForce(mushrooms[mushroomCount], 1);
                }
                else
                {
                    // Adds a new mushroom to the list created using the organic shroom prefab,
                    //  player position + offset, and default rotation
                    mushrooms.Add(Instantiate(metallicShroom, 
                                              new Vector2(playerRB.position.x - offset, 
                                                          playerRB.position.y), 
                                              Quaternion.identity));
                    // Calls AddForce method to launch shroom with a right direction
                    mushroom.AddForce(mushrooms[mushroomCount], -1);
                }
                break;
        }
    }

    /// <summary>
    /// Checks if the mushroom being thrown will cause there to be more than 3 mushrooms
    ///     spawned; if so, deletes the first shroom thrown and calls the ThrowShroom method
    ///              if not, calls the ThrowShroom method
    /// </summary>
    /// <param name="type"> Which type of mushroom is being thrown </param>
    void CheckShroomCount()
    {
        // Checks if the current number of spawned mushrooms is lower than the max amount
        if (mushroomCount < mushroomLimit)
        {
            // If so, ThrowMushroom is called
            ThrowMushrom();
        }
        else if (mushroomCount >= mushroomLimit)
        {
            // If not, ThrowMushroom is called and then the first shroom thrown is
            //      destroyed and removed from the list of spawned shrooms
            ThrowMushrom();
            Destroy(mushrooms[0]);
            mushrooms.RemoveAt(0);
        }
    }
}

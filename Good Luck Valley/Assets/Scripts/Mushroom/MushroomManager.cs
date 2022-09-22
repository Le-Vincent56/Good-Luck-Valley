using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomManager : MonoBehaviour
{
    [SerializeField] GameObject organicShroom;
    [SerializeField] GameObject metallicShroom;
    [SerializeField] Vector2 velocity;
    private List<GameObject> mushrooms;
    private const int mushroomLimit = 3;
    private int mushroomCount;
    private Rigidbody2D playerRB;     
    private PlayerMovement playerMove;
    private Mushroom mushroom;
    Transform firePoint;
    Vector2 lookDirection;
    float lookAngle;
    Vector2 mousePos;

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
        mousePos = new Vector2(Input.mousePosition.x % 5, Input.mousePosition.y % -5);
        
        mushroomCount = mushrooms.Count;
        int type;
        if (Input.GetKeyDown(KeyCode.E))
        {
            type = 0;
            DetermineTypeAndThrow(type);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            type = 1;
            DetermineTypeAndThrow(type);
        }

    }


    void ThrowMushrom(int type)
    {
        switch (type)
        {
            case 0:
                if (playerMove.IsFacingRight)
                {
                    //Vector2 pos = new Vector3(playerRB.position.x + 4, playerRB.position.y);
                    mushrooms.Add(Instantiate(organicShroom, new Vector2(playerRB.position.x, playerRB.position.y), Quaternion.identity));

                    mushroom.AddForce(mushrooms[mushroomCount], mousePos);
                }
                else
                {
                    Vector2 pos = new Vector2((playerRB.position.x - 4), playerRB.position.y);
                    mushrooms.Add(Instantiate(organicShroom, pos, Quaternion.identity));
                }
                break;

            case 1:
                if (playerMove.IsFacingRight)
                {
                    Vector2 pos = new Vector2(playerRB.position.x + 4, playerRB.position.y);
                    mushrooms.Add(Instantiate(metallicShroom, pos, Quaternion.identity));
                }
                else
                {
                    Vector2 pos = new Vector2((playerRB.position.x - 4), playerRB.position.y);
                    mushrooms.Add(Instantiate(metallicShroom, pos, Quaternion.identity));
                }
                break;
        }
    }


    void DetermineTypeAndThrow(int type)
    {
        if (mushroomCount < mushroomLimit)
        {
            ThrowMushrom(type);
        }
        else if (mushroomCount >= mushroomLimit)
        {
            ThrowMushrom(type);
            Destroy(mushrooms[0]);
            mushrooms.RemoveAt(0);
        }
    }
}

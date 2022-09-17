using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomManager : MonoBehaviour
{
    [SerializeField] GameObject organicShroom;
    [SerializeField] GameObject metallicShroom;
    private Queue<GameObject> mushrooms;
    private const int mushroomLimit = 3;
    private int mushroomCount;
    private PlayerData playerData;
    private Rigidbody2D playerRB;     
    private PlayerMovement playerMove;
    private ThrowShroom throw_script;


    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        playerMove = GetComponent<PlayerMovement>();
        playerData = playerMove.Data;
        mushrooms = new Queue<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
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
                    Vector3 pos = new Vector3(playerRB.position.x + 4, playerRB.position.y, 0f);
                    mushrooms.Enqueue(Instantiate(organicShroom, pos, Quaternion.identity));
                }
                else
                {
                    Vector3 pos = new Vector3((playerRB.position.x - 4), playerRB.position.y, 0f);
                    mushrooms.Enqueue(Instantiate(organicShroom, pos, Quaternion.identity));
                }
                break;

            case 1:
                if (playerMove.IsFacingRight)
                {
                    Vector3 pos = new Vector3(playerRB.position.x + 4, playerRB.position.y, 0f);
                    mushrooms.Enqueue(Instantiate(metallicShroom, pos, Quaternion.identity));
                }
                else
                {
                    Vector3 pos = new Vector3((playerRB.position.x - 4), playerRB.position.y, 0f);
                    mushrooms.Enqueue(Instantiate(metallicShroom, pos, Quaternion.identity));
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
            Destroy(mushrooms.Peek());
            mushrooms.Dequeue();
        }
    }
}

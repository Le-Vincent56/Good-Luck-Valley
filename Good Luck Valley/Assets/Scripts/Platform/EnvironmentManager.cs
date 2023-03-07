using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    [Header("Platform Lists")]
    public List<GameObject> weightedPlatforms;
    public List<GameObject> collidablePlatforms;
    public List<GameObject> nonCollidablePlatforms;
    public List<GameObject> decomposableTiles;

    // Start is called before the first frame update
    void Start()
    {
        // Get all environment objects and add to appropriate list
        weightedPlatforms.AddRange(GameObject.FindGameObjectsWithTag("Weighted"));
        collidablePlatforms.AddRange(GameObject.FindGameObjectsWithTag("Collidable"));
        nonCollidablePlatforms.AddRange(GameObject.FindGameObjectsWithTag("Non-Collidable"));
        decomposableTiles.AddRange(GameObject.FindGameObjectsWithTag("Decomposable"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    [SerializeField] List<GameObject> weightedPlatforms;
    [SerializeField] MushroomManager mushroomManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(GameObject m in mushroomManager.MushroomList)
        {
            foreach (GameObject wp in weightedPlatforms)
            {
                if (m.GetComponent<CircleCollider2D>().IsTouching(wp.GetComponent<BoxCollider2D>()))
                {
                    wp.GetComponent<TranslatablePlatform>().CheckWeight(m);
                }
            }
        }
        
    }

    
}

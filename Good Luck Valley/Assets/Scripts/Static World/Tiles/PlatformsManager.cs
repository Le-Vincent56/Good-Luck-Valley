using HiveMind.Mushroom;
using System.Collections.Generic;
using UnityEngine;

namespace HiveMind.Tiles
{
    public class PlatformsManager : MonoBehaviour
    {
        #region REFERENCES
        [Header("Platform Lists")]
        [SerializeField] private List<GameObject> weightedPlatforms;
        [SerializeField] private List<GameObject> collidablePlatforms;
        [SerializeField] private List<GameObject> nonCollidablePlatforms;
        [SerializeField] private List<GameObject> decomposableTiles;
        [SerializeField] MushroomManager mushroomManager;
        #endregion

        #region PROPERTIES
        public List<GameObject> WeightedPlatforms { get { return weightedPlatforms; } set { weightedPlatforms = value; } }
        public List<GameObject> DecomposableTiles { get { return decomposableTiles; } set { decomposableTiles = value; } }
        #endregion

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
            foreach (GameObject m in mushroomManager.MushroomList)
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
}
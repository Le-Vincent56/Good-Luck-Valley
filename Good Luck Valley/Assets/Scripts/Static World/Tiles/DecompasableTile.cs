using System.Collections.Generic;
using UnityEngine;

namespace HiveMind.Tiles
{
    public class DecompasableTile : MonoBehaviour
    {
        #region REFERENCES
        [SerializeField] private GameObject tile;
        private List<GameObject> decomposablePlatforms;
        #endregion

        #region FIELDS
        private bool isDecomposed;
        #endregion

        #region PROPERTIES
        public bool IsDecomposed { get { return isDecomposed; } set { isDecomposed = value; } }
        #endregion

        void Start()
        {
            decomposablePlatforms = GameObject.Find("Platforms Manager").GetComponent<PlatformsManager>().DecomposableTiles;
            isDecomposed = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (isDecomposed)
            {
                Debug.Log("Destroy Tile");
                tile.SetActive(false);
                //decomposablePlatforms.Remove(tile);
            }
            else
            {
                tile.SetActive(true);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log("Colliding with something" + collision.collider.gameObject.tag);
            if (collision.collider.gameObject.tag == "Mushroom")
            {
                isDecomposed = true;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.collider.gameObject.tag == "Ground")
            {
                isDecomposed = false;
            }
        }
    }
}
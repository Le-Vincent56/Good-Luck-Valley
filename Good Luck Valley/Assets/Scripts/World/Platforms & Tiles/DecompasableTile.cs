using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecompasableTile : MonoBehaviour
{
    #region REFERENCES
    [SerializeField] private GameObject tile;
    #endregion

    #region FIELDS
    private bool isDecomposed;
    #endregion

    #region PROPERTIES
    public bool IsDecomposed { get { return isDecomposed; } set { isDecomposed = value; } }
    #endregion



    void Start()
    {
        isDecomposed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDecomposed)
        {
            Debug.Log("Destroy Tile");
            Destroy(tile);
        }
    }
}

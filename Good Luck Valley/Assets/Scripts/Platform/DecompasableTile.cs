using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecompasableTile : MonoBehaviour
{
    public bool isDecomposed;
    [SerializeField] GameObject tile;


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

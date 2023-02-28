using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Journal : MonoBehaviour
{
    #region FIELDS
    [SerializeField] private List<Note> notes;
    #endregion

    #region PROPERTIES
    public List<Note> Notes { get { return notes; } set { notes = value; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

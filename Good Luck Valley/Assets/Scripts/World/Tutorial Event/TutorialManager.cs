using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    #region REFERENCES
    private List<TutorialMessage> messages;
    #endregion

    #region FIELDS
    private int currentLevel;
    #endregion

    #region PROPERTIES
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        currentLevel = SceneManager.GetActiveScene().buildIndex - 6;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

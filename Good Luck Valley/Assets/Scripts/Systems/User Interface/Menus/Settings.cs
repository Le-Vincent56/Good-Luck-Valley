using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
    #region REFERENCES
    private Canvas settings;
    #endregion

    #region FIELDS

    #endregion

    #region PROPERTIES

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        settings = GameObject.Find("Settings").GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableShroomIndicator()
    {

    }

    public void OnExit(int scene)
    {
        Debug.Log("Leaving Scene");
        SceneManager.LoadScene(scene);
    }
}

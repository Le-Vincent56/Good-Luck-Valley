using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    /// <summary>
    /// OnClick event for the Play Button
    /// </summary>
    public void OnClickPlay()
    {
        // Switch scenes to the Tutorial Tilemap
        SceneManager.LoadScene("TutorialTilemap");
    }
}

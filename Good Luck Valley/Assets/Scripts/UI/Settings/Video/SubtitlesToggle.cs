using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubtitlesToggle : MonoBehaviour
{
    [Header("Wwise Events")]
    [SerializeField] private AK.Wwise.Event playToggleOn;
    [SerializeField] private AK.Wwise.Event playToggleOff;

    [Header("References")]
    [SerializeField] private Toggle toggle;

    public void Init()
    {
        // Get components
        toggle = GetComponent<Toggle>();

        // Add listeners
        toggle.onValueChanged.AddListener(HandleToggle);
    }

    /// <summary>
    /// Handle subtitles toggling
    /// </summary>
    /// <param name="toggle"></param>
    public void HandleToggle(bool toggle)
    {
        // Handle if the toggle is turned on
        if(toggle)
        {
            // Play the toggle on sound effect
            playToggleOn.Post(gameObject);
        } 
        // Handle if the toggle is turned off
        else
        {
            // Play the toggle off sound effect
            playToggleOff.Post(gameObject);
        }
    }
}

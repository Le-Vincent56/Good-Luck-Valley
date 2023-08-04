using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideTutorialMessage : MonoBehaviour
{

    #region REFERENCES
    [SerializeField] private TutorialMessage tutorialMessage;
    #endregion

    #region FIELDS
    #endregion

    #region PROPERTIES
    #endregion
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            tutorialMessage.RemoveMessage = true;
        }
    }
}

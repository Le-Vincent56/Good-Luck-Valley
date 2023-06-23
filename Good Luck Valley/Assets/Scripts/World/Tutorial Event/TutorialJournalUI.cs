using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class TutorialJournalUI : MonoBehaviour
{
    #region REFERENCES
    private PlayerMovement playerMovement;
    private Tutorial tutorialManager;
    private PauseMenu pauseMenu;
    private Journal journal;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
        tutorialManager = GameObject.Find("TutorialUI").GetComponent<Tutorial>();
        pauseMenu = GameObject.Find("PauseUI").GetComponent<PauseMenu>();
        journal = GameObject.Find("JournalUI").GetComponent<Journal>();
    }

    public void EndJournalTutorial()
    {
        if(tutorialManager.ShowingThirdJournalUIText)
        {
            if(journal.HasOpened && journal.MenuOpen)
            {
                playerMovement.IsLocked = false;
                tutorialManager.FadeThirdJournalUITutorialText();
                tutorialManager.ShowingThirdJournalUIText = false;
            }
        }
    }
}

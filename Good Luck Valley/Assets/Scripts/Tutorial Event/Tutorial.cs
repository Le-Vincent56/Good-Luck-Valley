using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Tutorial : MonoBehaviour
{
    #region REFERENCES
    private Text movementTutorialText;
    private Image movePanelImage;
    private Text interactableTutorialText;
    private Image interPanelImage;
    private Text bounceTutorialText;
    private Image bouncePanelImage;
    private Text lotusTutorialText;
    private Image lotusPanelImage;
    private PlayerInput playerInput;
    private Text demoEndText;
    private Text titleButtonText;
    private Image buttonImage;
    #endregion

    #region FIELDS
    [SerializeField] private bool showingMovementText = false;
    [SerializeField] private bool showingInteractableText = false;
    [SerializeField] private bool showingBounceText = false;
    private bool showingLotusText = false;
    [SerializeField] private string currentControlScheme;

    // Demo Thanks Message
    
    private bool showingDemoEndText = false;
    #endregion

    #region PROPERTIES
    public bool ShowingMovementText { get { return showingMovementText; } set { showingMovementText = value; } }
    public bool ShowingInteractableText { get { return showingInteractableText; } set { showingInteractableText = value; } }
    public bool ShowingBounceText { get { return showingBounceText; } set { showingBounceText = value; } }
    public bool ShowingLotusText { get { return showingLotusText; } set { showingLotusText = value; } }
    public bool ShowingDemoEndText { get { return showingDemoEndText; } set { showingDemoEndText = value; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        movementTutorialText = GameObject.Find("Movement Tutorial Text").GetComponent<Text>();
        movePanelImage = GameObject.Find("Movement Tutorial Panel").GetComponent<Image>();
        interactableTutorialText = GameObject.Find("Interactable Tutorial Text").GetComponent<Text>();
        interPanelImage = GameObject.Find("Interactable Tutorial Panel").GetComponent<Image>();
        bounceTutorialText = GameObject.Find("Mushroom Bounce Tutorial Text").GetComponent<Text>();
        bouncePanelImage = GameObject.Find("Mushroom Bounce Tutorial Panel").GetComponent<Image>();
        lotusTutorialText = GameObject.Find("Anguish Lotus Tutorial Text").GetComponent<Text>();
        lotusPanelImage = GameObject.Find("Anguish Lotus Tutorial Panel").GetComponent<Image>();
        playerInput = GameObject.Find("Player").GetComponent<PlayerInput>();

        // Demo Thanks Message
        demoEndText = GameObject.Find("Demo Ending Text").GetComponent<Text>();
        titleButtonText = GameObject.Find("Title Text").GetComponent<Text>();
        buttonImage = GameObject.Find("Title Screen").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        currentControlScheme = playerInput.currentControlScheme;

        // Show/Hide Movement Tutorial Text
        if (showingMovementText)
        {
            showMovementTutorialText();
        } else
        {
            fadeMovementTutorialText();
        }

        // Show/Hide Interactable Tutorial Text
        if(showingInteractableText)
        {
            showInteractableTutorialText();
        } else
        {
            fadeInteractableTutorialText();
        }

        // Show/Hide Bounce Tutorial Text
        if(showingBounceText)
        {
            showBounceTutorialText();
        } else
        {
            fadeBounceTutorialText();
        }

        // Show/Hide Lotus Tutorial Text
        if(showingLotusText)
        {
            showLotusTutorialText();
        } else
        {
            fadeLotusTutorialText();
        }

        // Demo Thanks Message
        if (showingDemoEndText)
        {
            showDemoEndText();
        }
    }

    /// <summary>
    /// Show Movement tutorial text depending on the control scheme
    /// </summary>
    public void showMovementTutorialText()
    {
        string tutorialText = "";

        // Show correct controls for the different controllers
        switch (currentControlScheme)
        {
            case "Keyboard & Mouse":
                tutorialText = "Use WASD to Move Anari! Press Space to Jump!";
                break;

            case "Playstation Controller":
                tutorialText = "Use the Left Stick to Move Anari Around! Press Cross to Jump!";
                break;

            case "XBox Controller":
                tutorialText = "Use the Left Stick to Move Anari Around! Press A to Jump!";
                break;

            case "Switch Pro Controller":
                tutorialText = "Use the Left Stick to Move Anari Around! Press B to Jump!";
                break;

            default:
                break;
        }

        // Set text
        movementTutorialText.text = tutorialText;

        // Fade in the text using deltaTime and alpha values
        if (movePanelImage.color.a < 1 && movementTutorialText.color.a < 1)
        {
            movePanelImage.color = new Color(movePanelImage.color.r, movePanelImage.color.g, movePanelImage.color.b, movePanelImage.color.a + (Time.deltaTime * 2));
            movementTutorialText.color = new Color(movementTutorialText.color.r, movementTutorialText.color.g, movementTutorialText.color.b, movementTutorialText.color.a + (Time.deltaTime * 2));
        }
    }

    /// <summary>
    /// Fade out the Movement tutorial text
    /// </summary>
    public void fadeMovementTutorialText()
    {
        // Fade out the text using deltaTime and alpha values
        if (movePanelImage.color.a > 0 && movementTutorialText.color.a > 0)
        {
            movePanelImage.color = new Color(movePanelImage.color.r, movePanelImage.color.g, movePanelImage.color.b, movePanelImage.color.a - (Time.deltaTime * 2));
            movementTutorialText.color = new Color(movementTutorialText.color.r, movementTutorialText.color.g, movementTutorialText.color.b, movementTutorialText.color.a - (Time.deltaTime * 2));
        }
    }

    /// <summary>
    /// Show the Interactable tutorial text depending on the control scheme
    /// </summary>
    public void showInteractableTutorialText()
    {
        string tutorialText = "";

        // Show correct controls for the different controllers
        switch (currentControlScheme)
        {

            case "Keyboard & Mouse":
                tutorialText = "Press E to Interact with Objects!";
                break;

            case "Playstation Controller":
                tutorialText = "Press Circle to Interact with Objects!";
                break;

            case "XBox Controller":
                tutorialText = "Press B to Interact with Objects!";
                break;

            case "Switch Pro Controller":
                tutorialText = "Press A to Interact with Objects!";
                break;

            default:
                break;
        }

        // Set text
        interactableTutorialText.text = tutorialText;

        // Fade in the text using deltaTime and alpha values
        if (interPanelImage.color.a < 1 && interactableTutorialText.color.a < 1)
        {
            interPanelImage.color = new Color(interPanelImage.color.r, interPanelImage.color.g, interPanelImage.color.b, interPanelImage.color.a + (Time.deltaTime * 2));
            interactableTutorialText.color = new Color(interactableTutorialText.color.r, interactableTutorialText.color.g, interactableTutorialText.color.b, interactableTutorialText.color.a + (Time.deltaTime * 2));
        }
    }

    /// <summary>
    /// Fade out the Interactable tutorial text
    /// </summary>
    public void fadeInteractableTutorialText()
    {
        // Fade out the text using deltaTime and alpha values
        if (interPanelImage.color.a > 0 && interactableTutorialText.color.a > 0)
        {
            interPanelImage.color = new Color(interPanelImage.color.r, interPanelImage.color.g, interPanelImage.color.b, interPanelImage.color.a - (Time.deltaTime * 2));
            interactableTutorialText.color = new Color(interactableTutorialText.color.r, interactableTutorialText.color.g, interactableTutorialText.color.b, interactableTutorialText.color.a - (Time.deltaTime * 2));
        }
    }

    /// <summary>
    /// Show Bounce tutorial text depending on the control scheme
    /// </summary>
    public void showBounceTutorialText()
    {
        string tutorialText = "";

        // Show correct controls for the different controllers
        switch (currentControlScheme)
        {
            case "Keyboard & Mouse":
                tutorialText = "Hold Left-Click to aim towards your cursor and release to fire a Mushroom. \n\nColliding with the Mushroom will push you in the opposite direction." +
                    "\n\nRemove all Shrooms with Q and remove the last thrown Shroom using R";
                break;

            case "Playstation Controller":
                tutorialText = "Hold R2 to aim towards your cursor and release to fire a Mushroom. \n\nColliding with the Mushroom will push you in the opposite direction." +
                    "\n\nRemove all Shrooms with Triangle and remove the last thrown Shroom using Square";
                break;

            case "XBox Controller":
                tutorialText = "Hold RT to aim towards your cursor and release to fire a Mushroom. \n\nColliding with the Mushroom will push you in the opposite direction." +
                    "\n\nRemove all Shrooms with Y and remove the last thrown Shroom using X";
                break;

            case "Switch Pro Controller":
                tutorialText = "Hold ZR to aim towards your cursor and release to fire a Mushroom. \n\nColliding with the Mushroom will push you in the opposite direction." +
                    "\n\nRemove all Shrooms with X and remove the last thrown Shroom using Y";
                break;

            default:
                break;
        }

        // Set text
        bounceTutorialText.text = tutorialText;

        // Fade in the text using deltaTime and alpha values
        if (bouncePanelImage.color.a < 1 && bounceTutorialText.color.a < 1)
        {
            bouncePanelImage.color = new Color(bouncePanelImage.color.r, bouncePanelImage.color.g, bouncePanelImage.color.b, bouncePanelImage.color.a + (Time.deltaTime * 2));
            bounceTutorialText.color = new Color(bounceTutorialText.color.r, bounceTutorialText.color.g, bounceTutorialText.color.b, bounceTutorialText.color.a + (Time.deltaTime * 2));
        }
    }

    /// <summary>
    /// Fade out the Bounce tutorial text
    /// </summary>
    public void fadeBounceTutorialText()
    {
        // Fade out the text using deltaTime and alpha values
        if (bouncePanelImage.color.a > 0 && bounceTutorialText.color.a > 0)
        {
            bouncePanelImage.color = new Color(bouncePanelImage.color.r, bouncePanelImage.color.g, bouncePanelImage.color.b, bouncePanelImage.color.a - (Time.deltaTime * 2));
            bounceTutorialText.color = new Color(bounceTutorialText.color.r, bounceTutorialText.color.g, bounceTutorialText.color.b, bounceTutorialText.color.a - (Time.deltaTime * 2));
        }
    }

    /// <summary>
    /// Show Lotus tutorial text
    /// </summary>
    public void showLotusTutorialText()
    {
        // Set text
        lotusTutorialText.text = "Interact with the Anguish Lotus to Complete the Level!";

        // Fade in the text using deltaTime and alpha values
        if (lotusPanelImage.color.a < 1 && lotusTutorialText.color.a < 1)
        {
            lotusPanelImage.color = new Color(lotusPanelImage.color.r, lotusPanelImage.color.g, lotusPanelImage.color.b, lotusPanelImage.color.a + (Time.deltaTime * 2));
            lotusTutorialText.color = new Color(lotusTutorialText.color.r, lotusTutorialText.color.g, lotusTutorialText.color.b, lotusTutorialText.color.a + (Time.deltaTime * 2));
        }
    }

    /// <summary>
    /// Fade out the Lotus tutorial text
    /// </summary>
    public void fadeLotusTutorialText()
    {
        // Fade out the text using deltaTime and alpha values
        if (lotusPanelImage.color.a > 0 && lotusTutorialText.color.a > 0)
        {
            lotusPanelImage.color = new Color(lotusPanelImage.color.r, lotusPanelImage.color.g, lotusPanelImage.color.b, lotusPanelImage.color.a - (Time.deltaTime * 2));
            lotusTutorialText.color = new Color(lotusTutorialText.color.r, lotusTutorialText.color.g, lotusTutorialText.color.b, lotusTutorialText.color.a - (Time.deltaTime * 2));
        }
    }

    /// <summary>
    /// Show Thank You message for completing the Demo
    /// </summary>
    public void showDemoEndText()
    {
        // Set text
        demoEndText.text = "You have reached the end of the demo. Thank you for playing!";
        titleButtonText.text = "Return To Title";

        // Fade in the text using deltaTime and alpha values
        if (demoEndText.color.a < 1)
        {
            demoEndText.color = new Color(demoEndText.color.r, demoEndText.color.g, demoEndText.color.b, demoEndText.color.a + (Time.deltaTime * 1f));
            titleButtonText.color = new Color(titleButtonText.color.r, titleButtonText.color.g, titleButtonText.color.b, titleButtonText.color.a + (Time.deltaTime * 1f));
            buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, buttonImage.color.a + (Time.deltaTime * 1f));
        }
    }
}

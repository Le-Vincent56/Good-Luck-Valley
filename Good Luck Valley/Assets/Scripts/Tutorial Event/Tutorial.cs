using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Tutorial : MonoBehaviour
{
    #region FIELDS
    private Text movementTutorialText;
    private Image movePanelImage;
    [SerializeField] private bool showingMovementText = false;

    private Text interactableTutorialText;
    private Image interPanelImage;
    [SerializeField] private bool showingInteractableText = false;

    private Text bounceTutorialText;
    private Image bouncePanelImage;
    [SerializeField] private bool showingBounceText = false;

    private Text lotusTutorialText;
    private Image lotusPanelImage;
    private bool lotusTextSet = false;
    private bool showingLotusText = false;
    [SerializeField] private float lotusFadeInTimer = 0.0f;

    private PlayerInput playerInput;
    [SerializeField] private string currentControlScheme;

    // Demo Thanks Message
    private Text demoEndText;
    private Text titleButtonText;
    private Image buttonImage; 
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

        if (showingMovementText)
        {
            showMovementTutorialText();
        } else
        {
            fadeMovementTutorialText();
        }

        if(showingInteractableText)
        {
            showInteractableTutorialText();
        } else
        {
            fadeInteractableTutorialText();
        }

        if(showingBounceText)
        {
            showBounceTutorialText();
        } else
        {
            fadeBounceTutorialText();
        }

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

    public void showMovementTutorialText()
    {
        string tutorialText = "";

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

        movementTutorialText.text = tutorialText;

        if (movePanelImage.color.a < 1 && movementTutorialText.color.a < 1)
        {
            movePanelImage.color = new Color(movePanelImage.color.r, movePanelImage.color.g, movePanelImage.color.b, movePanelImage.color.a + (Time.deltaTime * 2));
            movementTutorialText.color = new Color(movementTutorialText.color.r, movementTutorialText.color.g, movementTutorialText.color.b, movementTutorialText.color.a + (Time.deltaTime * 2));
        }
    }
    public void fadeMovementTutorialText()
    {
        if (movePanelImage.color.a > 0 && movementTutorialText.color.a > 0)
        {
            movePanelImage.color = new Color(movePanelImage.color.r, movePanelImage.color.g, movePanelImage.color.b, movePanelImage.color.a - (Time.deltaTime * 2));
            movementTutorialText.color = new Color(movementTutorialText.color.r, movementTutorialText.color.g, movementTutorialText.color.b, movementTutorialText.color.a - (Time.deltaTime * 2));
        }
    }

    public void showInteractableTutorialText()
    {
        string tutorialText = "";

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

        interactableTutorialText.text = tutorialText;

        if (interPanelImage.color.a < 1 && interactableTutorialText.color.a < 1)
        {
            interPanelImage.color = new Color(interPanelImage.color.r, interPanelImage.color.g, interPanelImage.color.b, interPanelImage.color.a + (Time.deltaTime * 2));
            interactableTutorialText.color = new Color(interactableTutorialText.color.r, interactableTutorialText.color.g, interactableTutorialText.color.b, interactableTutorialText.color.a + (Time.deltaTime * 2));
        }
    }

    public void fadeInteractableTutorialText()
    {
        if (interPanelImage.color.a > 0 && interactableTutorialText.color.a > 0)
        {
            interPanelImage.color = new Color(interPanelImage.color.r, interPanelImage.color.g, interPanelImage.color.b, interPanelImage.color.a - (Time.deltaTime * 2));
            interactableTutorialText.color = new Color(interactableTutorialText.color.r, interactableTutorialText.color.g, interactableTutorialText.color.b, interactableTutorialText.color.a - (Time.deltaTime * 2));
        }
    }

    public void showBounceTutorialText()
    {
        string tutorialText = "";

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

        bounceTutorialText.text = tutorialText;

        if (bouncePanelImage.color.a < 1 && bounceTutorialText.color.a < 1)
        {
            bouncePanelImage.color = new Color(bouncePanelImage.color.r, bouncePanelImage.color.g, bouncePanelImage.color.b, bouncePanelImage.color.a + (Time.deltaTime * 2));
            bounceTutorialText.color = new Color(bounceTutorialText.color.r, bounceTutorialText.color.g, bounceTutorialText.color.b, bounceTutorialText.color.a + (Time.deltaTime * 2));
        }
    }

    public void fadeBounceTutorialText()
    {
        if (bouncePanelImage.color.a > 0 && bounceTutorialText.color.a > 0)
        {
            bouncePanelImage.color = new Color(bouncePanelImage.color.r, bouncePanelImage.color.g, bouncePanelImage.color.b, bouncePanelImage.color.a - (Time.deltaTime * 2));
            bounceTutorialText.color = new Color(bounceTutorialText.color.r, bounceTutorialText.color.g, bounceTutorialText.color.b, bounceTutorialText.color.a - (Time.deltaTime * 2));
        }
    }

    public void showLotusTutorialText()
    {
        lotusTutorialText.text = "Interact with the Anguish Lotus to Complete the Level!";

        if (lotusPanelImage.color.a < 1 && lotusTutorialText.color.a < 1)
        {
            lotusPanelImage.color = new Color(lotusPanelImage.color.r, lotusPanelImage.color.g, lotusPanelImage.color.b, lotusPanelImage.color.a + (Time.deltaTime * 2));
            lotusTutorialText.color = new Color(lotusTutorialText.color.r, lotusTutorialText.color.g, lotusTutorialText.color.b, lotusTutorialText.color.a + (Time.deltaTime * 2));
        }
    }

    public void fadeLotusTutorialText()
    {
        if (lotusPanelImage.color.a > 0 && lotusTutorialText.color.a > 0)
        {
            lotusPanelImage.color = new Color(lotusPanelImage.color.r, lotusPanelImage.color.g, lotusPanelImage.color.b, lotusPanelImage.color.a - (Time.deltaTime * 2));
            lotusTutorialText.color = new Color(lotusTutorialText.color.r, lotusTutorialText.color.g, lotusTutorialText.color.b, lotusTutorialText.color.a - (Time.deltaTime * 2));
        }
    }

    // Demo thanks message
    public void showDemoEndText()
    {
        demoEndText.text = "You have reached the end of the demo. Thank you for playing!";
        titleButtonText.text = "Return To Title";

        if (demoEndText.color.a < 1)
        {
            demoEndText.color = new Color(demoEndText.color.r, demoEndText.color.g, demoEndText.color.b, demoEndText.color.a + (Time.deltaTime * 1f));
            titleButtonText.color = new Color(titleButtonText.color.r, titleButtonText.color.g, titleButtonText.color.b, titleButtonText.color.a + (Time.deltaTime * 1f));
            buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, buttonImage.color.a + (Time.deltaTime * 1f));
        }
    }
}

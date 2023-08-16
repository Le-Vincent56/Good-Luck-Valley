using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using HiveMind.Events;

namespace HiveMind.Tutorial
{
    public class Tutorial : MonoBehaviour
    {
        #region REFERENCES
        [SerializeField] private JournalScriptableObj journalEvent;
        private Text movementTutorialText;
        private Image movePanelImage;
        private Text interactableTutorialText;
        private Image interPanelImage;
        private Text mushroomInteractText;
        private Image mushroomInteractImage;
        private Text bounceTutorialText;
        private Image bouncePanelImage;
        private Text removeTutorialText;
        private Image removePanelImage;
        private Text lotusTutorialText;
        private Image lotusPanelImage;
        private Text journalUITutorialText;
        private Image journalUIPanelImage;
        private Text secondJournalUITutorialText;
        private Image secondJournalUIPanelImage;
        private Text thirdJournalUITutorialText;
        private Image thirdJournalUIPanelImage;
        private PlayerInput playerInput;
        private TMP_Text demoEndText;
        private TMP_Text titleButtonText;
        private Image buttonImage;
        #endregion

        #region FIELDS
        [SerializeField] private bool showingMovementText = false;
        [SerializeField] private bool showingInteractableText = false;
        private bool showingMushroomInteractText = false;
        private bool mushroomInteracted = false;
        [SerializeField] private bool showingBounceText = false;
        [SerializeField] bool showingRemoveText = false;
        [SerializeField] private float removeTutorialTimer = 4.0f;
        private bool showingLotusText = false;
        [SerializeField] private bool showingFirstJournalUIText = false;
        [SerializeField] private bool showingSecondJournalUIText = false;
        [SerializeField] private bool showingThirdJournalUIText = false;
        [SerializeField] private string currentControlScheme;

        // Demo Thanks Message

        private bool showingDemoEndText = false;
        #endregion

        #region PROPERTIES
        public bool ShowingMovementText { get { return showingMovementText; } set { showingMovementText = value; } }
        public bool ShowingInteractableText { get { return showingInteractableText; } set { showingInteractableText = value; } }
        public bool ShowingMushroomInteractText { get { return showingMushroomInteractText; } set { showingMushroomInteractText = value; } }
        public bool MushroomInteracted { get { return mushroomInteracted; } set { mushroomInteracted = value; } }
        public bool ShowingBounceText { get { return showingBounceText; } set { showingBounceText = value; } }
        public bool ShowingRemoveText { get { return showingRemoveText; } set { showingRemoveText = value; } }
        public bool ShowingLotusText { get { return showingLotusText; } set { showingLotusText = value; } }
        public bool ShowingFirstJournalUIText { get { return showingFirstJournalUIText; } set { showingFirstJournalUIText = value; } }
        public bool ShowingSecondJournalUIText { get { return showingSecondJournalUIText; } set { showingSecondJournalUIText = value; } }
        public bool ShowingThirdJournalUIText { get { return showingThirdJournalUIText; } set { showingThirdJournalUIText = value; } }
        public bool ShowingDemoEndText { get { return showingDemoEndText; } set { showingDemoEndText = value; } }
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            movementTutorialText = GameObject.Find("Movement Tutorial Text").GetComponent<Text>();
            movePanelImage = GameObject.Find("Movement Tutorial Panel").GetComponent<Image>();
            interactableTutorialText = GameObject.Find("Interactable Tutorial Text").GetComponent<Text>();
            interPanelImage = GameObject.Find("Interactable Tutorial Panel").GetComponent<Image>();
            mushroomInteractText = GameObject.Find("Mushroom Interact Tutorial Text").GetComponent<Text>();
            mushroomInteractImage = GameObject.Find("Mushroom Interact Tutorial Panel").GetComponent<Image>();
            bounceTutorialText = GameObject.Find("Mushroom Bounce Tutorial Text").GetComponent<Text>();
            bouncePanelImage = GameObject.Find("Mushroom Bounce Tutorial Panel").GetComponent<Image>();
            removeTutorialText = GameObject.Find("Mushroom Remove Tutorial Text").GetComponent<Text>();
            removePanelImage = GameObject.Find("Mushroom Remove Tutorial Panel").GetComponent<Image>();
            lotusTutorialText = GameObject.Find("Anguish Lotus Tutorial Text").GetComponent<Text>();
            lotusPanelImage = GameObject.Find("Anguish Lotus Tutorial Panel").GetComponent<Image>();
            journalUITutorialText = GameObject.Find("Journal UI Tutorial Text").GetComponent<Text>();
            journalUIPanelImage = GameObject.Find("Journal UI Tutorial Panel").GetComponent<Image>();
            secondJournalUITutorialText = GameObject.Find("Journal UI Tutorial Text 2").GetComponent<Text>();
            secondJournalUIPanelImage = GameObject.Find("Journal UI Tutorial Panel 2").GetComponent<Image>();
            thirdJournalUITutorialText = GameObject.Find("Journal UI Tutorial Text 3").GetComponent<Text>();
            thirdJournalUIPanelImage = GameObject.Find("Journal UI Tutorial Panel 3").GetComponent<Image>();
            playerInput = GameObject.Find("Player").GetComponent<PlayerInput>();

            // Demo Thanks Message
            demoEndText = GameObject.Find("Demo Ending Text").GetComponent<TMP_Text>();
            titleButtonText = GameObject.Find("Title Text").GetComponent<TMP_Text>();
            buttonImage = GameObject.Find("Title Screen").GetComponent<Image>();
        }

        // Update is called once per frame
        void Update()
        {
            currentControlScheme = playerInput.currentControlScheme;

            // Show/Hide Movement Tutorial Text
            if (showingMovementText)
            {
                ShowMovementTutorialText();
            }
            else
            {
                FadeMovementTutorialText();
            }

            // Show/Hide Interactable Tutorial Text
            if (showingInteractableText)
            {
                ShowInteractableTutorialText();
            }
            else
            {
                FadeInteractableTutorialText();
            }

            if (showingFirstJournalUIText)
            {

                journalEvent.SetCanClose(false);
                ShowFirstJournalUITutorialText();
                if (journalEvent.GetOpenedOnce())
                {
                    showingFirstJournalUIText = false;
                    showingSecondJournalUIText = true;
                }
            }
            else
            {
                FadeFirstJournalUITutorialText();
            }

            if (showingSecondJournalUIText)
            {
                ShowSecondJournalUITutorialText();
                GameObject.Find("Continue Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Settings Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Quit Button").GetComponent<Button>().interactable = false;
                if (journalEvent.GetOpenedOnce())
                {
                    showingSecondJournalUIText = false;
                    showingThirdJournalUIText = true;
                }
            }
            else
            {
                FadeSecondJournalUITutorialText();
                GameObject.Find("Continue Button").GetComponent<Button>().interactable = true;
                //GameObject.Find("Settings Button").GetComponent<Button>().interactable = true;
                GameObject.Find("Quit Button").GetComponent<Button>().interactable = true;
            }

            if (showingThirdJournalUIText)
            {
                ShowThirdJournalUITutorialText();
                GameObject.Find("Journal Back Button").GetComponent<Button>().interactable = false;
            }
            else
            {
                journalEvent.SetCanClose(true);
                FadeThirdJournalUITutorialText();
                GameObject.Find("Journal Back Button").GetComponent<Button>().interactable = true;
            }

            if (showingMushroomInteractText && !mushroomInteracted)
            {
                ShowMushroomInteractText();
            }
            else
            {
                FadeMushroomInteractText();
            }


            // Show/Hide Bounce Tutorial Text
            if (showingBounceText)
            {
                ShowBounceTutorialText();
            }
            else
            {
                FadeBounceTutorialText();
            }

            if (showingRemoveText)
            {
                if (removeTutorialTimer > 0)
                {
                    removeTutorialTimer -= Time.deltaTime;
                }
                else
                {
                    showingRemoveText = false;
                }
                ShowRemoveTutorialText();

            }
            else
            {
                FadeRemoveTutorialText();
            }

            // Show/Hide Lotus Tutorial Text
            if (showingLotusText)
            {
                ShowLotusTutorialText();
            }
            else
            {
                FadeLotusTutorialText();
            }

            // Demo Thanks Message
            if (showingDemoEndText)
            {
                ShowDemoEndText();
            }
        }

        /// <summary>
        /// Show Movement tutorial text depending on the control scheme
        /// </summary>
        public void ShowMovementTutorialText()
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
        public void FadeMovementTutorialText()
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
        public void ShowInteractableTutorialText()
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
                interPanelImage.color = new Color(interPanelImage.color.r, interPanelImage.color.g, interPanelImage.color.b, interPanelImage.color.a + (Time.unscaledDeltaTime * 2));
                interactableTutorialText.color = new Color(interactableTutorialText.color.r, interactableTutorialText.color.g, interactableTutorialText.color.b, interactableTutorialText.color.a + (Time.unscaledDeltaTime * 2));
            }
        }

        /// <summary>
        /// Fade out the Interactable tutorial text
        /// </summary>
        public void FadeInteractableTutorialText()
        {
            // Fade out the text using deltaTime and alpha values
            if (interPanelImage.color.a > 0 && interactableTutorialText.color.a > 0)
            {
                interPanelImage.color = new Color(interPanelImage.color.r, interPanelImage.color.g, interPanelImage.color.b, interPanelImage.color.a - (Time.unscaledDeltaTime * 2));
                interactableTutorialText.color = new Color(interactableTutorialText.color.r, interactableTutorialText.color.g, interactableTutorialText.color.b, interactableTutorialText.color.a - (Time.unscaledDeltaTime * 2));
            }
        }

        /// <summary>
        /// Show the first part of the Journal UI Tutorial Text
        /// </summary>
        public void ShowFirstJournalUITutorialText()
        {
            string tutorialText = "";

            // Show correct controls for the different controllers
            switch (currentControlScheme)
            {

                case "Keyboard & Mouse":
                    tutorialText = "To read newly obtained notes, press ESC and select 'Journal' from the Pause Menu, or use TAB!";
                    break;

                case "Playstation Controller":
                    tutorialText = "To read newly obtained notes, press the Options button!";
                    break;

                case "XBox Controller":
                    tutorialText = "To read newly obtained notes, press the Menu button!";
                    break;

                case "Switch Pro Controller":
                    tutorialText = "To read newly obtained notes, press the Plus button!";
                    break;

                default:
                    break;
            }

            // Set text
            journalUITutorialText.text = tutorialText;

            // Fade in the text using deltaTime and alpha values
            if (journalUIPanelImage.color.a < 1 && journalUITutorialText.color.a < 1)
            {
                journalUIPanelImage.color = new Color(journalUIPanelImage.color.r, journalUIPanelImage.color.g, journalUIPanelImage.color.b, journalUIPanelImage.color.a + (Time.unscaledDeltaTime * 2));
                journalUITutorialText.color = new Color(journalUITutorialText.color.r, journalUITutorialText.color.g, journalUITutorialText.color.b, journalUITutorialText.color.a + (Time.unscaledDeltaTime * 2));
            }
        }

        /// <summary>
        /// Hide the first part of the Journal UI Tutorial Text
        /// </summary>
        public void FadeFirstJournalUITutorialText()
        {
            // Fade out the text using deltaTime and alpha values
            if (journalUIPanelImage.color.a > 0 && journalUITutorialText.color.a > 0)
            {
                journalUIPanelImage.color = new Color(journalUIPanelImage.color.r, journalUIPanelImage.color.g, journalUIPanelImage.color.b, journalUIPanelImage.color.a - (Time.unscaledDeltaTime * 2));
                journalUITutorialText.color = new Color(journalUITutorialText.color.r, journalUITutorialText.color.g, journalUITutorialText.color.b, journalUITutorialText.color.a - (Time.unscaledDeltaTime * 2));
            }
        }

        /// <summary>
        /// Show the second part of the Journal UI Tutorial Text
        /// </summary>
        public void ShowSecondJournalUITutorialText()
        {
            // Set text
            secondJournalUITutorialText.text = "Press the \"Journal\" Button to see the list of collected entries";

            // Fade in the text using deltaTime and alpha values
            if (secondJournalUIPanelImage.color.a < 1 && secondJournalUITutorialText.color.a < 1)
            {
                secondJournalUIPanelImage.color = new Color(secondJournalUIPanelImage.color.r, secondJournalUIPanelImage.color.g, secondJournalUIPanelImage.color.b, secondJournalUIPanelImage.color.a + (Time.unscaledDeltaTime * 2));
                secondJournalUITutorialText.color = new Color(secondJournalUITutorialText.color.r, secondJournalUITutorialText.color.g, secondJournalUITutorialText.color.b, secondJournalUITutorialText.color.a + (Time.unscaledDeltaTime * 2));
            }
        }

        /// <summary>
        /// Hide the first part of the Journal UI Tutorial Text
        /// </summary>
        public void FadeSecondJournalUITutorialText()
        {
            // Fade out the text using deltaTime and alpha values
            if (secondJournalUIPanelImage.color.a > 0 && secondJournalUITutorialText.color.a > 0)
            {
                secondJournalUIPanelImage.color = new Color(secondJournalUIPanelImage.color.r, secondJournalUIPanelImage.color.g, secondJournalUIPanelImage.color.b, secondJournalUIPanelImage.color.a - (Time.unscaledDeltaTime * 2));
                secondJournalUITutorialText.color = new Color(secondJournalUITutorialText.color.r, secondJournalUITutorialText.color.g, secondJournalUITutorialText.color.b, secondJournalUITutorialText.color.a - (Time.unscaledDeltaTime * 2));
            }
        }

        /// <summary>
        /// Show the third part of the Journal UI Tutorial Text
        /// </summary>
        public void ShowThirdJournalUITutorialText()
        {
            // Set text
            thirdJournalUITutorialText.text = "Click an Entry Title on the left to see its contents on the right";

            // Fade in the text using deltaTime and alpha values
            if (thirdJournalUIPanelImage.color.a < 1 && thirdJournalUITutorialText.color.a < 1)
            {
                thirdJournalUIPanelImage.color = new Color(thirdJournalUIPanelImage.color.r, thirdJournalUIPanelImage.color.g, thirdJournalUIPanelImage.color.b, thirdJournalUIPanelImage.color.a + (Time.unscaledDeltaTime * 2));
                thirdJournalUITutorialText.color = new Color(thirdJournalUITutorialText.color.r, thirdJournalUITutorialText.color.g, thirdJournalUITutorialText.color.b, thirdJournalUITutorialText.color.a + (Time.unscaledDeltaTime * 2));
            }
        }

        /// <summary>
        /// Hide the third part of the Journal UI Tutorial Text
        /// </summary>
        public void FadeThirdJournalUITutorialText()
        {
            // Fade out the text using deltaTime and alpha values
            if (thirdJournalUIPanelImage.color.a > 0 && thirdJournalUITutorialText.color.a > 0)
            {
                thirdJournalUIPanelImage.color = new Color(thirdJournalUIPanelImage.color.r, thirdJournalUIPanelImage.color.g, thirdJournalUIPanelImage.color.b, thirdJournalUIPanelImage.color.a - (Time.unscaledDeltaTime * 2));
                thirdJournalUITutorialText.color = new Color(thirdJournalUITutorialText.color.r, thirdJournalUITutorialText.color.g, thirdJournalUITutorialText.color.b, thirdJournalUITutorialText.color.a - (Time.unscaledDeltaTime * 2));
            }
        }

        /// <summary>
        /// Show Mushroom interact tutorial text depending on the control scheme
        /// </summary>
        public void ShowMushroomInteractText()
        {
            string tutorialText = "";

            // Show correct controls for the different controllers
            switch (currentControlScheme)
            {
                case "Keyboard & Mouse":
                    tutorialText = "Press E";
                    break;

                case "Playstation Controller":
                    tutorialText = "Press Circle";
                    break;

                case "XBox Controller":
                    tutorialText = "Press B";
                    break;

                case "Switch Pro Controller":
                    tutorialText = "Press A";
                    break;

                default:
                    break;
            }

            // Set text
            mushroomInteractText.text = tutorialText;

            // Fade in the text using deltaTime and alpha values
            if (mushroomInteractImage.color.a < 1 && mushroomInteractText.color.a < 1)
            {
                mushroomInteractImage.color = new Color(mushroomInteractImage.color.r, mushroomInteractImage.color.g, mushroomInteractImage.color.b, mushroomInteractImage.color.a + (Time.unscaledDeltaTime * 2));
                mushroomInteractText.color = new Color(mushroomInteractText.color.r, mushroomInteractText.color.g, mushroomInteractText.color.b, mushroomInteractText.color.a + (Time.unscaledDeltaTime * 2));
            }
        }

        /// <summary>
        /// Fade Mushroom interact tutorial text.
        /// </summary>
        public void FadeMushroomInteractText()
        {
            // Fade out the text using deltaTime and alpha values
            if (mushroomInteractImage.color.a > 0 && mushroomInteractText.color.a > 0)
            {
                mushroomInteractImage.color = new Color(mushroomInteractImage.color.r, mushroomInteractImage.color.g, mushroomInteractImage.color.b, mushroomInteractImage.color.a - (Time.unscaledDeltaTime * 2));
                mushroomInteractText.color = new Color(mushroomInteractText.color.r, mushroomInteractText.color.g, mushroomInteractText.color.b, mushroomInteractText.color.a - (Time.unscaledDeltaTime * 2));
            }
        }

        /// <summary>
        /// Show Bounce tutorial text depending on the control scheme
        /// </summary>
        public void ShowBounceTutorialText()
        {
            string tutorialText = "";

            // Show correct controls for the different controllers
            switch (currentControlScheme)
            {
                case "Keyboard & Mouse":
                    tutorialText = "Hold Left-Click to aim towards your cursor. Release to throw a Mushroom. \n\nJump on the Mushroom to bounce!";
                    break;

                case "Playstation Controller":
                    tutorialText = "Hold R2 to aim towards your cursor. Release to fire a Mushroom. \n\nJump on the Mushroom to bounce!";
                    break;

                case "XBox Controller":
                    tutorialText = "Hold RT to aim towards your cursor. Release to fire a Mushroom. \n\nJump on the Mushroom to bounce!";
                    break;

                case "Switch Pro Controller":
                    tutorialText = "Hold ZR to aim towards your cursor. Release to fire a Mushroom. \n\nJump on the Mushroom to bounce!";
                    break;

                default:
                    break;
            }

            // Set text
            bounceTutorialText.text = tutorialText;

            // Fade in the text using deltaTime and alpha values
            if (bouncePanelImage.color.a < 1 && bounceTutorialText.color.a < 1)
            {
                bouncePanelImage.color = new Color(bouncePanelImage.color.r, bouncePanelImage.color.g, bouncePanelImage.color.b, bouncePanelImage.color.a + (Time.unscaledDeltaTime * 2));
                bounceTutorialText.color = new Color(bounceTutorialText.color.r, bounceTutorialText.color.g, bounceTutorialText.color.b, bounceTutorialText.color.a + (Time.unscaledDeltaTime * 2));
            }
        }

        /// <summary>
        /// Fade out the Bounce tutorial text
        /// </summary>
        public void FadeBounceTutorialText()
        {
            // Fade out the text using deltaTime and alpha values
            if (bouncePanelImage.color.a > 0 && bounceTutorialText.color.a > 0)
            {
                bouncePanelImage.color = new Color(bouncePanelImage.color.r, bouncePanelImage.color.g, bouncePanelImage.color.b, bouncePanelImage.color.a - (Time.unscaledDeltaTime * 2));
                bounceTutorialText.color = new Color(bounceTutorialText.color.r, bounceTutorialText.color.g, bounceTutorialText.color.b, bounceTutorialText.color.a - (Time.unscaledDeltaTime * 2));
            }
        }

        /// <summary>
        /// Show Remove tutorial text depending on control scheme
        /// </summary>
        public void ShowRemoveTutorialText()
        {
            string tutorialText = "";

            // Show correct controls for the different controllers
            switch (currentControlScheme)
            {
                case "Keyboard & Mouse":
                    tutorialText = "You can have a maximum of 3 shrooms out at a time." +
                        "\nPress Q to remove all shrooms" +
                        "\nPress R to remove the last thrown shroom";
                    break;

                case "Playstation Controller":
                    tutorialText = "You can have a maximum of 3 shrooms out at a time." +
                        "\nPress Triangle to remove all shrooms" +
                        "\nPress Square to remove the last thrown shroom";
                    break;

                case "XBox Controller":
                    tutorialText = "You can have a maximum of 3 shrooms out at a time." +
                        "\nPress Y to remove all shrooms" +
                        "\nPress X to remove the last thrown shroom";
                    break;

                case "Switch Pro Controller":
                    tutorialText = "You can have a maximum of 3 shrooms out at a time." +
                        "\nPress X to remove all shrooms" +
                        "\nPress Y to remove the last thrown shroom";
                    break;

                default:
                    break;
            }

            // Set text
            removeTutorialText.text = tutorialText;

            // Fade in the text using deltaTime and alpha values
            if (removePanelImage.color.a < 1 && removeTutorialText.color.a < 1)
            {
                removePanelImage.color = new Color(removePanelImage.color.r, removePanelImage.color.g, removePanelImage.color.b, removePanelImage.color.a + (Time.unscaledDeltaTime * 2));
                removeTutorialText.color = new Color(removeTutorialText.color.r, removeTutorialText.color.g, removeTutorialText.color.b, removeTutorialText.color.a + (Time.unscaledDeltaTime * 2));
            }
        }

        /// <summary>
        /// Fade out the Remove tutorial text
        /// </summary>
        public void FadeRemoveTutorialText()
        {
            if (removePanelImage.color.a > 0 && removeTutorialText.color.a > 0)
            {
                removePanelImage.color = new Color(removePanelImage.color.r, removePanelImage.color.g, removePanelImage.color.b, removePanelImage.color.a - (Time.unscaledDeltaTime * 2));
                removeTutorialText.color = new Color(removeTutorialText.color.r, removeTutorialText.color.g, removeTutorialText.color.b, removeTutorialText.color.a - (Time.unscaledDeltaTime * 2));
            }
        }

        /// <summary>
        /// Show Lotus tutorial text
        /// </summary>
        public void ShowLotusTutorialText()
        {
            // Set text
            lotusTutorialText.text = "Interact with the Anguish Lotus to Complete the Level!";

            // Fade in the text using deltaTime and alpha values
            if (lotusPanelImage.color.a < 1 && lotusTutorialText.color.a < 1)
            {
                lotusPanelImage.color = new Color(lotusPanelImage.color.r, lotusPanelImage.color.g, lotusPanelImage.color.b, lotusPanelImage.color.a + (Time.unscaledDeltaTime * 2));
                lotusTutorialText.color = new Color(lotusTutorialText.color.r, lotusTutorialText.color.g, lotusTutorialText.color.b, lotusTutorialText.color.a + (Time.unscaledDeltaTime * 2));
            }
        }

        /// <summary>
        /// Fade out the Lotus tutorial text
        /// </summary>
        public void FadeLotusTutorialText()
        {
            // Fade out the text using deltaTime and alpha values
            if (lotusPanelImage.color.a > 0 && lotusTutorialText.color.a > 0)
            {
                lotusPanelImage.color = new Color(lotusPanelImage.color.r, lotusPanelImage.color.g, lotusPanelImage.color.b, lotusPanelImage.color.a - (Time.unscaledDeltaTime * 2));
                lotusTutorialText.color = new Color(lotusTutorialText.color.r, lotusTutorialText.color.g, lotusTutorialText.color.b, lotusTutorialText.color.a - (Time.unscaledDeltaTime * 2));
            }
        }

        /// <summary>
        /// Show Thank You message for completing the Demo
        /// </summary>
        public void ShowDemoEndText()
        {
            // Set text
            demoEndText.text = "You have reached the end of the demo. \nThank you for playing!";
            titleButtonText.text = "Return To Title";

            // Fade in the text using deltaTime and alpha values
            if (demoEndText.color.a < 1)
            {
                demoEndText.color = new Color(demoEndText.color.r, demoEndText.color.g, demoEndText.color.b, demoEndText.color.a + (Time.unscaledDeltaTime * 1f));
                titleButtonText.color = new Color(titleButtonText.color.r, titleButtonText.color.g, titleButtonText.color.b, titleButtonText.color.a + (Time.unscaledDeltaTime * 1f));
                buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, buttonImage.color.a + (Time.unscaledDeltaTime * 1f));
                buttonImage.gameObject.GetComponent<Button>().interactable = true;
            }
        }
    }
}

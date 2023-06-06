using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DevTools : MonoBehaviour
{
    #region REFERENCES
    [SerializeField] private MushroomManager mushMan;
    [SerializeField] private PlayerMovement playerMove;
    private Text devText;
    private Text noClipText;
    private Text instantThrowText;
    private Text infiniteShroomText;
    #endregion

    #region FIELDS
    [SerializeField] bool devToolsEnabled;
    [SerializeField] bool noClip;
    [SerializeField] bool instantThrow;
    [SerializeField] bool infiniteShrooms;
    private BoxCollider2D playerCollider;
    private CapsuleCollider2D capsuleCollider;
    #endregion

    #region PROPERTIES
    public bool NoClip { get { return noClip; } set {  noClip = value; } }
    public bool DevToolsEnabled { get { return devToolsEnabled; } set { devToolsEnabled = value; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        mushMan = GameObject.Find("Mushroom Manager").GetComponent<MushroomManager>();
        playerMove = GameObject.Find("Player").GetComponent<PlayerMovement>();
        playerCollider = playerMove.GetComponentInParent<BoxCollider2D>();
        capsuleCollider = playerMove.GetComponentInParent<CapsuleCollider2D>();
        devText = GameObject.Find("DevText").GetComponent<Text>();
        noClipText = GameObject.Find("NoClipText").GetComponent<Text>();
        instantThrowText = GameObject.Find("InstantShroomText").GetComponent<Text>();
        infiniteShroomText = GameObject.Find("InfiniteShroomText").GetComponent<Text>();

        if (devToolsEnabled)
        {
            noClip = false;
            instantThrow = false;
            infiniteShrooms = true;
            mushMan.ThrowUnlocked = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // No Clip text Change
        if (noClip == true)
        {
            noClipText.text = "Press F1 for no-clip: Enabled";
        }
        else if (noClip == false)
        {
            noClipText.text = "Press F1 for no-clip: Disabled";
        }

        // Instant Throw text change
        if (instantThrow)
        {
            instantThrowText.text = "Press F2 for 'instant' shroom throw: Enabled";
        }
        else if (instantThrow == false)
        {
            instantThrowText.text = "Press F2 for 'instant' shroom throw: Disabled";
        }

        // Infinite shrooms text change
        if (infiniteShrooms)
        {
            infiniteShroomText.text = "Press F3 for infinite shrooms: Enabled";
        }
        else if (infiniteShrooms == false)
        {
            infiniteShroomText.text = "Press F3 for infinite shrooms: Disabled";
        }
    }

    #region INPUT HANDLERS
    public void OnActivateNoClip(InputAction.CallbackContext context)
    {
        // Check if devTools is enabled
        if (devToolsEnabled)
        {
            // Switch noClip 
            noClip = !noClip;

            // Switch collider's isTrigger bool
            playerCollider.isTrigger = !playerCollider.isTrigger;
            capsuleCollider.isTrigger = !capsuleCollider.isTrigger;

            // Check if the rigid body is dynamic type, if it is then set it to static
            if (playerMove.RB.bodyType == RigidbodyType2D.Dynamic)
            {
                playerMove.RB.bodyType = RigidbodyType2D.Static;
            }
            // Otherwise set it to dynamic
            else
            {
                playerMove.RB.bodyType = RigidbodyType2D.Dynamic;
            }
        }
    }

    public void OnActivateInstantThrow(InputAction.CallbackContext context)
    {
        if (devToolsEnabled)
        {
            instantThrow = !instantThrow;

            if (instantThrow)
            {
                mushMan.ThrowMultiplier = 30;
            }
            else if (instantThrow == false)
            {
                mushMan.ThrowMultiplier = 8;
            }
        }
    }

    public void OnEnableInfiniteShrooms(InputAction.CallbackContext context)
    {
        if (devToolsEnabled)
        {
            infiniteShrooms = !infiniteShrooms;

            if (infiniteShrooms)
            {
                mushMan.MushroomLimit = int.MaxValue;
            }
            else if (infiniteShrooms == false)
            {
                mushMan.MushroomLimit = 3;
            }
        } 
    }
    #endregion
}
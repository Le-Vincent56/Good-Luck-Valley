using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    #region FIELDS
    private PlayerMovement playerMovement;
    private AudioSource footstepSound;
    private Animator playerAnimator;
    [SerializeField] string animatorClipName;
    private AnimatorClipInfo[] currentClipInfo;
    [SerializeField] float currentClipLength;
    float stepTimerMax;
    float stepTimer;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Get movement and sound components
        playerMovement = GetComponent<PlayerMovement>();
        footstepSound = GetComponent<AudioSource>();

        // Retrieve animator
        playerAnimator = GetComponent<Animator>();

        // Fetch the current Animation clip information for the base layer
        currentClipInfo = this.playerAnimator.GetCurrentAnimatorClipInfo(0);

        // Access the current length of the clip
        currentClipLength = currentClipInfo[0].clip.length;

        // Access the Animation clip name
        animatorClipName = currentClipInfo[0].clip.name;

        // Set step timers
        stepTimerMax = currentClipLength;
        stepTimer = stepTimerMax;
    }

    // Update is called once per frame
    void Update()
    {
        // Fetch the current Animation clip information for the base layer
        currentClipInfo = this.playerAnimator.GetCurrentAnimatorClipInfo(0);

        // Access the current length of the clip
        currentClipLength = currentClipInfo[0].clip.length;
        stepTimerMax = currentClipLength;

        // Access the Animation clip name
        animatorClipName = currentClipInfo[0].clip.name;

        // Check if the player is grounded, has horizontal input, and can make another noise
        if (playerMovement.isGrounded && playerMovement.inputHorizontal && 
            (stepTimer <= stepTimerMax / 2f || stepTimer <= 0f) && animatorClipName == "Player_Run")
        {
            // Create a random variation of the noise
            footstepSound.volume = Random.Range(0.3f, 0.5f);
            footstepSound.pitch = Random.Range(2.0f, 2.5f);

            // Play the noise
            footstepSound.Play();

            // Reset step timer
            stepTimer = stepTimerMax;
        } else if(stepTimer > 0f)
        {
            // If step timer is greater than 0, keep reducing
            stepTimer -= Time.deltaTime;
        }
    }
}

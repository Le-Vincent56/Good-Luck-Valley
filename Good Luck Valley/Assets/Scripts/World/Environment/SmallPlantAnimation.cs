using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallPlantAnimation : MonoBehaviour
{
    #region REFERENCES
    Animator animator;
    #endregion

    #region FIELDS
    #endregion

    #region PROPERTIES
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log(collider.tag);

        if (collider.attachedRigidbody.velocity.x > 0)
        {
            animator.Play("RotateSmallPlantRight");
        }
        else if (collider.attachedRigidbody.velocity.x < 0)
        {
            animator.Play("RotateSmallPlantLeft");
        }
        else
        {
            Debug.Log("Something else happened...");
            Debug.Log(collider.attachedRigidbody.velocity.x);
            if (collider.transform.position.x > transform.position.x) 
            {
                animator.Play("RotateSmallPlantLeft");
            }
            else if (collider.transform.position.x < transform.position.x)
            {
                animator.Play("RotateSmallPlantRight");
            }
        }
    }
}
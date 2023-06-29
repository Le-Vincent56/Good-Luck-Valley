using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateOnCollision : MonoBehaviour
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

        if (collider.tag == "Player")
        {
            animator.SetTrigger("Animate");
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public interface IShroom
{
    /// <summary>
    /// Updates shroom counter's filling and timer
    /// </summary>
    void UpdateShroomCounter();

    /// <summary>
    /// Resets the mushroom counter to filled and plays the particle effect
    /// </summary>
    /// <param name="shroomIcon"></param>
    public void ResetCounter(GameObject shroomIcon)
    {
        shroomIcon.GetComponent<Image>().fillAmount = 1;
        shroomIcon.GetComponent<ParticleSystem>().Play();
    }

    /// <summary>
    /// Starts the mushroom counter's filling
    /// </summary>
    /// <param name="shroomIcon"></param>
    public void StartCounter(GameObject shroomIcon)
    {
        shroomIcon.GetComponent<Image>().fillAmount = 0;
    }

    /// <summary>
    /// When the mushroom hits an object
    /// </summary>
    public void OnCollisionEnter2D(Collision2D collision);

    /// <summary>
    /// Rotates and Freezes the mushroom 
    /// </summary>
    void RotateAndFreeze();
}

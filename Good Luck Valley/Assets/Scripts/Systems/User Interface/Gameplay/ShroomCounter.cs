using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShroomCounter : MonoBehaviour
{
    #region REFERENCES
    private SpriteRenderer shroomIcon1;
    private SpriteRenderer shroomIcon2;
    private SpriteRenderer shroomIcon3;
    private MushroomManager mushMan;
    private float originalR;
    private float originalG;
    private float originalB;
    private float newR;
    private float newG;
    private float newB;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        shroomIcon1 = GameObject.Find("Shroom Icon 1").GetComponent<SpriteRenderer>();
        shroomIcon2 = GameObject.Find("Shroom Icon 2").GetComponent<SpriteRenderer>();
        shroomIcon3 = GameObject.Find("Shroom Icon 3").GetComponent<SpriteRenderer>();
        mushMan = GameObject.Find("Mushroom Manager").GetComponent<MushroomManager>();
        originalR = shroomIcon1.color.r;
        originalG = shroomIcon1.color.g;
        originalB = shroomIcon1.color.b;
        newR = shroomIcon1.color.r - .5f;
        newG = shroomIcon1.color.g - .5f;
        newB = shroomIcon1.color.b - .5f;
    }

    // Update is called once per frame
    void Update()
    {
        // Check the amount of Mushrooms placed
        if (mushMan.ThrowUnlocked)
        {
            switch (mushMan.MushroomCount)
            {
                // If 0, show all the shroom icons
                case 0:
                    shroomIcon1.color = new Color(originalR, originalG, originalB, 1f);
                    shroomIcon2.color = new Color(originalR, originalG, originalB, 1f);
                    shroomIcon3.color = new Color(originalR, originalG, originalB, 1f);
                    break;

                // If 1, show two shroom icons
                case 1:
                    shroomIcon1.color = new Color(originalR, originalG, originalB, 1f);
                    shroomIcon2.color = new Color(originalR, originalG, originalB, 1f);
                    shroomIcon3.color = new Color(newR, newG, newB, .6f);
                    break;

                // If 2, show one shroom icon
                case 2:
                    shroomIcon1.color = new Color(originalR, originalG, originalB, 1f);
                    shroomIcon2.color = new Color(newR, newG, newB, .6f);
                    shroomIcon3.color = new Color(newR, newG, newB, .6f);
                    break;

                // If 3, show none of the shroom icons
                case 3:
                    shroomIcon1.color = new Color(newR, newG, newB, .6f);
                    shroomIcon2.color = new Color(newR, newG, newB, .6f);
                    shroomIcon3.color = new Color(newR, newG, newB, .6f);
                    break;
            }
        }
        else
        {
            shroomIcon1.color = new Color(newR, newG, newB, 0f);
            shroomIcon2.color = new Color(newR, newG, newB, 0f);
            shroomIcon3.color = new Color(newR, newG, newB, 0f);
        }
    }
}

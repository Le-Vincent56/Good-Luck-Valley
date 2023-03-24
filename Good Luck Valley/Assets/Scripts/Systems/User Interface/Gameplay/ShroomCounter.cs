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
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        shroomIcon1 = GameObject.Find("Shroom Icon 1").GetComponent<SpriteRenderer>();
        shroomIcon2 = GameObject.Find("Shroom Icon 2").GetComponent<SpriteRenderer>();
        shroomIcon3 = GameObject.Find("Shroom Icon 3").GetComponent<SpriteRenderer>();
        mushMan = GameObject.Find("Mushroom Manager").GetComponent<MushroomManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check the amount of Mushrooms placed
        switch (mushMan.MushroomCount)
        {
            // If 0, show all the shroom icons
            case 0:
                shroomIcon1.color = new Color(shroomIcon1.color.r, shroomIcon1.color.g, shroomIcon1.color.b, .8f);
                shroomIcon2.color = new Color(shroomIcon2.color.r, shroomIcon2.color.g, shroomIcon2.color.b, .8f);
                shroomIcon3.color = new Color(shroomIcon3.color.r, shroomIcon3.color.g, shroomIcon3.color.b, .8f);
                break;

            // If 1, show two shroom icons
            case 1:
                shroomIcon1.color = new Color(shroomIcon1.color.r, shroomIcon1.color.g, shroomIcon1.color.b, .8f);
                shroomIcon2.color = new Color(shroomIcon2.color.r, shroomIcon2.color.g, shroomIcon2.color.b, .8f);
                shroomIcon3.color = new Color(shroomIcon3.color.r, shroomIcon3.color.g, shroomIcon3.color.b, 0f);
                break;

            // If 2, show one shroom icon
            case 2:
                shroomIcon1.color = new Color(shroomIcon1.color.r, shroomIcon1.color.g, shroomIcon1.color.b, .8f);
                shroomIcon2.color = new Color(shroomIcon2.color.r, shroomIcon2.color.g, shroomIcon2.color.b, 0f);
                shroomIcon3.color = new Color(shroomIcon3.color.r, shroomIcon3.color.g, shroomIcon3.color.b, 0f);
                break;

            // If 3, show none of the shroom icons
            case 3:
                shroomIcon1.color = new Color(shroomIcon1.color.r, shroomIcon1.color.g, shroomIcon1.color.b, 0f);
                shroomIcon2.color = new Color(shroomIcon2.color.r, shroomIcon2.color.g, shroomIcon2.color.b, 0f);
                shroomIcon3.color = new Color(shroomIcon3.color.r, shroomIcon3.color.g, shroomIcon3.color.b, 0f);
                break;
        }

    }
}

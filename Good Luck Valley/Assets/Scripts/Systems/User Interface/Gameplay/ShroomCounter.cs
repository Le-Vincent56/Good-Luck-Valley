using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShroomCounter : MonoBehaviour
{
    #region REFERENCES
    private GameObject shroomIcon1;
    private GameObject shroomIcon2;
    private GameObject shroomIcon3;
    private Image shroomOutline1;
    private Image shroomOutline2;
    private Image shroomOutline3;
    private Image shroomFill1;
    private Image shroomFill2;
    private Image shroomFill3;
    private MushroomManager mushMan;
    #endregion

    #region FIELDS
    private float originalR;
    private float originalG;
    private float originalB;
    private float newR;
    private float newG;
    private float newB;
    private List<GameObject> shroomIconList;
    #endregion

    #region PROPERTIES
    public GameObject ShroomIcon1 { get { return shroomIcon1; } }
    public GameObject ShroomIcon2 { get { return shroomIcon2; } }
    public GameObject ShroomIcon3 { get { return shroomIcon3; } }
    public List<GameObject> ShroomIconQueue { get { return shroomIconList; } }
    public float oR { get { return originalR; } }
    public float oG { get { return originalG; } }
    public float oB { get { return originalB; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Figure this out after gettting a working build, less .Find calls
        // GameObject parent = gameObject;

        shroomIcon1 = GameObject.Find("Shroom Icon 1");
        shroomIcon2 = GameObject.Find("Shroom Icon 2");
        shroomIcon3 = GameObject.Find("Shroom Icon 3");
        mushMan = GameObject.Find("Mushroom Manager").GetComponent<MushroomManager>();
        originalR = shroomIcon1.GetComponent<Image>().color.r;
        originalG = shroomIcon1.GetComponent<Image>().color.g;
        originalB = shroomIcon1.GetComponent<Image>().color.b;
        newR = shroomIcon1.GetComponent<Image>().color.r - .5f;
        newG = shroomIcon1.GetComponent<Image>().color.g - .5f;
        newB = shroomIcon1.GetComponent<Image>().color.b - .5f;
        shroomIcon1.GetComponent<Image>().fillAmount = 0f;
        shroomIcon2.GetComponent<Image>().fillAmount = 0f;
        shroomIcon3.GetComponent<Image>().fillAmount = 0f;

        shroomOutline1 = GameObject.Find("Shroom Outline 1").GetComponent<Image>();
        shroomOutline2 = GameObject.Find("Shroom Outline 2").GetComponent<Image>();
        shroomOutline3 = GameObject.Find("Shroom Outline 3").GetComponent<Image>();
        
        shroomFill1 = GameObject.Find("ShroomFill 1").GetComponent<Image>();
        shroomFill2 = GameObject.Find("ShroomFill 2").GetComponent<Image>();
        shroomFill3 = GameObject.Find("ShroomFill 3").GetComponent<Image>();

        shroomIconList = new List<GameObject>()
        {
            shroomIcon3,
            shroomIcon2,
            shroomIcon1
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (!mushMan.ThrowUnlocked)
        {
            shroomIcon1.GetComponent<Image>().color = new Color(originalR, originalG, originalB, 0);
            shroomIcon2.GetComponent<Image>().color = new Color(originalR, originalG, originalB, 0);
            shroomIcon3.GetComponent<Image>().color = new Color(originalR, originalG, originalB, 0);
            shroomOutline1.color = new Color(originalR, originalG, originalB, 0);
            shroomOutline2.color = new Color(originalR, originalG, originalB, 0);
            shroomOutline3.color = new Color(originalR, originalG, originalB, 0);
            shroomFill1.color = new Color(originalR, originalG, originalB, 0);
            shroomFill2.color = new Color(originalR, originalG, originalB, 0);
            shroomFill3.color = new Color(originalR, originalG, originalB, 0);
        }   
        else
        {
            shroomOutline1.color = new Color(originalR, originalG, originalB, 1);
            shroomOutline2.color = new Color(originalR, originalG, originalB, 1);
            shroomOutline3.color = new Color(originalR, originalG, originalB, 1);
            shroomFill1.color = new Color(originalR, originalG, originalB, 0.3882353f);
            shroomFill2.color = new Color(originalR, originalG, originalB, 0.3882353f);
            shroomFill3.color = new Color(originalR, originalG, originalB, 0.3882353f);
        }
    }

    /// <summary>
    /// Resets the queue and the color values of all shroom icons
    /// </summary>
    public void ResetQueue()
    {
        // Clears the queue
        
        if (!shroomIconList.Contains(shroomIcon1))
        {
            shroomIcon1.GetComponent<ParticleSystem>().Play();
        }

        if (!shroomIconList.Contains(shroomIcon2))
        {
            shroomIcon2.GetComponent<ParticleSystem>().Play();
        }

        if (!shroomIconList.Contains(shroomIcon3))
        {
            shroomIcon3.GetComponent<ParticleSystem>().Play();
        }

        shroomIconList.Clear();
        shroomIconList.Add(shroomIcon3);
        // Enqueues the first shroom icon and resets its color values
        shroomIcon3.GetComponent<Image>().fillAmount = 1;
        shroomIconList.Add(shroomIcon2);
        // Enqueues the first shroom icon and resets its color values
        shroomIcon2.GetComponent<Image>().fillAmount = 1;
        shroomIconList.Add(shroomIcon1);
        // Enqueues the first shroom icon and resets its color values
        shroomIcon1.GetComponent<Image>().fillAmount = 1;
    }
}

using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShroomCounter : MonoBehaviour
{
    #region REFERENCES
    private GameObject shroomIcon1;
    private GameObject shroomIcon2;
    private GameObject shroomIcon3;
    private MushroomManager mushMan;
    #endregion

    #region FIELDS
    private float originalR;
    private float originalG;
    private float originalB;
    private float newR;
    private float newG;
    private float newB;
    private Queue<GameObject> shroomIconQueue;
    #endregion

    #region PROPERTIES
    public GameObject ShroomIcon1 { get { return shroomIcon1; } }
    public GameObject ShroomIcon2 { get { return shroomIcon2; } }
    public GameObject ShroomIcon3 { get { return shroomIcon3; } }
    public Queue<GameObject> ShroomIconQueue { get { return shroomIconQueue; } }
    public float oR { get { return originalR; } }
    public float oG { get { return originalG; } }
    public float oB { get { return originalB; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        shroomIcon1 = GameObject.Find("Shroom Icon 1");
        shroomIcon2 = GameObject.Find("Shroom Icon 2");
        shroomIcon3 = GameObject.Find("Shroom Icon 3");
        mushMan = GameObject.Find("Mushroom Manager").GetComponent<MushroomManager>();
        originalR = shroomIcon1.GetComponent<SpriteRenderer>().color.r;
        originalG = shroomIcon1.GetComponent<SpriteRenderer>().color.g;
        originalB = shroomIcon1.GetComponent<SpriteRenderer>().color.b;
        newR = shroomIcon1.GetComponent<SpriteRenderer>().color.r - .5f;
        newG = shroomIcon1.GetComponent<SpriteRenderer>().color.g - .5f;
        newB = shroomIcon1.GetComponent<SpriteRenderer>().color.b - .5f;
        shroomIcon1.GetComponent<Image>().fillAmount = 0f;
        shroomIcon2.GetComponent<Image>().fillAmount = 0f;
        shroomIcon3.GetComponent<Image>().fillAmount = 0f;

        shroomIconQueue = new Queue<GameObject>();
        shroomIconQueue.Enqueue(shroomIcon3);
        shroomIconQueue.Enqueue(shroomIcon2);
        shroomIconQueue.Enqueue(shroomIcon1);
    }

    // Update is called once per frame
    void Update()
    {
        if (!mushMan.ThrowUnlocked)
        {
            shroomIcon1.GetComponent<SpriteRenderer>().color = new Color(originalR, originalG, originalB, 1);
            shroomIcon2.GetComponent<SpriteRenderer>().color = new Color(originalR, originalG, originalB, 1);
            shroomIcon3.GetComponent<SpriteRenderer>().color = new Color(originalR, originalG, originalB, 1);
        }
    }

    /// <summary>
    /// Resets the queue and the color values of all shroom icons
    /// </summary>
    public void ResetQueue()
    {
        // Clears the queue
        shroomIconQueue.Clear();

        // Enqueues the third shroom icon and resets its color values
        shroomIcon3.GetComponent<SpriteRenderer>().color = new Color(originalR, originalG, originalB, 1f);
        shroomIcon3.GetComponent<Image>().fillAmount = 0;
        shroomIconQueue.Enqueue(shroomIcon3);

        // Enqueues the second shroom icon and resets its color values
        shroomIcon2.GetComponent<SpriteRenderer>().color = new Color(originalR, originalG, originalB, 1f);
        shroomIcon2.GetComponent<Image>().fillAmount = 0;
        shroomIconQueue.Enqueue(shroomIcon2);

        // Enqueues the first shroom icon and resets its color values
        shroomIcon1.GetComponent<SpriteRenderer>().color = new Color(originalR, originalG, originalB, 1f);
        shroomIcon1.GetComponent<Image>().fillAmount = 0;
        shroomIconQueue.Enqueue(shroomIcon1);
    }
}

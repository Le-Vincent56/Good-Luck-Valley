using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public GameObject player;
    private PlayerMovement pM;
    private float camLeftBound;
    private float camRightBound;

    // Fields for background trees (shadow trees)
    public GameObject bgTrees1;
    public GameObject bgTrees2;
    private float bgTrees1LeftBound;
    private float bgTrees2LeftBound;
    private float bgTrees1RightBound;
    private float bgTrees2RightBound;
    private float bgTreesWidth;

    // Fields for foreground trees (closest trees)
    public GameObject fgTrees1;
    public GameObject fgTrees2;
    private float fgTrees1LeftBound;
    private float fgTrees2LeftBound;
    private float fgTrees1RightBound;
    private float fgTrees2RightBound;
    private float fgTreesWidth;

    // Parallax Scrolling Fields
    [SerializeField] private float bgTreesParallaxSpeed;
    [SerializeField] private float fgTreesParallaxSpeed;

    // Start is called before the first frame update
    void Start()
    {
        pM = player.GetComponent<PlayerMovement>();
        InitializeBGTreesFields();
        InitializeFGTreesFields();
        camLeftBound = player.transform.position.x - 7.5f;
        camRightBound = player.transform.position.x + 7.5f;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFields();
        CheckBGTreesMove();
        CheckFGTreesMove();
        if (pM._isMoving && pM.RB.velocity.x > 0)
        {
            bgTreesParallaxSpeed = Mathf.Abs(bgTreesParallaxSpeed);
            fgTreesParallaxSpeed = Mathf.Abs(fgTreesParallaxSpeed);
            ParallaxScrolling();
        }
        else if (pM._isMoving && pM.RB.velocity.x < 0)
        {
            bgTreesParallaxSpeed = Mathf.Abs(bgTreesParallaxSpeed) * -1;
            fgTreesParallaxSpeed = Mathf.Abs(fgTreesParallaxSpeed) * -1;
            ParallaxScrolling();
        }
    }

    private void CheckBGTreesMove()
    {
        if (bgTrees1LeftBound > camRightBound)
        {
            bgTrees1.transform.position = new Vector2(camLeftBound - (bgTreesWidth / 2), 0f);
        }
        if (bgTrees1RightBound < camLeftBound)
        {
            bgTrees1.transform.position = new Vector2(camRightBound + (bgTreesWidth / 2), 0f);
        }
        if (bgTrees2LeftBound > camRightBound)
        {
            bgTrees2.transform.position = new Vector2(camLeftBound - (bgTreesWidth / 2), 0f);
        }
        if (bgTrees2RightBound < camLeftBound)
        {
            bgTrees2.transform.position = new Vector2(camRightBound + (bgTreesWidth / 2), 0f);
        }
    }
    private void CheckFGTreesMove()
    {
        if (fgTrees1LeftBound > camRightBound)
        {
            fgTrees1.transform.position = new Vector2(camLeftBound - (fgTreesWidth / 2), 0f);
        }
        if (fgTrees1RightBound < camLeftBound)
        {
            fgTrees1.transform.position = new Vector2(camRightBound + (fgTreesWidth / 2), 0f);
        }
        if (fgTrees2LeftBound > camRightBound)
        {
            fgTrees2.transform.position = new Vector2(camLeftBound - (fgTreesWidth / 2), 0f);
        }
        if (fgTrees2RightBound < camLeftBound)
        {
            fgTrees2.transform.position = new Vector2(camRightBound + (fgTreesWidth / 2), 0f);
        }
    }

    private void UpdateFields()
    {
        bgTrees1LeftBound = bgTrees1.transform.position.x - (bgTreesWidth / 2f);
        bgTrees2LeftBound = bgTrees2.transform.position.x - (bgTreesWidth / 2f);
        bgTrees1RightBound = bgTrees1.transform.position.x + (bgTreesWidth / 2f);
        bgTrees2RightBound = bgTrees2.transform.position.x + (bgTreesWidth / 2f);
        fgTrees1LeftBound = fgTrees1.transform.position.x - (fgTreesWidth / 2f);
        fgTrees2LeftBound = fgTrees2.transform.position.x - (fgTreesWidth / 2f);
        fgTrees1RightBound = fgTrees1.transform.position.x + (fgTreesWidth / 2f);
        fgTrees2RightBound = fgTrees2.transform.position.x + (fgTreesWidth / 2f);
        camLeftBound = player.transform.position.x - 7.5f;
        camRightBound = player.transform.position.x + 7.5f;
    }

    private void ParallaxScrolling()
    {
        bgTrees1.transform.position = new Vector2(bgTrees1.transform.position.x - bgTreesParallaxSpeed, 0f);
        bgTrees2.transform.position = new Vector2(bgTrees2.transform.position.x - bgTreesParallaxSpeed, 0f);
        fgTrees1.transform.position = new Vector2(fgTrees1.transform.position.x - fgTreesParallaxSpeed, 0f);
        fgTrees2.transform.position = new Vector2(fgTrees2.transform.position.x - fgTreesParallaxSpeed, 0f);
        UpdateFields();
    }

    private void InitializeBGTreesFields()
    {
        bgTreesWidth = 19f;
        bgTrees1.transform.position = new Vector2(player.transform.position.x, 0f);
        bgTrees2.transform.position = new Vector2(player.transform.position.x + bgTreesWidth, 0f);
        bgTrees1LeftBound = bgTrees1.transform.position.x - (bgTreesWidth / 2f);
        bgTrees2LeftBound = bgTrees2.transform.position.x - (bgTreesWidth / 2f);
        bgTrees1RightBound = bgTrees1.transform.position.x + (bgTreesWidth / 2f);
        bgTrees2RightBound = bgTrees2.transform.position.x + (bgTreesWidth / 2f);
    }

    private void InitializeFGTreesFields()
    {
        fgTreesWidth = 10f;
        fgTrees1.transform.position = new Vector2(player.transform.position.x, 0f);
        fgTrees2.transform.position = new Vector2(player.transform.position.x + bgTreesWidth, 0f);
        fgTrees1LeftBound = fgTrees1.transform.position.x - (fgTreesWidth / 2f);
        fgTrees2LeftBound = fgTrees2.transform.position.x - (fgTreesWidth / 2f);
        fgTrees1RightBound =fgTrees1.transform.position.x + (fgTreesWidth / 2f);
        fgTrees2RightBound =fgTrees2.transform.position.x + (fgTreesWidth / 2f);
    }
}

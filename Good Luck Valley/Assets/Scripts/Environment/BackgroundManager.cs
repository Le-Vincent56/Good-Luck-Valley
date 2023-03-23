using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    #region REFERENCES
    private GameObject player;
    private PauseMenu pauseMenu;
    private GameObject cam;
    private PlayerMovement pM;

    [Header("Background Trees Colored")]
    [SerializeField] private GameObject bgTrees1;
    [SerializeField] private GameObject bgTrees2;

    [Header("Background Trees Shadow")]
    [SerializeField] private GameObject fgTrees1;
    [SerializeField] private GameObject fgTrees2;

    [Header("Clouds")]
    [SerializeField] private GameObject cloud1;
    [SerializeField] private GameObject cloud2;
    #endregion

    #region FIELDS
    // Camera
    private float camLeftBound;
    private float camRightBound;

    // Fields for background trees (shadow trees)
    private float bgTrees1LeftBound;
    private float bgTrees2LeftBound;
    private float bgTrees1RightBound;
    private float bgTrees2RightBound;
    private float bgTreesWidth;

    // Fields for foreground trees (closest trees)
    private float fgTrees1LeftBound;
    private float fgTrees2LeftBound;
    private float fgTrees1RightBound;
    private float fgTrees2RightBound;
    private float fgTreesWidth;

    // Fields for clouds
    private float cloud1LeftBound;
    private float cloud2LeftBound;
    private float cloud1RightBound;
    private float cloud2RightBound;
    private float cloud1Width;
    private float cloud2Width;
    private float cloud1YPos;
    private float cloud2YPos;

    // Parallax Scrolling Fields
    [Header("Parallax Speeds")]
    [SerializeField] private float bgTreesParallaxSpeed;
    [SerializeField] private float fgTreesParallaxSpeed;
    [SerializeField] private float cloudParallaxSpeed;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        pauseMenu = GameObject.Find("PauseUI").GetComponent<PauseMenu>();
        cam = GameObject.Find("Main Camera");

        pM = player.GetComponent<PlayerMovement>();
        //InitializeBGTreesFields();
        //InitializeFGTreesFields();
        InitializeCloudsFields();
        camLeftBound = cam.transform.position.x - 9f;
        camRightBound = cam.transform.position.x + 9f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!pauseMenu.Paused)
        {
            UpdateFields();

            MoveClouds();

            // Commented out for now
            //CheckBGTreesMove();
            //CheckFGTreesMove();
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

    private void CheckCloudsMove()
    {
        if (cloud1LeftBound > camRightBound)
        {
            cloud1.transform.position = new Vector2(camLeftBound - (cloud1Width / 2), cloud1YPos);
        }
        if (cloud1RightBound < camLeftBound)
        {
            cloud1.transform.position = new Vector2(camRightBound + (cloud1Width / 2), cloud1YPos);
        }
        if (cloud2LeftBound > camRightBound)
        {
            cloud2.transform.position = new Vector2(camLeftBound - (cloud2Width / 2), cloud2YPos);
        }
        if (cloud2RightBound < camLeftBound)
        {
            cloud2.transform.position = new Vector2(camRightBound + (cloud2Width / 2), cloud2YPos);
        }

    }

    private void UpdateFields()
    {
        // Colored trees
        // bgTrees1LeftBound = bgTrees1.transform.position.x - (bgTreesWidth / 2f);
        // bgTrees2LeftBound = bgTrees2.transform.position.x - (bgTreesWidth / 2f);
        // bgTrees1RightBound = bgTrees1.transform.position.x + (bgTreesWidth / 2f);
        // bgTrees2RightBound = bgTrees2.transform.position.x + (bgTreesWidth / 2f);

        // Shadow Trees
        // fgTrees1LeftBound = fgTrees1.transform.position.x - (fgTreesWidth / 2f);
        // fgTrees2LeftBound = fgTrees2.transform.position.x - (fgTreesWidth / 2f);
        // fgTrees1RightBound = fgTrees1.transform.position.x + (fgTreesWidth / 2f);
        // fgTrees2RightBound = fgTrees2.transform.position.x + (fgTreesWidth / 2f);

        // Clouds
        cloud1RightBound = cloud1.transform.position.x + (cloud1Width / 2f);
        cloud2RightBound = cloud2.transform.position.x + (cloud2Width / 2f);
        cloud1LeftBound = cloud1.transform.position.x - (cloud1Width / 2f);
        cloud2LeftBound = cloud2.transform.position.x - (cloud2Width / 2f);

        // Camera
        camLeftBound = cam.transform.position.x - 9f;
        camRightBound = cam.transform.position.x + 9f;
    }

    private void ParallaxScrolling()
    {
        //bgTrees1.transform.position = new Vector2(bgTrees1.transform.position.x - bgTreesParallaxSpeed, 0f);
        //bgTrees2.transform.position = new Vector2(bgTrees2.transform.position.x - bgTreesParallaxSpeed, 0f);
        //fgTrees1.transform.position = new Vector2(fgTrees1.transform.position.x - fgTreesParallaxSpeed, 0f);
        //fgTrees2.transform.position = new Vector2(fgTrees2.transform.position.x - fgTreesParallaxSpeed, 0f);
        UpdateFields();
    }
    
    private void MoveClouds()
    {
        CheckCloudsMove();
        cloud1.transform.position = new Vector2(cloud1.transform.position.x - cloudParallaxSpeed, cloud1YPos);
        cloud2.transform.position = new Vector2(cloud2.transform.position.x - cloudParallaxSpeed, cloud2YPos);
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

    private void InitializeCloudsFields()
    {
        cloud1Width = 18f;
        cloud2Width = 18f;
        cloud1YPos = 7f;
        cloud2YPos = 7f;
        cloud1.transform.position = new Vector2(player.transform.position.x, cloud1YPos);
        cloud2.transform.position = new Vector2(player.transform.position.x + cloud2Width, cloud2YPos);
        cloud1LeftBound = cloud1.transform.position.x - (cloud1Width / 2f);
        cloud2LeftBound = cloud2.transform.position.x - (cloud2Width / 2f);
        cloud1RightBound = cloud1.transform.position.x + (cloud1Width / 2f);
        cloud2RightBound = cloud2.transform.position.x + (cloud2Width / 2f);
    }
}

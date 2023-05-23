using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    #region REFERENCES
    private PauseMenu pauseMenu;
    private GameObject cam;
    #endregion

    #region FIELDS
    // Camera
    private float camLeftBound;
    private float camRightBound;

    // Fields for clouds
    [SerializeField] private List<CloudEffect> clouds;

    // Parallax Scrolling Fields
    [Header("Parallax Speeds")]
    [SerializeField] private float cloudParallaxSpeed;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu = GameObject.Find("PauseUI").GetComponent<PauseMenu>();
        cam = GameObject.Find("Main Camera");

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
        }
    }

    private void CheckCloudsMove()
    {
        foreach (CloudEffect cloud in clouds)
        {
            if (cloud.CloudLB > camRightBound)
            {
                cloud.transform.position = new Vector2(camLeftBound - (cloud.CloudWidth / 2), cloud.CloudY);
            }
            if (cloud.CloudRB < camLeftBound)
            {
                cloud.transform.position = new Vector2(camRightBound + (cloud.CloudWidth / 2), cloud.CloudY);
            }
        }

        //if (cloud1LeftBound > camRightBound)
        //{
        //    cloud1.transform.position = new Vector2(camLeftBound - (cloud1Width / 2), cloud1YPos);
        //}
        //if (cloud1RightBound < camLeftBound)
        //{
        //    cloud1.transform.position = new Vector2(camRightBound + (cloud1Width / 2), cloud1YPos);
        //}
        //if (cloud2LeftBound > camRightBound)
        //{
        //    cloud2.transform.position = new Vector2(camLeftBound - (cloud2Width / 2), cloud2YPos);
        //}
        //if (cloud2RightBound < camLeftBound)
        //{
        //    cloud2.transform.position = new Vector2(camRightBound + (cloud2Width / 2), cloud2YPos);
        //}
    }

    private void UpdateFields()
    {
        // Clouds
        foreach(CloudEffect cloud in clouds)
        {
            cloud.CloudRB = cloud.transform.position.x + (cloud.CloudWidth / 2f);
            cloud.CloudLB = cloud.transform.position.x - (cloud.CloudWidth / 2f);
        }

        //cloud1RightBound = cloud1.transform.position.x + (cloud1Width / 2f);
        //cloud2RightBound = cloud2.transform.position.x + (cloud2Width / 2f);
        //cloud1LeftBound = cloud1.transform.position.x - (cloud1Width / 2f);
        //cloud2LeftBound = cloud2.transform.position.x - (cloud2Width / 2f);

        // Camera
        camLeftBound = cam.transform.position.x - 9f;
        camRightBound = cam.transform.position.x + 9f;
    }
    
    private void MoveClouds()
    {
        CheckCloudsMove();
        foreach(CloudEffect cloud in clouds)
        {
            cloud.transform.position = new Vector2(cloud.transform.position.x - cloud.CloudSpeed, cloud.CloudY);
        }
        //cloud1.transform.position = new Vector2(cloud1.transform.position.x - cloudParallaxSpeed, cloud1YPos);
        //cloud2.transform.position = new Vector2(cloud2.transform.position.x - cloudParallaxSpeed, cloud2YPos);
    }

    private void InitializeCloudsFields()
    {
        //cloud1Width = 18f;
        //cloud2Width = 18f;
        //cloud1YPos = 7f;
        //cloud2YPos = 7f;
        //cloud1.transform.position = new Vector2(player.transform.position.x, cloud1YPos);
        //cloud2.transform.position = new Vector2(player.transform.position.x + cloud2Width, cloud2YPos);
        //cloud1LeftBound = cloud1.transform.position.x - (cloud1Width / 2f);
        //cloud2LeftBound = cloud2.transform.position.x - (cloud2Width / 2f);
        //cloud1RightBound = cloud1.transform.position.x + (cloud1Width / 2f);
        //cloud2RightBound = cloud2.transform.position.x + (cloud2Width / 2f);
    }
}

using GoodLuckValley.Entities;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionLevel : MonoBehaviour
{
    private AreaCollider areaCollider;
    [SerializeField] private GameObject demoEndText;

    [SerializeField] public string sceneToTransitionTo;

    private void Awake()
    {
        areaCollider = GetComponent<AreaCollider>();
    }

    private void OnEnable()
    {
        areaCollider.OnTriggerEnter += GoToLevel;
        areaCollider.OnTriggerExit += RemoveText;
    }

    private void OnDisable()
    {
        areaCollider.OnTriggerEnter -= GoToLevel;
        areaCollider.OnTriggerExit += RemoveText;
    }

    private void GoToLevel(GameObject gameObject)
    {
        //SceneManager.LoadScene(sceneToTransitionTo);
        // Temp for build:
        demoEndText.SetActive(true);
    }

    private void RemoveText(GameObject gameObject)
    {
        //SceneManager.LoadScene(sceneToTransitionTo);
        // Temp for build:
        demoEndText.SetActive(false);
    }

    public void GoToThisLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
}

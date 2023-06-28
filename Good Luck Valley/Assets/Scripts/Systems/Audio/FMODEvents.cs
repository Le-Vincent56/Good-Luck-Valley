using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Ambience")]
    [field: SerializeField] public EventReference Ambience { get; private set; }
    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference PlayerFootsteps { get; private set; }

    [field: Header("Music")]
    [field: SerializeField] public EventReference GameMusic { get; private set; }
    [field: SerializeField] public EventReference TitleMusic { get; private set; }
    [field: SerializeField] public EventReference ForestMusic { get; private set; }

    public static FMODEvents Instance { get; private set; }


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one FMODEvents instance in the scene");
        }
        Instance = this;
    }
}

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
    [field: SerializeField] public List<EventReference> BushRustles { get; private set; }

    [field: Header("Music")]
    [field: SerializeField] public EventReference GameMusic { get; private set; }

    public static FMODEvents Instance { get; private set; }


    private void Awake()
    {
        // Check if there's already an FMODEvents
        if (Instance != null)
        {
            // If there is, destroy this one to retain singleton design
            Debug.LogWarning("Found more than one FMOD Events in the scene. Destroying the newest one");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}

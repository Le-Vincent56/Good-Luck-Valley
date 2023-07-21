using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Ambience")]
    [field: SerializeField] public EventReference Ambience { get; private set; }
    [field: SerializeField] public List<EventReference> BirdCalls { get; private set; }
    [field: SerializeField] public List<EventReference> Crickets { get; private set; }
    [field: SerializeField] public List<EventReference> TreeSettles { get; private set; }
    [field: SerializeField] public List<EventReference> Wind { get; private set; }

    [field: Header("UI")]
    [field: SerializeField] public EventReference UIButton { get; private set; }
    [field: SerializeField] public EventReference UICheckmark { get; private set; }
    [field: SerializeField] public EventReference UITab { get; private set; }

    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference PlayerFootstepsGrass { get; private set; }
    [field: SerializeField] public EventReference PlayerLandGrass { get; private set; }
    [field: SerializeField] public EventReference PlayerFootstepsDirt { get; private set; }
    [field: SerializeField] public EventReference PlayerLandDirt { get; private set; }
    [field: SerializeField] public List<EventReference> ShroomBounces { get; private set; }
    [field: SerializeField] public EventReference ShroomPlant { get; private set; }
    [field: SerializeField] public EventReference ShroomThrow { get; private set; }

    [field: Header("Journal SFX")]
    [field: SerializeField] public EventReference JournalOpen { get; private set; }
    [field: SerializeField] public EventReference JournalClose { get; private set; }
    [field: SerializeField] public EventReference JournalEntrySelected { get; private set; }
    [field: SerializeField] public EventReference NotePickup { get; private set; }


    [field: Header("Environment SFX - Forest")]
    [field: SerializeField] public List<EventReference> BushRustles { get; private set; }
    [field: SerializeField] public EventReference VineFlee { get; private set; }


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

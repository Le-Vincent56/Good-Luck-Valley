using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResetAllBindings : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset inputActions;

    public void ResetBindings()
    {
        // Play sound
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIButton, transform.position);

        foreach (InputActionMap map in inputActions.actionMaps)
        {
            map.RemoveAllBindingOverrides();
        }
        PlayerPrefs.DeleteKey("rebinds");
    }
}

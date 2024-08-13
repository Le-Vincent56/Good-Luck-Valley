using GoodLuckValley.Player.Control;
using GoodLuckValley.Player.Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static GoodLuckValley.Player.Input.PauseInputActions;

[CreateAssetMenu(fileName = "PauseInputReader", menuName = "Input/Pause Input Reader")]
public class PauseInputReader : ScriptableObject, IUIActions, IInputReader
{
    PauseInputActions inputActions;

    public event UnityAction<bool> Cancel = delegate { };
    public event UnityAction<bool> Click = delegate { };
    public event UnityAction<bool> MiddleClick = delegate { };
    public event UnityAction<Vector2> Navigate = delegate { };
    public event UnityAction<Vector2> Point = delegate { };
    public event UnityAction<bool> RightClick = delegate { };
    public event UnityAction<Vector2> ScrollWheel = delegate { };
    public event UnityAction<bool> Submit = delegate { };
    public event UnityAction<bool> Escape = delegate { };
    public event UnityAction<bool> OpenJournal = delegate { };

    private void OnEnable()
    {
        // Check if the input actions have been set
        if (inputActions == null)
        {
            // If not, create a new one and set callbacks
            inputActions = new PauseInputActions();
            inputActions.UI.SetCallbacks(this);
        }

        // Enable the input actions
        inputActions.Enable();
    }

    /// <summary>
    /// Enable the input actions map
    /// </summary>
    public void Enable() => inputActions.Enable();

    /// <summary>
    /// Disable the input actions map
    /// </summary>
    public void Disable() => inputActions.Disable();

    public void OnCancel(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            Cancel.Invoke(context.started);
        } else if (context.canceled)
        {
            Cancel.Invoke(context.started);
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Click.Invoke(context.started);
        }
        else if (context.canceled)
        {
            Click.Invoke(context.started);
        }
    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            MiddleClick.Invoke(context.started);
        }
        else if (context.canceled)
        {
            MiddleClick.Invoke(context.started);
        }
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        Navigate.Invoke(context.ReadValue<Vector2>());
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Navigate.Invoke(context.ReadValue<Vector2>());
        }
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            RightClick.Invoke(context.started);
        }
        else if (context.canceled)
        {
            RightClick.Invoke(context.started);
        }
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Navigate.Invoke(context.ReadValue<Vector2>());
        }
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Submit.Invoke(context.started);
        }
        else if (context.canceled)
        {
            Submit.Invoke(context.started);
        }
    }

    public void OnEscape(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Escape.Invoke(context.started);
        }
        else if (context.canceled)
        {
            Escape.Invoke(context.started);
        }
    }

    public void OnOpenJournal(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OpenJournal.Invoke(context.started);
        }
        else if (context.canceled)
        {
            OpenJournal.Invoke(context.started);
        }
    }

    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context) { }

    public void OnTrackedDevicePosition(InputAction.CallbackContext context) { }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TypedEvent:UnityEvent<object>
{

}

public class EventManager : MonoBehaviour
{
    private Dictionary<string, UnityEvent> eventDictionary;
    private Dictionary<string, TypedEvent> typedEventDictionary;
    private static EventManager eventManager;
    public static EventManager Instance
    {
        get
        {
            if(!eventManager)
            {
                // Try to find an eventManager if there is not one
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                // Check again if eventManager exists
                if(!eventManager)
                {
                    // If not, log an error
                    Debug.LogError("There needs to be one active EventManager script on a GameObject in your scene");
                } else
                {
                    // If there is an eventManager, initialize it
                    eventManager.Init();
                }
            }

            // Return the eventManager
            return eventManager;
        }
    }

    /// <summary>
    /// Initialiez the eventDictionary
    /// </summary>
    private void Init()
    {
        if(eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, UnityEvent>();
            typedEventDictionary = new Dictionary<string, TypedEvent>();
        }
    }

    /// <summary>
    /// Add a UnityAction listener to an existing event, or create a new one
    /// </summary>
    /// <param name="eventName">The name of the event</param>
    /// <param name="listener">A function-pointer that will listen for events</param>
    public static void StartListening(string eventName, UnityAction listener)
    {
        // Check the dictionary for a key-value pair
        UnityEvent thisEvent = null;

        // If the eventName exists, then output the event to thisEvent
        if(Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            // If the event exists, add the listener
            thisEvent.AddListener(listener);
        } else
        {
            // Create a new UnityEvent
            thisEvent = new UnityEvent();

            // Add the listener to the new UnityEvent
            thisEvent.AddListener(listener);

            // Add this new event to the eventDictionary
            Instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StartListening(string eventName, UnityAction<object> listener)
    {
        // Check the dictionary for a key-value pair
        TypedEvent thisEvent = null;

        // If the eventName exists, then output the event to thisEvent
        if (Instance.typedEventDictionary.TryGetValue(eventName, out thisEvent))
        {
            // If the event exists, add the listener
            thisEvent.AddListener(listener);
        }
        else
        {
            // Create a new UnityEvent
            thisEvent = new TypedEvent();

            // Add the listener to the new UnityEvent
            thisEvent.AddListener(listener);

            // Add this new event to the eventDictionary
            Instance.typedEventDictionary.Add(eventName, thisEvent);
        }
    }

    /// <summary>
    /// Remove a UnityAction listener from an event
    /// </summary>
    /// <param name="eventName">The name of the event</param>
    /// <param name="listener">A function-pointer that will listen for events</param>
    public static void StopListening(string eventName, UnityAction listener)
    {
        // If EventManager does not exist anymore, return immediately to prevent issues
        if(eventManager == null)
        {
            return;
        }

        // Check the dictionary for a key-value pair
        UnityEvent thisEvent = null;

        // If the eventName exists, then output the event to thisEvent
        if(Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            // If the event exists, remove the listener
            thisEvent.RemoveListener(listener);
        }
    }

    public static void StopListening(string eventName, UnityAction<object> listener)
    {
        // If EventManager does not exist anymore, return immediately to prevent issues
        if (eventManager == null)
        {
            return;
        }

        // Check the dictionary for a key-value pair
        TypedEvent thisEvent = null;

        // If the eventName exists, then output the event to thisEvent
        if (Instance.typedEventDictionary.TryGetValue(eventName, out thisEvent))
        {
            // If the event exists, remove the listener
            thisEvent.RemoveListener(listener);
        }
    }

    /// <summary>
    /// Trigger UnityAction event listeners of a specific event
    /// </summary>
    /// <param name="eventName">The name of the event</param>
    public static void TriggerEvent(string eventName)
    {
        // Check the dictionay for a key-value pair
        UnityEvent thisEvent = null;

        // If the eventName exists, then output the event to thisEvent
        if(Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            // If the event exists, invoke the listener
            thisEvent.Invoke();
        }
    }

    public static void TriggerEvent(string eventName, object data)
    {
        // Check the dictionay for a key-value pair
        TypedEvent thisEvent = null;

        // If the eventName exists, then output the event to thisEvent
        if (Instance.typedEventDictionary.TryGetValue(eventName, out thisEvent))
        {
            // If the event exists, invoke the listener
            thisEvent.Invoke(data);
        }
    }
}

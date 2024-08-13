using UnityEngine;

namespace GoodLuckValley.Patterns.Singletons
{
    public class Singleton<T> : MonoBehaviour where T :Component
    {
        protected static T instance;
        public static bool HasInstance => instance != null;
        public static T TryGetInstance() => HasInstance ? instance : null;

        public static T Instance
        {
            get
            {
                // Check if the instance is null
                if(instance == null)
                {
                    // Try to to find an object of type T
                    instance = FindAnyObjectByType<T>();

                    // If still null, create a game object and add the component
                    if(instance == null)
                    {
                        GameObject go = new GameObject(typeof(T).Name + " Auto Generated");
                        instance = go.AddComponent<T>();
                    }
                }

                return instance;
            }
        }

        protected virtual void Awake()
        {
            // Initialize the singleton
            InitializeSingleton();
        }

        /// <summary>
        /// Initialize the Singleton by setting its Instance
        /// </summary>
        protected virtual void InitializeSingleton()
        {
            // If in Editor mode, return
            if (!Application.isPlaying) return;

            // Set the instance
            instance = this as T;
        }
    }
}
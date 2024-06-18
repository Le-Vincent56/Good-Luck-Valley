using UnityEngine;

namespace GoodLuckValley.Patterns.Singletons
{
    public class PersistentSingleton<T> : MonoBehaviour where T : Component
    {
        public bool AutoUnparentOnAwake = true;
        protected static T instance;
        public static bool HasInstance => instance != null;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    // Find any object of the Persistent Singleton's Component type
                    instance = FindAnyObjectByType<T>();

                    // Auto generate an instance if no GameObject/Component is found
                    if (instance == null)
                    {
                        GameObject gameObject = new GameObject(typeof(T).Name + " (Auto-Generated)");
                        instance = gameObject.AddComponent<T>();
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Try to get the Persistent Singleton Instance
        /// </summary>
        /// <returns>The Instance if it exists, null if not</returns>
        public static T TryGetInstance() => HasInstance ? instance : null;

        protected virtual void Awake()
        {
            // Initialize the Persistent Singleton
            InitializeSingleton();
        }

        /// <summary>
        /// Initialize the Singleton
        /// </summary>
        protected virtual void InitializeSingleton()
        {
            // If the Application is in play (prevents being called in Edit Mode)
            if (!Application.isPlaying) return;

            // DontDestroyOnLoad only works on objects on the root level,
            // so make sure that there's no parent to this Persistent Singleton
            if(AutoUnparentOnAwake)
            {
                transform.SetParent(null);
            }

            if(instance == null)
            {
                // Set the instance and flag it to no
                // be destroyed on load
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            } else
            {
                // Destroy any clones
                if(instance != this)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
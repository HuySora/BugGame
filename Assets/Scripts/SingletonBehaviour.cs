namespace BugGame
{
    using System;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        public const string kCurrentSuffix = "(Current)";

        protected static T m_Current;
        public static T Current
        {
            get
            {
                if (m_Current != null) return m_Current;

                var instances = FindObjectsOfType<T>();
                var s1 = $"<b>[{nameof(T)}]</b>";

                // Found nothing
                if (instances.Length == 0)
                {
                    Debug.LogWarning($"No instance of type {s1} found.");
                }
                // Found
                else
                {
                    // Found more than 1
                    if (instances.Length > 1) Debug.LogWarning($"More than 1 instance of type {s1} found.", instances[0].gameObject);
                    
                    m_Current = instances[0];
                    m_Current.name = nameof(T) + kCurrentSuffix;
                    DontDestroyOnLoad(m_Current);
                }

                return m_Current;
            }
            private set
            {
                if (m_Current != null)
                {
                    m_Current.name = nameof(T);
                    SceneManager.MoveGameObjectToScene(m_Current.gameObject, SceneManager.GetActiveScene());
                }

                m_Current = value;
                m_Current.name += kCurrentSuffix;
                DontDestroyOnLoad(m_Current);
            }
        }
        
        protected void SingletonAwake()
        {
            if (Current != (T)Convert.ChangeType(this, typeof(T)))
            {
                var s1 = $"<b>[{nameof(T)}]</b>";
                Debug.LogWarning($"Another instance of type {s1} tried to Awake(), will disabled it", gameObject);
                Destroy(this);
            }
        }
    }
}


namespace BugGame.Serialization
{
    using MyBox;
    using System.Collections.Generic;
    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
    using System;

    public partial class DataManager
    {
        [ButtonMethod]
        public void EditorSave() => Save(Get());
        [ButtonMethod]
        public void EditorLoad() => Load();
    }
#endif

    public partial class DataManager : SingletonBehaviour<DataManager>
    {
        #region Static ----------------------------------------------------------------------------------------------------
        public static void Save(DataContainer container) => Current.Instance_Save(container);
        public static DataContainer Get() => Current.Instance_Get();
        public static DataContainer Load() => Current.Instance_Load();        
        #endregion

        [SerializeField] private string m_FolderPath;

        private string m_FullPath;
        private DataContainer m_CurrentData;

        private void Awake()
        {
            m_FullPath = Application.persistentDataPath + "/" + m_FolderPath;
        }

        private void Instance_Save(DataContainer container)
        {
            Debug.Log("DataManager.Save()");
        }

        /// <summary>
        /// Get the current data without reloading it
        /// </summary>
        private DataContainer Instance_Get()
        {
            // Try to load first
            if (m_CurrentData == null)
                m_CurrentData = Load();
            
            // Create new
            if (m_CurrentData == null)
            {
                m_CurrentData = new DataContainer();
                m_CurrentData.LevelToStarRating = new Dictionary<int, int>();
            }

            return m_CurrentData;
        }

        private DataContainer Instance_Load()
        {
            Debug.Log("DataManager.Load()");
            return m_CurrentData;
        }
    }
}


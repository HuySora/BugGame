namespace BugGame.Stage
{
    using MyBox;
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;

    public partial class StageManager
    {
        [ButtonMethod(ButtonMethodDrawOrder.BeforeInspector)]
        public void Generate100Seeds()
        {
            var newSeeds = new int[m_StageSeeds.Length + 100];
            // Get the 'old' one if we have some
            m_StageSeeds.CopyTo(newSeeds, 0);

            // Generate seeds
            for (int i = m_StageSeeds.Length; i < m_StageSeeds.Length + 100; i++)
            {
                newSeeds[i] = Random.Range(int.MinValue, int.MaxValue) + Random.Range(0, 2);
            }

            // Record then set to support undo
            Undo.RecordObject(this, "Generate100Seeds");
            m_StageSeeds = newSeeds;
        }

        [ContextMenu("Save")]
        public void Save()
        {

        }

        [ContextMenu("Load")]
        public void Load()
        {

        }
    }
#endif

    public partial class StageManager : SingletonBehaviour<StageManager>
    {
        #region Static ----------------------------------------------------------------------------------------------------
        public static void LoadStage(int index)
        {

        }
        #endregion
        [Header("Dependencies")]
        [SerializeField] private RectTransform m_ContentTransform;
        [SerializeField] private StageItem m_ItemPrefab;

        [SerializeField] private int[] m_StageSeeds;


        private void Awake()
        {
            SingletonAwake();
            Load();

            //System.Random random = new System.Random();
        }
    }
}
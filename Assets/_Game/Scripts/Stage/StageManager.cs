using BugGame.Serialization;

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
        public void EditorGenerate100Seeds()
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
    }
#endif

    public partial class StageManager : SingletonBehaviour<StageManager>
    {
        #region Static ----------------------------------------------------------------------------------------------------
        #endregion

        [Header("Dependencies")]
        [SerializeField] private RectTransform m_ContentTransform;
        [SerializeField] private StageItem m_ItemPrefab;

        [SerializeField] private int[] m_StageSeeds;

        private StageItem[] m_StageItems;

        private void Awake()
        {
            SingletonAwake();
        }

        private void Start()
        {
            m_StageItems = new StageItem[m_StageSeeds.Length];

            // Populate the UI with stages
            for (int i = 0; i < m_StageSeeds.Length; i++)
            {
                m_StageItems[i] = Instantiate(m_ItemPrefab, m_ContentTransform);
                Instance_RefreshStage(i);
            }
        }

        private void Instance_RefreshStage(int index)
        {
            var data = DataManager.Get();

            // Event on stage button click
            m_StageItems[index].Button.onClick.RemoveAllListeners();
            m_StageItems[index].Button.onClick.AddListener(() =>
            {
                int width = Mathf.Clamp(index + 1, 2, 20);
                int height = Mathf.Clamp(index + 4, 2, 20);
                GameManager.StartGame(width, height, Current.m_StageSeeds[index]);
            });

            // Star rating
            if (data.LevelToStarRating.ContainsKey(index))
                m_StageItems[index].Refresh(index, data.LevelToStarRating[index]);
        }
    }
}
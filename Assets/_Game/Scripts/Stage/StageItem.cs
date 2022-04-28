namespace BugGame.Stage
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

#if UNITY_EDITOR
    using UnityEditor;

    public partial class StageItem
    {
    }
#endif

    public partial class StageItem : MonoBehaviour
    {
        [field: SerializeField] public Button Button { get; private set; }
        [SerializeField] private TextMeshProUGUI m_NumberText;
        [SerializeField] private Image m_LockedImage;
        [SerializeField] private Image m_Star1;
        [SerializeField] private Image m_Star2;
        [SerializeField] private Image m_Star3;

        public void Refresh(int level, int star)
        {
            m_NumberText.text = level.ToString();
            m_Star1.enabled = star >= 1;
            m_Star2.enabled = star >= 2;
            m_Star3.enabled = star >= 3;
        }
    }
}


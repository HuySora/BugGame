namespace BugGame
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
        [SerializeField] private TextMeshProUGUI m_NumberText;
        [SerializeField] private Image m_LockedImage;
        [SerializeField] private Image m_Star1;
        [SerializeField] private Image m_Star2;
        [SerializeField] private Image m_Star3;
    }
}


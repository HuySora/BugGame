namespace BugGame
{
    using MyBox;
    using TMPro;
    using UnityEngine;

    public partial class StageIndexUpdater : MonoBehaviour
    {
        [SerializeField, AutoProperty] private TextMeshProUGUI m_StageText;

        private void OnEnable() => GameManager.MazeStarted += OnStageChanged;
        private void OnDisable() => GameManager.MazeStarted -= OnStageChanged;
        private void OnStageChanged(int index) => m_StageText.text = "Level: " + index;
    }
}


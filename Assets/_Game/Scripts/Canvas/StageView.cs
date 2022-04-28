namespace BugGame.UI
{
    using DG.Tweening;
    using MyBox;
    using UnityEngine;
    using UnityEngine.UI;

    public partial class StageView : ViewBehaviour
    {
        [Header("Dependencies")]
        [SerializeField, AutoProperty] private Canvas m_Canvas;
        [SerializeField, AutoProperty] private CanvasGroup m_CanvasGroup;
        [SerializeField, AutoProperty] private GraphicRaycaster m_GraphicRaycaster;

        [Header("Settings")]
        [SerializeField] private float m_FadeDuration = 1f;

        private bool m_IsOpened = true;
        private Tweener m_Tweener;

        public override void Open()
        {
            if (m_IsOpened) return;

            // OPTIMIZABLE: Reuse tweens? (1)
            m_Tweener.Kill();
            m_Canvas.enabled = true;
            m_GraphicRaycaster.enabled = true;
            m_Tweener = m_CanvasGroup.DOFade(1f, m_FadeDuration);

            m_IsOpened = true;
        }

        public override void Close()
        {
            if (!m_IsOpened) return;

            // OPTIMIZABLE: Reuse tweens? (2)
            m_Tweener.Kill();
            m_GraphicRaycaster.enabled = false;
            m_Tweener = m_CanvasGroup.DOFade(0f, m_FadeDuration)
                                     .OnComplete(() => m_Canvas.enabled = false);

            m_IsOpened = false;
        }
    }
}
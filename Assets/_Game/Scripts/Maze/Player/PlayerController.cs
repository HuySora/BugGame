namespace BugGame.Maze
{
    using DG.Tweening;
    using MyBox;
    using UnityEngine;

    public class PlayerController : MonoBehaviour
    {
        public ControllerScheme ControllerScheme
        {
            get => m_ControllerScheme;
            set => m_ControllerScheme = value;
        }

        [Separator("-----Settings-----")]
        [SerializeField, Range(0f, 10f)] private float m_TweenSpeed;
        [SerializeField, ReadOnly] private ControllerScheme m_ControllerScheme;

        private Tweener m_Tweener;

        private void Update()
        {
            // OPTIMIZABLE: We could have an bool to check if the ControllerScheme required to move or not
            // (player vs AI) before doing fromCellPos calculation
            var fromCellPos = MazeManager.WorldToCell(transform.position);
            if (!m_ControllerScheme.TryGetDesiredCellPositionFrom(fromCellPos, out Vector2Int desiredCellPos))
                return;

            // Snapped world position on maze grid
            var toSnappedPos = MazeManager.CellToWorld(desiredCellPos);

            // Move the player
            Vector2 dir = toSnappedPos - transform.position;
            var eulerRot = new Vector3(transform.rotation.x, transform.rotation.y, dir.ToAngle2D());
            transform.rotation = Quaternion.Euler(eulerRot);
            // OPTIMIZABLE: Reuse tweens?
            m_Tweener.Kill();
            m_Tweener = transform.DOMove(toSnappedPos, m_TweenSpeed).OnComplete(() =>
            {
                // Trigger cell event (only gate for now)
                MazeManager.TryInvokeCell(desiredCellPos);
            });
        }

        private void OnDestroy()
        {
            m_Tweener.Kill();
        }
    }
}


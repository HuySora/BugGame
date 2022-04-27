namespace BugGame.Maze
{
    using DG.Tweening;
    using MyBox;
    using UnityEngine;

    public class MazePlayerController : MonoBehaviour
    {
        [Separator("-----Settings-----")]
        [SerializeField, Range(0f, 10f)] private float m_TweenSpeed;

        private Tweener m_Tweener;

        public void Update()
        {
            if (PrimaryPointer.IsPressed() && PrimaryPointer.TryGetPosition(out Vector3 screenPos))
            {
                // World position of pointer
                var worldPos = CameraManager.Main.ScreenToWorldPoint(screenPos);

                // Cell positions on grid
                var fromCellPos = MazeManager.WorldToCell(transform.position);
                var toCellPos = MazeManager.WorldToCell(worldPos);

                // Return if meet these conditions
                if (fromCellPos == toCellPos) return;
                if (!MazeManager.IsPassable(fromCellPos, toCellPos)) return;
                
                // Snapped world position on maze grid
                var toSnappedPos = MazeManager.CellToWorld(toCellPos);

                // Move the player
                Vector2 dir = toSnappedPos - transform.position;
                var eulerRot = new Vector3(transform.rotation.x, transform.rotation.y, dir.ToAngle2D());
                transform.rotation = Quaternion.Euler(eulerRot);
                // OPTIMIZABLE: Reuse tweens?
                m_Tweener.Kill();
                m_Tweener = transform.DOMove(toSnappedPos, m_TweenSpeed).OnComplete(() =>
                {
                    // Trigger cell event (only gate for now)
                    MazeManager.InvokeCell(toCellPos);
                });
            }
        }
    }
}


namespace BugGame
{
    using MyBox;
    using UnityEngine;



    public class MazePlayerController : MonoBehaviour
    {
        public void Update()
        {
            if (PrimaryPointer.WasPressedThisFrame() && PrimaryPointer.TryGetPosition(out Vector3 screenPos))
            {
                var worldPos = CameraManager.Main.ScreenToWorldPoint(screenPos);

                var fromCellPos = MazeManager.WorldToCell(transform.position);
                var toCellPos = MazeManager.WorldToCell(worldPos);
                
                if (!MazeManager.IsPassable(fromCellPos, toCellPos)) return;
                
                var toSnappedPos = MazeManager.CellToWorld(toCellPos);
                transform.position = toSnappedPos;
            }
        }
    }
}


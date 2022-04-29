namespace BugGame.Maze
{
    using MyBox;
    using UnityEngine;

    [CreateAssetMenu(fileName = "PlayerControllerScheme", menuName = "ControllerScheme/Player")]
    public partial class PlayerControllerScheme : ControllerScheme
    {
        public override bool TryGetDesiredCellPositionFrom(Vector2Int fromCellPos, out Vector2Int desiredCellPos)
        {
            // No pointer pressed yet
            if (!PrimaryPointer.IsPressed()
            || !PrimaryPointer.TryGetPosition(out Vector3 screenPos))
            {
                desiredCellPos = Vector2Int.zero;
                return false;
            }

            // World & cell position of pointer
            var worldPos = CameraManager.Main.ScreenToWorldPoint(screenPos);
            var toCellPos = MazeManager.WorldToCell(worldPos);

            // Same cell or not passable
            if (fromCellPos == toCellPos
            || !MazeManager.IsPassable(fromCellPos, toCellPos))
            {
                desiredCellPos = Vector2Int.zero;
                return false;
            }
            
            // Return result
            desiredCellPos = toCellPos;
            return true;
        }
    }
}


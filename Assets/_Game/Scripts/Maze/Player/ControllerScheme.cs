namespace BugGame.Maze
{
    using UnityEngine;

    public abstract class ControllerScheme : ScriptableObject
    {
        public abstract bool TryGetDesiredCellPositionFrom(Vector2Int fromCellPos, out Vector2Int desiredCellPos);
    }
}


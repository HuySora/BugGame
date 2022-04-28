namespace BugGame.Maze
{
    using System;
    using System.Collections;
    using UnityEngine;

    public abstract class MazeSolver : ScriptableObject
    {
        public event Action<Vector2Int[]> PathGenerated;
        protected virtual void OnPathGenerated(Vector2Int[] pathCellPositions)
        {
            PathGenerated?.Invoke(pathCellPositions);
        }

        public abstract void Initialize(CellTile[,] map, Vector2Int fromCellPos, Vector2Int toCellPos);
        public abstract IEnumerator DoAlgorithm();
    }
}


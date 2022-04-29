namespace BugGame.Maze
{
    using System;
    using System.Collections;
    using UnityEngine;

    public abstract class MazeGenerator : ScriptableObject
    {
        public event Action<Vector2Int, Vector2Int> CellPairModified;
        protected virtual void OnCellPairModified(Vector2Int prevPos, Vector2Int newPos)
        {
            CellPairModified?.Invoke(prevPos, newPos);
        }

        public event Action MazeGenerated;
        protected virtual void OnMazeGenerated()
        {
            MazeGenerated?.Invoke();
        }
        
        public abstract void Initialize(MazeCell[,] map, System.Random rng);
        public abstract IEnumerator DoAlgorithm();
    }
}


namespace BugGame.Maze
{
    using System;
    using System.Collections;
    using UnityEngine;

    // TODO: Making this to be a scriptable object so it give us more flexibility
    public abstract class MazeAlgorithm : MonoBehaviour
    {
        public event Action<Vector2Int> HeadCellPositionChanged;
        protected virtual void OnHeadCellPositionChanged(Vector2Int pos)
        {
            HeadCellPositionChanged?.Invoke(pos);
        }

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
        
        public abstract void Initialize(CellTile[,] wallstateMap, System.Random rng);
        public abstract IEnumerator DoAlgorithm();
    }
}


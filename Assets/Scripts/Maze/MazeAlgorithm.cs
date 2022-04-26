namespace BugGame
{
    using System;
    using System.Collections;
    using UnityEngine;

    // TODO: Maybe making this to be a scriptable object
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
        
        public abstract void Initialize(Cell[,] map, int seed);
        public abstract IEnumerator DoAlgorithm();
    }
}


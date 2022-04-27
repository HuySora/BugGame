namespace BugGame
{
    using MyBox;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class RecursiveBacktracking : MazeAlgorithm
    {
        private WallState[,] m_WallStateMap;
        private bool[,] m_VisitedMap;
        private int m_Seed;
        
        public override void Initialize(WallState[,] wallStateMap, int seed)
        {
            m_WallStateMap = wallStateMap;
            // bool array to keep track of visited cells
            m_VisitedMap = new bool[wallStateMap.GetLength(0), wallStateMap.GetLength(1)]; 
            m_Seed = seed;
        }

        public override IEnumerator DoAlgorithm()
        {
            // Setup
            var rng = new System.Random(m_Seed);
            var posStack = new Stack<Vector2Int>();
            // Start at top-left
            var startPos = new Vector2Int(0, m_WallStateMap.GetLength(1) - 1);

            // First cell
            m_VisitedMap[startPos.x, startPos.y] = true;
            posStack.Push(startPos);

            // Recursive loop
            while (posStack.TryPeek(out Vector2Int pos))
            {
                //yield return null;

                OnHeadCellPositionChanged(pos);
                // Continue to next position as we have no neighbour
                if (TryGetUnvisitedNeighbours(pos, out List<Vector2Int> unvisitedPositions) == false)
                {
                    posStack.Pop();
                    continue;
                }

                // Get random neighbour position
                var nextPos = unvisitedPositions[rng.Next(0, unvisitedPositions.Count)];

                // Adjust the current cell as well as the target cell
                m_WallStateMap.TryRemoveWall(pos, nextPos);
                m_VisitedMap[nextPos.x, nextPos.y] = true;
                OnCellPairModified(pos, nextPos);

                posStack.Push(nextPos);
            }

            OnMazeGenerated();
            yield return null;
        }

        private bool TryGetUnvisitedNeighbours(Vector2Int pos, out List<Vector2Int> result)
        {
            result = new List<Vector2Int>();

            // Left
            var leftPos = new Vector2Int(pos.x - 1, pos.y);
            if (leftPos.x >= 0 && !m_VisitedMap[leftPos.x, leftPos.y])
            {
                result.Add(new Vector2Int(leftPos.x, leftPos.y));
            }
            // Right
            var rightPos = new Vector2Int(pos.x + 1, pos.y);
            if (rightPos.x < m_WallStateMap.GetLength(0) && !m_VisitedMap[rightPos.x, rightPos.y])
            {
                result.Add(new Vector2Int(rightPos.x, rightPos.y));
            }
            // Down
            var downPos = new Vector2Int(pos.x, pos.y - 1);
            if (downPos.y >= 0 && !m_VisitedMap[downPos.x, downPos.y])
            {
                result.Add(new Vector2Int(downPos.x, downPos.y));
            }
            // Up
            var upPos = new Vector2Int(pos.x, pos.y + 1);
            if (upPos.y < m_WallStateMap.GetLength(1) && !m_VisitedMap[upPos.x, upPos.y])
            {
                result.Add(new Vector2Int(upPos.x, upPos.y));
            }

            return result.Count > 0;
        }

        
    }
}


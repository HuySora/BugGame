namespace BugGame
{
    using MyBox;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class RecursiveBacktracking : MazeAlgorithm
    {
        private Cell[,] m_CellMap;
        private bool[,] m_Visited;
        private int m_Seed;
        
        public override void Initialize(Cell[,] map, int seed)
        {
            m_CellMap = map;
            // bool array to keep track of visited cells
            m_Visited = new bool[map.GetLength(0), map.GetLength(1)]; 
            m_Seed = seed;
        }

        public override IEnumerator DoAlgorithm()
        {
            // Setup
            var rng = new System.Random(m_Seed);
            var posStack = new Stack<Vector2Int>();
            // Start at top-left
            var startPos = new Vector2Int(0, m_CellMap.GetLength(1) - 1);

            // First cell
            m_Visited[startPos.x, startPos.y] = true;
            posStack.Push(startPos);

            // Recursive loop
            while (posStack.TryPeek(out Vector2Int pos))
            {
                yield return null;

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
                TryRemoveWall(pos, nextPos);
                m_Visited[nextPos.x, nextPos.y] = true;
                OnCellPairModified(pos, nextPos);

                posStack.Push(nextPos);
            }
        }

        private bool TryGetUnvisitedNeighbours(Vector2Int pos, out List<Vector2Int> result)
        {
            result = new List<Vector2Int>();

            // Left
            var leftPos = new Vector2Int(pos.x - 1, pos.y);
            if (leftPos.x >= 0 && !m_Visited[leftPos.x, leftPos.y])
            {
                result.Add(new Vector2Int(leftPos.x, leftPos.y));
            }
            // Right
            var rightPos = new Vector2Int(pos.x + 1, pos.y);
            if (rightPos.x < m_CellMap.GetLength(0) && !m_Visited[rightPos.x, rightPos.y])
            {
                result.Add(new Vector2Int(rightPos.x, rightPos.y));
            }
            // Down
            var downPos = new Vector2Int(pos.x, pos.y - 1);
            if (downPos.y >= 0 && !m_Visited[downPos.x, downPos.y])
            {
                result.Add(new Vector2Int(downPos.x, downPos.y));
            }
            // Up
            var upPos = new Vector2Int(pos.x, pos.y + 1);
            if (upPos.y < m_CellMap.GetLength(1) && !m_Visited[upPos.x, upPos.y])
            {
                result.Add(new Vector2Int(upPos.x, upPos.y));
            }

            return result.Count > 0;
        }

        private bool TryRemoveWall(Vector2Int first, Vector2Int second)
        {
            // |F|S|
            if (first.x < second.x)
            {
                m_CellMap[first.x, first.y].WallState &= ~WallState.Right;
                m_CellMap[second.x, second.y].WallState &= ~WallState.Left;
                return true;
            }
            // |S|F|
            else if (first.x > second.x)
            {
                m_CellMap[first.x, first.y].WallState &= ~WallState.Left;
                m_CellMap[second.x, second.y].WallState &= ~WallState.Right;
                return true;
            }
            // F
            // S
            else if (first.y > second.y)
            {
                m_CellMap[first.x, first.y].WallState &= ~WallState.Down;
                m_CellMap[second.x, second.y].WallState &= ~WallState.Up;
                return true;
            }
            // S
            // F
            else if (first.y < second.y)
            {
                m_CellMap[first.x, first.y].WallState &= ~WallState.Up;
                m_CellMap[second.x, second.y].WallState &= ~WallState.Down;
                return true;
            }

            return false;
        }
    }
}


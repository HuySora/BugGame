
namespace BugGame
{
    using MyBox;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Tilemaps;

#if UNITY_EDITOR
    public partial class MazeGenerator
    {
        public int EditorWidth;
        public int EditorHeight;
        public int EditorSeed;

        [ButtonMethod]
        public void EditorGenerate()
        {
            Generate(EditorWidth, EditorHeight, EditorSeed);
        }
    }

#endif

    public partial class MazeGenerator : MonoBehaviour
    {
        public static event Action<Vector2Int> OnPositionChanged;

        public Cell[,] CellMap { get; private set; }
        private Coroutine m_CurrentRoutine;

        public void Generate(int width, int height, int seed)
        {
            if (m_CurrentRoutine != null) StopCoroutine(m_CurrentRoutine);
            InitializeCellMap(width, height);
            // Maybe we will add IAlgorithm later if we have time
            m_CurrentRoutine = StartCoroutine(RecursiveBacktrackerRoutine(seed));
        }

        public void InitializeCellMap(int width, int height)
        {
            CellMap = new Cell[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    // Initilize new cell with .WallState = WallState.All
                    CellMap[i, j] = new Cell();
                }
            }
        }

        public IEnumerator RecursiveBacktrackerRoutine(int seed)
        {
            // Setup
            var rng = new System.Random(seed);
            var posStack = new Stack<Vector2Int>();
            // Start at top-left
            var startPos = new Vector2Int(0, CellMap.GetLength(1) - 1);

            // First cell
            CellMap[startPos.x, startPos.y].IsVisited = true;
            posStack.Push(startPos);

            // Recursive loop
            while (posStack.TryPeek(out Vector2Int pos))
            {
                OnPositionChanged?.Invoke(pos);
                yield return null;

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
                CellMap[nextPos.x, nextPos.y].IsVisited = true;

                posStack.Push(nextPos);
                Debug.Log("A");
            }
        }

        private bool TryGetUnvisitedNeighbours(Vector2Int pos, out List<Vector2Int> result)
        {
            result = new List<Vector2Int>();

            // Left
            var leftPos = new Vector2Int(pos.x - 1, pos.y);
            if (leftPos.x >= 0 && !CellMap[leftPos.x, leftPos.y].IsVisited)
            {
                result.Add(new Vector2Int(leftPos.x, leftPos.y));
            }
            // Right
            var rightPos = new Vector2Int(pos.x + 1, pos.y);
            if (rightPos.x < CellMap.GetLength(0) && !CellMap[rightPos.x, rightPos.y].IsVisited)
            {
                result.Add(new Vector2Int(rightPos.x, rightPos.y));
            }
            // Down
            var downPos = new Vector2Int(pos.x, pos.y - 1);
            if (downPos.y >= 0 && !CellMap[downPos.x, downPos.y].IsVisited)
            {
                result.Add(new Vector2Int(downPos.x, downPos.y));
            }
            // Up
            var upPos = new Vector2Int(pos.x, pos.y + 1);
            if (upPos.y < CellMap.GetLength(1) && !CellMap[upPos.x, upPos.y].IsVisited)
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
                CellMap[first.x, first.y].WallState &= ~WallState.Right;
                CellMap[second.x, second.y].WallState &= ~WallState.Left;
                return true;
            }
            // |S|F|
            else if (first.x > second.x)
            {
                CellMap[first.x, first.y].WallState &= ~WallState.Left;
                CellMap[second.x, second.y].WallState &= ~WallState.Right;
                return true;
            }
            // F
            // S
            else if (first.y > second.y)
            {
                CellMap[first.x, first.y].WallState &= ~WallState.Down;
                CellMap[second.x, second.y].WallState &= ~WallState.Up;
                return true;
            }
            // S
            // F
            else if (first.y < second.y)
            {
                CellMap[first.x, first.y].WallState &= ~WallState.Up;
                CellMap[second.x, second.y].WallState &= ~WallState.Down;
                return true;
            }

            return false;
        }
    }
}
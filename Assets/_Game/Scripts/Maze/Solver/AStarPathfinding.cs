namespace BugGame.Maze
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// TODO: Very scuffed A* pathfinding lol.. (tried to use dictionary be like)
    /// </summary>
    [CreateAssetMenu(fileName = "AStarPathfinding", menuName = "MazeSolver/AStarPathfinding")]
    public class AStarPathfinding : MazeSolver
    {
        private CellTile[,] m_CellMap;
        private Vector2Int m_FromCellPos;
        private Vector2Int m_ToCellPos;

        public override void Initialize(CellTile[,] map, Vector2Int fromCellPos, Vector2Int toCellPos)
        {
            m_CellMap = map;
            // bool array to keep track of visited cells
            m_FromCellPos = fromCellPos;
            m_ToCellPos = toCellPos;
        }
        
        public override IEnumerator DoAlgorithm()
        {
            // TODO: Implementing heap collection
            var cellPosToCellTile = new Dictionary<Vector2Int, AStarNode>();

            // Start cell & position
            var startCell = new AStarNode(0f, ManhattanDistance(m_FromCellPos, m_ToCellPos), m_FromCellPos);
            cellPosToCellTile[m_FromCellPos] = startCell;

            while (cellPosToCellTile.Count > 0)
            {
                //yield return null;

                // OPTIMIZABLE: Retrieve the lowest F cost cell
                var currPair = cellPosToCellTile.OrderBy(cell => cell.Value.FCost).First();

                // We have found the path
                if (currPair.Key == m_ToCellPos)
                {
                    var pathPosStack = new Stack<Vector2Int>();
                    var currCellPos = m_ToCellPos;

                    // If the current and parent at the same position then we have reached the start
                    while (currCellPos != cellPosToCellTile[currCellPos].ParentCellPos)
                    {
                        pathPosStack.Push(currCellPos);
                        currCellPos = cellPosToCellTile[currCellPos].ParentCellPos;
                    }
                    
                    // Push in the start position
                    pathPosStack.Push(currCellPos);

                    // To array will reverse the stack to "start to end"
                    OnPathGenerated(pathPosStack.ToArray());
                    break;
                }

                // REFACTOR: This just a quickfix to mark position as visited because the coder is dumb :D
                cellPosToCellTile[currPair.Key] = new AStarNode(float.MaxValue, float.MaxValue, cellPosToCellTile[currPair.Key].ParentCellPos);

                // Continue to next position as we have no valid neighbours
                if (!TryGetValidNeighbours(currPair.Key, out List<Vector2Int> validCellPositions))
                    continue;

                // Going thought each valid neighbour
                foreach (var nextCellPos in validCellPositions)
                {
                    // Already visited
                    if (cellPosToCellTile.ContainsKey(nextCellPos))
                        continue;

                    float nextCellGCost = currPair.Value.GCost + ManhattanDistance(currPair.Key, nextCellPos);
                    // We have better path (lower G cost)
                    if (currPair.Value.GCost < nextCellGCost)
                    {
                        var nextCell = new AStarNode(nextCellGCost, ManhattanDistance(nextCellPos, m_ToCellPos), currPair.Key);
                        cellPosToCellTile[nextCellPos] = nextCell;
                    }
                }
            }

            yield return null;
        }

        private int ManhattanDistance(Vector2Int from, Vector2Int to)
            => Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y);

        private bool TryGetValidNeighbours(Vector2Int pos, out List<Vector2Int> result)
        {
            result = new List<Vector2Int>();

            // Only add if not blocked by walls
            var leftPos = new Vector2Int(pos.x - 1, pos.y);
            if (leftPos.x >= 0 && m_CellMap[pos.x, pos.y].WallStates.HasFlag(WallStates.Left) == false)
                result.Add(leftPos);

            var rightPos = new Vector2Int(pos.x + 1, pos.y);
            if (rightPos.x >= 0 && m_CellMap[pos.x, pos.y].WallStates.HasFlag(WallStates.Right) == false)
                result.Add(rightPos);
            
            var downPos = new Vector2Int(pos.x, pos.y - 1);
            if (downPos.x >= 0 && m_CellMap[pos.x, pos.y].WallStates.HasFlag(WallStates.Down) == false)
                result.Add(downPos);

            var upPos = new Vector2Int(pos.x, pos.y + 1);
            if (upPos.x >= 0 && m_CellMap[pos.x, pos.y].WallStates.HasFlag(WallStates.Up) == false)
                result.Add(upPos);

            return result.Count > 0;
        }        
    }
}


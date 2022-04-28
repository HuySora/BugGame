namespace BugGame.Maze
{
    using MyBox;
    using UnityEngine;
    
    public struct AStarNode
    {
        public readonly float GCost;
        public readonly float HCost;
        public readonly float FCost;
        public readonly Vector2Int ParentCellPos;

        public AStarNode(float gCost, float hCost, Vector2Int parentCellPos)
        {
            GCost = gCost;
            HCost = hCost;
            FCost = gCost + hCost;
            ParentCellPos = parentCellPos;
        }
    }
}


namespace BugGame.Maze
{
    using MyBox;
    using UnityEngine;

    public class MazePathNode
    {
        public readonly Vector2Int CellPos;
        public readonly MazePathNode PreviousPathNode;

        public MazePathNode(Vector2Int cellPos, MazePathNode previousPathNode)
        {
            CellPos = cellPos;
            PreviousPathNode = previousPathNode;
        }
    }
}


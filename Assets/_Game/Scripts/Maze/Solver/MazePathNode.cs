namespace BugGame.Maze
{
    using UnityEngine;

    public class MazePathNode
    {
        public readonly Vector2Int CellPos;
        public readonly MazePathNode PrevPathNode;
        public readonly MazePathNode NextPathNode;

        public MazePathNode(Vector2Int cellPos, MazePathNode prevPathNode, MazePathNode nextPathNode)
        {
            CellPos = cellPos;
            PrevPathNode = prevPathNode;
            NextPathNode = nextPathNode;
        }
    }
}


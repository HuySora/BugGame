namespace BugGame.Maze
{
    using UnityEngine;

    public static class MazeExtension
    { 
        public static bool IsPassable(this MazeCell[,] map, Vector2Int from, Vector2Int to)
        {
            // Out of bound
            if (!map.IsInBound(from) || !map.IsInBound(to))
                return false;

            // Cells are not in the same row or column that's one block away
            if ((to - from).sqrMagnitude > 1)
                return false;

            // |F|S|
            if (from.x < to.x)
            {
                // Has wall (we don't really need to check both cell since theoretically it's the same wall)
                if (map[from.x, from.y].WallStates.HasFlag(WallStates.Right))
                    return false;
                
                if (map[to.x, to.y].WallStates.HasFlag(WallStates.Left))
                    return false;

                return true;
            }
            // |S|F|
            else if (from.x > to.x)
            {
                // Has wall (we don't really need to check both cell since theoretically it's the same wall)
                if (map[from.x, from.y].WallStates.HasFlag(WallStates.Left))
                    return false;

                if (map[to.x, to.y].WallStates.HasFlag(WallStates.Right))
                    return false;

                return true;
            }
            // F
            // S
            else if (from.y > to.y)
            {
                // Has wall (we don't really need to check both cell since theoretically it's the same wall)
                if (map[from.x, from.y].WallStates.HasFlag(WallStates.Down))
                    return false;

                if (map[to.x, to.y].WallStates.HasFlag(WallStates.Up))
                    return false;

                return true;
            }
            // S
            // F
            else if (from.y < to.y)
            {
                // Has wall (we don't really need to check both cell since theoretically it's the same wall)
                if (map[from.x, from.y].WallStates.HasFlag(WallStates.Up))
                    return false;

                if (map[to.x, to.y].WallStates.HasFlag(WallStates.Down))
                    return false;

                return true;
            }

            return false;
        }
        
        public static void RemoveWall(this MazeCell[,] map, Vector2Int from, Vector2Int to)
        {
            // Out of bound
            if (!map.IsInBound(from) || !map.IsInBound(to))
                return;

            // Cells are not in the same row or column that's one block away
            if ((to - from).sqrMagnitude > 1)
                return;

            // |F|S|
            if (from.x < to.x)
            {
                map[from.x, from.y].UpdateWalls(map[from.x, from.y].WallStates & ~WallStates.Right);
                map[to.x, to.y].UpdateWalls(map[to.x, to.y].WallStates & ~WallStates.Left);
            }
            // |S|F|
            else if (from.x > to.x)
            {
                map[from.x, from.y].UpdateWalls(map[from.x, from.y].WallStates & ~WallStates.Left);
                map[to.x, to.y].UpdateWalls(map[to.x, to.y].WallStates & ~WallStates.Right);
            }
            // F
            // S
            else if (from.y > to.y)
            {
                map[from.x, from.y].UpdateWalls(map[from.x, from.y].WallStates & ~WallStates.Down);
                map[to.x, to.y].UpdateWalls(map[to.x, to.y].WallStates & ~WallStates.Up);
            }
            // S
            // F
            else if (from.y < to.y)
            {
                map[from.x, from.y].UpdateWalls(map[from.x, from.y].WallStates & ~WallStates.Up);
                map[to.x, to.y].UpdateWalls(map[to.x, to.y].WallStates & ~WallStates.Down);
            }
        }
    }
}
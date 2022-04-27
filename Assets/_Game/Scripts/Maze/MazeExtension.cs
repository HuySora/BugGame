namespace BugGame.Maze
{
    using UnityEngine;

    public static class MazeExtension
    { 
        public static bool IsPassable(this CellTile[,] map, Vector2Int first, Vector2Int second)
        {
            // Bound check
            if (map.IsInBound(first) == false
            || map.IsInBound(second) == false) return false;

            // Check if the two cells are the same row or column that's one block away
            var offset = second - first;
            if (offset.sqrMagnitude > 1) return false;

            // |F|S|
            if (first.x < second.x)
            {
                // Has wall (we don't really need to check both cell since theoretically it's the same wall)
                if (map[first.x, first.y].WallStates.HasFlag(WallStates.Right)) return false;
                if (map[second.x, second.y].WallStates.HasFlag(WallStates.Left)) return false;

                return true;
            }
            // |S|F|
            else if (first.x > second.x)
            {
                // Has wall (we don't really need to check both cell since theoretically it's the same wall)
                if (map[first.x, first.y].WallStates.HasFlag(WallStates.Left)) return false;
                if (map[second.x, second.y].WallStates.HasFlag(WallStates.Right)) return false;

                return true;
            }
            // F
            // S
            else if (first.y > second.y)
            {
                // Has wall (we don't really need to check both cell since theoretically it's the same wall)
                if (map[first.x, first.y].WallStates.HasFlag(WallStates.Down)) return false;
                if (map[second.x, second.y].WallStates.HasFlag(WallStates.Up)) return false;

                return true;
            }
            // S
            // F
            else if (first.y < second.y)
            {
                // Has wall (we don't really need to check both cell since theoretically it's the same wall)
                if (map[first.x, first.y].WallStates.HasFlag(WallStates.Up)) return false;
                if (map[second.x, second.y].WallStates.HasFlag(WallStates.Down)) return false;

                return true;
            }

            return false;
        }
        
        public static void RemoveWall(this CellTile[,] map, Vector2Int first, Vector2Int second)
        {
            // Bound check
            if (map.IsInBound(first) == false
            || map.IsInBound(second) == false) return;

            // Check if the two cells are the same row or column that's one block away
            var offset = second - first;
            if (offset.sqrMagnitude > 1) return;

            // |F|S|
            if (first.x < second.x)
            {
                map[first.x, first.y].UpdateWalls(map[first.x, first.y].WallStates & ~WallStates.Right);
                map[second.x, second.y].UpdateWalls(map[second.x, second.y].WallStates & ~WallStates.Left);
            }
            // |S|F|
            else if (first.x > second.x)
            {
                map[first.x, first.y].UpdateWalls(map[first.x, first.y].WallStates & ~WallStates.Left);
                map[second.x, second.y].UpdateWalls(map[second.x, second.y].WallStates & ~WallStates.Right);
            }
            // F
            // S
            else if (first.y > second.y)
            {
                map[first.x, first.y].UpdateWalls(map[first.x, first.y].WallStates & ~WallStates.Down);
                map[second.x, second.y].UpdateWalls(map[second.x, second.y].WallStates & ~WallStates.Up);
            }
            // S
            // F
            else if (first.y < second.y)
            {
                map[first.x, first.y].UpdateWalls(map[first.x, first.y].WallStates & ~WallStates.Up);
                map[second.x, second.y].UpdateWalls(map[second.x, second.y].WallStates & ~WallStates.Down);
            }
        }
    }
}
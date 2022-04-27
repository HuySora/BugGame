namespace BugGame
{
    using MyBox;
    using UnityEngine;

    public static class MazeExtension
    { 
        public static bool IsInBound(this WallState[,] map, Vector2Int first)
        {
            if (first.x < 0 || first.x >= map.GetLength(0)
            || first.y < 0 || first.y >= map.GetLength(1)) return false;

            return true;
        }

        public static bool IsPassable(this WallState[,] map, Vector2Int first, Vector2Int second)
        {
            // Bound check
            if (map.IsInBound(first) == false
            || map.IsInBound(second) == false) return false;            

            // Position check
            var offset = second - first;
            if (offset.magnitude > 1) return false;

            // |F|S|
            if (first.x < second.x)
            {
                // Has wall (we don't really need to check both cell since theoretically it's the same wall)
                if (map[first.x, first.y].HasFlag(WallState.Right)) return false;
                if (map[second.x, second.y].HasFlag(WallState.Left)) return false;

                return true;
            }
            // |S|F|
            else if (first.x > second.x)
            {
                // Has wall (we don't really need to check both cell since theoretically it's the same wall)
                if (map[first.x, first.y].HasFlag(WallState.Left)) return false;
                if (map[second.x, second.y].HasFlag(WallState.Right)) return false;

                return true;
            }
            // F
            // S
            else if (first.y > second.y)
            {
                // Has wall (we don't really need to check both cell since theoretically it's the same wall)
                if (map[first.x, first.y].HasFlag(WallState.Down)) return false;
                if (map[second.x, second.y].HasFlag(WallState.Up)) return false;

                return true;
            }
            // S
            // F
            else if (first.y < second.y)
            {
                // Has wall (we don't really need to check both cell since theoretically it's the same wall)
                if (map[first.x, first.y].HasFlag(WallState.Up)) return false;
                if (map[second.x, second.y].HasFlag(WallState.Down)) return false;

                return true;
            }

            return false;
        }

        public static bool TryRemoveWall(this WallState[,] map, Vector2Int first, Vector2Int second)
        {
            // Bound check
            if (first.x < 0 || first.x >= map.GetLength(0)
            || first.y < 0 || first.y >= map.GetLength(1)
            || second.x < 0 || second.x >= map.GetLength(0)
            || second.y < 0 || second.y >= map.GetLength(1)) return false;

            // Position check
            var offset = second - first;
            if (offset.magnitude > 1) return false;

            // |F|S|
            if (first.x < second.x)
            {
                map[first.x, first.y] &= ~WallState.Right;
                map[second.x, second.y] &= ~WallState.Left;
                return true;
            }
            // |S|F|
            else if (first.x > second.x)
            {
                map[first.x, first.y] &= ~WallState.Left;
                map[second.x, second.y] &= ~WallState.Right;
                return true;
            }
            // F
            // S
            else if (first.y > second.y)
            {
                map[first.x, first.y] &= ~WallState.Down;
                map[second.x, second.y] &= ~WallState.Up;
                return true;
            }
            // S
            // F
            else if (first.y < second.y)
            {
                map[first.x, first.y] &= ~WallState.Up;
                map[second.x, second.y] &= ~WallState.Down;
                return true;
            }

            return false;
        }
    }
}
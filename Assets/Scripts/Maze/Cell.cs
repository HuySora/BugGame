namespace BugGame
{
    using System;
    using UnityEngine;

    [Flags]
    public enum WallState
    {
        None = 0,
        Left = 1 << 0,
        Right = 1 << 1,
        Down = 1 << 2,
        Up = 1 << 3,
        All = Left | Right | Down | Up
    }

    [Serializable]
    public class Cell
    {
        public bool IsVisited;
        public WallState WallState = WallState.All;
    }
}

namespace BugGame
{
    using MyBox;
    using UnityEngine;

    public class CellTile : MonoBehaviour
    {
        public SpriteRenderer Left;
        public SpriteRenderer Right;
        public SpriteRenderer Down;
        public SpriteRenderer Up;

        public void UpdateCell(WallState wallStates)
        {
            Left.gameObject.SetActive(wallStates.HasFlag(WallState.Left));
            Right.gameObject.SetActive(wallStates.HasFlag(WallState.Right));
            Down.gameObject.SetActive(wallStates.HasFlag(WallState.Down));
            Up.gameObject.SetActive(wallStates.HasFlag(WallState.Up));
        }
    }
}


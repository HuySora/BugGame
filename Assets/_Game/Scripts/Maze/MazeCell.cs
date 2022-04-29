namespace BugGame.Maze
{
    using MyBox;
    using System;
    using UnityEngine;

    public class MazeCell : MonoBehaviour
    {
        // OPTIMIZABLE: Maybe spawn portal, wall on runtime
        [Separator("-----Dependencies-----")]
        [SerializeField] private SpriteRenderer m_Gate;
        [SerializeField] private SpriteRenderer m_LeftWall;
        [SerializeField] private SpriteRenderer m_RightWall;
        [SerializeField] private SpriteRenderer m_DownWall;
        [SerializeField] private SpriteRenderer m_UpWall;
        
        public bool IsGate { get; private set; }
        public WallStates WallStates { get; private set; }

        private void Awake() => m_Gate.enabled = false;

        public void UpdateWalls(WallStates wallStates)
        {
            WallStates = wallStates;
            m_LeftWall.gameObject.SetActive(wallStates.HasFlag(WallStates.Left));
            m_RightWall.gameObject.SetActive(wallStates.HasFlag(WallStates.Right));
            m_DownWall.gameObject.SetActive(wallStates.HasFlag(WallStates.Down));
            m_UpWall.gameObject.SetActive(wallStates.HasFlag(WallStates.Up));
        }

        public void SetPortal(bool isPortal)
        {
            m_Gate.enabled = isPortal;
            IsGate = true;
        }
    }
}


namespace BugGame.Maze
{
    using MyBox;
    using UnityEngine;

    public class AIControllerScheme : ControllerScheme
    {
        private float m_NextPathTime;
        private Vector2Int? m_DesiredCellPos;

        public override bool TryGetDesiredCellPositionFrom(Vector2Int fromCellPos, out Vector2Int desiredCellPos)
        {
            if (Time.unscaledTime >= m_NextPathTime)
            {
                MazeManager.TrySolve(fromCellPos);
                MazeManager.PathGenerated -= OnPathGenerated;
                MazeManager.PathGenerated += OnPathGenerated;
                // Hard-coded since this just a "test" game
                m_NextPathTime = Time.unscaledTime + 0.04f;

                desiredCellPos = fromCellPos;
                return false;
            }

            if (m_DesiredCellPos != fromCellPos && m_DesiredCellPos != null)
            {
                desiredCellPos = (Vector2Int)m_DesiredCellPos;
                return true;
            }
            
            desiredCellPos = fromCellPos;
            return false;
        }
        
        // TODO: Cache generated path to a dictionary maybe
        private void OnPathGenerated(Vector2Int[] pathCellPositions)
        {
            if (pathCellPositions.Length > 1)
                m_DesiredCellPos = pathCellPositions[1];
            else
                m_DesiredCellPos = pathCellPositions[0];
        }
    }
}


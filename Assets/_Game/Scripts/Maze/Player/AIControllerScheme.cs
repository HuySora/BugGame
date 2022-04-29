namespace BugGame.Maze
{
    using MyBox;
    using UnityEngine;

    [CreateAssetMenu(fileName = "AIControllerScheme", menuName = "ControllerScheme/AI")]
    public class AIControllerScheme : ControllerScheme
    {
        private float m_NextPathTim;
        private Vector2Int m_DesiredCellPos;

        public override bool TryGetDesiredCellPositionFrom(Vector2Int fromCellPos, out Vector2Int desiredCellPos)
        {
            if (Time.unscaledTime >= m_NextPathTim)
            {
                Debug.Log("A");

                MazeManager.TrySolve(fromCellPos);
                MazeManager.PathGenerated -= OnPathGenerated;
                MazeManager.PathGenerated += OnPathGenerated;
                // Hard-coded since this just a "test" game
                m_NextPathTim = Time.unscaledTime + 0.05f;

                desiredCellPos = fromCellPos;
                return false;
            }

            Debug.Log("B");

            if (m_DesiredCellPos != fromCellPos)
            {
                Debug.Log("C");

                desiredCellPos = m_DesiredCellPos;
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


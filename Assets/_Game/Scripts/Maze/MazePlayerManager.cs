namespace BugGame
{
    using MyBox;
    using UnityEngine;

    public partial class MazePlayerManager : SingletonBehaviour<MazePlayerManager>
    {
        #region Static ----------------------------------------------------------------------------------------------------
        public static void Spawn(Vector2Int cellPos) => Current.Instance_Spawn(cellPos);
        #endregion

        [Separator("-----Dependencies-----")]
        [SerializeField] private MazePlayerController m_PlayerPrefab;

        private MazePlayerController m_CurrentPlayer;

        public void Instance_Spawn(Vector2Int cellPos)
        {
            if (m_CurrentPlayer == null)
            {
                m_CurrentPlayer = Instantiate(m_PlayerPrefab);
            }

            var pos = MazeManager.CellToWorld(cellPos);
            m_CurrentPlayer.transform.SetPositionAndRotation(pos, Quaternion.identity);
        }
    }
}


namespace BugGame.Maze
{
    using MyBox;
    using System;
    using UnityEngine;

    public partial class MazePlayerManager : SingletonBehaviour<MazePlayerManager>
    {
        #region Static ----------------------------------------------------------------------------------------------------
        public static MazePlayerController Player => Current.m_Player;
        public static void Spawn(Vector2Int cellPos) => Current.Instance_Spawn(cellPos);
        public static void Despawn() => Current.Instance_Despawn();
        public static void SetAI(bool isAuto) => Current.Instance_SetAI(isAuto);
        #endregion

        [Separator("-----Dependencies-----")]
        [SerializeField] private MazePlayerController m_PlayerPrefab;

        private MazePlayerController m_Player;

        public void Instance_Spawn(Vector2Int cellPos)
        {
            if (m_Player == null)
                m_Player = Instantiate(m_PlayerPrefab);

            var pos = MazeManager.CellToWorld(cellPos);
            m_Player.transform.SetPositionAndRotation(pos, Quaternion.identity);
        }

        public void Instance_Despawn()
        {
            if (m_Player != null)
                Destroy(m_Player.gameObject);
        }

        private void Instance_SetAI(bool isAuto)
        {
            //MazeManager.MazeGenerated
            MazeManager.SolveForPlayer();
        }
    }
}


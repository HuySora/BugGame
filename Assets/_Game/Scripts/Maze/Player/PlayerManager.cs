namespace BugGame.Maze
{
    using MyBox;
    using System;
    using UnityEngine;

    public partial class PlayerManager : SingletonBehaviour<PlayerManager>
    {
        #region Static ----------------------------------------------------------------------------------------------------
        public static PlayerController Player => Current.m_Player;
        public static void Spawn(Vector2Int cellPos) => Current.Instance_Spawn(cellPos);
        public static void Despawn() => Current.Instance_Despawn();
        public static void ToggleAI() => Current.Instance_SetAI();
        #endregion

        [Separator("-----Dependencies-----")]
        [SerializeField] private PlayerController m_PlayerPrefab;
        [SerializeField] private ControllerScheme m_PlayerControllerScheme;
        [SerializeField] private ControllerScheme m_AIControllerScheme;

        private PlayerController m_Player;
        private bool m_IsAI;

        public void Instance_Spawn(Vector2Int cellPos)
        {
            if (m_Player == null)
                m_Player = Instantiate(m_PlayerPrefab);

            var pos = MazeManager.CellToWorld(cellPos);
            m_Player.transform.SetPositionAndRotation(pos, Quaternion.identity);
            m_Player.ControllerScheme = m_IsAI ? m_AIControllerScheme : m_PlayerControllerScheme;
        }

        public void Instance_Despawn()
        {
            if (m_Player != null)
                Destroy(m_Player.gameObject);
        }

        private void Instance_SetAI()
        {
            m_IsAI = !m_IsAI;
            if (m_Player != null)
                m_Player.ControllerScheme = m_IsAI ? m_AIControllerScheme : m_PlayerControllerScheme;
        }
    }
}


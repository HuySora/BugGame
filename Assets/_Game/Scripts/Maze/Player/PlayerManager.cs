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
        public static void SetAI(bool isAuto) => Current.Instance_SetAI(isAuto);
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

        private void Instance_SetAI(bool isAI)
        {
            m_IsAI = isAI;
            if (m_Player != null)
                m_Player.ControllerScheme = isAI ? m_AIControllerScheme : m_PlayerControllerScheme;
        }
    }
}


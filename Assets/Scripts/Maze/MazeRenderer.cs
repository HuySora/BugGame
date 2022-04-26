namespace BugGame
{
    using MyBox;
    using System;
    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;

    public partial class MazeRenderer
    {
        private void OnDrawGizmos()
        {
            for (int i = 0; i < m_Generator.CellMap.GetLength(0); i++)
            {
                for (int j = 0; j < m_Generator.CellMap.GetLength(1); j++)
                {
                    var gridPos = new Vector2(m_Generator.transform.position.x + m_CellSize * i, m_Generator.transform.position.y + m_CellSize * j);
                    if (m_Generator.CellMap[i, j].WallState.HasFlag(WallState.Left))
                    {
                        var bothX = gridPos.x - m_CellSize / 2;
                        var fromY = gridPos.y - m_CellSize / 2;
                        var toY = gridPos.y + m_CellSize / 2;

                        var from = new Vector3(bothX, fromY, m_Generator.transform.position.z);
                        var to = new Vector3(bothX, toY, m_Generator.transform.position.z);
                        Gizmos.DrawLine(from, to);
                    }
                    if (m_Generator.CellMap[i, j].WallState.HasFlag(WallState.Right))
                    {
                        var bothX = gridPos.x + m_CellSize / 2;
                        var fromY = gridPos.y + m_CellSize / 2;
                        var toY = gridPos.y - m_CellSize / 2;

                        var from = new Vector3(bothX, fromY, m_Generator.transform.position.z);
                        var to = new Vector3(bothX, toY, m_Generator.transform.position.z);
                        Gizmos.DrawLine(from, to);
                    }
                    if (m_Generator.CellMap[i, j].WallState.HasFlag(WallState.Down))
                    {
                        var fromx = gridPos.x + m_CellSize / 2;
                        var bothY = gridPos.y - m_CellSize / 2;
                        var toX = gridPos.x - m_CellSize / 2;

                        var from = new Vector3(fromx, bothY, m_Generator.transform.position.z);
                        var to = new Vector3(toX, bothY, m_Generator.transform.position.z);
                        
                        Gizmos.DrawLine(from, to);
                    }
                    if (m_Generator.CellMap[i, j].WallState.HasFlag(WallState.Up))
                    {
                        var fromx = gridPos.x - m_CellSize / 2;
                        var bothY = gridPos.y + m_CellSize / 2;
                        var toX = gridPos.x + m_CellSize / 2;

                        var from = new Vector3(fromx, bothY, m_Generator.transform.position.z);
                        var to = new Vector3(toX, bothY, m_Generator.transform.position.z);
                        
                        Gizmos.DrawLine(from, to);
                    }
                }
            }

            var worldPos = new Vector2(transform.position.x + m_CellSize * m_currentGridPos.x, transform.position.y + m_CellSize * m_currentGridPos.y);
            Gizmos.DrawSphere(worldPos, m_CellSize * 0.5f);
        }
    }
#endif

    public partial class MazeRenderer : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField, AutoProperty] private MazeGenerator m_Generator;
        [SerializeField] private Sprite m_SquareSprite;

        [Header("Settings")]
        [SerializeField] private float m_CellSize;


        private Vector2Int m_currentGridPos;

        private void Awake()
        {
            MazeGenerator.OnPositionChanged += pos => m_currentGridPos = pos;
        }
        
        private void DrawAt(int x, int y)
        {
            if (x >= m_Generator.CellMap.GetLength(0)
            || y >= m_Generator.CellMap.GetLength(1))
            {
                throw new ArgumentOutOfRangeException(nameof(x), "Tried to draw cell outside of MazeGenerator");
            }

            var state = m_Generator.CellMap[x, y];

            // |0,2|1,2|2,2|
            // |0,1|1,1|2,1|
            // |0,0|1,0|2,0|


            //if (state.HasFlag(WallState.Up))
            //{
            //    m_WallMap[x + 1, y + 2] = true;
            //}
            //if (state.HasFlag(WallState.Down))
            //{
            //    m_WallMap[x + 1, y] = true;
            //}
            //if (state.HasFlag(WallState.Left))
            //{
            //    m_WallMap[x, y + 1] = true;
            //}
            //if (state.HasFlag(WallState.Right))
            //{
            //    m_WallMap[x + 2, y + 1] = true;
            //}
        }

        private void DrawAll()
        {
            for (int i = 0; i < m_Generator.CellMap.GetLength(0); i++)
            {
                for (int j = 0; j < m_Generator.CellMap.GetLength(1); j++)
                {
                    DrawAt(i, j);
                }
            }
        }
    }
}
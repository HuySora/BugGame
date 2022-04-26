
namespace BugGame
{
    using MyBox;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Tilemaps;

#if UNITY_EDITOR
    public partial class MazeGenerator
    {
        [Range(2, 50)]
        public int EditorWidth;
        [Range(2, 50)]
        public int EditorHeight;
        public int EditorSeed;

        [ButtonMethod]
        public void EditorGenerate()
        {
            Generate(EditorWidth, EditorHeight, EditorSeed);
        }
    }

#endif

    public partial class MazeGenerator : MonoBehaviour
    {
        public static event Action<Vector2Int> HeadCellPositionChanged;

        [Separator("-----Dependencies-----")]
        [SerializeField] private MazeAlgorithm m_MazeAlgorithm;

        public Cell[,] CellMap { get; private set; }
        private Coroutine m_CurrentRoutine;

        public void Generate(int width, int height, int seed)
        {
            if (m_CurrentRoutine != null) StopCoroutine(m_CurrentRoutine);
            InitializeCellMap(width, height);
            m_MazeAlgorithm.Initialize(CellMap, seed);
            m_MazeAlgorithm.HeadCellPositionChanged += HeadCellPositionChanged;
            m_CurrentRoutine = StartCoroutine(m_MazeAlgorithm.DoAlgorithm());
        }

        public void InitializeCellMap(int width, int height)
        {
            CellMap = new Cell[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    // Initilize new cell with .WallState = WallState.All
                    CellMap[i, j] = new Cell();
                }
            }
        }
    }
}
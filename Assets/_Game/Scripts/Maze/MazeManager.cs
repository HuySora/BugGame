namespace BugGame.Maze
{
    using MyBox;
    using System;
    using System.Collections;
    using UnityEngine;

#if UNITY_EDITOR
    public partial class MazeManager
    {
        [Range(2, 200)]
        public int EditorWidth = 2;
        [Range(2, 200)]
        public int EditorHeight = 2;
        public int EditorSeed;

        [ButtonMethod]
        public void EditorGenerate()
        {
            if (!Application.isPlaying) return;

            Instance_Generate(EditorWidth, EditorHeight, EditorSeed);
        }
    }
#endif

    [Flags]
    public enum WallStates
    {
        None = 0,
        Left = 1 << 0,
        Right = 1 << 1,
        Down = 1 << 2,
        Up = 1 << 3,
        All = Left | Right | Down | Up
    }

    public partial class MazeManager : SingletonBehaviour<MazeManager>
    {
        #region Static ----------------------------------------------------------------------------------------------------
        public static event Action<Vector2Int> HeadCellPositionChanged;
        public static event Action<int, int> MazeGenerated;
        public static event Action GateReached;

        /// <summary>
        /// Generate new maze map.
        /// </summary>
        public static void Generate(int width, int height, int seed) => Current.Instance_Generate(width, height, seed);

        /// <summary>
        /// World coordinate to cell coordinate of <see cref="m_Grid"/> with it Z axis.
        /// </summary>
        public static Vector2Int WorldToCell(Vector3 pos) => Current.Instance_WorldToCell(pos);
        /// <summary>
        /// Cell coordinate of <see cref="m_Grid"/> to world coordinate with it Z axis.
        /// </summary>
        public static Vector3 CellToWorld(Vector2Int cellPos) => Current.Instance_CellToWorld(cellPos);

        public static void InvokeCell(Vector2Int cellPos) => Current.Instance_InvokeCell(cellPos);

        /// <summary>
        /// True if cell in maze bound.
        /// </summary>
        public static bool IsInBound(Vector2Int cellPos)
            => Current.m_CellMap.IsInBound(cellPos);

        /// <summary>
        /// True if these 2 cells are passable.
        /// </summary>
        public static bool IsPassable(Vector2Int fromCellPos, Vector2Int toCellPos)
            => Current.m_CellMap.IsPassable(fromCellPos, toCellPos);
        #endregion

        [Separator("-----Dependencies-----")]
        [SerializeField, AutoProperty] private Grid m_Grid;
        [SerializeField] private MazeGenerator m_MazeGenerator;
        [SerializeField] private CellTile m_CellSprite;

        [Separator("-----Settings------")]
        [OverrideLabel("Camera Fit Padding (%)")]
        [SerializeField, Range(0f, 1f)] private float m_CameraFitPadding = 0.2f;

        private CellTile[,] m_CellMap;
        private System.Random m_CurrentRng;
        private Coroutine m_CurrentRoutine;

        public void Awake()
        {
            SingletonAwake();

            // TODO: Add support for m_Grid.cellSize, m_Grid.cellLayout,... as we will hard set it for now
            m_Grid.hideFlags = HideFlags.NotEditable;
            m_Grid.cellSize = Vector3.one;
            m_Grid.cellLayout = GridLayout.CellLayout.Rectangle;
            m_Grid.cellGap = Vector3.zero;
            m_Grid.cellSwizzle = GridLayout.CellSwizzle.XYZ;
        }

        private void Instance_Generate(int width, int height, int seed)
        {
            if (m_CurrentRoutine != null) StopCoroutine(m_CurrentRoutine);

            // Initialize
            InitializeCellMap(width, height);
            m_CurrentRng = new System.Random(seed);
            m_MazeGenerator.Initialize(m_CellMap, m_CurrentRng);

            // Make sure we only subcribe once
            m_MazeGenerator.HeadCellPositionChanged -= HeadCellPositionChanged;
            m_MazeGenerator.HeadCellPositionChanged += HeadCellPositionChanged;
            m_MazeGenerator.MazeGenerated -= OnMazeGenerated;
            m_MazeGenerator.MazeGenerated += OnMazeGenerated;

            m_CurrentRoutine = StartCoroutine(m_MazeGenerator.DoAlgorithm());
        }
        private void InitializeCellMap(int width, int height)
        {
            // Setup, clear old data
            if (m_CellMap != null)
            {
                for (int i = 0; i < m_CellMap.GetLength(0); i++)
                {
                    for (int j = 0; j < m_CellMap.GetLength(1); j++)
                    {
                        // OPTIMIZABLE: Pool this (1)
                        Destroy(m_CellMap[i, j].gameObject);
                    }
                }
            }
            m_CellMap = new CellTile[width, height];

            // Initialize values
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var pos = Instance_CellToWorld(new Vector2Int(i, j));
                    // OPTIMIZABLE: Pool this (1)
                    m_CellMap[i, j] = Instantiate(m_CellSprite, pos, Quaternion.identity, transform);
                    m_CellMap[i, j].UpdateWalls(WallStates.All);
                }
            }
        }
        private void OnMazeGenerated()
        {
            // Adjust camera size to fit the maze
            float widthInUnityUnit = m_Grid.cellSize.x * m_CellMap.GetLength(0);
            float heightInUnityUnit = m_Grid.cellSize.y * m_CellMap.GetLength(1) * (1f + m_CameraFitPadding);
            CameraManager.SetOrthographicSizeToFit(widthInUnityUnit, heightInUnityUnit);

            // Adjust camera position so that the maze "anchor" to bottom center
            float heightFromOrtho = CameraManager.Main.GetOrthographicHeight();
            float posX = m_Grid.transform.position.x + widthInUnityUnit * 0.5f;
            float posY = m_Grid.transform.position.y + heightFromOrtho * 0.5f;
            CameraManager.Main.transform.position = new Vector3(posX, posY, CameraManager.Main.transform.position.z);

            // Set 1 cell to be the gate cell
            int ranX = m_CurrentRng.Next(0, m_CellMap.GetLength(0) - 1);            
            int ranY = m_CurrentRng.Next(0, m_CellMap.GetLength(1) - 1);

            m_CellMap[ranX, ranY].SetPortal(true);
            MazeGenerated?.Invoke(m_CellMap.GetLength(0), m_CellMap.GetLength(1));
        }

        private void Instance_InvokeCell(Vector2Int cellPos)
        {
            if (!m_CellMap.IsInBound(cellPos)) return;

            // TODO: Maybe add custom event for cells?
            if (!m_CellMap[cellPos.x, cellPos.y].IsGate) return;
            GateReached?.Invoke();
        }

        // TODO: Look deeper into how tilemap Z-axis work to avoid casting
        /// <summary>
        /// World coordinate to cell coordinate of <see cref="m_Grid"/> with it Z axis.
        /// </summary>
        private Vector2Int Instance_WorldToCell(Vector3 pos) => (Vector2Int)m_Grid.WorldToCell(new Vector3(pos.x, pos.y, m_Grid.transform.position.z));
        /// <summary>
        /// Cell coordinate of <see cref="m_Grid"/> to world coordinate with it Z axis.
        /// </summary>
        private Vector3 Instance_CellToWorld(Vector2Int cellPos)
        {
            var pos = m_Grid.GetCellCenterWorld((Vector3Int)cellPos);
            pos.z = m_Grid.transform.position.z;
            return pos;
        }
    }
}


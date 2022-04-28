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

        [ButtonMethod]
        public void EditorTrySolve()
        {
            if (!Application.isPlaying) return;

            Instance_TrySolve(WorldToCell(FindObjectOfType<MazePlayerController>().transform.position));
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

        public static bool TrySolve(Vector2Int fromCellPos) => Current.Instance_TrySolve(fromCellPos);

        //public static void ShowHint
        public static bool TryInvokeCell(Vector2Int cellPos) => Current.Instance_TryInvokeCell(cellPos);

        /// <summary>
        /// World coordinate to cell coordinate of <see cref="m_Grid"/> with it Z axis.
        /// </summary>
        public static Vector2Int WorldToCell(Vector3 pos) => Current.Instance_WorldToCell(pos);
        /// <summary>
        /// Cell coordinate of <see cref="m_Grid"/> to world coordinate with it Z axis.
        /// </summary>
        public static Vector3 CellToWorld(Vector2Int cellPos) => Current.Instance_CellToWorld(cellPos);
        
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
        [SerializeField] private CellTile m_CellTilePrefab;
        [SerializeField] private MazeGenerator m_MazeGenerator;
        [SerializeField] private MazeSolver m_MazeSolver;

        [Separator("-----Settings------")]
        [OverrideLabel("Camera Fit Top Padding (%)")]
        [SerializeField, Range(0f, 1f)] private float m_CameraFitTopPadding = 0.18f;
        [OverrideLabel("Camera Fit Bottom Padding (%)")]
        [SerializeField, Range(0f, 1f)] private float m_CameraFitBottomPadding = 0.01f;

        private CellTile[,] m_CellMap;
        private System.Random m_GeneratorRng;
        private Coroutine m_GeneratorRoutine;
        
        private Vector2Int m_GateCellPos;
        private Coroutine m_SolverRoutine;

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
            if (m_GeneratorRoutine != null) StopCoroutine(m_GeneratorRoutine);

            // Initialize
            InitializeCellMap(width, height);
            m_GeneratorRng = new System.Random(seed);
            m_MazeGenerator.Initialize(m_CellMap, m_GeneratorRng);

            // Make sure we only subcribe once
            m_MazeGenerator.HeadCellPositionChanged -= HeadCellPositionChanged;
            m_MazeGenerator.HeadCellPositionChanged += HeadCellPositionChanged;
            m_MazeGenerator.MazeGenerated -= OnMazeGenerated;
            m_MazeGenerator.MazeGenerated += OnMazeGenerated;

            m_GeneratorRoutine = StartCoroutine(m_MazeGenerator.DoAlgorithm());
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
                    m_CellMap[i, j] = Instantiate(m_CellTilePrefab, pos, Quaternion.identity, transform);
                    m_CellMap[i, j].UpdateWalls(WallStates.All);
                }
            }
        }
        private void OnMazeGenerated()
        {
            // Adjust camera size to fit the maze
            float widthInUnityUnit = m_Grid.cellSize.x * m_CellMap.GetLength(0);
            float heightInUnityUnit = m_Grid.cellSize.y * m_CellMap.GetLength(1) * (1f + m_CameraFitTopPadding + m_CameraFitBottomPadding);
            CameraManager.SetOrthographicSizeToFit(widthInUnityUnit, heightInUnityUnit);

            // Adjust camera position so that the maze "anchor" to bottom center
            float heightFromOrtho = CameraManager.Main.GetOrthographicHeight();
            float posX = m_Grid.transform.position.x + widthInUnityUnit * 0.5f;
            float posY = m_Grid.transform.position.y + heightFromOrtho * 0.5f - heightFromOrtho * m_CameraFitBottomPadding;
            CameraManager.Main.transform.position = new Vector3(posX, posY, CameraManager.Main.transform.position.z);

            // Set 1 cell to be the gate cell
            int ranX = m_GeneratorRng.Next(0, m_CellMap.GetLength(0) - 1);            
            int ranY = m_GeneratorRng.Next(0, m_CellMap.GetLength(1) - 1);
            // TODO: Create gate on runtime rather than embedded into the cell tile
            m_CellMap[ranX, ranY].SetPortal(true);
            m_GateCellPos = new Vector2Int(ranX, ranY);
            
            MazeGenerated?.Invoke(m_CellMap.GetLength(0), m_CellMap.GetLength(1));
        }

        private bool Instance_TrySolve(Vector2Int fromCellPos)
        {
            if (!m_CellMap.IsInBound(fromCellPos))
                return false;

            if (m_SolverRoutine != null)
                StopCoroutine(m_SolverRoutine);

            // Make sure we only subcribe once
            m_MazeSolver.PathGenerated -= OnPathGenerated;
            m_MazeSolver.PathGenerated += OnPathGenerated;

            m_MazeSolver.Initialize(m_CellMap, fromCellPos, m_GateCellPos);

            // TODO: Currently the coroutine finished instantly so we can safely return true, will
            // implementing "async operation handler" later if have time
            m_SolverRoutine = StartCoroutine(m_MazeSolver.DoAlgorithm());
            return true;
        }
        private void OnPathGenerated(Vector2Int[] pathCellPositions)
        {
            for(int i = 1; i < pathCellPositions.Length; i++)
            {
                var fromPos = CellToWorld(pathCellPositions[i - 1]);
                var toPos = CellToWorld(pathCellPositions[i]);
                Debug.DrawLine(fromPos, toPos, Color.green, 3f);
            }
        }

        private bool Instance_TryInvokeCell(Vector2Int cellPos)
        {
            if (!m_CellMap.IsInBound(cellPos))
                return false;

            // TODO: Could add custom event for cell types
            if (!m_CellMap[cellPos.x, cellPos.y].IsGate)
                return false;
            
            GateReached?.Invoke();
            return true;
        }

        // TODO: Look deeper into how tilemap Z-axis work to avoid casting
        /// <summary>
        /// World coordinate to cell coordinate of <see cref="m_Grid"/> with it Z axis.
        /// </summary>
        private Vector2Int Instance_WorldToCell(Vector3 pos)
            => (Vector2Int)m_Grid.WorldToCell(new Vector3(pos.x, pos.y, m_Grid.transform.position.z));
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


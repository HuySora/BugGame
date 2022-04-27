namespace BugGame
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
    public enum WallState
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

        /// <summary>
        /// True if cell in maze bound.
        /// </summary>
        public static bool IsInBound(Vector2Int cellPos)
            => Current.m_WallStateMap.IsInBound(cellPos);

        /// <summary>
        /// True if these 2 cells are passable.
        /// </summary>
        public static bool IsPassable(Vector2Int fromCellPos, Vector2Int toCellPos)
            => Current.m_WallStateMap.IsPassable(fromCellPos, toCellPos);
        #endregion

        [Separator("-----Dependencies-----")]
        [SerializeField] private MazeAlgorithm m_MazeAlgorithm;
        [SerializeField] private CellTile m_CellSprite;
        // TODO: Add support for m_Grid.cellSize, m_Grid.cellLayout,... ?
        [SerializeField, ReadOnly] private Grid m_Grid;

        [Separator("-----Settings------")]
        [OverrideLabel("Camera Fit Padding (%)")]
        [SerializeField, Range(0f, 1f)] private float m_CameraFitPadding = 0.2f;

        private WallState[,] m_WallStateMap;
        private Coroutine m_CurrentRoutine;
        private CellTile[,] m_TileMap;

        public void Awake()
        {
            SingletonAwake();
            m_Grid = gameObject.AddComponent<Grid>();
            m_Grid.hideFlags = HideFlags.HideAndDontSave;
        }

        public void Instance_Generate(int width, int height, int seed)
        {
            if (m_CurrentRoutine != null) StopCoroutine(m_CurrentRoutine);

            // Initialize
            InitializeCellMap(width, height);
            m_MazeAlgorithm.Initialize(m_WallStateMap, seed);

            // Make sure we only subcribe once
            m_MazeAlgorithm.HeadCellPositionChanged -= HeadCellPositionChanged;
            m_MazeAlgorithm.HeadCellPositionChanged += HeadCellPositionChanged;
            m_MazeAlgorithm.MazeGenerated -= OnMazeGenerated;
            m_MazeAlgorithm.MazeGenerated += OnMazeGenerated;

            m_CurrentRoutine = StartCoroutine(m_MazeAlgorithm.DoAlgorithm());
        }

        public void InitializeCellMap(int width, int height)
        {
            // Setup, clear old data
            m_WallStateMap = new WallState[width, height];
            if (m_TileMap != null)
            {
                for (int i = 0; i < m_TileMap.GetLength(0); i++)
                {
                    for (int j = 0; j < m_TileMap.GetLength(1); j++)
                    {
                        // OPTIMIZABLE: Pool this (1)
                        Destroy(m_TileMap[i, j].gameObject);
                    }
                }
            }
            m_TileMap = new CellTile[width, height];

            // Initialize values
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    m_WallStateMap[i, j] = WallState.All;
                    var pos = Instance_CellToWorld(new Vector2Int(i, j));
                    // OPTIMIZABLE: Pool this (1)
                    m_TileMap[i, j] = Instantiate(m_CellSprite, pos, Quaternion.identity, transform);
                    m_TileMap[i, j].UpdateCell(WallState.All);
                }
            }
        }
        public void OnMazeGenerated()
        {
            // Update cell visual (walls)
            for(int i = 0; i < m_WallStateMap.GetLength(0); i++)
            {
                for(int j = 0; j < m_WallStateMap.GetLength(1); j++)
                {
                    m_TileMap[i, j].UpdateCell(m_WallStateMap[i, j]);
                }
            }

            // Adjust camera size to fit the maze
            float widthInUnityUnit = m_Grid.cellSize.x * m_WallStateMap.GetLength(0);
            float heightInUnityUnit = m_Grid.cellSize.y * m_WallStateMap.GetLength(1) * (1f + m_CameraFitPadding);
            CameraManager.SetOrthographicSizeToFit(widthInUnityUnit, heightInUnityUnit);

            // Adjust camera position so that the maze "anchor" to bottom center
            float heightFromOrtho = CameraManager.Main.GetOrthographicHeight();
            float posX = m_Grid.transform.position.x + widthInUnityUnit * 0.5f;
            float posY = m_Grid.transform.position.y + heightFromOrtho * 0.5f;
            CameraManager.Main.transform.position = new Vector3(posX, posY, CameraManager.Main.transform.position.z);
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


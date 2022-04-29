using BugGame.Maze;
using BugGame.Stage;
using BugGame.UI;

namespace BugGame
{
    using System;
    using UnityEngine;

    public enum GameState
    {
        Stage,
        Maze
    }

    public class GameManager : SingletonBehaviour<GameManager>
    {
        #region Static ----------------------------------------------------------------------------------------------------
        public static event Action<int> MazeStarted;
        public static int CurrentStage => Current.m_CurrentStage;
        public static void StartMaze(int index) => Current.Instance_StartMaze(index);
        public static void RestartMaze() => Current.Instance_StartMaze(CurrentStage);
        public static void NextMaze() => Current.Instance_StartMaze(CurrentStage + 1);
        public static void GoToStageMap() => Current.Instance_GoToStageMap();
        #endregion

        private GameState m_GameState = GameState.Stage;
        private int m_CurrentStage;

        public void Awake()
        {
            SingletonAwake();
            Physics.autoSimulation = false;
            Physics2D.simulationMode = SimulationMode2D.Script;
            ViewManager.SwitchTo<StageView>();
        }

        private void Instance_StartMaze(int index)
        {
            // Lazy code here since it doesn't matter much (seed will always have a value)
            StageManager.TryGetData(index, out int seed);
            m_CurrentStage = index;

            // Switch view & state
            ViewManager.SwitchTo<GameView>();
            m_GameState = GameState.Maze;

            // Do maze generating
            MazeManager.MazeGenerated -= OnMazeGenerated;
            MazeManager.MazeGenerated += OnMazeGenerated;
            MazeManager.GateReached -= OnGateReached;
            MazeManager.GateReached += OnGateReached;

            // TODO: Maybe make this a feature? (Hard-code width & size because we don't really need it atm)
            int width = Mathf.Clamp(index, 2, 10);
            int height = Mathf.Clamp(index + 3, 2, 15);
            MazeManager.Generate(width, height, seed);
            
            MazeStarted?.Invoke(index);

            #region Local Functions
            static void OnMazeGenerated(int width, int height)
            {
                // Spawn at top-left
                PlayerManager.Spawn(new Vector2Int(0, height - 1));
            }
            static void OnGateReached()
            {
                StartMaze(CurrentStage + 1);
            }
            #endregion
        }

        private void Instance_GoToStageMap()
        {
            // Switch view & state
            ViewManager.SwitchTo<StageView>();
            m_GameState = GameState.Stage;

            MazeManager.Clear();
            PlayerManager.Despawn();
        }
    }
}


using BugGame.Maze;
using BugGame.Stage;
using BugGame.UI;

namespace BugGame
{
    using UnityEngine;

    public enum GameState
    {
        Stage,
        Maze
    }

    public class GameManager : SingletonBehaviour<GameManager>
    {
        #region Static ----------------------------------------------------------------------------------------------------
        public static int CurrentStage => Current.m_CurrentStage;
        public static void StartMaze(int index) => Current.Instance_StartMaze(index);
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
            // Switch view & state
            ViewManager.SwitchTo<GameView>();
            m_GameState = GameState.Maze;

            // Stage data only contain seed atm
            int seed = StageManager.GetData(index);
            m_CurrentStage = index;

            // Do maze generating
            MazeManager.MazeGenerated -= OnMazeGenerated;
            MazeManager.MazeGenerated += OnMazeGenerated;
            MazeManager.GateReached -= OnGateReached;
            MazeManager.GateReached += OnGateReached;

            // TODO: Maybe make this a feature? (Hard-code width & size because we don't really need it atm)
            int width = Mathf.Clamp(index, 2, 20);
            int height = Mathf.Clamp(index + 3, 2, 20);
            MazeManager.Generate(width, height, seed);

            #region Local Functions
            static void OnMazeGenerated(int width, int height)
            {
                // Spawn at top-left
                MazePlayerManager.Spawn(new Vector2Int(0, height - 1));
            }
            static void OnGateReached()
            {
                StartMaze(CurrentStage + 1);
            }
            #endregion
        }
    }
}


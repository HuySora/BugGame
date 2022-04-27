using BugGame.Maze;
using BugGame.UI;

namespace BugGame
{
    using UnityEngine;

    public class GameManager : SingletonBehaviour<GameManager>
    {
        #region Static ----------------------------------------------------------------------------------------------------
        public static void StartGame(int width, int height, int seed) => Current.Instance_StartGame(width, height, seed);
        #endregion

        public void Awake()
        {
            SingletonAwake();
            Physics.autoSimulation = false;
            Physics2D.simulationMode = SimulationMode2D.Script;
        }

        public void Instance_StartGame(int width, int height, int seed)
        {
            //ViewManager.SwitchTo<GameView>
            MazeManager.MazeGenerated -= OnMazeGenerated;
            MazeManager.MazeGenerated += OnMazeGenerated;
            MazeManager.GateReached -= OnGateReached;
            MazeManager.GateReached += OnGateReached;

            MazeManager.Generate(width, height, seed);

            static void OnMazeGenerated(int width, int height)
            {
                // Spawn at top-left
                MazePlayerManager.Spawn(new Vector2Int(0, height - 1));
            }
            static void OnGateReached()
            {
                StartGame(Random.Range(2, 5), Random.Range(2, 5), Random.Range(0, int.MaxValue));
            }
        }

    }
}


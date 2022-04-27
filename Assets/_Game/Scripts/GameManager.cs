namespace BugGame
{
    using MyBox;
    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;

    public partial class GameManager : IPrepare
    {
        [ButtonMethod]
        public bool Prepare() {
            bool wasNull = false;
            
            return wasNull;
        }
    }
#endif

    public partial class GameManager : MonoBehaviour
    {
        public void Awake()
        {
            StartNewGame(10, 13, 0);
        }

        public void StartNewGame(int width, int height, int seed)
        {
            MazeManager.Generate(width, height, seed);
            // Spawn at top-left
            MazePlayerManager.Spawn(new Vector2Int(0, height - 1));
        }
    }
}


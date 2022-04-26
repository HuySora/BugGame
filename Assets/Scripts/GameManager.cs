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
    }
}


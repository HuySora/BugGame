namespace BugGame
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>
    /// Unify mouse and first touch input.
    /// </summary>
    public class PrimaryPointer
    {
        /// <summary>
        /// <see cref="Input.GetMouseButtonDown(int)">Input.GetMouseButtonDown(0)</see> or
        /// <see cref="Input.GetTouch(int)">Input.GetTouch(0)</see>.phase is <see cref="TouchPhase.Began"/>.
        /// </summary>
        public static bool WasPressedThisFrame()
        {
            // Touch
            if (Input.touchCount > 0 &&  Input.GetTouch(0).phase == TouchPhase.Began)
                return true;

            // Mouse
            return Input.mousePresent && Input.GetMouseButtonDown(0);
        }

        /// <summary>
        /// <see cref="Input.GetMouseButton(int)">Input.GetMouseButton(0)</see> or 
        /// <see cref="Input.GetTouch(int)">Input.GetTouch(0)</see>.phase is <see cref="TouchPhase.Moved"/> or <see cref="TouchPhase.Stationary"/>.
        /// </summary>
        public static bool IsPressed()
        {
            // Touch
            if (Input.touchCount > 0 &&  Input.GetTouch(0).phase is TouchPhase.Moved or TouchPhase.Stationary)
                return true;

            // Mouse
            return Input.mousePresent && Input.GetMouseButton(0);
        }

        /// <summary>
        /// <see cref="Input.GetMouseButtonUp(int)">Input.GetMouseButtonUp(0)</see> or 
        /// <see cref="Input.GetTouch(int)">Input.GetTouch(0)</see>.phase is <see cref="TouchPhase.Ended"/>.
        /// </summary>
        public static bool WasReleasedThisFrame()
        {
            // Touch
            if (Input.touchCount > 0 &&  Input.GetTouch(0).phase == TouchPhase.Ended)
                return true;            
            
            // Mouse
            return Input.mousePresent && Input.GetMouseButtonUp(0);
        }
        
        /// <summary>
        /// Get "primary pointer" position if avaiable.
        /// </summary>
        public static bool TryGetPosition(out Vector3 pos)
        {
            if (Input.touchCount > 0)
            {
                pos = Input.GetTouch(0).position;
                return true;
            }
            if (Input.mousePresent)
            {
                pos = Input.mousePosition;
                return true;
            }

            pos = Vector3.zero;
            return false;
        }

        /// <summary>
        /// <see cref="EventSystem.current"/>.IsPointerOverGameObject() but also work for touches
        /// </summary>
        public static bool IsOverGameObject()
        {
            // Touch
            if (Input.touchCount > 0)
                EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
            
            // Mouse
            return EventSystem.current.IsPointerOverGameObject();
        }
    }
}


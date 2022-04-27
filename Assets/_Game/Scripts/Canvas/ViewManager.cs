namespace BugGame.UI
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    
    public partial class ViewManager : SingletonBehaviour<ViewManager> {
        #region Static ----------------------------------------------------------------------------------------------------
        public static Canvas Canvas => Current.m_Canvas;
        public static void SwitchTo<T>() where T : ViewBehaviour => Current.InnerSwitchTo<T>();
        public static void SwitchTo(ViewBehaviour view) => Current.InnerSwitchTo(view);
        #endregion

        [SerializeField] private Canvas m_Canvas;
        [SerializeField] private ViewBehaviour m_InitialView;

        // OPTIMIZABLE: Not sure if this cost performance
        private Dictionary<Type, ViewBehaviour> m_TypeToInstance;

        private void Awake()
        {
            m_TypeToInstance = new Dictionary<Type, ViewBehaviour>();
            foreach (var view in FindObjectsOfType<ViewBehaviour>())
            {
                m_TypeToInstance[view.GetType()] = view;
            }
        }

        private void Start() => m_InitialView.Open();

        private void InnerSwitchTo<T>()
        where T : ViewBehaviour
        {
            if (!m_TypeToInstance.TryGetValue(typeof(T), out var instance))
            {
                DebugEx.LogNull($"No instance of type {typeof(T)} in the dictionary.", this);
                return;
            }

            Toggle(instance);
        }
        private void InnerSwitchTo(ViewBehaviour view)
        {
            if (!m_TypeToInstance.ContainsValue(view))
            {
                DebugEx.LogNull($"No instance of type {view.GetType()} in the dictionary.", this);
                return;
            }

            Toggle(view);
        }

        private void Toggle(ViewBehaviour targetView)
        {
            foreach (var view in m_TypeToInstance.Values)
            {
                if (view != targetView) view.Close();
            }

            targetView.Open();
        }
    }
}


namespace BugGame
{
    using MyBox;
    using System;
    using UnityEngine;

    /// <summary>
    /// Currently only hold reference to the main camera
    /// </summary>
    public partial class CameraManager : SingletonBehaviour<CameraManager>
    {
        #region Static ----------------------------------------------------------------------------------------------------
        public static Camera Main => Current.m_Main;
        public static void SetOrthographicSizeToFit(float widthInUnityUnit, float heightInUnityUnit) => Current.Instance_ChangeOrthographicSizeToFit(widthInUnityUnit, heightInUnityUnit);
        #endregion

        [Separator("-----Depedencies-----")]
        [SerializeField] private Camera m_Main;

        private void Instance_ChangeOrthographicSizeToFit(float widthInUnityUnit, float heightInUnityUnit)
        {
            var sizeFromX = (widthInUnityUnit / m_Main.aspect) / 2;
            var sizeFromY = heightInUnityUnit / 2;

            m_Main.orthographicSize = Mathf.Max(sizeFromX, sizeFromY);
        }
    }
}


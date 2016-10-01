namespace Gridia
{
    using System;
    using System.Collections;

    using UnityEngine;

    public static class SceneManager
    {
        #region Fields

        private static Hashtable _sceneArguments;

        #endregion Fields

        #region Methods

        public static T GetArguement<T>(String key)
        {
            return (T)_sceneArguments[key];
        }

        public static void LoadScene(String sceneName, Hashtable sceneArguments = null)
        {
            _sceneArguments = sceneArguments;
            MainThreadQueue.Add(() => Application.LoadLevel(sceneName));
        }

        #endregion Methods
    }
}
using System;
using System.Collections;
using UnityEngine;

namespace Gridia
{
    public static class SceneManager
    {
        private static Hashtable _sceneArguments;

        public static void LoadScene(String sceneName, Hashtable sceneArguments = null)
        {
            _sceneArguments = sceneArguments;
            Application.LoadLevel(sceneName);
        }

        public static T GetArguement<T>(String key)
        {
            return (T)_sceneArguments[key];
        }
    }
}

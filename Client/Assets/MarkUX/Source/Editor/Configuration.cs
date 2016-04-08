#region Using Statements
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEngine;
#endregion

namespace MarkUX.Editor
{
    /// <summary>
    /// Serializable system configuration used by the asset processor.
    /// </summary>
    public class Configuration : ScriptableObject
    {
        #region Fields
                
        public string UILayer;
        public List<string> ViewPaths;
        private static Configuration _instance;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Configuration()
        {
            ViewPaths = new List<string>();
            ViewPaths.Add("Assets/MarkUX/Views/");
            ViewPaths.Add("Assets/Views/");
            UILayer = "UI";
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads global configuration asset.
        /// </summary>
        public static Configuration Load()
        {
            return null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets global configuration instance.
        /// </summary>
        public static Configuration Instance
        {
            get
            {
                if (_instance == null)
                {
                    // attempt to load configuration asset
                    Configuration configuration = AssetDatabase.LoadAssetAtPath("Assets/MarkUX/Configuration/Configuration.asset", typeof(Configuration)) as Configuration;
                    if (configuration == null)
                    {
                        // create new asset                        
                        System.IO.Directory.CreateDirectory("Assets/MarkUX/Configuration/");
                        configuration = ScriptableObject.CreateInstance<Configuration>();
                        AssetDatabase.CreateAsset(configuration, "Assets/MarkUX/Configuration/Configuration.asset");
                        AssetDatabase.Refresh();
                    }

                    // validate some values
                    if (!configuration.ViewPaths.Any())
                    {
                        Debug.LogError("[MarkUX.356] No view paths found. Using default configuration.");
                        configuration = ScriptableObject.CreateInstance<Configuration>();
                    }
                    else if (String.IsNullOrEmpty(configuration.UILayer))
                    {
                        Debug.LogError("[MarkUX.357] UILayer not set. Using default configuration.");
                        configuration = ScriptableObject.CreateInstance<Configuration>();
                    }
                    else
                    {
                        foreach (var viewPath in configuration.ViewPaths)
                        {
                            if (String.IsNullOrEmpty(viewPath) ||
                                !viewPath.StartsWith("Assets/") ||
                                !viewPath.EndsWith("/"))
                            {
                                Debug.LogError("[MarkUX.358] Invalid view path in configuration. The path must start with 'Assets/' and end with '/'. Using default configuration.");
                                Debug.LogError("This sometimes happens if Unity hasn't converted the configuration asset to correct serialization mode. To fix go to [Edit -> Project settings -> Editor] and change Asset Serialization Mode to another mode and back to the desired mode. If you inspect the Configuration asset at MarkUX/Configuration/Configuration.asset the values should be in plain text and the view paths should look like file path strings (not a bunch of numbers).");

                                configuration = ScriptableObject.CreateInstance<Configuration>();
                                break;
                            }
                        }
                    }                  

                    _instance = configuration;
                }

                return _instance;
            }
        }

        #endregion
    }
}

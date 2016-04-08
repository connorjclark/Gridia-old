#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
#endregion

namespace MarkUX
{
    /// <summary>
    /// View that contains content.
    /// </summary>
    public class ContentView : View
    {
        #region Fields

        private GameObject _contentGameObject = null;

        #endregion

        #region Properties

        /// <summary>
        /// Returns gameObject containing the content of this view.
        /// </summary>
        public GameObject ContentContainer
        {
            get
            {
                if (_contentGameObject == null)
                {
                    // check if there is a content container that has this view as parent
                    Transform[] children = transform.GetComponentsInChildren<Transform>(true);
                    foreach (Transform child in children)
                    {
                        if (child.gameObject.name == "ContentContainer")
                        {
                            var childv = child.gameObject.GetComponent<View>();
                            if (childv != null && childv.Parent == transform.gameObject)
                            {
                                _contentGameObject = child.gameObject;
                            }
                        }
                    }
                }

                return _contentGameObject;
            }
        }

        #endregion
    }
}

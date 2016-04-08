#region Using Statements
using MarkUX.Views;
using System;
using UnityEngine.EventSystems;
#endregion

namespace MarkUX.Views
{
    /// <summary>
    /// Contains list selection action data.
    /// </summary>
    public class ListSelectionActionData : ActionData
    {
        public ListItem ListItem;
    }
}

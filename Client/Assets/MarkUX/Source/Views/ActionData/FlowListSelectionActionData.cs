#region Using Statements
using MarkUX.Views;
using System;
using UnityEngine.EventSystems;
#endregion

namespace MarkUX.Views
{
    /// <summary>
    /// Contains flow list selection action data.
    /// </summary>
    public class FlowListSelectionActionData : ActionData
    {
        public FlowListItem FlowListItem;
    }
}

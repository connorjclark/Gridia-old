using Gridia;
using MarkUX;
using MarkUX.ValueConverters;
using MarkUX.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MarkUX.UnityProject
{
    public class ContainerView : View
    {
        public int NumInRow = 10;
        public List<ItemInstanceWrapper> Items;
        public Views.FlowList Container;

        public override void Initialize()
        {
            base.Initialize();

            float rowSpacing = Container.Spacing.Pixels;
            float rowWidth = NumInRow * 32 + (NumInRow - 2) * 2 * rowSpacing;
            Container.SetValue(() => Container.Width, ElementSize.GetPixels(rowWidth));
        }
    }
}

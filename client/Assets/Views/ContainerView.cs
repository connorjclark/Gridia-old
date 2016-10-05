namespace MarkLight.UnityProject
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Gridia;

    using MarkLight;
    using MarkLight.ValueConverters;
    using MarkLight.Views.UI;

    using UnityEngine;
    using UnityEngine.EventSystems;

    public class ContainerView : UIView
    {
        #region Fields

        public ViewAction BeginDrag;
        public Views.UI.List Container;
        public int ContainerId;
        public String ContainerName;
        public Views.UI.Label ContainerNameLabel;
        // public ViewAction Drag;
        // public ViewAction EndDrag;
        public ObservableList<ItemInstance> Items;
        public int NumInRow = 10;
        public Views.UI.ListItem Template;

        #endregion Fields

        #region Methods

        public override void Initialize()
        {
            base.Initialize();

            float rowSpacing = Container.Spacing.Value.Pixels;
            float rowWidth = NumInRow * Template.Width.Value.Pixels + (NumInRow + 1) * rowSpacing;
            Container.Width.Value = ElementSize.FromPixels(rowWidth);
        }

        public void OnBeginDrag(ItemView source)
        {
            int index = Items.IndexOf(source.ItemInstance);

            Main.Instance.MouseDraggingItemDone = false;

            Main.Instance.MouseDownContainer = this;
            Main.Instance.MouseDownIndex = index;
        }

        public void OnDrag(ItemView source, PointerEventData data)
        {
            // Debug.LogWarning("drag");
            // Debug.LogWarning(source == null ? "false" : source.ItemInstance.Item.Name);
        }

        // public void OnDrop(ItemView source)
        // {
        //     int index = Items.IndexOf(source.ItemInstance);

        //     Main.Instance.MouseUpContainer = this;
        //     Main.Instance.MouseUpIndex = index;
        // }

        public void OnDrop(ItemView source)
        {
            int index = Items.IndexOf(source.ItemInstance);

            Main.Instance.MouseUpContainer = this;
            Main.Instance.MouseUpIndex = index;
        }

        public void OnEndDrag(ItemView source)
        {
            Main.Instance.MouseDraggingItemDone = true;
        }

        // TODO is there a way to run a callback when a field changes?
        public void SetItems(ObservableList<ItemInstance> items)
        {
            SetValue("Items", items);

            int NumInColumn = (int) Math.Ceiling((float)items.Count() / NumInRow);
            Height.Value = ElementSize.FromPixels(NumInColumn * Template.Height.Value.Pixels + ContainerNameLabel.Height.Value.Pixels);
        }

        #endregion Methods
    }
}
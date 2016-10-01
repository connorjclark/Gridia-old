namespace Gridia
{
    using System.Collections.Generic;

    using MarkLight;

    #region Delegates

    public delegate void ContainerChanged(int containerId, ItemInstance itemInstance, int indexChanged);

    public delegate void ContainerCreated(int containerId, ObservableList<ItemInstance> items);

    #endregion Delegates

    public class GameState
    {
        #region Fields

        public static GameState Instance;

        public ContainerChanged ContainerChanged;
        public ContainerCreated ContainerCreated;
        public Dictionary<int, ObservableList<ItemInstance>> ContainerItemLists = new Dictionary<int, ObservableList<ItemInstance>>(); // :(
        public int InventoryContainerId;

        #endregion Fields

        #region Constructors

        private GameState()
        {
        }

        #endregion Constructors

        #region Methods

        public static void Create()
        {
            Instance = new GameState();
        }

        public void SetContainerItem(int containerId, ItemInstance itemInstance, int index)
        {
            ContainerItemLists[containerId][index] = itemInstance;
            ContainerChanged(containerId, itemInstance, index);
        }

        public void SetContainerItems(int containerId, List<ItemInstance> items)
        {
            if (!ContainerItemLists.ContainsKey(containerId))
            {
                var containerItems = ContainerItemLists[containerId] = new ObservableList<ItemInstance>(items);
                ContainerCreated(containerId, ContainerItemLists[containerId]);
            }
            else
            {
                ContainerItemLists[containerId].Replace(items);
                ContainerCreated(containerId, ContainerItemLists[containerId]);
            }
        }

        #endregion Methods
    }
}
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

        private static GameState _instance;

        public static GameState Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameState();
                }

                return _instance;
            }
        }

        public ContainerChanged ContainerChanged;
        public ContainerCreated ContainerCreated;
        public int EquipmentContainerId;
        public int InventoryContainerId;

        private Dictionary<int, ObservableList<ItemInstance>> ContainerItemLists = new Dictionary<int, ObservableList<ItemInstance>>(); // :(

        #endregion Fields

        #region Constructors

        private GameState()
        {
        }

        #endregion Constructors

        #region Methods

        public static void Clear()
        {
            _instance = new GameState();
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
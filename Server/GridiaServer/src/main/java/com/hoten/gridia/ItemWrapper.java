package com.hoten.gridia;

import com.hoten.gridia.content.ItemInstance;
import com.hoten.gridia.map.Coord;
import com.hoten.gridia.serving.ServingGridia;

public interface ItemWrapper {

    ItemInstance getItemInstance();

    boolean addItemToSource(ItemInstance itemToAdd);

    boolean addItemHere(ItemInstance itemToAdd);

    void changeWrappedItem(ItemInstance newItem);

    public static class WorldItemWrapper implements ItemWrapper {

        private final ServingGridia _server; // :(
        private final Coord _location;
        private final ItemInstance _item;

        public WorldItemWrapper(ServingGridia server, Coord location) {
            _server = server;
            _location = location;
            _item = server.tileMap.getItem(location);
        }

        @Override
        public boolean addItemToSource(ItemInstance itemToAdd) {
            return _server.addItemNear(itemToAdd, _location, 6, true) != null;
        }

        @Override
        public void changeWrappedItem(ItemInstance newItem) {
            _server.changeItem(_location, newItem);
        }

        @Override
        public boolean addItemHere(ItemInstance itemToAdd) {
            return _server.addItem(itemToAdd, _location) != null;
        }

        @Override
        public ItemInstance getItemInstance() {
            return _item;
        }

        public boolean isHighestLevel() {
            return _location.z == 0;
        }

        public boolean isLowestLevel() {
            return _location.z == _server.tileMap.depth - 1;
        }

        public ItemInstance getItemBelow() {
            return _server.tileMap.getItem(_location.add(0, 0, 1));
        }

        public ItemInstance getItemAbove() {
            return _server.tileMap.getItem(_location.add(0, 0, -1));
        }

        public void setItemBelow(ItemInstance newItem) {
            _server.changeItem(_location.add(0, 0, 1), newItem);
        }

        public void setItemAbove(ItemInstance newItem) {
            _server.changeItem(_location.add(0, 0, -1), newItem);
        }

        public boolean moveItemAbove() {
            return _server.moveItemOutOfTheWay(_location.add(0, 0, -1));
        }

        public boolean moveItemBelow() {
            return _server.moveItemOutOfTheWay(_location.add(0, 0, 1));
        }
    }

    public static class ContainerItemWrapper implements ItemWrapper {

        private final Container _container;
        private final int _slot;
        private final ItemInstance _item;

        public ContainerItemWrapper(Container container, int slot) {
            _container = container;
            _slot = slot;
            _item = slot != -1 ? container.get(slot) : ItemInstance.NONE;
        }

        @Override
        public boolean addItemToSource(ItemInstance itemToAdd) {
            return _container.add(itemToAdd);
        }

        @Override
        public void changeWrappedItem(ItemInstance newItem) {
            _container.set(_slot, newItem);
        }

        @Override
        public boolean addItemHere(ItemInstance itemToAdd) {
            return _container.add(_slot, itemToAdd);
        }

        @Override
        public ItemInstance getItemInstance() {
            return _item;
        }
    }
}

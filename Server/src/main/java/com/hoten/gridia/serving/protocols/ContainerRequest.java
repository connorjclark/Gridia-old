package com.hoten.gridia.serving.protocols;

import com.google.gson.JsonObject;
import com.hoten.gridia.Container;
import com.hoten.gridia.Player;
import com.hoten.gridia.content.Item;
import com.hoten.gridia.content.ItemInstance;
import com.hoten.gridia.map.Coord;
import com.hoten.gridia.serializers.GridiaGson;
import com.hoten.gridia.serving.ConnectionToGridiaClientHandler;
import com.hoten.gridia.serving.ServingGridia;
import com.hoten.servingjava.message.JsonMessageHandler;
import java.io.IOException;

public class ContainerRequest extends JsonMessageHandler<ConnectionToGridiaClientHandler> {

    @Override
    protected void handle(ConnectionToGridiaClientHandler connection, JsonObject data) throws IOException {
        ServingGridia server = connection.getServer();
        Coord location = GridiaGson.get().fromJson(data.get("loc"), Coord.class);
        ItemInstance containerItem = server.tileMap.getTile(location).item;
        requestContainer(server, connection, containerItem);
    }

    public void requestContainer(ServingGridia server, ConnectionToGridiaClientHandler connection, ItemInstance item) throws IOException {
        Player player = connection.getPlayer();
        if (item.getItem().itemClass == Item.ItemClass.Container) {
            Container container;
            synchronized (item) {
                JsonObject itemData = item.getData();
                if (itemData.has("containerId") && server.containerFactory.exists(itemData.get("containerId").getAsInt())) {
                    container = server.containerFactory.get(itemData.get("containerId").getAsInt());
                } else {
                    container = server.containerFactory.create(Container.ContainerType.Other, 20);
                    itemData.addProperty("containerId", container.id);
                    server.sendToAll(server.messageBuilder.chat(player.getUsername(), "Ah! That brand new container smell.", player.creature.location));
                }
            }

            if (!player.openedContainers.contains(container.id)) {
                connection.send(server.messageBuilder.container(container, item.getItem().id));
                player.openedContainers.add(container.id);
            }
        }
    }
}

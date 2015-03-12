package com.hoten.gridia.serving.protocols;

import com.google.gson.JsonObject;
import com.hoten.gridia.ItemWrapper;
import com.hoten.gridia.Player;
import com.hoten.gridia.content.Item;
import com.hoten.gridia.content.ItemInstance;
import com.hoten.gridia.content.ItemUse;
import com.hoten.gridia.serving.ConnectionToGridiaClientHandler;
import com.hoten.gridia.serving.ServingGridia;
import com.hoten.servingjava.message.JsonMessageHandler;
import java.io.IOException;
import java.util.List;

public class UseItem extends JsonMessageHandler<ConnectionToGridiaClientHandler> {

    @Override
    protected void handle(ConnectionToGridiaClientHandler connection, JsonObject data) throws IOException {
        ServingGridia server = connection.getServer();
        Player player = connection.getPlayer();
        int source = data.get("source").getAsInt();
        int dest = data.get("dest").getAsInt();
        int sourceIndex = data.get("si").getAsInt();
        int destIndex = data.get("di").getAsInt();

        ItemWrapper tool = server.getItemFrom(player, source, sourceIndex);
        ItemWrapper focus = server.getItemFrom(player, dest, destIndex);

        List<ItemUse> uses = server.contentManager.getItemUses(tool.getItemInstance().getItem(), focus.getItemInstance().getItem());

        if (uses.isEmpty()) {
            if (tool.getItemInstance() == ItemInstance.NONE && focus.getItemInstance().getItem().itemClass == Item.ItemClass.Container) {
                new ContainerRequest().requestContainer(server, connection, focus.getItemInstance());
            }
            return;
        }

        if (uses.size() == 1) {
            server.executeItemUse(connection, uses.get(0), tool, focus, destIndex);
        } else {
            connection.send(server.messageBuilder.itemUsePick(uses));
            player.useSource = source;
            player.useDest = dest;
            player.useSourceIndex = sourceIndex;
            player.useDestIndex = destIndex;
        }
    }
}

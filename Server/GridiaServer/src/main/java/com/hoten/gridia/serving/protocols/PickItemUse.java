package com.hoten.gridia.serving.protocols;

import com.google.gson.JsonObject;
import com.hoten.gridia.ItemWrapper;
import com.hoten.gridia.Player;
import com.hoten.gridia.content.ItemInstance;
import com.hoten.gridia.content.ItemUse;
import com.hoten.gridia.serving.ConnectionToGridiaClientHandler;
import com.hoten.gridia.serving.ServingGridia;
import com.hoten.servingjava.message.JsonMessageHandler;
import java.io.IOException;
import java.util.List;

public class PickItemUse extends JsonMessageHandler<ConnectionToGridiaClientHandler> {

    @Override
    protected void handle(ConnectionToGridiaClientHandler connection, JsonObject data) throws IOException {
        ServingGridia server = connection.getServer();
        Player player = connection.getPlayer();
        int useIndex = data.get("useIndex").getAsInt();

        ItemWrapper tool = server.getItemFrom(player, player.useSource, player.useSourceIndex);
        ItemWrapper focus = server.getItemFrom(player, player.useDest, player.useDestIndex);
        List<ItemUse> uses = server.contentManager.getItemUses(tool.getItemInstance().getItem(), focus.getItemInstance().getItem());
        ItemUse use = uses.get(useIndex);
        server.executeItemUse(connection, use, tool, focus, player.useDest);
    }
}

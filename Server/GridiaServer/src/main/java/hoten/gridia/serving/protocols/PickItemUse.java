package hoten.gridia.serving.protocols;

import com.google.gson.JsonObject;
import hoten.gridia.Player;
import hoten.gridia.content.ItemInstance;
import hoten.gridia.content.ItemUse;
import hoten.gridia.serving.ConnectionToGridiaClientHandler;
import hoten.gridia.serving.ServingGridia;
import hoten.serving.message.JsonMessageHandler;
import java.io.IOException;
import java.util.List;

public class PickItemUse extends JsonMessageHandler<ConnectionToGridiaClientHandler> {

    @Override
    protected void handle(ConnectionToGridiaClientHandler connection, JsonObject data) throws IOException {
        ServingGridia server = connection.getServer();
        Player player = connection.getPlayer();
        int useIndex = data.get("useIndex").getAsInt();

        ItemInstance tool = server.getItemFrom(player, player.useSource, player.useSourceIndex);
        ItemInstance focus = server.getItemFrom(player, player.useDest, player.useDestIndex);
        List<ItemUse> uses = server.contentManager.getItemUses(tool.data, focus.data);
        ItemUse use = uses.get(useIndex);
        server.executeItemUse(connection, use, tool, focus, player.useSource, player.useDest, player.useSourceIndex, player.useDestIndex);
    }
}

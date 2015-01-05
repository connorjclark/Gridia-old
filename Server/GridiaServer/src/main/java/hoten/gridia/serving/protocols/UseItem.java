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

public class UseItem extends JsonMessageHandler<ConnectionToGridiaClientHandler> {

    @Override
    protected void handle(ConnectionToGridiaClientHandler connection, JsonObject data) throws IOException {
        ServingGridia server = connection.getServer();
        Player player = connection.getPlayer();
        String source = data.get("source").getAsString();
        String dest = data.get("dest").getAsString();
        int sourceIndex = data.get("si").getAsInt();
        int destIndex = data.get("di").getAsInt();

        ItemInstance tool = server.getItemFrom(player, source, sourceIndex);
        ItemInstance focus = server.getItemFrom(player,dest, destIndex);

        List<ItemUse> uses = server.contentManager.getItemUses(tool.data, focus.data);

        if (uses.isEmpty()) {
            return;
        }

        if (uses.size() == 1) {
            server.executeItemUse(connection, uses.get(0), tool, focus, source, dest, sourceIndex, destIndex);
        } else {
            connection.send(server.messageBuilder.itemUsePick(uses));
            player.useSource = source;
            player.useDest = dest;
            player.useSourceIndex = sourceIndex;
            player.useDestIndex = destIndex;
        }
    }
}

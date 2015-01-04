package hoten.gridia.serving.protocol;

import com.google.gson.JsonObject;
import hoten.gridia.Player;
import hoten.gridia.content.ItemInstance;
import hoten.gridia.serving.ConnectionToGridiaClientHandler;
import hoten.gridia.serving.ServingGridia;
import hoten.serving.message.JsonMessageHandler;

public class MoveItem extends JsonMessageHandler<ConnectionToGridiaClientHandler> {

    @Override
    protected void handle(ConnectionToGridiaClientHandler connection, JsonObject data) {
        ServingGridia server = connection.getServer();
        Player player = connection.getPlayer();
        String source = data.get("source").getAsString();
        String dest = data.get("dest").getAsString();
        int sourceIndex = data.get("si").getAsInt();
        int quantityToMove = data.get("quantity").getAsInt();
        int destIndex = data.get("di").getAsInt();

        if (source.equals(dest) && sourceIndex == destIndex) {
            return;
        }

        ItemInstance item = server.getItemFrom(player, source, sourceIndex);
        if (item == ItemInstance.NONE || (!player.accountDetails.isAdmin && !item.data.moveable)) {
            return;
        }
        if (quantityToMove == -1) {
            quantityToMove = item.quantity;
        }
        item = new ItemInstance(item);
        item.quantity = quantityToMove;

        boolean moveSuccessful = false;
        switch (dest) {
            case "world":
                moveSuccessful = server.addItem(server.tileMap.getCoordFromIndex(destIndex), item);
                break;
            case "inv":
                if (destIndex == -1) {
                    moveSuccessful = player.creature.inventory.add(item);
                } else {
                    moveSuccessful = player.creature.inventory.add(item, destIndex);
                }
                break;
        }

        if (!moveSuccessful) {
            return;
        }

        server.removeItemAt(player, source, sourceIndex, quantityToMove);
    }
}

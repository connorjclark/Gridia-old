package hoten.gridia.serving.protocols;

import com.google.gson.JsonObject;
import hoten.gridia.Player;
import hoten.gridia.content.ItemInstance;
import hoten.gridia.content.UsageProcessor.ItemWrapper;
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

        ItemWrapper itemToMoveWrapped = server.getItemFrom(player, source, sourceIndex);
        ItemInstance itemToMove = itemToMoveWrapped.getItemInstance();
        if (itemToMove == ItemInstance.NONE || (!player.accountDetails.isAdmin && !itemToMove.data.moveable)) {
            return;
        }
        if (quantityToMove == -1) { // :(
            quantityToMove = itemToMove.quantity;
        }
        itemToMove = new ItemInstance(itemToMove);
        itemToMove.quantity = quantityToMove;

        ItemWrapper destItemWrapped = server.getItemFrom(player, dest, destIndex);

        boolean moveSuccessful = destItemWrapped.addItemHere(itemToMove);

        if (!moveSuccessful) {
            return;
        }

        server.removeItemAt(player, source, sourceIndex, quantityToMove);
    }
}

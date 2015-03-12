package com.hoten.gridia.serving.protocols;

import com.google.gson.JsonObject;
import com.hoten.gridia.Player;
import com.hoten.gridia.content.ItemInstance;
import com.hoten.gridia.serving.ConnectionToGridiaClientHandler;
import com.hoten.gridia.serving.ServingGridia;
import com.hoten.servingjava.message.JsonMessageHandler;
import java.io.IOException;

public class UnequipItem extends JsonMessageHandler<ConnectionToGridiaClientHandler> {

    @Override
    protected void handle(ConnectionToGridiaClientHandler connection, JsonObject data) throws IOException {
        ServingGridia server = connection.getServer();
        Player player = connection.getPlayer();
        int slotIndex = data.get("slotIndex").getAsInt();
        
        ItemInstance itemToUnequip = player.equipment.get(slotIndex);
        if (player.creature.inventory.add(itemToUnequip)) {
            player.equipment.deleteSlot(slotIndex);
            player.updatePlayerImage(server);
        } else {
            connection.send(server.messageBuilder.chat("Your inventory is full.", player.creature.location));
        }
    }
}

package com.hoten.gridia.serving.protocols;

import com.google.gson.JsonObject;
import com.hoten.gridia.Player;
import com.hoten.gridia.content.ItemInstance;
import com.hoten.gridia.serving.ConnectionToGridiaClientHandler;
import com.hoten.gridia.serving.ServingGridia;
import com.hoten.servingjava.message.JsonMessageHandler;
import java.io.IOException;

public class EquipItem extends JsonMessageHandler<ConnectionToGridiaClientHandler> {

    @Override
    protected void handle(ConnectionToGridiaClientHandler connection, JsonObject data) throws IOException {
        ServingGridia server = connection.getServer();
        Player player = connection.getPlayer();
        int slotIndex = data.get("slotIndex").getAsInt();
        
        ItemInstance item = player.creature.inventory.get(slotIndex);
        // :(
        if (item.getItem().isEquipable()) {
            int armorSlotIndex = item.getItem().armorSpot.ordinal();
            if (player.equipment.isEmpty(armorSlotIndex)) {
                player.creature.inventory.deleteSlot(slotIndex);
                player.equipment.set(armorSlotIndex, item);
            } else {
                player.creature.inventory.set(slotIndex, player.equipment.get(armorSlotIndex));
                player.equipment.set(armorSlotIndex, item);
            }
            player.updatePlayerImage(server);
        } else {
            connection.send(server.messageBuilder.chat("You cannot equip a " + item.getItem().name, player.creature.location));
        }
    }
}

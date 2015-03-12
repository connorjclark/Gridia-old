package com.hoten.gridia.serving.protocols;

import com.google.gson.JsonObject;
import com.hoten.gridia.Creature;
import com.hoten.gridia.Player;
import com.hoten.gridia.content.Item;
import com.hoten.gridia.map.Coord;
import com.hoten.gridia.map.Tile;
import com.hoten.gridia.serializers.GridiaGson;
import com.hoten.gridia.serving.ConnectionToGridiaClientHandler;
import com.hoten.gridia.serving.ServingGridia;
import com.hoten.servingjava.message.JsonMessageHandler;
import java.io.IOException;

// hit or mine
public class Hit extends JsonMessageHandler<ConnectionToGridiaClientHandler> {

    @Override
    protected void handle(ConnectionToGridiaClientHandler connection, JsonObject data) throws IOException {
        ServingGridia server = connection.getServer();
        Player player = connection.getPlayer();
        Coord loc = GridiaGson.get().fromJson(data.get("loc"), Coord.class);
        
        Tile tile = server.tileMap.getTile(loc);
        Creature creature = tile.cre;
        if (creature != null && !creature.belongsToPlayer) {
            if (creature.isFriendly) {
                connection.send(server.messageBuilder.chat(creature.name, creature.friendlyMessage, creature.location));
            } else {
                server.hurtCreature(creature, 1);
            }
        } else if (tile.floor == 0) {
            if (player.creature.inventory.containsItemId(901)) {
                server.changeFloor(loc, 19);
                int oreId = Math.random() > 0.7 ? 0 : server.contentManager.getRandomItemOfClassByRarity(Item.ItemClass.Ore).id;
                server.addItem(loc, server.contentManager.createItemInstance(oreId));
                server.sendToClientsWithAreaLoaded(server.messageBuilder.animation("MiningSound", loc), loc);
            } else {
                connection.send(server.messageBuilder.chat("You need a pickaxe to mine!", player.creature.location));
            }
        }
    }
}

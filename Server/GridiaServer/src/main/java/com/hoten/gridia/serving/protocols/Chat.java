package com.hoten.gridia.serving.protocols;

import com.google.gson.JsonObject;
import com.hoten.gridia.CustomPlayerImage;
import com.hoten.gridia.DefaultCreatureImage;
import com.hoten.gridia.Player;
import com.hoten.gridia.content.Item;
import com.hoten.gridia.content.ItemInstance;
import com.hoten.gridia.content.Monster;
import com.hoten.gridia.map.Coord;
import com.hoten.gridia.map.Sector;
import com.hoten.gridia.map.Tile;
import com.hoten.gridia.serving.ConnectionToGridiaClientHandler;
import com.hoten.gridia.serving.ServingGridia;
import com.hoten.servingjava.message.JsonMessageHandler;
import com.hoten.servingjava.message.Message;
import java.io.IOException;
import java.util.Arrays;
import java.util.NoSuchElementException;
import org.codehaus.groovy.control.CompilationFailedException;

// :( OCP!
public class Chat extends JsonMessageHandler<ConnectionToGridiaClientHandler> {

    @Override
    protected void handle(ConnectionToGridiaClientHandler connection, JsonObject data) throws IOException {
        ServingGridia server = connection.getServer();
        Player player = connection.getPlayer();
        String msg = data.get("msg").getAsString();

        if (msg.startsWith("!script ")) {
            String script = msg.split(" ", 2)[1];
            try {
                server.addScript(script, "CustomScript" + script.hashCode());
            } catch (CompilationFailedException ex) {
                connection.send(server.messageBuilder.chat("That didn't parse!\n" + ex, player.creature.location));
            }
        } else if (msg.startsWith("!image ")) {
            try {
                String[] split = msg.split(" ");
                int image = split.length > 1 ? Integer.parseInt(split[1]) : 0;
                int width = split.length == 4 && split[2].matches("\\d+") ? Integer.parseInt(split[2]) : 1;
                int height = split.length == 4 && split[3].matches("\\d+") ? Integer.parseInt(split[3]) : 1;
                if (image == 0) {
                    player.creature.setAttribute("image", new CustomPlayerImage());
                } else if (image > 0 && image <= 700 && width > 0 && width <= 3 && height > 0 && height <= 3) {
                    player.creature.setAttribute("image", new DefaultCreatureImage(image, width, height));
                }
                connection.getPlayer().updatePlayerImage(server);
            } catch (NumberFormatException e) {
            }
        } else if (msg.startsWith("!friendly ") && player.isAdmin()) {
            try {
                String[] split = msg.split(" ", 3);
                if (split.length == 3) {
                    int id = Integer.parseInt(split[1]);
                    Monster monster = server.contentManager.getMonster(id);
                    if (monster != null) {
                        String friendlyMessage = split[2];
                        com.hoten.gridia.scripting.Entity creature = server.createCreature(monster, player.creature.location.add(0, 1, 0), true);
                        creature.setAttribute("friendlyMessage", friendlyMessage);
                    }
                }
            } catch (NumberFormatException e) {
            }
        } else if (msg.startsWith("!monster ") && player.isAdmin()) {
            try {
                String[] split = msg.split(" ", 3);
                if (split.length == 2) {
                    int id = Integer.parseInt(split[1]);
                    Monster monster = server.contentManager.getMonster(id);
                    if (monster != null) {
                        server.createCreature(monster, player.creature.location.add(0, 1, 0));
                    }
                }
            } catch (NumberFormatException e) {
                Message message = server.messageBuilder.chat(e.getMessage(), player.creature.location);
                connection.send(message);
            }
        } else if (msg.equals("!kill")) {
            com.hoten.gridia.scripting.Entity cre = server.tileMap.getCreature(player.creature.location.add(0, 1, 0));
            if (cre != null) {
                cre.callMethod("hurt", Arrays.asList(10000, "was punished by " + player.creature.getAttribute("name")));
            }
        } else if (msg.equals("!del") && player.isAdmin()) {
            server.changeItem(player.creature.location.add(0, 1, 0), ItemInstance.NONE);
        } else if (msg.equals("!clr") && player.isAdmin()) {
            for (int i = 0; i < 20; i++) {
                for (int j = 0; j < 20; j++) {
                    server.changeItem(player.creature.location.add(i, j, 0), ItemInstance.NONE);
                }
            }
        } else if (msg.equals("!loc")) {
            Message message = server.messageBuilder.chat("You are at: " + player.creature.location.toString(), player.creature.location);
            connection.send(message);
        } else if (msg.equals("!online")) {
            Message message = server.messageBuilder.chat(server.whoIsOnline(), player.creature.location);
            connection.send(message);
        } else if (msg.equals("!die")) {
            player.creature.callMethod("hurt", Arrays.asList(10000, "gave up"));
        } else if (msg.startsWith("!warp ")) {
            String[] split = msg.split("\\s+");
            if (split.length == 4) {
                try {
                    int x = Integer.parseInt(split[1]);
                    int y = Integer.parseInt(split[2]);
                    int z = Integer.parseInt(split[3]);
                    if (server.tileMap.inBounds(x, y, z)) {
                        server.teleport(player.creature, new Coord(x, y, z));
                    }
                } catch (NumberFormatException e) {
                }
            }
        } else if (msg.startsWith("!tp ")) {
            String playerName = msg.split("\\s+", 2)[1];
            Player otherPlayer = server.getPlayerWithName(playerName);
            if (otherPlayer != null) {
                server.playAnimation("WarpOut", player.creature.location);
                server.moveCreatureTo(player.creature, otherPlayer.creature.location.add(0, -1, 0), true);
                server.playAnimation("WarpIn", player.creature.location);
            } else {
                connection.send(server.messageBuilder.chat("Invalid player.", player.creature.location));
            }
        } else if (msg.equals("!save") && player.isAdmin()) {
            server.save();
        } else if (msg.startsWith("!item ") && player.isAdmin()) {
            String[] split = msg.replaceFirst("!item ", "").split(",", 2);
            String itemInput = split[0];
            String quantityInput = split.length == 2 ? split[1].trim() : "1";

            Item item = ItemInstance.NONE.getItem();
            try {
                item = server.contentManager.getItem(Integer.parseInt(itemInput));
            } catch (NumberFormatException e) {
                try {
                    item = server.contentManager.getItemByNameIgnoreCase(itemInput);
                } catch (NoSuchElementException ex) {
                }
            }
            int quantity;
            try {
                quantity = Integer.parseInt(quantityInput);
            } catch (NumberFormatException e) {
                quantity = -1;
            }
            if (item == ItemInstance.NONE.getItem()) {
                connection.send(server.messageBuilder.chat("Invalid item.", player.creature.location));
            } else if (quantity == -1) {
                connection.send(server.messageBuilder.chat("Invalid quantity.", player.creature.location));
            } else {
                server.addItemNear(server.contentManager.createItemInstance(item.id, quantity), player.creature.location.add(0, 1, 0), 3, true);
            }
        } else if (msg.startsWith("!admin ") && player.isAdmin()) {
            String playerName = msg.split("\\s+", 2)[1];
            Player otherPlayer = server.getPlayerWithName(playerName);
            if (otherPlayer != null) {
                otherPlayer.setIsAdmin(true);
                server.savePlayer(otherPlayer);
                server.sendToAll(server.messageBuilder.chat(otherPlayer.creature.getAttribute("name") + " is now an admin.", otherPlayer.creature.location));
            } else {
                connection.send(server.messageBuilder.chat("Invalid player.", player.creature.location));
            }
        } else if (msg.startsWith("!claim")) {
            Sector sector = server.tileMap.getSectorOf(player.creature.location);
            if (sector.isUnclaimed()) {
                sector.setOwner(player.getPlayerId());
                connection.send(server.messageBuilder.chat("You have claimed this plot of land.", player.creature.location));
            } else {
                connection.send(server.messageBuilder.chat("This plot of land is already claimed.", player.creature.location));
            }
        } else if (msg.startsWith("!serverclaim") && player.isAdmin()) {
            Sector sector = server.tileMap.getSectorOf(player.creature.location);
            sector.setOwner(Tile.OWNER_SERVER);
            connection.send(server.messageBuilder.chat("This plot of land has been claimed by the server.", player.creature.location));
        } else if (msg.startsWith("!unclaim")) {
            Sector sector = server.tileMap.getSectorOf(player.creature.location);
            if (sector.getOwner() == player.getPlayerId()) {
                sector.setOwner(Tile.OWNER_UNCLAIMED);
                connection.send(server.messageBuilder.chat("You have unclaimed this plot of land.", player.creature.location));
            } else {
                connection.send(server.messageBuilder.chat("You do not own this land.", player.creature.location));
            }
        } else if (msg.startsWith("!owner")) {
            Sector sector = server.tileMap.getSectorOf(player.creature.location);
            if (sector.isUnclaimed()) {
                connection.send(server.messageBuilder.chat("This plot of land is unclaimed.", player.creature.location));
            } else {
                connection.send(server.messageBuilder.chat("This plot of land is owned by: " + server.getOwnerName(sector.getOwner()), player.creature.location));
            }
        } else if (msg.startsWith("!")) {
            connection.send(server.messageBuilder.chat("Invalid command.", player.creature.location));
        } else {
            server.sendToAll(server.messageBuilder.chat((String) player.creature.getAttribute("name"), msg, player.creature.location));
        }
        if (msg.startsWith("!")) {
            connection.send(server.messageBuilder.chat("Command: " + msg, player.creature.location));
        }
    }
}

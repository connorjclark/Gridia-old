package hoten.gridia.serving.protocols;

import com.google.gson.JsonObject;
import hoten.gridia.Creature;
import hoten.gridia.CustomPlayerImage;
import hoten.gridia.DefaultCreatureImage;
import hoten.gridia.Player;
import hoten.gridia.content.ItemInstance;
import hoten.gridia.content.Monster;
import hoten.gridia.map.Coord;
import hoten.gridia.serving.ConnectionToGridiaClientHandler;
import hoten.gridia.serving.ServingGridia;
import hoten.serving.message.JsonMessageHandler;
import hoten.serving.message.Message;
import java.io.IOException;

// :( OCP!
public class Chat extends JsonMessageHandler<ConnectionToGridiaClientHandler> {

    @Override
    protected void handle(ConnectionToGridiaClientHandler connection, JsonObject data) throws IOException {
        ServingGridia server = connection.getServer();
        Player player = connection.getPlayer();
        String msg = data.get("msg").getAsString();

        if (msg.startsWith("!image ")) {
            try {
                String[] split = msg.split(" ");
                int image = split.length > 1 ? Integer.parseInt(split[1]) : 0;
                int width = split.length == 4 && split[2].matches("\\d+") ? Integer.parseInt(split[2]) : 1;
                int height = split.length == 4 && split[3].matches("\\d+") ? Integer.parseInt(split[3]) : 1;
                if (image == 0) {
                    player.creature.image = new CustomPlayerImage();
                } else if (image > 0 && image <= 700 && width > 0 && width <= 3 && height > 0 && height <= 3) {
                    player.creature.image = new DefaultCreatureImage(image, width, height);
                }
                connection.getPlayer().updatePlayerImage(server);
            } catch (NumberFormatException e) {
            }
        } else if (msg.startsWith("!friendly ") && player.accountDetails.isAdmin) {
            try {
                String[] split = msg.split(" ", 3);
                if (split.length == 3) {
                    int id = Integer.parseInt(split[1]);
                    Monster monster = server.contentManager.getMonster(id);
                    if (monster != null) {
                        String friendlyMessage = split[2];
                        Creature creature = server.createCreature(monster, player.creature.location.add(0, 1, 0));
                        creature.isFriendly = true;
                        creature.friendlyMessage = friendlyMessage;
                    }
                }
            } catch (NumberFormatException e) {
            }
        } else if (msg.startsWith("!monster ") && player.accountDetails.isAdmin) {
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
            }
        } else if (msg.equals("!kill")) {
            Creature cre = server.tileMap.getCreature(player.creature.location.add(0, 1, 0));
            if (cre != null) {
                server.hurtCreature(cre, 100000);
            }
        } else if (msg.equals("!del") && player.accountDetails.isAdmin) {
            server.changeItem(player.creature.location.add(0, 1, 0), ItemInstance.NONE);
        } else if (msg.equals("!clr") && player.accountDetails.isAdmin) {
            for (int i = 0; i < 20; i++) {
                for (int j = 0; j < 20; j++) {
                    server.changeItem(player.creature.location.add(i, j, 0), ItemInstance.NONE);
                }
            }
        } else if (msg.equals("!loc")) {
            Message message = server.messageBuilder.chat("You are at: " + player.creature.location.toString(), player.creature.location);
            connection.send(message);
        } else if (msg.equals("!die")) {
            server.hurtCreature(player.creature, 100000);
        } else if (msg.startsWith("!warp ")) {
            String[] split = msg.split("\\s+");
            if (split.length == 4) {
                try {
                    int x = Integer.parseInt(split[1]);
                    int y = Integer.parseInt(split[2]);
                    int z = Integer.parseInt(split[3]);
                    if (server.tileMap.inBounds(x, y, z)) {
                        server.moveCreatureTo(player.creature, new Coord(x, y, z), true);
                    }
                } catch (NumberFormatException e) {
                }
            }
        } else if (msg.startsWith("!tp ")) {
            String playerName = msg.split("\\s+", 2)[1];
            Player otherPlayer = server.getPlayerWithName(playerName);
            if (otherPlayer != null) {
                server.moveCreatureTo(player.creature, otherPlayer.creature.location.add(0, -1, 0), true);
            } else {
                connection.send(server.messageBuilder.chat("Invalid player.", player.creature.location));
            }
        } else if (msg.equals("!save") && player.accountDetails.isAdmin) {
            server.save();
        } else if (msg.startsWith("!")) {
            connection.send(server.messageBuilder.chat("Invalid command.", player.creature.location));
        } else {
            server.sendToAll(server.messageBuilder.chat(player.creature.name + " says: " + msg, player.creature.location));
        }
    }
}

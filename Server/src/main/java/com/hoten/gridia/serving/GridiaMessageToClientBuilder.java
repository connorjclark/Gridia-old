package com.hoten.gridia.serving;

import com.hoten.gridia.Container;
import com.hoten.gridia.CreatureImage;
import com.hoten.gridia.CustomPlayerImage;
import com.hoten.gridia.DefaultCreatureImage;
import com.hoten.gridia.content.ItemInstance;
import com.hoten.gridia.content.ItemUse;
import com.hoten.gridia.map.Coord;
import com.hoten.gridia.map.Sector;
import com.hoten.gridia.map.Tile;
import com.hoten.gridia.scripting.Entity;
import com.hoten.gridia.serializers.GridiaGson;
import com.hoten.servingjava.message.BinaryMessageBuilder;
import com.hoten.servingjava.message.JsonMessageBuilder;
import com.hoten.servingjava.message.Message;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

public class GridiaMessageToClientBuilder {

    public Message addCreature(Entity cre) {
        return new JsonMessageBuilder()
                .type("AddCreature")
                .set("id", cre.id)
                .set("image", cre.getAttribute("image"))
                .set("name", cre.getString("name"))
                .set("loc", cre.location)
                .build();
    }

    public Message renameCreature(Entity cre) {
        return new JsonMessageBuilder()
                .type("RenameCreature")
                .set("id", cre.id)
                .set("name", cre.getAttribute("name"))
                .build();
    }

    public Message moveCreature(Entity cre, int timeoffset, boolean isTeleport, boolean onRaft, boolean tellMover) {
        return new JsonMessageBuilder()
                .type("MoveCreature")
                .set("time", System.currentTimeMillis() + timeoffset)
                .set("id", cre.id)
                .set("loc", cre.location)
                .set("isTeleport", isTeleport)
                .set("onRaft", onRaft)
                .set("tellMover", tellMover)
                .build();
    }

    public Message removeCreature(Entity cre) {
        return new JsonMessageBuilder()
                .type("RemoveCreature")
                .set("id", cre.id)
                .build();
    }

    public Message sectorRequest(Sector sector) throws IOException {
        List<Entity> creatures = new ArrayList();
        Tile[][] tiles = sector._tiles;

        BinaryMessageBuilder builder = new BinaryMessageBuilder()
                .type("SectorData")
                .writeInt(sector.sx)
                .writeInt(sector.sy)
                .writeInt(sector.sz);

        for (Tile[] tile : tiles) {
            for (int y = 0; y < tiles.length; y++) {
                builder.writeShort(tile[y].floor);
                builder.writeShort(tile[y].item.getItem().id);
                builder.writeShort(tile[y].item.getQuantity());
                if (tile[y].cre != null) {
                    creatures.add(tile[y].cre);
                }
            }
        }

        builder.writeInt(creatures.size());
        creatures.stream().forEach((cre) -> {
            builder.writeShort(cre.id)
                    .writeUTF(cre.getString("name"))
                    .writeShort(cre.location.x)
                    .writeShort(cre.location.y)
                    .writeShort(cre.location.z);
            CreatureImage image = (CreatureImage) cre.getAttribute("image");
            if (image instanceof DefaultCreatureImage) {
                DefaultCreatureImage defaultImage = (DefaultCreatureImage) image;
                builder.writeShort(0)
                        .writeShort(defaultImage.getSpriteIndex())
                        .writeShort(defaultImage.getWidth())
                        .writeShort(defaultImage.getHeight());
            } else if (image instanceof CustomPlayerImage) {
                CustomPlayerImage customImage = (CustomPlayerImage) image;
                builder.writeShort(1)
                        .writeShort(customImage.head)
                        .writeShort(customImage.chest)
                        .writeShort(customImage.legs)
                        .writeShort(customImage.arms)
                        .writeShort(customImage.weapon)
                        .writeShort(customImage.shield);
            }
        });

        return builder.build();
    }

    public Message setFocus(int id, boolean isAdmin) {
        return new JsonMessageBuilder()
                .type("SetFocus")
                .set("id", id)
                .set("isAdmin", isAdmin)
                .build();
    }

    public Message initialize(String version, String worldName, int size, int depth, int sectorSize) {
        return new JsonMessageBuilder()
                .type("Initialize")
                .set("time", System.currentTimeMillis())
                .set("version", version)
                .set("worldName", worldName)
                .set("size", size)
                .set("depth", depth)
                .set("sectorSize", sectorSize)
                .build();
    }

    public Message container(Container container, int tabGfxItemId) {
        return new JsonMessageBuilder()
                .type("Container")
                .set("items", container.getItems())
                .set("type", container.type)
                .set("id", container.id)
                .set("tabGfxItemId", tabGfxItemId)
                .gson(GridiaGson.get()) // :(
                .build();
    }

    public Message chat(String user, String text, Coord loc) {
        return new JsonMessageBuilder()
                .type("Chat")
                .set("user", user)
                .set("text", text)
                .set("loc", loc)
                .build();
    }

    // :(
    public Message chat(String text, Coord loc) {
        return chat("WORLD", text, loc);
    }

    public Message updateTile(Coord loc, Tile tile) {
        return new JsonMessageBuilder()
                .type("TileUpdate")
                .set("loc", loc)
                .set("item", tile.item.getItem().id)
                .set("quantity", tile.item.getQuantity())
                .set("floor", tile.floor)
                .build();
    }

    public Message updateContainerSlot(Container container, int slotIndex) {
        ItemInstance item = container.get(slotIndex);
        return new JsonMessageBuilder()
                .type("ContainerUpdate")
                .set("id", container.id)
                .set("index", slotIndex)
                .set("item", item.getItem().id)
                .set("quantity", item.getQuantity())
                .build();
    }

    public Message itemUsePick(List<ItemUse> uses) {
        return new JsonMessageBuilder()
                .type("ItemUsePick")
                .set("uses", uses)
                .build();
    }

    public Message animation(String name, Coord loc) {
        return new JsonMessageBuilder()
                .type("Animation")
                .set("name", name)
                .set("loc", loc)
                .build();
    }

    public Message updateCreatureImage(Entity creature) {
        return new JsonMessageBuilder()
                .type("UpdateCreatureImage")
                .set("id", creature.id)
                .set("image", creature.getAttribute("image"))
                .build();
    }

    public Message genericEventHandler(Object obj) {
        return new JsonMessageBuilder()
                .type("GenericEventHandler")
                .set("obj", obj)
                .build();
    }

    public Message alert(String message) {
        return new JsonMessageBuilder()
                .type("Alert")
                .set("message", message)
                .build();
    }

    public Message setLife(Entity entity) {
        return new JsonMessageBuilder()
                .type("SetLife")
                .set("id", entity.id)
                .set("currentLife", entity.getAttribute("life"))
                .set("maxLife", entity.getAttribute("maxLife"))
                .build();
    }
}

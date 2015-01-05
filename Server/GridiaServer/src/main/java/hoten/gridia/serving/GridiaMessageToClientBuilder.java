package hoten.gridia.serving;

import hoten.gridia.Creature;
import hoten.gridia.Container;
import hoten.gridia.CustomPlayerImage;
import hoten.gridia.DefaultCreatureImage;
import hoten.gridia.content.ItemInstance;
import hoten.gridia.content.ItemUse;
import hoten.gridia.map.Coord;
import hoten.gridia.map.Sector;
import hoten.gridia.map.Tile;
import hoten.gridia.serializers.GridiaGson;
import hoten.serving.message.BinaryMessageBuilder;
import hoten.serving.message.JsonMessageBuilder;
import hoten.serving.message.Message;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

public class GridiaMessageToClientBuilder {

    public Message addCreature(Creature cre) {
        return new JsonMessageBuilder()
                .type("AddCreature")
                .set("id", cre.id)
                .set("image", cre.image)
                .set("name", cre.name)
                .set("loc", cre.location)
                .build();
    }

    public Message renameCreature(Creature cre) {
        return new JsonMessageBuilder()
                .type("RenameCreature")
                .set("id", cre.id)
                .set("name", cre.name)
                .build();
    }

    public Message moveCreature(Creature cre, int timeoffset, boolean isTeleport, boolean onRaft) {
        return new JsonMessageBuilder()
                .type("MoveCreature")
                .set("time", System.currentTimeMillis() + timeoffset)
                .set("id", cre.id)
                .set("loc", cre.location)
                .set("isTeleport", isTeleport)
                .set("onRaft", onRaft)
                .build();
    }

    public Message removeCreature(Creature cre) {
        return new JsonMessageBuilder()
                .type("RemoveCreature")
                .set("id", cre.id)
                .build();
    }

    public Message sectorRequest(Sector sector) throws IOException {
        List<Creature> creatures = new ArrayList();
        Tile[][] tiles = sector._tiles;

        BinaryMessageBuilder builder = new BinaryMessageBuilder()
                .type("SectorData")
                .writeInt(sector.sx)
                .writeInt(sector.sy)
                .writeInt(sector.sz);

        for (int x = 0; x < tiles.length; x++) {
            for (int y = 0; y < tiles.length; y++) {
                builder.writeShort(tiles[x][y].floor);
                builder.writeShort(tiles[x][y].item.data.id);
                builder.writeShort(tiles[x][y].item.quantity);
                if (tiles[x][y].cre != null) {
                    creatures.add(tiles[x][y].cre);
                }
            }
        }

        builder.writeInt(creatures.size());
        creatures.stream().forEach((cre) -> {
            builder.writeShort(cre.id)
                    .writeUTF(cre.name)
                    .writeShort(cre.location.x)
                    .writeShort(cre.location.y)
                    .writeShort(cre.location.z);
            if (cre.image instanceof DefaultCreatureImage) {
                DefaultCreatureImage defaultImage = (DefaultCreatureImage) cre.image;
                builder.writeShort(0)
                        .writeShort(defaultImage.spriteIndex)
                        .writeShort(defaultImage.width)
                        .writeShort(defaultImage.height);
            } else if (cre.image instanceof CustomPlayerImage) {
                CustomPlayerImage customImage = (CustomPlayerImage) cre.image;
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

    public Message container(Container container) {
        return new JsonMessageBuilder()
                .type("Container")
                .set("items", container.getItems())
                .set("type", container.type)
                .set("id", container.id)
                .gson(GridiaGson.get()) // :(
                .build();
    }

    public Message chat(String msg, Coord loc) {
        return new JsonMessageBuilder()
                .type("Chat")
                .set("msg", msg)
                .set("loc", loc)
                .build();
    }

    public Message updateTile(Coord loc, Tile tile) {
        // :(
        if (tile.item.quantity <= 0) {
            tile.item = ItemInstance.NONE;
        }
        return new JsonMessageBuilder()
                .type("TileUpdate")
                .set("loc", loc)
                .set("item", tile.item.data.id)
                .set("quantity", tile.item.quantity)
                .set("floor", tile.floor)
                .build();
    }

    public Message updateContainerSlot(Container container, int slotIndex) {
        ItemInstance item = container.get(slotIndex);
        return new JsonMessageBuilder()
                .type("ContainerUpdate")
                .set("type", container.type)
                .set("index", slotIndex)
                .set("item", item.data.id)
                .set("quantity", item.quantity)
                .build();
    }

    public Message itemUsePick(List<ItemUse> uses) {
        return new JsonMessageBuilder()
                .type("ItemUsePick")
                .set("uses", uses)
                .build();
    }

    public Message animation(int animation, Coord loc) {
        return new JsonMessageBuilder()
                .type("Animation")
                .set("anim", animation)
                .set("loc", loc)
                .build();
    }

    public Message updateCreatureImage(Creature creature) {
        return new JsonMessageBuilder()
                .type("UpdateCreatureImage")
                .set("id", creature.id)
                .set("image", creature.image)
                .build();
    }

    public Message genericEventHandler(Object obj) {
        return new JsonMessageBuilder()
                .type("GenericEventHandler")
                .set("obj", obj)
                .build();
    }
}

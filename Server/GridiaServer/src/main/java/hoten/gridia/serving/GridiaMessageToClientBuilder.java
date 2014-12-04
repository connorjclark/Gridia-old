package hoten.gridia.serving;

import hoten.gridia.Creature;
import hoten.gridia.Container;
import hoten.gridia.content.ItemInstance;
import hoten.gridia.content.ItemUse;
import hoten.gridia.map.Coord;
import hoten.gridia.map.Sector;
import hoten.gridia.map.Tile;
import hoten.gridia.serializers.GridiaGson;
import static hoten.gridia.serving.GridiaProtocols.Clientbound.*;
import hoten.serving.message.BinaryMessageBuilder;
import hoten.serving.message.JsonMessageBuilder;
import hoten.serving.message.Message;
import hoten.serving.message.Protocols.Protocol;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import java.util.function.Function;

public class GridiaMessageToClientBuilder {

    private final Function<Enum, Protocol> _outbound;

    public GridiaMessageToClientBuilder(Function<Enum, Protocol> outbound) {
        _outbound = outbound;
    }

    public Message addCreature(Creature cre) {
        return new JsonMessageBuilder()
                .protocol(outbound(AddCreature))
                .set("id", cre.id)
                .set("image", cre.image)
                .set("loc", cre.location)
                .build();
    }

    public Message moveCreature(Creature cre, int timeoffset, boolean isTeleport) {
        return new JsonMessageBuilder()
                .protocol(outbound(MoveCreature))
                .set("time", System.currentTimeMillis() + timeoffset)
                .set("id", cre.id)
                .set("loc", cre.location)
                .set("isTeleport", isTeleport)
                .build();
    }

    public Message removeCreature(Creature cre) {
        return new JsonMessageBuilder()
                .protocol(outbound(RemoveCreature))
                .set("id", cre.id)
                .build();
    }

    public Message sectorRequest(Sector sector) throws IOException {
        List<Creature> creatures = new ArrayList();
        Tile[][] tiles = sector._tiles;

        BinaryMessageBuilder builder = new BinaryMessageBuilder()
                .protocol(outbound(SectorData))
                .writeInt(sector.sx)
                .writeInt(sector.sy)
                .writeInt(sector.sz);

        for (int x = 0; x < tiles.length; x++) {
            for (int y = 0; y < tiles.length; y++) {
                builder.writeShort(tiles[x][y].floor);
                builder.writeShort(tiles[x][y].item.data.id);
                if (tiles[x][y].cre != null) {
                    creatures.add(tiles[x][y].cre);
                }
            }
        }

        //builder.writeInt(creatures.size());
        builder.writeInt(0);
        // .remove?
        /*creatures.stream().forEach((cre) -> {
         builder.writeShort(cre.id)
         .writeShort(cre.image)
         .writeShort(cre.location.x)
         .writeShort(cre.location.y)
         .writeShort(cre.location.z);
         });*/

        return builder.build();
    }

    public Message setFocus(int id) {
        return new JsonMessageBuilder()
                .protocol(outbound(SetFocus))
                .set("id", id)
                .build();
    }

    public Message initialize(int size, int depth, int sectorSize) {
        return new JsonMessageBuilder()
                .protocol(outbound(Initialize))
                .set("time", System.currentTimeMillis())
                .set("size", size)
                .set("depth", depth)
                .set("sectorSize", sectorSize)
                .set("isAdmin", true)
                .build();
    }

    public Message container(Container container) {
        return new JsonMessageBuilder()
                .protocol(outbound(Container))
                .set("items", container.getItems())
                .set("type", container.type)
                .set("id", container.id)
                .gson(GridiaGson.get()) // :(
                .build();
    }

    public Message chat(String msg) {
        return new JsonMessageBuilder()
                .protocol(outbound(Chat))
                .set("msg", msg)
                .build();
    }

    public Message updateTile(Coord loc, Tile tile) {
        // :(
        if (tile.item.quantity <= 0) {
            tile.item = ItemInstance.NONE;
        }
        return new JsonMessageBuilder()
                .protocol(outbound(TileUpdate))
                .set("loc", loc)
                .set("item", tile.item.data.id)
                .set("quantity", tile.item.quantity)
                .set("floor", tile.floor)
                .build();
    }

    public Message updateContainerSlot(Container container, int slotIndex) {
        ItemInstance item = container.get(slotIndex);
        return new JsonMessageBuilder()
                .protocol(outbound(ContainerUpdate))
                .set("type", container.type)
                .set("index", slotIndex)
                .set("item", item.data.id)
                .set("quantity", item.quantity)
                .build();
    }

    public Message itemUsePick(List<ItemUse> uses) {
        return new JsonMessageBuilder()
                .protocol(outbound(ItemUsePick))
                .set("uses", uses)
                .build();
    }

    public Message animation(int animation) {
        return new JsonMessageBuilder()
                .protocol(outbound(Animation))
                .set("anim", animation)
                .build();
    }

    public Message updateCreatureImage(Creature creature) {
        return new JsonMessageBuilder()
                .protocol(outbound(UpdateCreatureImage))
                .set("id", creature.id)
                .set("image", creature.image)
                .build();
    }

    private Protocol outbound(Enum en) {
        return _outbound.apply(en);
    }
}

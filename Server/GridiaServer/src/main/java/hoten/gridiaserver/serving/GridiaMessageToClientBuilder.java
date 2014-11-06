package hoten.gridiaserver.serving;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import hoten.gridiaserver.Creature;
import hoten.gridiaserver.Inventory;
import hoten.gridiaserver.content.ItemInstance;
import hoten.gridiaserver.map.Sector;
import hoten.gridiaserver.map.Tile;
import hoten.gridiaserver.serializers.ItemInstanceSerializer;
import static hoten.gridiaserver.serving.GridiaProtocols.Clientbound.*;
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

    public Message moveCreature(Creature cre) {
        return new JsonMessageBuilder()
                .protocol(outbound(MoveCreature))
                .set("id", cre.id)
                .set("loc", cre.location)
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

        builder.writeInt(creatures.size());
        creatures.stream().forEach((cre) -> {
            builder.writeShort(cre.id)
                    .writeShort(cre.image)
                    .writeShort(cre.location.x)
                    .writeShort(cre.location.y)
                    .writeShort(cre.location.z);
        });

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
                .set("size", size)
                .set("depth", depth)
                .set("sectorSize", sectorSize)
                .build();
    }

    public Message inventory(Inventory inv) {
        // :(
        Gson gson = new GsonBuilder()
                .registerTypeAdapter(ItemInstance.class, new ItemInstanceSerializer())
                .create();

        return new JsonMessageBuilder()
                .protocol(outbound(Inventory))
                .set("inv", inv.items)
                .gson(gson)
                .build();
    }

    public Message chat(String msg) {
        return new JsonMessageBuilder()
                .protocol(outbound(Chat))
                .set("msg", msg)
                .build();
    }

    private Protocol outbound(Enum en) {
        return _outbound.apply(en);
    }
}

package hoten.gridiaserver;

import static hoten.gridiaserver.GridiaProtocols.Clientbound.*;
import hoten.serving.BinaryMessageBuilder;
import hoten.serving.JsonMessageBuilder;
import hoten.serving.Message;
import hoten.serving.Protocols.Protocol;
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
                builder.writeShort(tiles[x][y].item);
                if (tiles[x][y].cre != null) {
                    creatures.add(tiles[x][y].cre);
                }
            }
        }

        builder.writeInt(creatures.size());
        creatures.stream().forEach((cre) -> {
            builder.writeShort(cre.id)
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

    private Protocol outbound(Enum en) {
        return _outbound.apply(en);
    }
}

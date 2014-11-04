package hoten.gridiaserver;

import hoten.serving.message.Protocols;

public class GridiaProtocols extends Protocols {

    public enum Clientbound {

        AddCreature, MoveCreature, RemoveCreature, SectorData, Chat, SetFocus, Initialize, TileUpdate

    }

    public enum Serverbound {

        PlayerMove, SectorRequest, CreatureRequest, MoveItem
    }

    public GridiaProtocols() {
        add(BoundDest.CLIENT, 0, DataMethod.JSON, false);
        add(BoundDest.CLIENT, 1, DataMethod.JSON, false);
        add(BoundDest.CLIENT, 2, DataMethod.JSON, false);
        add(BoundDest.CLIENT, 3, DataMethod.BINARY, false);
        add(BoundDest.CLIENT, 4, DataMethod.JSON, false);
        add(BoundDest.CLIENT, 5, DataMethod.JSON, false);
        add(BoundDest.CLIENT, 6, DataMethod.JSON, false);
        add(BoundDest.CLIENT, 7, DataMethod.JSON, false);

        add(BoundDest.SERVER, 0, DataMethod.JSON, false);
        add(BoundDest.SERVER, 1, DataMethod.JSON, false);
        add(BoundDest.SERVER, 2, DataMethod.JSON, false);
        add(BoundDest.SERVER, 3, DataMethod.JSON, false);
    }
}

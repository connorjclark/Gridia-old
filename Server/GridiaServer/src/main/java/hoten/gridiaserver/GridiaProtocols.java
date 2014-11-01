package hoten.gridiaserver;

import hoten.serving.Protocols;

public class GridiaProtocols extends Protocols {

    public enum Clientbound {

        AddCreature, MoveCreature, RemoveCreature, SectorData

    }

    public enum Serverbound {

        PlayerMove, SectorRequest
    }

    public GridiaProtocols() {
        add(BoundDest.CLIENT, 0, DataMethod.JSON, false);
        add(BoundDest.CLIENT, 1, DataMethod.JSON, false);
        add(BoundDest.CLIENT, 2, DataMethod.JSON, false);
        add(BoundDest.CLIENT, 3, DataMethod.BINARY, false);

        add(BoundDest.SERVER, 0, DataMethod.JSON, false);
        add(BoundDest.SERVER, 1, DataMethod.JSON, false);
    }
}

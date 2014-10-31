package hoten.gridiaserver;

import hoten.serving.Protocols;

public class GridiaProtocols extends Protocols {

    public enum Clientbound {

        AddCreature, MoveCreature, RemoveCreature

    }

    public enum Serverbound {

        PlayerMove
    }

    public GridiaProtocols() {
        add(BoundDest.CLIENT, 0, DataMethod.JSON, false);
        add(BoundDest.CLIENT, 1, DataMethod.JSON, false);
        add(BoundDest.CLIENT, 2, DataMethod.JSON, false);

        add(BoundDest.SERVER, 0, DataMethod.JSON, false);
    }
}

using Serving;

public class GridiaProtocols : Protocols
{
    public enum Clientbound { AddCreature, MoveCreature, RemoveCreature, SectorData, Chat, SetFocus, Initialize }
    public enum Serverbound { PlayerMove, RequestSector, RequestCreature }

    public GridiaProtocols()
    {
        Add(BoundDest.CLIENT, 0, DataMethod.JSON, false);
        Add(BoundDest.CLIENT, 1, DataMethod.JSON, false);
        Add(BoundDest.CLIENT, 2, DataMethod.JSON, false);
        Add(BoundDest.CLIENT, 3, DataMethod.BINARY, false);
        Add(BoundDest.CLIENT, 4, DataMethod.JSON, false);
        Add(BoundDest.CLIENT, 5, DataMethod.JSON, false);
        Add(BoundDest.CLIENT, 6, DataMethod.JSON, false);

        Add(BoundDest.SERVER, 0, DataMethod.JSON, false);
        Add(BoundDest.SERVER, 1, DataMethod.JSON, false);
        Add(BoundDest.SERVER, 2, DataMethod.JSON, false);
    }
}
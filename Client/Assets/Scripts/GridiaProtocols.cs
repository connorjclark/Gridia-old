using Serving;

public class GridiaProtocols : Protocols
{
    public enum Clientbound { AddCreature, MoveCreature, RemoveCreature, SectorData, Chat, SetFocus, Initialize, TileUpdate, Inventory, InventoryUpdate, ItemUsePick }
    public enum Serverbound { PlayerMove, RequestSector, RequestCreature, MoveItem, Chat, UseItem }

    public GridiaProtocols()
    {
        Add(BoundDest.CLIENT, 0, DataMethod.JSON, false);
        Add(BoundDest.CLIENT, 1, DataMethod.JSON, false);
        Add(BoundDest.CLIENT, 2, DataMethod.JSON, false);
        Add(BoundDest.CLIENT, 3, DataMethod.BINARY, false);
        Add(BoundDest.CLIENT, 4, DataMethod.JSON, false);
        Add(BoundDest.CLIENT, 5, DataMethod.JSON, false);
        Add(BoundDest.CLIENT, 6, DataMethod.JSON, false);
        Add(BoundDest.CLIENT, 7, DataMethod.JSON, false);
        Add(BoundDest.CLIENT, 8, DataMethod.JSON, false);
        Add(BoundDest.CLIENT, 9, DataMethod.JSON, false);
        Add(BoundDest.CLIENT, 10, DataMethod.JSON, false);

        Add(BoundDest.SERVER, 0, DataMethod.JSON, false);
        Add(BoundDest.SERVER, 1, DataMethod.JSON, false);
        Add(BoundDest.SERVER, 2, DataMethod.JSON, false);
        Add(BoundDest.SERVER, 3, DataMethod.JSON, false);
		Add(BoundDest.SERVER, 4, DataMethod.JSON, false);
		Add(BoundDest.SERVER, 5, DataMethod.JSON, false);
	}
}
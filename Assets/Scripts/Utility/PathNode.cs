
public class PathNode 
{
    public HexTile MyTile;

    public HexTile ParentTile;

    public PathNode(HexTile myTile, HexTile parentTile)
    {
        MyTile = myTile;
        ParentTile = parentTile;
    }
}

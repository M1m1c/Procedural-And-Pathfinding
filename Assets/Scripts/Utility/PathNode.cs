
public class PathNode 
{
    public HexTile MyTile;

    public PathNode ParentNode;

    public PathNode(HexTile myTile, PathNode parentNode)
    {
        MyTile = myTile;
        ParentNode = parentNode;
    }
}

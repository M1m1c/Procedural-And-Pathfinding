
public class PathNode 
{
    public int gCost;
    public int hCost;
    public int fCost { get { return gCost + hCost; } }

    public HexTile parent;
}

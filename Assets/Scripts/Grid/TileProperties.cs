
public struct TileProperties 
{
}

[System.Flags]
public enum TileTags
{
    None,
    Impassable,
    Destructable,
    PlayerSpawn = 1 << 3
}
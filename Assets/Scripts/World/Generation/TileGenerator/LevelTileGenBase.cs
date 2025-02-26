public abstract class TileGenBase
{
    public WorldTile Tile { get; protected set; }

    public int TileSizeX;
    public int TileSizeY;
    public int TileSizeZ; // For potential multi-layer levels

    // Common functionality that may be shared
    protected WorldTile.WorldTileType GetTileType(WorldTile tile)
    {
        return tile.TileType;
    }

    // Abstract method for generating a level
    public abstract void GenerateLevel();
}

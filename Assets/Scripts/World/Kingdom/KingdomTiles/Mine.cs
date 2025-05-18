using UnityEngine;

public class Mine : KingdomTile
{
    public string MineName { get; set; }
    public int MineDepth { get; set; } // zLevels
    public int MineWidth { get; set; } // xLevels
    public KingdomBase KingdomOwner { get; set; }
    public TownBase TownInfluencer { get; set; }
    public WorldTilePos WorldTilePos { get; set; }
    public override KingdomTileType Type { get; set; }

    public Mine(string mineName, int mineDepth, int mineWidth, KingdomBase kingdomOwner, TownBase townInfluencer, WorldTilePos worldTilePos)
    {
        MineName = mineName;
        MineDepth = mineDepth;
        MineWidth = mineWidth;
        KingdomOwner = kingdomOwner;
        TownInfluencer = townInfluencer;
        WorldTilePos = worldTilePos;
        // Get Tile Data
        if (Game_Manager.Instance.worldData.WorldTileData.TryGetValue(worldTilePos, out WorldTile tile))
        {
            tile.Features.Add("Mine");
            tile.IsTileTransversable = false;
            tile.IsPOI = true;
            tile.POI = WorldTile.POIType.Mine;
            tile.UpdateBaseDisplayString();
        }
        Type = KingdomTileType.Mine;
        kingdomOwner.KingdomTiles.Add(this);
        townInfluencer.KingdomTilesUnderTown.Add(this);
    }
    public Mine(string mineName, int mineDepth, int mineWidth, KingdomBase kingdomOwner, WorldTilePos worldTilePos)
    {
        MineName = mineName;
        MineDepth = mineDepth;
        MineWidth = mineWidth;
        KingdomOwner = kingdomOwner;
        WorldTilePos = worldTilePos;
        if (Game_Manager.Instance.worldData.WorldTileData.TryGetValue(worldTilePos, out WorldTile tile))
        {
            tile.Features.Add("Mine");
            tile.IsTileTransversable = false;
            tile.IsPOI = true;
            tile.POI = WorldTile.POIType.Mine;
            tile.UpdateBaseDisplayString();
        }
        Type = KingdomTileType.Mine;
        kingdomOwner.KingdomTiles.Add(this);
    }
    public Mine(string mineName, int mineDepth, int mineWidth, WorldTilePos worldTilePos)
    {
        MineName = mineName;
        MineDepth = mineDepth;
        MineWidth = mineWidth;
        WorldTilePos = worldTilePos;
        if (Game_Manager.Instance.worldData.WorldTileData.TryGetValue(worldTilePos, out WorldTile tile))
        {
            tile.Features.Add("Mine");
            tile.IsTileTransversable = false;
            tile.IsPOI = true;
            tile.POI = WorldTile.POIType.Mine;
            tile.UpdateBaseDisplayString();
        }
        Type = KingdomTileType.Mine;
    }
    public Mine(string mineName, int mineDepth, int mineWidth, TownBase town, WorldTilePos worldTilePos)
    {
        MineName = mineName;
        MineDepth = mineDepth;
        MineWidth = mineWidth;
        WorldTilePos = worldTilePos;
        if (Game_Manager.Instance.worldData.WorldTileData.TryGetValue(worldTilePos, out WorldTile tile))
        {
            tile.Features.Add("Mine");
            tile.IsTileTransversable = false;
            tile.IsPOI = true;
            tile.POI = WorldTile.POIType.Mine;
            tile.UpdateBaseDisplayString();
        }
        TownInfluencer = town;
        Type = KingdomTileType.Mine;
        town.KingdomTilesUnderTown.Add(this);
    }

}

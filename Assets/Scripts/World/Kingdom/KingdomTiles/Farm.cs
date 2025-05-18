using UnityEngine;
using static KingdomTile;

public class Farm : KingdomTile
{
    public enum Feritility
    {
        VeryLow,
        Low,
        Medium,
        High
    }

    public string MineName { get; set; }

    public KingdomBase KingdomOwner { get; set; }
    public TownBase TownInfluencer { get; set; }
    public WorldTilePos WorldTilePos { get; set; }
    public override KingdomTileType Type { get; set; }
    public Feritility Ferit { get; set; }

    public Farm(string mineName, KingdomBase kingdomOwner, TownBase townInfluencer, WorldTilePos worldTilePos)
    {
        MineName = mineName;
        KingdomOwner = kingdomOwner;
        TownInfluencer = townInfluencer;
        WorldTilePos = worldTilePos;
        // Get Tile Data
        if (Game_Manager.Instance.worldData.WorldTileData.TryGetValue(worldTilePos, out WorldTile tile))
        {
            tile.Features.Add("Mine");
            tile.IsTileTransversable = false;
            tile.IsPOI = true;
            tile.POI = WorldTile.POIType.Farm;
            setFertility(tile);
            tile.UpdateBaseDisplayString();
        }
        Type = KingdomTileType.Farm;
        kingdomOwner.KingdomTiles.Add(this);
        townInfluencer.KingdomTilesUnderTown.Add(this);
    }
    public Farm(string mineName, KingdomBase kingdomOwner, WorldTilePos worldTilePos)
    {
        MineName = mineName;

        KingdomOwner = kingdomOwner;
        WorldTilePos = worldTilePos;
        if (Game_Manager.Instance.worldData.WorldTileData.TryGetValue(worldTilePos, out WorldTile tile))
        {
            tile.Features.Add("Mine");
            tile.IsTileTransversable = false;
            tile.IsPOI = true;
            tile.POI = WorldTile.POIType.Farm;
            setFertility(tile);
            tile.UpdateBaseDisplayString();
        }
        Type = KingdomTileType.Farm;
        kingdomOwner.KingdomTiles.Add(this);
    }
    public Farm(string mineName, WorldTilePos worldTilePos)
    {
        MineName = mineName;
        WorldTilePos = worldTilePos;
        if (Game_Manager.Instance.worldData.WorldTileData.TryGetValue(worldTilePos, out WorldTile tile))
        {
            tile.Features.Add("Mine");
            tile.IsTileTransversable = false;
            tile.IsPOI = true;
            tile.POI = WorldTile.POIType.Farm;
            setFertility(tile);
            tile.UpdateBaseDisplayString();
        }
        Type = KingdomTileType.Farm;
    }
    public Farm(string mineName, TownBase town, WorldTilePos worldTilePos)
    {
        MineName = mineName;
        WorldTilePos = worldTilePos;
        if (Game_Manager.Instance.worldData.WorldTileData.TryGetValue(worldTilePos, out WorldTile tile))
        {
            tile.Features.Add("Mine");
            tile.IsTileTransversable = false;
            tile.IsPOI = true;
            tile.POI = WorldTile.POIType.Farm;
            setFertility(tile);
            tile.UpdateBaseDisplayString();
        }
        TownInfluencer = town;
        Type = KingdomTileType.Farm;
        town.KingdomTilesUnderTown.Add(this);
    }


    void setFertility(WorldTile tile)
    {
        switch (tile.TileType)
        {
            case WorldTile.WorldTileType.Desert:
            case WorldTile.WorldTileType.Mountain:
                Ferit = Feritility.Low;
                break;
            case WorldTile.WorldTileType.Snow:
                Ferit = Feritility.VeryLow;
                break;
            case WorldTile.WorldTileType.Forest:
            case WorldTile.WorldTileType.Plains:
            case WorldTile.WorldTileType.Jungle:
                Ferit = Feritility.High;
                break;
            case WorldTile.WorldTileType.Swamp:
                Ferit = Feritility.Medium;
                break;
            default:
                Ferit = Feritility.Medium;
                break;
        }
    }
}

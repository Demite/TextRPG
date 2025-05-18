
using NUnit.Framework;
using System;
using System.Collections.Generic;
using static TileEnums;

public class LevelTile
{
    public const bool ZLevelsSupported = false;

    public int TileX { get; set; }
    public int TileY { get; set; }
    public int TileZ { get; set; }


    public WorldTilePos MyParentsPosition;
    public LevelPOS MyPosition;
    public IEntity entity;
    public IFoliage foliage;
    public ITileAttribute attribute;
    public BuildingPart BuildingPart;
    public Building Building;

    public LevelTileBiome Biome { get; set; }
    public ForestTiles ForestTileType { get; set; }
    public SwampTiles SwampTileType { get; set; }
    public JungleTiles JungleTileType { get; set; }

    public bool IsVisible = false;
    public bool IsExplored = false;
    public bool IsPath = false;
    public bool IsOccupiedByEnitiy = false;
    public bool IsOccupiedByFoliage = false;
    public bool IsTransversable = true;
    public bool IsPlayersSpawnTile = false;
    public bool IsOccupiedByAttribute = false; // Not All Attributes are bad. Some are good.
    public bool IsOccupiedByBuilding = false;

    // Cached base display string
    public string BaseDisplayString { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x">X position of the tile</param>
    /// <param name="y">Y Position of the tile</param>
    /// <param name="z"> Z will default to 0 since its not supported yet</param>
    /// <param name="tile"></param>
    public LevelTile(int x, int y, int z)
    {
        TileX = x;
        TileY = y;
        if (!ZLevelsSupported)
        { TileZ = 0; }
        MyPosition = new LevelPOS(x, y, z);
        MyParentsPosition = new WorldTilePos(x, y);

        UpdateBaseDisplayString();
    }
    public LevelTile() { }

    public void UpdateBaseDisplayString()
    {
        if (Biome == TileEnums.LevelTileBiome.Forest)
        {
            switch (ForestTileType)
            {
                case TileEnums.ForestTiles.LushFloor:
                    BaseDisplayString = $"<color={TextAtlas.ForestFloorLush}>{TextAtlas.ForestFloorLushChar}</color>";
                    break;
                case TileEnums.ForestTiles.DirtFloor:
                    BaseDisplayString = $"<color={TextAtlas.ForestFloorDirt}>{TextAtlas.ForestFloorDirtChar}</color>";
                    break;
                case TileEnums.ForestTiles.GrassFloor:
                    BaseDisplayString = $"<color={TextAtlas.ForestFloorGrass}>{TextAtlas.ForestFloorGrassChar}</color>";
                    break;
                case TileEnums.ForestTiles.MudFloor:
                    BaseDisplayString = $"<color={TextAtlas.ForestFloorMud}>{TextAtlas.ForestFloorMudChar}</color>";
                    break;
                case TileEnums.ForestTiles.LeavesFloor:
                    BaseDisplayString = $"<color={TextAtlas.ForestFloorLeaves}>{TextAtlas.ForestFloorLeavesChar}</color>";
                    break;
                case TileEnums.ForestTiles.RockyGroundFloor:
                    BaseDisplayString = $"<color={TextAtlas.ForestFloorRockyGround}>{TextAtlas.ForestFloorRockyGroundChar}</color>";
                    break;
                default:
                    BaseDisplayString = $"<color={TextAtlas.ForestFloorLush}>{TextAtlas.ForestFloorLushChar}</color>";
                    break;
            }
        }
        else if (Biome == TileEnums.LevelTileBiome.Swamp)
        {
            switch (SwampTileType)
            {
                case TileEnums.SwampTiles.WaterFloor:
                    BaseDisplayString = $"<color={TextAtlas.SwampFloorWater}>{TextAtlas.SwampFloorWaterChar}</color>";
                    break;
                case TileEnums.SwampTiles.MudFloor:
                    BaseDisplayString = $"<color={TextAtlas.SwampFloorMud}>{TextAtlas.SwampFloorMudChar}</color>";
                    break;
                case TileEnums.SwampTiles.GrassFloor:
                    BaseDisplayString = $"<color={TextAtlas.SwampFloorGrass}>{TextAtlas.SwampFloorGrassChar}</color>";
                    break;
                case TileEnums.SwampTiles.MossFloor:
                    BaseDisplayString = $"<color={TextAtlas.SwampFloorMoss}>{TextAtlas.SwampFloorMossChar}</color>";
                    break;
                case TileEnums.SwampTiles.RockyGroundFloor:
                    BaseDisplayString = $"<color={TextAtlas.SwampFloorRockyGround}>{TextAtlas.SwampFloorRockyGroundChar}</color>";
                    break;
                default:
                    BaseDisplayString = $"<color={TextAtlas.SwampFloorGrass}>{TextAtlas.SwampFloorGrassChar}</color>";
                    break;
            }
        }
        else if (Biome == TileEnums.LevelTileBiome.Jungle)
        {
            switch (JungleTileType)
            {
                case TileEnums.JungleTiles.LushFloor:
                    BaseDisplayString = $"<color={TextAtlas.JungleFloorLush}>{TextAtlas.JungleFloorLushChar}</color>";
                    break;
                case TileEnums.JungleTiles.DirtFloor:
                    BaseDisplayString = $"<color={TextAtlas.JungleFloorDirt}>{TextAtlas.JungleFloorDirtChar}</color>";
                    break;
                case TileEnums.JungleTiles.GrassFloor:
                    BaseDisplayString = $"<color={TextAtlas.JungleFloorGrass}>{TextAtlas.JungleFloorGrassChar}</color>";
                    break;
                case TileEnums.JungleTiles.MudFloor:
                    BaseDisplayString = $"<color={TextAtlas.JungleFloorMud}>{TextAtlas.JungleFloorMudChar}</color>";
                    break;
                case TileEnums.JungleTiles.RockyGroundFloor:
                    BaseDisplayString = $"<color={TextAtlas.JungleFloorRockyGround}>{TextAtlas.JungleFloorRockyGroundChar}</color>";
                    break;
                default:
                    BaseDisplayString = $"<color={TextAtlas.JungleFloorLush}>{TextAtlas.JungleFloorLushChar}</color>";
                    break;
            }
        }
        else
        {
            BaseDisplayString = $"<color={TextAtlas.forest}>{TextAtlas.forestChar}</color>";
        }
    }
}


public struct LevelPOS : IEquatable<LevelPOS>
{
    public readonly int x;
    public readonly int y;
    public readonly int z;
    public LevelPOS(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public LevelPOS(int x, int y)
    {
        this.x = x;
        this.y = y;
        this.z = 0;
    }
    public override bool Equals(object obj)
    {
        return obj is LevelPOS other && Equals(other);
    }

    public bool Equals(LevelPOS other)
    {
        return this.x == other.x && this.y == other.y;
    }

    public override int GetHashCode()
    {
        unchecked // Allow arithmetic overflow
        {
            int hash = 17;
            hash = hash * 23 + x;
            hash = hash * 23 + y;
            return hash;
        }
    }

    public static bool operator ==(LevelPOS left, LevelPOS right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(LevelPOS left, LevelPOS right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return $"({x}, {y})";
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a single tile in the world.
/// </summary>
public class WorldTile
{
    public enum WorldTileHostility
    {
        None,
        Low,
        Medium,
        High
    }
    public enum WorldTileType
    {
        Ground,
        Water,
        Mountain,
        Forest,
        Desert,
        Snow
    }
    public enum POIType
    { 
        None,
        Town,
        Road,
        Mine,
        AbandonedMine,
        Farm
    }

    public int TileX { get; set; }
    public int TileY { get; set; }
    public WorldTileType TileType { get; set; } = WorldTileType.Ground;
    public POIType POI { get; set; }
    public WorldTileHostility Hostility { get; set; } = WorldTileHostility.None;
    public bool HasBeenDiscovered { get; set; } = false;
    public bool IsTileTransversable { get; set; } = true;
    public bool HasEntityOnTile { get; set; } = false;
    public bool BelongsToRegion { get; private set; }
    public bool IsPOI { get; set; } = false;
    public bool IsRoad { get; set; } = false;
    public bool IsTown { get; set; } = false;
    public WorldRegions Region { get; set; }
    public TownBase TownOnTile { get; set; }
    public IEntity EntityOnTile { get; set; }
    public List<string> Features = new List<string>(); // e.g., "Flowers", "Rocks", "Grass"

    // Tile Desnity Percentages
    public float TreeDensity { get; set; }
    public float RockDensity { get; set; }
    public float FoliageDensity { get; set; } // Flowers/Bushes/etc.

    public float MonsterDensity { get; set; }
    public float ResourceDensity { get; set; }

    public WorldTile(int x, int y, WorldTileType tileType = WorldTileType.Ground)
    {
        TileX = x;
        TileY = y;
        TileType = tileType;
        IsTileTransversable = SetTransversable(tileType);
        Region = null;
        POI = POIType.None;
        if (tileType == WorldTileType.Forest)
        {
            Features.Add("Grass");
            Features.Add("Flowers");
            SetForestDensitys(.75f, .35f, .20f);
        }
        if (tileType == WorldTileType.Mountain)
        {
            Features.Add("Rocks");
        }
        if(tileType == WorldTileType.Desert)
        {
            Features.Add("Sand");
        }
        if(tileType == WorldTileType.Snow)
        {
            Features.Add("Snow");
        }
    }
    /// <summary>
    /// Gets the Density of the tile and returns it as a percentage in a string.
    /// </summary>
    /// <param name="density">Must match a Density Float</param>
    /// <returns></returns>
    public string DensityToString(float density)
    {
        // Multiply by 100 to get the percentage value and format to 2 decimal places.
        return $"{(density * 100):0.00}%";
    }
    /// <summary>
    /// Sets the density of the tile based on the type of tile.
    /// </summary>
    /// <param name="treecap">Sets the max Percentage of Tree Density</param>
    /// <param name="rockcap">Sets the max percentage of Rock Density</param>
    /// <param name="resourcecap">Sets the Max Percentage of Resource Density</param>
    private void SetForestDensitys(float treecap, float rockcap, float resourcecap)
    {
        TreeDensity = UnityEngine.Random.Range(.35f, treecap);
        RockDensity = UnityEngine.Random.Range(.05f, rockcap);
        ResourceDensity = UnityEngine.Random.Range(.05f, resourcecap);
    }
    public void SetTownOnTile(TownBase town)
    {
        IsTown = true;
        TownOnTile = town;
    }

    bool SetTransversable(WorldTileType type)
    {
        if (type == WorldTileType.Water || type == WorldTileType.Mountain)
        {
            return false;

        }
        return true;
    }
    public void SetRegion(WorldRegions region)
    {
        Region = region;
        BelongsToRegion = true;
    }
}

/// <summary>
/// Represents a position in the world.
/// </summary>
public sealed class WorldTilePos : IEquatable<WorldTilePos>
{
    public readonly int x;
    public readonly int y;

    public WorldTilePos(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override bool Equals(object obj)
    {
        return obj is WorldTilePos other && Equals(other);
    }

    public bool Equals(WorldTilePos other)
    {
        return other != null && this.x == other.x && this.y == other.y;
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

    public static bool operator ==(WorldTilePos left, WorldTilePos right)
    {
        if (ReferenceEquals(left, right))
            return true;
        if (left is null || right is null)
            return false;
        return left.Equals(right);
    }

    public static bool operator !=(WorldTilePos left, WorldTilePos right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return $"({x}, {y})";
    }
}

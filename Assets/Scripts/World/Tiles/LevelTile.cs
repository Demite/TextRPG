
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

    public LevelTileBiome Biome { get; set; }
    public ForestTiles ForestTileType { get; set; }

    public bool IsVisible = false;
    public bool IsExplored = false;
    public bool IsPath = false;
    public bool IsOccupiedByEnitiy = false;
    public bool IsOccupiedByFoliage = false;
    public bool IsTransversable = true;
    public bool IsPlayersSpawnTile = false;
    public bool IsOccupiedByAttribute = false; // Not All Attributes are bad. Some are good.

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
    }
    public LevelTile() { }
}


public class LevelPOS : IEquatable<LevelPOS>
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

    public static bool operator ==(LevelPOS left, LevelPOS right)
    {
        if (ReferenceEquals(left, right))
            return true;
        if (left is null || right is null)
            return false;
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
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class WorldRegions
{
    public string regionName;
    public int RegionSize;           // Total number of tiles in the region.
    public int RegionID { get; private set; }
    public int RegionDifficulty;
    public KingdomBase Kingdom;

    // New fields to store region parameters.
    public WorldTilePos RegionStartTile;
    public int Width;   // Half-width of the region (i.e. region spans from -Width to +Width around the start tile).
    public int Height;  // Half-height of the region.

    private readonly List<WorldTile> regionTiles = new List<WorldTile>();
    public IReadOnlyList<WorldTile> RegionTiles => regionTiles;

    private System.Random random = new System.Random();

    public WorldRegions(int id, int difficulty)
    {
        RegionID = id;
        RegionDifficulty = difficulty;
    }
    public void SetID(int id)
    {
        RegionID = id;
    }
    /// <summary>
    /// Retrieves tile information from the Game Manager's WorldData.
    /// </summary>
    private WorldTile GetTileInfo(int x, int y)
    {
        var pos = new WorldTilePos(x, y);
        if (Game_Manager.Instance.worldData.WorldTileData.TryGetValue(pos, out WorldTile tile))
        {
            return tile;
        }
        return null;
    }

    /// <summary>
    /// Returns a list of tiles in a rectangular area centered on the given position.
    /// </summary>
    public List<WorldTile> MyTilesToClaim(WorldTilePos pos, int sizex, int sizey)
    {
        List<WorldTile> tiles = new List<WorldTile>();
        for (int x = -sizex; x <= sizex; x++)
        {
            for (int y = -sizey; y <= sizey; y++)
            {
                WorldTile tile = GetTileInfo(pos.x + x, pos.y + y);
                if (tile != null)
                {
                    tiles.Add(tile);
                }
            }
        }
        return tiles;
    }

    /// <summary>
    /// Claims tiles for this region. In this simple example, the region claims
    /// all transversable tiles within a rectangular area centered on RegionStartTile.
    /// </summary>
    public void ClaimRegionTiles(WorldRegions region)
    {
        // Ensure the RegionStartTile is claimed (force claim even if it doesn't meet usual conditions).
        WorldTile centerTile = GetTileInfo(RegionStartTile.x, RegionStartTile.y);
        if (centerTile != null)
        {
            if (!centerTile.BelongsToRegion)
            {
                centerTile.SetRegion(region);
                if (!regionTiles.Contains(centerTile))
                {
                    regionTiles.Add(centerTile);
                }
                Debug.Log($"[Region {RegionID}] Center tile at ({centerTile.TileX}, {centerTile.TileY}) {centerTile.Region.regionName} forcefully claimed. Upped");
            }
        }
        else
        {
            Debug.LogError($"[Region {RegionID}] Center tile at ({RegionStartTile.x}, {RegionStartTile.y}) not found in world data!");
        }

        // Get all candidate tiles in the rectangular area around RegionStartTile.
        List<WorldTile> tilesToClaim = MyTilesToClaim(RegionStartTile, Width, Height);
        Debug.Log($"[Region {RegionID}] Attempting to claim {tilesToClaim.Count} candidate tiles around center ({RegionStartTile.x}, {RegionStartTile.y}) with Width: {Width} and Height: {Height}.");

        foreach (var tile in tilesToClaim)
        {
            // Skip the center tile since it's already handled.
            if (tile.TileX == RegionStartTile.x && tile.TileY == RegionStartTile.y)
                continue;

            if (tile.IsTileTransversable && !tile.BelongsToRegion)
            {
                tile.SetRegion(this);
                regionTiles.Add(tile);
                Debug.Log($"[Region {RegionID}] Tile at ({tile.TileX}, {tile.TileY}) claimed.");
                if (tile.Region == null)
                {
                    Debug.LogWarning($"[Region {RegionID}] Tile at ({tile.TileX}, {tile.TileY}) has no region assigned!");
                }
                else
                {
                    Debug.Log($"[Region {RegionID}] Skipping tile at ({tile.TileX}, {tile.TileY}). " +
                              $"Transversable: {tile.IsTileTransversable}, Already Claimed: {tile.BelongsToRegion}");
                }
            }

            RegionSize = regionTiles.Count;
            Debug.Log($"[Region {RegionID}] Claim complete. Total claimed tiles: {RegionSize}.");
        }

    }
}

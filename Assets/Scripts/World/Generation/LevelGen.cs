using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelGen : MonoBehaviour
{
    // Screen display is 57 x 12. Having inflated level size for view scrolling and to allow for larger levels.
    public int LevelX = 100; // Width
    public int LevelY = 100; // Height
    public int LevelZ = 1;

    private Dictionary<WorldTile.WorldTileType, Func<WorldTile, TileGenBase>> generatorMap;

    private void Awake()
    {
        generatorMap = new Dictionary<WorldTile.WorldTileType, Func<WorldTile, TileGenBase>>
        {
            { WorldTile.WorldTileType.Forest, tile => new ForestTileGen(tile) },
            { WorldTile.WorldTileType.Swamp, tile => new SwampTileGen(tile) },
            { WorldTile.WorldTileType.Jungle, tile => new JungleTileGen(tile) }
        };
    }

    private void ConfigureGenerator(TileGenBase gen)
    {
        gen.TileSizeX = LevelX;
        gen.TileSizeY = LevelY;
        gen.TileSizeZ = LevelZ;
        Game_Manager.Instance.levelDisplay.SetLevelSize(LevelX, LevelY);
    }

    public void GenerateLevelForTile(WorldTile tile)
    {
        // First check if this tile represents a town via POI or the IsTown flag
        if (tile.POI == WorldTile.POIType.Town || tile.IsTown)
        {
            TileGenBase gen = new HumanTownGen(tile, tile.TownOnTile);
            ConfigureGenerator(gen);
            gen.GenerateLevel();
            return;
        }

        if (generatorMap.TryGetValue(tile.TileType, out var factory))
        {
            TileGenBase gen = factory(tile);
            ConfigureGenerator(gen);
            gen.GenerateLevel();
        }
        else
        {
            Debug.LogWarning($"No level generator for tile type {tile.TileType}");
        }
    }

    // Backwards compatibility methods
    public void GenerateForestLevel(WorldTile tile) => GenerateLevelForTile(tile);
    public void GenerateSwampLevel(WorldTile tile) => GenerateLevelForTile(tile);
    public void GenerateJungleLevel(WorldTile tile) => GenerateLevelForTile(tile);
    public void GenerateHumanTownLevel(WorldTile tile) => GenerateLevelForTile(tile);
}

using System.Collections.Generic;
using UnityEngine;
using static TileEnums;

public class SwampTileGen : TileGenBase
{
    public float TreeDensity { get; private set; }
    public float MonsterDensity { get; private set; }
    public float ResourceDensity { get; private set; }

    public int TileSizeX = 100;
    public int TileSizeY = 100;
    public WorldTile Tile { get; private set; }

    public List<TreeBase> Trees { get; private set; }

    public SwampTileGen(WorldTile tile)
    {
        Tile = tile;
        TreeDensity = 0.05f;
        MonsterDensity = tile.MonsterDensity;
        ResourceDensity = tile.ResourceDensity;
        SetTrees();
    }

    public override void GenerateLevel()
    {
        Dictionary<LevelPOS, LevelTile> generatedLevel = new Dictionary<LevelPOS, LevelTile>();

        float noiseScale = 0.1f;
        float offsetX = Random.Range(0f, 100f);
        float offsetY = Random.Range(0f, 100f);

        for (int x = 0; x < TileSizeX; x++)
        {
            for (int y = 0; y < TileSizeY; y++)
            {
                float noise = Mathf.PerlinNoise((x + offsetX) * noiseScale,
                                               (y + offsetY) * noiseScale);
                SwampTiles floorType = DetermineFloorType(noise);

                LevelTile tile = new LevelTile
                {
                    TileX = x,
                    TileY = y,
                    Biome = LevelTileBiome.Swamp,
                    SwampTileType = floorType,
                    IsTransversable = true
                };

                if ((floorType == SwampTiles.MudFloor || floorType == SwampTiles.GrassFloor) &&
                    Trees != null && Trees.Count > 0 && Random.value < TreeDensity)
                {
                    TreeBase tree = Trees[Random.Range(0, Trees.Count)];
                    tile.foliage = tree;
                    tile.IsOccupiedByFoliage = true;
                    tile.IsTransversable = false;
                }

                generatedLevel.Add(new LevelPOS(x, y, 0), tile);
            }
        }

        Game_Manager.Instance.worldData.ActiveLevelData = generatedLevel;
        Debug.Log("Swamp level generated with " + generatedLevel.Count + " tiles.");
        Game_Manager.Instance.LoadLevel();
    }

    private SwampTiles DetermineFloorType(float noise)
    {
        if (noise < 0.25f)
            return SwampTiles.WaterFloor;
        else if (noise < 0.5f)
            return SwampTiles.MudFloor;
        else if (noise < 0.75f)
            return SwampTiles.GrassFloor;
        else if (noise < 0.9f)
            return SwampTiles.MossFloor;
        else
            return SwampTiles.RockyGroundFloor;
    }

    private void SetTrees()
    {
        TreeBase cypress = new TreeBase(TreeBase.TreeType.Cypress, "#556B2F");
        TreeBase willow = new TreeBase(TreeBase.TreeType.Willow, "#6B8E23");
        Trees = new List<TreeBase> { cypress, willow };
    }
}


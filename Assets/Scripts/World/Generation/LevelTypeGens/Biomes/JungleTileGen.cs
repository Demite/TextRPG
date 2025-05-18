using System.Collections.Generic;
using UnityEngine;
using static TileEnums;

public class JungleTileGen : TileGenBase
{
    public float TreeDensity { get; private set; }
    public float MonsterDensity { get; private set; }
    public float ResourceDensity { get; private set; }

    public int TileSizeX = 100;
    public int TileSizeY = 100;
    public WorldTile Tile { get; private set; }

    public List<TreeBase> Trees { get; private set; }

    public JungleTileGen(WorldTile tile)
    {
        Tile = tile;
        TreeDensity = 0.12f;
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
                JungleTiles floorType = DetermineFloorType(noise);

                LevelTile tile = new LevelTile
                {
                    TileX = x,
                    TileY = y,
                    Biome = LevelTileBiome.Jungle,
                    JungleTileType = floorType,
                    IsTransversable = true
                };

                if ((floorType == JungleTiles.LushFloor || floorType == JungleTiles.GrassFloor) &&
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
        Debug.Log("Jungle level generated with " + generatedLevel.Count + " tiles.");
        Game_Manager.Instance.LoadLevel();
    }

    private JungleTiles DetermineFloorType(float noise)
    {
        if (noise < 0.2f)
            return JungleTiles.DirtFloor;
        else if (noise < 0.4f)
            return JungleTiles.MudFloor;
        else if (noise < 0.8f)
            return JungleTiles.LushFloor;
        else if (noise < 0.9f)
            return JungleTiles.GrassFloor;
        else
            return JungleTiles.RockyGroundFloor;
    }

    private void SetTrees()
    {
        TreeBase banana = new TreeBase(TreeBase.TreeType.Banana, "#228B22");
        TreeBase palm = new TreeBase(TreeBase.TreeType.Palm, "#008000");
        TreeBase mango = new TreeBase(TreeBase.TreeType.Mango, "#FFD700");
        Trees = new List<TreeBase> { banana, palm, mango };
    }
}


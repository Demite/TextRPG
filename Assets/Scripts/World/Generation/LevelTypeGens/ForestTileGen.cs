using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static TileEnums;

// Define generation profiles for different terrain distributions.
public enum GenerationProfile
{
    Standard,
    GrassDominant,
    DirtMudDominant
}

public class ForestTileGen : TileGenBase
{
    // Densities and thresholds.
    public float TreeDensity { get; private set; }
    public float RockDensity { get; private set; }
    public float FoliageDensity { get; private set; }
    public float MonsterDensity { get; private set; }
    public float ResourceDensity { get; private set; }
    // Hill generation is handled via a secondary noise map.
    // HillThreshold defines the cutoff for a tile to be considered part of a hill.
    public float HillThreshold { get; private set; } = 0.7f;

    public List<EntityNPCBase> Monsters { get; private set; }
    public List<TreeBase> Trees { get; private set; }

    // Level dimensions.
    public int TileSizeX = 100;
    public int TileSizeY = 100;
    public WorldTile Tile { get; private set; }

    // Current generation profile.
    public GenerationProfile Profile { get; private set; }

    // Constructor: specify the world tile and optionally the generation profile.
    public ForestTileGen(WorldTile tile, GenerationProfile profile = GenerationProfile.GrassDominant)
    {
        Tile = tile;
        Profile = profile;
        TileSizeX = 100;
        TileSizeY = 100;
        TreeDensity = 0.10f;      // Chance to place a tree on eligible ground.
        RockDensity = 0.01f;
        MonsterDensity = tile.MonsterDensity;
        ResourceDensity = tile.ResourceDensity;
        SetTrees();
    }

    /// <summary>
    /// Generates a forest level using two Perlin noise maps:
    /// 1. A "ground" noise map for determining floor types.
    /// 2. A "hill" noise map (with a lower noise scale) to produce larger, contiguous hill regions.
    /// Tiles within a hill region receive a climbable cliff edge with a randomized height.
    /// </summary>
    public override void GenerateLevel()
    {
        Dictionary<LevelPOS, LevelTile> generatedLevel = new Dictionary<LevelPOS, LevelTile>();

        // These parameters are for the ground-level noise that creates fine-grained variation
        // in the floor types (such as patches of grass, dirt, or rocky areas).

        // groundNoiseScale controls the frequency of the noise pattern.
        // A higher value (e.g., 0.1 or above) means the noise changes more rapidly,
        // resulting in smaller, more frequent patches of different floor types.
        // A lower value results in larger, smoother regions.
        // In this example, 0.1f gives a moderate level of detail.
        float groundNoiseScale = 0.1f;

        // groundOffsetX and groundOffsetY are random offsets for the noise function.
        // They effectively shift the noise pattern along the X and Y axes.
        // Changing these values will produce an entirely different arrangement of floor types,
        // even if the scale remains the same.
        // They ensure that each level generation yields a unique pattern.
        float groundOffsetX = UnityEngine.Random.Range(0f, 100f);
        float groundOffsetY = UnityEngine.Random.Range(0f, 100f);


        // These parameters are for the hill noise, which is used to create larger, contiguous regions
        // such as hills or elevated terrain.

        // hillNoiseScale controls the frequency of the hill noise pattern.
        // A lower hillNoiseScale (like 0.05f) means the noise varies more slowly,
        // which produces larger, smoother hills or elevated areas.
        // Increasing this value would lead to smaller, more frequent changes in elevation.
        float hillNoiseScale = 0.05f;

        // hillOffsetX and hillOffsetY are random offsets for the hill noise.
        // They shift the hill pattern so that the location of hills varies between different level generations.
        // Just like the ground offsets, these values help ensure each generated terrain is unique.
        float hillOffsetX = UnityEngine.Random.Range(0f, 100f);
        float hillOffsetY = UnityEngine.Random.Range(0f, 100f);


        for (int x = 0; x < TileSizeX; x++)
        {
            for (int y = 0; y < TileSizeY; y++)
            {
                // Determine the ground type via the ground noise.
                float groundNoise = Mathf.PerlinNoise((x + groundOffsetX) * groundNoiseScale,
                                                      (y + groundOffsetY) * groundNoiseScale);
                ForestTiles floorType = DetermineFloorType(groundNoise);

                // Create the tile; by default, it is traversable.
                LevelTile tile = new LevelTile
                {
                    TileX = x,
                    TileY = y,
                    ForestTileType = floorType,
                    IsTransversable = true
                };

                // Determine hill membership using a secondary noise map.
                float hillNoise = Mathf.PerlinNoise((x + hillOffsetX) * hillNoiseScale,
                                                    (y + hillOffsetY) * hillNoiseScale);
                bool isHillTile = hillNoise > HillThreshold;

                if (isHillTile)
                {
                    // This tile is part of a hill (cliff edge) region.
                    // Randomly choose a MineralType for visual variety.
                    MineralType randomMineral = (MineralType)UnityEngine.Random.Range(
                        0, System.Enum.GetValues(typeof(MineralType)).Length);
                    CliffEdge hillEdge = new CliffEdge(randomMineral);
                    // For hills, mark them as climbable and assign a random height (e.g., between 2 and 5 tiles tall).
                    hillEdge.IsClimbable = true;
                    hillEdge.CliffEdgeHeight = UnityEngine.Random.Range(2, 6);
                    tile.attribute = hillEdge;
                    // A hill tile is not directly traversable until climbed.
                    tile.IsTransversable = false;
                    tile.IsOccupiedByAttribute = true;
                }
                else
                {
                    // For non-hill tiles that can support trees, potentially place a tree.
                    if (floorType == ForestTiles.LushFloor ||
                        floorType == ForestTiles.GrassFloor ||
                        floorType == ForestTiles.LeavesFloor)
                    {
                        if (UnityEngine.Random.value < TreeDensity && Trees != null && Trees.Count > 0)
                        {
                            TreeBase selectedTree = Trees[UnityEngine.Random.Range(0, Trees.Count)];
                            tile.foliage = selectedTree;
                            tile.IsOccupiedByFoliage = true;
                            tile.IsTransversable = false;
                        }
                    }
                }

                // Use LevelPOS as the key (assuming z = 0).
                LevelPOS pos = new LevelPOS(x, y, 0);
                generatedLevel.Add(pos, tile);
            }
        }

        // Save and load the generated level.
        Game_Manager.Instance.worldData.ActiveLevelData = generatedLevel;
        Debug.Log("Forest level generated with " + generatedLevel.Count + " tiles using profile: " + Profile);
        Debug.Log("Loading Level...");
        Game_Manager.Instance.LoadLevel();
    }

    /// <summary>
    /// Determines the forest floor type based on the noise value and the generation profile.
    /// Adjust thresholds to create different terrain distributions.
    /// </summary>
    private ForestTiles DetermineFloorType(float noise)
    {
        switch (Profile)
        {
            case GenerationProfile.GrassDominant:
                if (noise < 0.15f)
                    return ForestTiles.RockyGroundFloor;
                else if (noise < 0.30f)
                    return ForestTiles.DirtFloor;
                else if (noise < 0.35f)
                    return ForestTiles.MudFloor;
                else if (noise < 0.75f)
                    return ForestTiles.GrassFloor;
                else if (noise < 0.90f)
                    return ForestTiles.LushFloor;
                else
                    return ForestTiles.LeavesFloor;

            case GenerationProfile.DirtMudDominant:
                if (noise < 0.25f)
                    return ForestTiles.RockyGroundFloor;
                else if (noise < 0.55f)
                    return ForestTiles.DirtFloor;
                else if (noise < 0.65f)
                    return ForestTiles.MudFloor;
                else if (noise < 0.75f)
                    return ForestTiles.GrassFloor;
                else if (noise < 0.85f)
                    return ForestTiles.LushFloor;
                else
                    return ForestTiles.LeavesFloor;

            case GenerationProfile.Standard:
            default:
                if (noise < 0.2f)
                    return ForestTiles.RockyGroundFloor;
                else if (noise < 0.4f)
                    return ForestTiles.DirtFloor;
                else if (noise < 0.5f)
                    return ForestTiles.MudFloor;
                else if (noise < 0.8f)
                    return ForestTiles.GrassFloor;
                else if (noise < 0.95f)
                    return ForestTiles.LushFloor;
                else
                    return ForestTiles.LeavesFloor;
        }
    }

    /// <summary>
    /// Initializes the available tree types.
    /// </summary>
    public void SetTrees()
    {
        TreeBase OakTree = new TreeBase(TreeBase.TreeType.Oak, "#8B4513");
        TreeBase PineTree = new TreeBase(TreeBase.TreeType.Pine, "#228B22");
        TreeBase BirchTree = new TreeBase(TreeBase.TreeType.Birch, "#FFFFFF");
        TreeBase MapleTree = new TreeBase(TreeBase.TreeType.Maple, "#FF4500");
        TreeBase WillowTree = new TreeBase(TreeBase.TreeType.Willow, "#6B8E23");

        Trees = new List<TreeBase> { OakTree, PineTree, BirchTree, MapleTree, WillowTree };
    }
}

using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class WorldGen : MonoBehaviour
{
    [Header("World Settings")]
    [Tooltip("Total width of the world in tiles.")]
    public int worldWidth = 1200;
    [Tooltip("Total height of the world in tiles.")]
    public int worldHeight = 1200;
    [Tooltip("Scale of the Perlin noise used to generate the terrain.")]
    public float noiseScale = 100f;

    [Header("Biome Thresholds (0-1)")]
    [Tooltip("Below this value, tiles will be water.")]
    public float waterThreshold = 0.7f;
    [Tooltip("Tiles with noise values below this become desert.")]
    public float desertThreshold = 0.76f;
    [Tooltip("Tiles with noise values below this become forest.")]
    public float forestThreshold = 0.88f;
    [Tooltip("Tiles with noise values below this become mountain.")]
    public float mountainThreshold = 0.95f;
    // Tiles with noise values above mountainThreshold become snow.

    private System.Random random = new System.Random();
    private List<WorldTilePos> BlackListedTiles;

    [Header("Entity Spawning")]
    [Tooltip("Chance (0 to 1) for an entity to spawn on a valid tile.")]
    [UnityEngine.Range(0f, 1f)]
    public float spawnChance = 0.1f;

    [Header("References")]
    [Tooltip("Reference to the WorldData component.")]
    private WorldData worldData;
    private WorldDisplay worldDisplay;

    private void Start()
    {
        BlackListedTiles = new List<WorldTilePos>();
        worldData = Game_Manager.Instance.worldData;
        worldDisplay = Game_Manager.Instance.worldDisplay;
        if (worldDisplay == null)
        {
            Debug.LogError("WorldDisplay not found!");
        }
    }

    /// <summary>
    /// Generates the world asynchronously with periodic loading text updates.
    /// </summary>
    public IEnumerator GenerateWorldCoroutine()
    {
        // --- WORLD TILE GENERATION ---
        for (int y = 0; y < worldHeight; y++)
        {
            for (int x = 0; x < worldWidth; x++)
            {
                // Sample Perlin noise.
                float sampleX = x / noiseScale;
                float sampleY = y / noiseScale;
                float noiseValue = Mathf.PerlinNoise(sampleX, sampleY);

                // Determine the tile type based on noise thresholds.
                WorldTile.WorldTileType tileType;
                if (noiseValue < waterThreshold)
                {
                    tileType = WorldTile.WorldTileType.Water;
                }
                else if (noiseValue < desertThreshold)
                {
                    tileType = WorldTile.WorldTileType.Desert;
                }
                else if (noiseValue < forestThreshold)
                {
                    tileType = WorldTile.WorldTileType.Forest;
                }
                else if (noiseValue < mountainThreshold)
                {
                    tileType = WorldTile.WorldTileType.Mountain;
                }
                else
                {
                    tileType = WorldTile.WorldTileType.Snow;
                }

                // Create and add the tile to WorldData.
                WorldTile tile = new WorldTile(x, y, tileType);
                worldData.AddTile(tile);

                // Update loading progress.
                float percentComplete = (float)(y * worldWidth + x) / (worldWidth * worldHeight);
                Game_Manager.Instance.displayPanels.UpdateLoadingText($"Generating World: {Mathf.RoundToInt(percentComplete * 100)}%");
            }

            // Yield every 50 rows to keep the game responsive.
            if (y % 50 == 0)
                yield return null;
        }
        Game_Manager.Instance.displayPanels.UpdateLoadingText("World Generation Complete");
        yield return null;

        // --- REGION & KINGDOM GENERATION ---
        Game_Manager.Instance.displayPanels.UpdateLoadingText("Generating Regions and Kingdoms...");
        Thread.Sleep(1000);
        GenerateRegionsForWorld(5);
        yield return null;

        // --- BUILDING WORLD DISPLAY ---
        Game_Manager.Instance.displayPanels.UpdateLoadingText("Building World Display...");
        worldDisplay.BuildWorldDisplay();
        yield return null;

        // --- FINALIZING ---
        Game_Manager.Instance.displayPanels.UpdateLoadingText("Finalizing World Data...");
        Thread.Sleep(1000);


        Game_Manager.Instance.GameGenerated = true;
        //Game_Manager.Instance.CheckAllTilesForAnyRegionOwnership();
        yield return null;

        Game_Manager.Instance.displayPanels.UpdateLoadingText("World Generation is now completel. Please Wait Game is starting.");

    }

    /// <summary>
    /// Generates regions, assigns kingdoms, and then spawns towns (with POI mines).
    /// </summary>
    private void GenerateRegionsForWorld(int regions)
    {
        // Create a list of transversable tiles that are not already part of a region.
        List<WorldTilePos> transversableTiles = new List<WorldTilePos>();
        foreach (var tile in worldData.WorldTileData.Values)
        {
            if (tile.IsTileTransversable && !tile.BelongsToRegion)
                transversableTiles.Add(new WorldTilePos(tile.TileX, tile.TileY));
        }

        // Loop and set up the regions.
        for (int i = 0; i < regions; i++)
        {
            // Generate random dimensions for the region.
            int randomXSize = random.Next(10, 20);
            int randomYSize = random.Next(10, 20);

            // Choose a random starting tile.
            WorldTilePos startTile = transversableTiles[random.Next(transversableTiles.Count)];

            // Create and initialize the region.
            WorldRegions region = new WorldRegions(i, 1)
            {
                RegionStartTile = startTile,
                Width = randomXSize,
                Height = randomYSize
            };
            Game_Manager.Instance.WorldRegions.Add(region);

            // Set region ID and name.
            region.SetID(i);
            region.regionName = "Region " + i;
            region.ClaimRegionTiles(region);

            // --- KINGDOM CREATION ---
            KingdomBase kingdom = CreateKingdom("Kingdom " + i, new HumanoidGen(EntityGender.Male), new HumanoidGen(EntityGender.Female), region, i);
            region.Kingdom = kingdom;
            kingdom.KingdomRegion = region;
            Game_Manager.Instance.KingdomInGame.Add(kingdom);

            // Update progress.
            Game_Manager.Instance.displayPanels.UpdateLoadingText($"Generating Region {i + 1}/{regions} and Kingdom");
            // --- TOWN GENERATION ---
            GenerateTownsForKingdoms(kingdom);

            Game_Manager.Instance.displayPanels.UpdateLoadingText($"Region {i + 1}/{regions} and Kingdom {kingdom.KingdomName} complete.");
            kingdom.GenerateMyKingdomsRoads(100f);
        }
    }

    /// <summary>
    /// Adds mines as a POI for a town.
    /// </summary>
    private void AddMines(TownBase townOwner, WorldTilePos mineLocation, int min, int max)
    {
        int amountGenerated = 0;
        // Update the loading text before starting mine generation.
        Game_Manager.Instance.displayPanels.UpdateLoadingText($"Generating POI's'");

        if (townOwner == null)
        {
            Mine mine = new Mine("Mine", 10, 10, mineLocation);
            amountGenerated++;
            return;
        }
        WorldTilePos tile = FindSuitablePOITile(min, max, townOwner);
        if (tile != null)
        {
            Mine mine = new Mine("Mine", 10, 10, townOwner, tile);
            amountGenerated++;
        }
        Debug.Log("Mines generated: " + amountGenerated + " AA3");
    }
    /// <summary>
    /// Method to Generate Farms for each town in their respective kingdom.
    /// </summary>
    /// <param name="townOwner">Town the Farm belongs to</param>
    /// <param name="poiLocation">The World Position of the Farm</param>
    /// <param name="min"> Minimum distance from the town the farm will spawn.</param>
    /// <param name="max">Maximium distaance from the town the farm will spawn.</param>
    /// <param name="amount">Max possible amount of farms that will be generated. Random number will be choosen from 0 to amount. Other Factors apply, Tile Type and such.</param>
    private void GenerateFarmForKingdoms(TownBase townOwner, WorldTilePos poiLocation, int min, int max, int amount)
    {
        int farms = Random.Range(0, amount); // Tentative amount of farms to generate
        Debug.Log("Farms to Generate: " + farms + " FFF1");
        // update the loading text before starting POI Generation
        Game_Manager.Instance.displayPanels.UpdateLoadingText($"Generating POI's'");

        if (townOwner == null)
            return;
        for (int i = 0; i < farms; i++)
        {
            // If Town is located In the Snow or Desert, it will not have a farms -- Exception we will extend search of
            // Suitable Tile by 2x distance. TODO: If Towns POI's are further away they will have more problems.
            if (worldData.WorldTileData.TryGetValue(townOwner.TownLocation, out WorldTile tile1))
            {
                if (tile1.TileType == WorldTile.WorldTileType.Snow || tile1.TileType == WorldTile.WorldTileType.Desert)
                {
                    int adjustedMin = (min+2) * 2;
                    int adjustedMax = max * 4;
                    Debug.Log($"Orignal Min: {min} Adjusted {adjustedMin}, Max: {max} Adjusted {adjustedMax}");
                    Debug.Log("Farm attempting to be generated in The Desert or Snow. FFF1");
                    WorldTilePos tile = FindSuitablePOITile(adjustedMin, adjustedMax, townOwner, new WorldTile.WorldTileType[] { WorldTile.WorldTileType.Snow, WorldTile.WorldTileType.Desert });
                    if (tile != null)
                    {
                        Farm farm = new Farm("Farm", townOwner, tile);
                        Debug.Log("Farm Generated FFF1 " + townOwner.TownName);
                    }
                }
                else
                {
                    WorldTilePos tile = FindSuitablePOITile(min, max, townOwner);
                    if (tile != null)
                    {
                        Farm farm = new Farm("Farm", townOwner, tile);
                        Debug.Log("Farm Generated FFF1 " + townOwner.TownName);
                    }
                }
            }

        }
    }

    /// <summary>
    /// Generates towns for a given kingdom, then adds mines (POIs) to each town.
    /// </summary>
    private void GenerateTownsForKingdoms(KingdomBase kingdom)
    {
        WorldRegions myregion = kingdom.KingdomRegion;
        var regionTiles = myregion.RegionTiles;
        int townCount = Random.Range(4, 10);
        List<WorldTilePos> possibleTownTiles = new List<WorldTilePos>();

        // Find suitable tiles for towns.
        foreach (var tile in regionTiles)
        {
            if (tile.IsTileTransversable && !tile.IsTown)
            {
                possibleTownTiles.Add(new WorldTilePos(tile.TileX, tile.TileY));
            }
        }

        for (int i = 0; i < townCount; i++)
        {
            bool CapitalSet = false;
            if (possibleTownTiles.Count == 0)
            {
                Game_Manager.Instance.displayPanels.UpdateLoadingText("No more suitable tiles for towns.");
                break;
            }

            WorldTilePos townTile = possibleTownTiles[Random.Range(0, possibleTownTiles.Count)];
            if (IsSuggestionTownTileXAway(10, Game_Manager.Instance.TownsInGame.ToArray(), townTile))
            {
                if(!CapitalSet)
                {
                    TownBase TownCapital = new TownBase(kingdom);
                    if (townTile != null && worldData.WorldTileData.TryGetValue(townTile, out WorldTile ctile))
                    {
                        ctile.SetTownOnTile(TownCapital);
                        ctile.IsTileTransversable = false;
                        ctile.IsPOI = true;
                        ctile.POI = WorldTile.POIType.Town;
                        TownCapital.TownLocation = townTile;
                        TownCapital.IsCapital = true;
                        CapitalSet = true;
                        kingdom.Captial = TownCapital;
                    }
                    possibleTownTiles.Remove(townTile);
                    Game_Manager.Instance.TownsInGame.Add(TownCapital);
                }
                TownBase town = new TownBase(kingdom);
                if (townTile != null && worldData.WorldTileData.TryGetValue(townTile, out WorldTile tile))
                {
                    tile.SetTownOnTile(town);
                    tile.IsTileTransversable = false;
                    tile.IsPOI = true;
                    tile.POI = WorldTile.POIType.Town;
                    town.TownLocation = townTile;

                    Game_Manager.Instance.displayPanels.UpdateLoadingText($"Generating Towns");
                }
                possibleTownTiles.Remove(townTile);
                Game_Manager.Instance.TownsInGame.Add(town);

                // --- MINE / POI GENERATION ---
                Debug.Log("Adding mines to town. AA1");
                AddMines(town, town.TownLocation, 8, 13); // Mines will be generated further out from the towns
                GenerateFarmForKingdoms(town, town.TownLocation, 0, 2, 3); // Farms Will be generated near the towns
            }
        }
    }

    /// <summary>
    /// Finds a suitable tile (POI) for placing a mine.
    /// </summary>
    private WorldTilePos FindSuitablePOITile(int distanceMin, int distanceMax, TownBase owner)
    {
        const int maxAttempts = 100; // Limit to prevent infinite loops
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // Optional: update every few attempts (commented out to avoid spamming)
            // if (attempt % 10 == 0)
            //     Game_Manager.Instance.displayPanels.UpdateLoadingText($"Finding POI tile (Attempt {attempt + 1}/{maxAttempts})");

            WorldTilePos tile = GetRandomTileAway(owner.TownLocation, distanceMax);
            if (tile == null)
            {
                continue; // Skip null tiles.
            }

            if (IsSuggestionTownTileXAway(distanceMin, Game_Manager.Instance.TownsInGame.ToArray(), tile))
            {
                return tile;
            }
        }

        Debug.LogError("Could not find a suitable POI tile after maximum attempts.");
        return null;
    }
    /// <summary>
    /// Finds a suitable tile (POI) for placing a mine.
    /// </summary>
    private WorldTilePos FindSuitablePOITile(int distanceMin, int distanceMax, TownBase owner, WorldTile.WorldTileType[] RejectTypes)
    {
        Debug.Log($"Finding POI tile for {owner.TownName} that is {distanceMin} and {distanceMax} away. FFF1");
        const int maxAttempts = 100; // Limit to prevent infinite loops
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            WorldTilePos tile = GetRandomTileAway(owner.TownLocation, distanceMax);
            if (tile == null)
            {
                continue; // Skip null tiles.
            }

            if (worldData.WorldTileData.TryGetValue(tile, out WorldTile t))
            {
                if (t.POI == WorldTile.POIType.Road || t.POI == WorldTile.POIType.Town || t.POI == WorldTile.POIType.Mine || t.POI == WorldTile.POIType.Farm)
                {
                    continue;
                }
                bool rejected = false;
                foreach (var type in RejectTypes)
                {
                    if (t.TileType == type)
                    {
                        rejected = true;
                        break;
                    }
                }
                if (rejected)
                {
                    // We will lower generation chance by 75% if the tile is rejected.
                    if (Random.value < 0.75f)
                    {
                        continue;
                    }
                    if (IsSuggestionTownTileXAway(distanceMin, Game_Manager.Instance.TownsInGame.ToArray(), tile))
                    {
                        return tile;
                    }
                }

                if (IsSuggestionTownTileXAway(distanceMin, Game_Manager.Instance.TownsInGame.ToArray(), tile))
                {
                    return tile;
                }
            }
        }

        Debug.Log("Could not find a suitable POI tile after maximum attempts. FFF1"); // This is ok, we will just not place a POI.
        return null;
    }


    /// <summary>
    /// Checks if the suggested location is sufficiently far from existing towns.
    /// </summary>
    private bool IsSuggestionTownTileXAway(int requiredDistance, TownBase[] towns, WorldTilePos suggestedLocation)
    {
        if (towns.Length == 0)
            return true;

        foreach (TownBase town in towns)
        {
            int distance = Mathf.Abs(town.TownLocation.x - suggestedLocation.x) + Mathf.Abs(town.TownLocation.y - suggestedLocation.y);
            if (distance < requiredDistance)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Creates a new region and adds it to the Game Manager's list.
    /// </summary>
    private WorldRegions CreateRegion(WorldTilePos startTile, int regionID, int regionDifficulty, int sizeX, int sizeY)
    {
        WorldRegions region = new WorldRegions(regionID, regionDifficulty)
        {
            RegionStartTile = startTile,
            Width = sizeX,
            Height = sizeY
        };
        Game_Manager.Instance.WorldRegions.Add(region);
        return region;
    }

    /// <summary>
    /// Creates a new kingdom.
    /// </summary>
    private KingdomBase CreateKingdom(string name, IEntity king, IEntity queen, WorldRegions region, int kingdomID)
    {
        KingdomBase kingdom = new KingdomBase(name, king, queen, region, kingdomID);
        Game_Manager.Instance.displayPanels.UpdateLoadingText($"Creating Kingdom: {name}");
        return kingdom;
    }

    /// <summary>
    /// Returns a random tile away from an initial position, given a maximum distance.
    /// </summary>
    private WorldTilePos GetRandomTileAway(WorldTilePos initial, int distance, int regionCount)
    {
        BlackListedTiles.Clear();
        int maxRetries = 15 * regionCount;

        while (maxRetries > 0)
        {
            int x = initial.x + random.Next(-distance, distance);
            int y = initial.y + random.Next(-distance, distance);
            WorldTilePos tryPos = new WorldTilePos(x, y);

            if (BlackListedTiles.Contains(tryPos))
            {
                maxRetries--;
                continue;
            }

            if (worldData.WorldTileData.TryGetValue(tryPos, out WorldTile tile))
            {
                if (tile.IsTileTransversable)
                {
                    return tryPos;
                }
            }
            else
            {
                BlackListedTiles.Add(tryPos);
                maxRetries--;
                continue;
            }
            maxRetries--;
        }

        Debug.LogError("Could not find a valid tile to start a region within the allowed retries.");
        return null;
    }

    /// <summary>
    /// Returns a random tile away from an initial position.
    /// </summary>
    private WorldTilePos GetRandomTileAway(WorldTilePos initial, int distance)
    {
        BlackListedTiles.Clear();
        int maxRetries = 15;

        while (maxRetries > 0)
        {
            int x = initial.x + random.Next(-distance, distance);
            int y = initial.y + random.Next(-distance, distance);
            WorldTilePos tryPos = new WorldTilePos(x, y);

            if (BlackListedTiles.Contains(tryPos))
            {
                maxRetries--;
                continue;
            }

            if (worldData.WorldTileData.TryGetValue(tryPos, out WorldTile tile))
            {
                if (tile.IsTileTransversable)
                {
                    return tryPos;
                }
            }
            else
            {
                BlackListedTiles.Add(tryPos);
                maxRetries--;
                continue;
            }
            maxRetries--;
        }

        Debug.LogError("Could not find a valid tile away from the initial position within the allowed retries.");
        return null;
    }

    /// <summary>
    /// Spawns entities on valid tiles based on a random chance.
    /// </summary>
    public void SpawnEntities()
    {
        int currentSpawn = 0;
        int maxSpawn = 1;

        if (worldData == null)
        {
            Debug.LogError("WorldData is null. Cannot spawn entities.");
            return;
        }

        foreach (var tile in worldData.WorldTileData.Values)
        {
            if (currentSpawn >= maxSpawn)
            {
                Debug.Log("Maximum spawn reached.");
                break;
            }

            if (Random.value < spawnChance)
            {
                Rabbit rabbit = new Rabbit();
                rabbit.SpawnRabbitNearPlayer(1);
                currentSpawn++;
            }
        }
    }
}

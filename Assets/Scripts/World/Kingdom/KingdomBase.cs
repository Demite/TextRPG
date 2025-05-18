using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class KingdomBase
{
    public string KingdomName;
    public IEntity KingdomKing;
    public IEntity KingdomQueen;
    public WorldRegions KingdomRegion;
    public TownBase Captial;
    private List<string> DesertKingdomNames = new List<string>();
    private List<string> ForestKingdomNames = new List<string>();
    private List<string> MountainKingdomNames = new List<string>();
    private List<string> SnowKingdomNames = new List<string>();
    public List<KingdomTile> KingdomTiles = new List<KingdomTile>();
    // List that contains all towns under this kingdom.
    public List<TownBase> KingdomTowns = new List<TownBase>();

    public int kingdomID;

    public KingdomBase(string name, IEntity king, IEntity queen, WorldRegions region, int id)
    {
        FillKingdomNames();
        KingdomName = GetRandomKingdomName();
        KingdomKing = king;
        KingdomQueen = queen;
        KingdomRegion = region;
        kingdomID = id;
        Game_Manager.Instance.EntitiesInGame.Add(KingdomKing);
        Game_Manager.Instance.EntitiesInGame.Add(KingdomQueen);
    }

    private void FillKingdomNames()
    {
        DesertKingdomNames.Add("Desert Kingdom");
        DesertKingdomNames.Add("Desert Empire");
        DesertKingdomNames.Add("Desert Republic");
        DesertKingdomNames.Add("Desert Dominion");
        DesertKingdomNames.Add("Desert Federation");

        ForestKingdomNames.Add("Forest Kingdom");
        ForestKingdomNames.Add("Forest Empire");
        ForestKingdomNames.Add("Forest Republic");
        ForestKingdomNames.Add("Forest Dominion");
        ForestKingdomNames.Add("Forest Federation");

        MountainKingdomNames.Add("Mountain Kingdom");
        MountainKingdomNames.Add("Mountain Empire");
        MountainKingdomNames.Add("Mountain Republic");
        MountainKingdomNames.Add("Mountain Dominion");
        MountainKingdomNames.Add("Mountain Federation");

        SnowKingdomNames.Add("Snow Kingdom");
        SnowKingdomNames.Add("Snow Empire");
        SnowKingdomNames.Add("Snow Republic");
        SnowKingdomNames.Add("Snow Dominion");
        SnowKingdomNames.Add("Snow Federation");
    }


    public string GetKingdomName()
    {
        return KingdomName;
    }

    private List<string> GetNameList()
    {
        int desertTileCount = 0;
        int forestTileCount = 0;
        int mountainTileCount = 0;
        int snowTileCount = 0;
        foreach (var tile in Game_Manager.Instance.worldData.WorldTileData)
        {
            if (tile.Value.TileType == WorldTile.WorldTileType.Snow)
            {
                snowTileCount++;
            }
            else if (tile.Value.TileType == WorldTile.WorldTileType.Desert)
            {
                desertTileCount++;
            }
            else if (tile.Value.TileType == WorldTile.WorldTileType.Forest)
            {
                forestTileCount++;
            }
            else if (tile.Value.TileType == WorldTile.WorldTileType.Mountain)
            {
                mountainTileCount++;
            }
        }
        if( desertTileCount > forestTileCount && desertTileCount > mountainTileCount && desertTileCount > snowTileCount)
        {
            return DesertKingdomNames;
        }
        else if (forestTileCount > desertTileCount && forestTileCount > mountainTileCount && forestTileCount > snowTileCount)
        {
            return ForestKingdomNames;
        }
        else if (mountainTileCount > desertTileCount && mountainTileCount > forestTileCount && mountainTileCount > snowTileCount)
        {
            return MountainKingdomNames;
        }
        else
        {
            return SnowKingdomNames;
        }
    }
    private string GetRandomKingdomName()
    {
        List<string> names = GetNameList();
        return names[Random.Range(0, names.Count)];
    }
    /// <summary>
    /// Generate roads connecting the towns in this kingdom without overwriting town (or other POI) tiles.
    /// This method finds all towns belonging to the kingdom (via Game_Manager.Instance.TownsInGame),
    /// designates the first town as the capital, and then generates a road from the capital to every other town,
    /// skipping any tiles that are already marked as towns (or POIs).
    /// </summary>
    /// <param name="RoadUpkeepPercentage">The percentage that the road will be fully intact. (For now, assumed 100%)</param>
    public void GenerateMyKingdomsRoads(float RoadUpkeepPercentage)
    {
        // Filter towns belonging to this kingdom.
        List<TownBase> kingdomTowns = Game_Manager.Instance.TownsInGame.FindAll(t => t.myKingdom == this);
        if (kingdomTowns == null || kingdomTowns.Count == 0)
        {
            Debug.LogWarning("No towns found for this kingdom to generate roads.");
            return;
        }

        // Build a set of positions that contain towns (or other POIs you want to protect).
        HashSet<WorldTilePos> protectedPositions = new HashSet<WorldTilePos>();
        foreach (TownBase town in kingdomTowns)
        {
            protectedPositions.Add(town.TownLocation);
        }

        // Assume the first town is the capital.
        TownBase capital = kingdomTowns[0];

        // For each town (other than the capital), generate a road from the capital.
        foreach (TownBase town in kingdomTowns)
        {
            if (town == capital)
                continue;

            // Use the Manhattan path to ensure only horizontal/vertical moves.
            List<WorldTilePos> roadPath = GetManhattanPath(capital.TownLocation, town.TownLocation);

            // For each position along the path, update the tile to be a road if it isn't a protected tile.
            foreach (WorldTilePos pos in roadPath)
            {
                // Skip positions that are protected (e.g. town or POI locations).
                if (protectedPositions.Contains(pos))
                    continue;

                if (Game_Manager.Instance.worldData.WorldTileData.TryGetValue(pos, out WorldTile tile))
                {
                    // Optionally adjust the tile appearance based on RoadUpkeepPercentage.
                    tile.POI = WorldTile.POIType.Road;
                    Debug.Log($"Added: {roadPath.Count} roads to the world data.");
                    tile.IsPOI = false; // Roads are not POIs During world gen we will grab the tile type for the surrounding area.
                    tile.IsRoad = true;
                    tile.UpdateBaseDisplayString();
                }
            }
            Game_Manager.Instance.displayPanels.loadingtext.text = $"Road from {capital.TownName} to {town.TownName} generated.";
        }
    }


    /// <summary>
    /// Returns a list of WorldTilePos representing an L-shaped (Manhattan) path between two points,
    /// moving only UP/DOWN/LEFT/RIGHT. It first moves horizontally, then vertically (or vice versa).
    /// </summary>
    /// <param name="start">The starting world tile position.</param>
    /// <param name="end">The ending world tile position.</param>
    /// <returns>A list of WorldTilePos along the path from start to end.</returns>
    private List<WorldTilePos> GetManhattanPath(WorldTilePos start, WorldTilePos end)
    {
        List<WorldTilePos> path = new List<WorldTilePos>();

        int currentX = start.x;
        int currentY = start.y;

        // Start by adding the starting tile.
        path.Add(new WorldTilePos(currentX, currentY));

        // Move horizontally until x coordinate matches.
        while (currentX != end.x)
        {
            currentX += (end.x > currentX) ? 1 : -1;
            path.Add(new WorldTilePos(currentX, currentY));
        }

        // Then move vertically until y coordinate matches.
        while (currentY != end.y)
        {
            currentY += (end.y > currentY) ? 1 : -1;
            path.Add(new WorldTilePos(currentX, currentY));
        }

        return path;
    }


}

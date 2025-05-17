using System.Collections.Generic;
using UnityEngine;

public class HumanTownGen : TileGenBase
{
    public int townSize = 10;
    private int NumberofHomes;
    private int NumberofShops;
    public WorldTile Tile { get; private set; }
    public TownBase Town { get; private set; }
    public WorldTile.WorldTileType type;

    public int TileSizeX = 100;
    public int TileSizeY = 100;
    public int minBuildingSpacing = 2;

    public HumanTownGen(WorldTile tile, TownBase town)
    {
        Tile = tile;
        Town = town;
        townSize = town.AmountOfBuildings;
        NumberofHomes = town.NumberOfHomes;
        NumberofShops = town.NumberOfShops;
        type = tile.TileType;
    }

    public override void GenerateLevel()
    {
        Dictionary<LevelPOS, LevelTile> generatedLevel = new Dictionary<LevelPOS, LevelTile>();

        for (int x = 0; x < TileSizeX; x++)
            for (int y = 0; y < TileSizeY; y++)
                generatedLevel[new LevelPOS(x, y)] = new LevelTile(x, y, 0);

        GenerateRoads(generatedLevel);

        for (int i = 0; i < NumberofHomes; i++)
            PlaceBuildingRandomly(generatedLevel, GetHouse());

        Game_Manager.Instance.worldData.ActiveLevelData = generatedLevel;
        Debug.Log("Loading Level...");
        Game_Manager.Instance.LoadLevel();
    }

    void GenerateRoads(Dictionary<LevelPOS, LevelTile> tiles)
    {
        int roadSpacing = 15;

        for (int y = roadSpacing; y < TileSizeY; y += roadSpacing)
            for (int x = 0; x < TileSizeX; x++)
                SetAsRoad(tiles[new LevelPOS(x, y)]);

        for (int x = roadSpacing; x < TileSizeX; x += roadSpacing)
            for (int y = 0; y < TileSizeY; y++)
                SetAsRoad(tiles[new LevelPOS(x, y)]);
    }

    void SetAsRoad(LevelTile tile)
    {
        tile.IsPath = true;
        tile.IsTransversable = true;
        tile.IsOccupiedByBuilding = false;
        tile.BuildingPart = BuildingPart.None;
    }

    void PlaceBuildingRandomly(Dictionary<LevelPOS, LevelTile> tiles, HumanHouse house)
    {
        int maxAttempts = 100;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            int x = Random.Range(0, TileSizeX - house.house[0].Length);
            int y = Random.Range(0, TileSizeY - house.house.Length);

            if (CanPlaceBuilding(tiles, x, y, house))
            {
                PlaceBuildingAt(tiles, x, y, house);
                Town.BuildingsInTown.Add(house);
                break;
            }
        }
    }

    bool CanPlaceBuilding(Dictionary<LevelPOS, LevelTile> tiles, int xStart, int yStart, HumanHouse house)
    {
        int startX = Mathf.Max(0, xStart - minBuildingSpacing);
        int endX = Mathf.Min(TileSizeX, xStart + house.house[0].Length + minBuildingSpacing);
        int startY = Mathf.Max(0, yStart - minBuildingSpacing);
        int endY = Mathf.Min(TileSizeY, yStart + house.house.Length + minBuildingSpacing);

        for (int y = startY; y < endY; y++)
            for (int x = startX; x < endX; x++)
            {
                var pos = new LevelPOS(x, y);
                if (tiles.ContainsKey(pos) && (tiles[pos].IsOccupiedByBuilding || tiles[pos].IsPath))
                    return false;
            }
        return true;
    }

    void PlaceBuildingAt(Dictionary<LevelPOS, LevelTile> tiles, int xStart, int yStart, HumanHouse house)
    {
        for (int y = 0; y < house.house.Length; y++)
            for (int x = 0; x < house.house[y].Length; x++)
            {
                var pos = new LevelPOS(xStart + x, yStart + y);
                LevelTile tile = tiles[pos];
                tile.Building = house;
                tile.IsOccupiedByBuilding = true;
                char c = house.house[y][x];

                switch (c)
                {
                    case '#': tile.BuildingPart = BuildingPart.Wall; tile.IsTransversable = false; break;
                    case '.': tile.BuildingPart = BuildingPart.Floor; tile.IsTransversable = true; break;
                    case '"': tile.BuildingPart = BuildingPart.Door; tile.IsTransversable = true; break;
                    case '>': tile.BuildingPart = BuildingPart.Stairs; tile.IsTransversable = true; break;
                    default: tile.BuildingPart = BuildingPart.None; tile.IsTransversable = true; break;
                }
            }
    }

    public HumanHouse GetHouse()
    {
        return Random.Range(0, 2) == 0
            ? new HumanHouse("House", "A House", HouseType.House1)
            : new HumanHouse("House2", "A House2", HouseType.House2);
    }
}

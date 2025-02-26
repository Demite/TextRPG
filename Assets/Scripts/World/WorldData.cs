using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;

/// <summary>
/// Holds and manages the data for the entire world.
/// </summary>
[System.Serializable]
public class WorldData : MonoBehaviour
{
    // Handles World Data for Game Play
    public Dictionary<WorldTilePos, WorldTile> WorldTileData = new Dictionary<WorldTilePos, WorldTile>();
    // Handles Current Level Data for Game Play
    public Dictionary<LevelPOS, LevelTile> ActiveLevelData = new Dictionary<LevelPOS, LevelTile>();
    // Handles Last Level Data to Serialize
    public Dictionary<LevelPOS, LevelTile> InactiveLevelData = new Dictionary<LevelPOS, LevelTile>();

    // Example method to add a tile.
    public void AddTile(WorldTile tile)
    {
        var pos = new WorldTilePos(tile.TileX, tile.TileY);
        if (!WorldTileData.ContainsKey(pos))
        {
            WorldTileData.Add(pos, tile);
        }
        else
        {
            Debug.LogWarning("A tile already exists at position: " + pos.x + ", " + pos.y);
        }
    }
    public void AddTile(LevelTile tile, Dictionary<LevelPOS, LevelTile> data)
    {
        var pos = new LevelPOS(tile.TileX, tile.TileY, tile.TileY);
        if (!data.ContainsKey(pos))
        {
            data.Add(pos, tile);
        }
        else
        {
            Debug.LogWarning("A tile already exists at position: " + pos.x + ", " + pos.y);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10))
        {
            SaveData();
        }
        if (Input.GetKeyDown(KeyCode.F11))
        {
            LoadData();
        }
    }
    #region Entity Methods
    public void SetEntityOnLevelTile(IEntity ientity, LevelPOS pos)
    {
        if (ActiveLevelData.TryGetValue(pos, out LevelTile tile))
        {
            tile.IsOccupiedByEnitiy = true;
            tile.entity = ientity;
            tile.IsTransversable = false;
        }
    }
    public void RemoveEntityOnLevelTile(IEntity ientity, LevelPOS pos)
    {
        if (ActiveLevelData.TryGetValue(pos, out LevelTile tile))
        {
            tile.IsOccupiedByEnitiy = false;
            tile.entity = null;
            tile.IsTransversable = true;
        }
    }
    public void SetEntityOnTile(IEntity entity, WorldTilePos pos)
    {

        if (WorldTileData.TryGetValue(pos, out WorldTile tile))
        {
            tile.HasEntityOnTile = true;
            tile.EntityOnTile = entity;
            tile.IsTileTransversable = false;
        }
    }
    public void RemoveEntityOnTile(IEntity entity, WorldTilePos pos)
    {
        if (WorldTileData.TryGetValue(pos, out WorldTile tile))
        {
            tile.HasEntityOnTile = false;
            tile.EntityOnTile = null;
            tile.IsTileTransversable = true;
        }
    }
    public bool FindSuitableTileForPlayerToSpawn()
    {
        List<LevelTile> tiles = new List<LevelTile>();
        foreach (var tile in Game_Manager.Instance.worldData.ActiveLevelData.Values)
        {
            if (tile.IsTransversable && !tile.IsOccupiedByEnitiy && !tile.IsOccupiedByFoliage)
            {
                tiles.Add(tile);
            }
        }
        if (tiles.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, tiles.Count);
            LevelTile chosenTile = tiles[index];
            // Compute the spawn location from the tile's coordinates
            LevelPOS spawnLocation = new LevelPOS(chosenTile.TileX, chosenTile.TileY, chosenTile.TileZ);
            Game_Manager.Instance.PlayerLevelSpawnLocation = spawnLocation;
            Debug.Log("Player spawn location found at " + spawnLocation);
            return true;
        }
        Debug.LogError("No suitable tile found for player to spawn.");
        return false;
    }

    #endregion
    #region Foliage Methods
    public void SetFoliageOnLevelTile(IFoliage foliage, LevelPOS pos)
    {
        if (ActiveLevelData.TryGetValue(pos, out LevelTile tile))
        {
            tile.IsOccupiedByFoliage = true;
            tile.foliage = foliage;
            tile.IsTransversable = false;
        }
    }
    public void RemoveFoliageOnLevelTile(IFoliage foliage, LevelPOS pos)
    {
        if (ActiveLevelData.TryGetValue(pos, out LevelTile tile))
        {
            tile.IsOccupiedByFoliage = false;
            tile.foliage = null;
            tile.IsTransversable = true;
        }
    }
    #endregion
    #region Serialization and Deserialization Methods
    public void SaveData()
    {
        WorldData worldData = this;
        string worldDataPath = Application.dataPath + "/WorldData.json";
        SerializationUtility.SerializeData(worldData, worldDataPath);
    }
    public void LoadData()
    {
        string worldDataPath = Application.dataPath + "/WorldData.json";
        WorldData loadedWorldData = SerializationUtility.DeserializeData<WorldData>(worldDataPath);
    }
    #endregion
}


public static class SerializationUtility
{
    /// <summary>
    /// Serializes any object to JSON and writes it to the specified file path.
    /// </summary>
    public static void SerializeData<T>(T data, string filePath)
    {
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(filePath, json);
        Debug.Log($"Data serialized to {filePath}");
    }

    /// <summary>
    /// Reads JSON from the specified file path and deserializes it into an object of type T.
    /// </summary>
    public static T DeserializeData<T>(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"File not found: {filePath}");
            return default;
        }

        string json = File.ReadAllText(filePath);
        T data = JsonConvert.DeserializeObject<T>(json);
        Debug.Log($"Data deserialized from {filePath}");
        return data;
    }
}

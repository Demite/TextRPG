using UnityEngine;

public class LevelGen : MonoBehaviour
{
    // Screen display is 57 x 12. Having inflated level size for view scrolling and to allow for larger levels.
    public int LevelX = 100; // Width    
    public int LevelY = 100; // Hight 
    public int LevelZ = 1;

    public void GenerateWorld()
    {
        // Generate the world layout here (e.g., deciding which type of level comes where)
    }

    public void GenerateForestLevel(WorldTile tile)
    {
        // Create an instance of your ForestTileGen to generate a forest level.
        ForestTileGen forestGen = new ForestTileGen(tile);
        Debug.Log($"Number of available tree to spawn in the level: {forestGen.Trees.Count}.");
        forestGen.TileSizeX = LevelX;
        forestGen.TileSizeY = LevelY;
        forestGen.TileSizeZ = LevelZ;
        Game_Manager.Instance.levelDisplay.SetLevelSize(LevelX, LevelY);
        forestGen.GenerateLevel();
    }
    public void GenerateHumanTownLevel(WorldTile tile)
    {
        // Create an instance of your HumanTownGen to generate a town level.
        HumanTownGen humantown = new HumanTownGen(tile, tile.TownOnTile);
        humantown.TileSizeX = LevelX;
        humantown.TileSizeY = LevelY;
        humantown.TileSizeZ = LevelZ;
        Game_Manager.Instance.levelDisplay.SetLevelSize(LevelX, LevelY);
        humantown.GenerateLevel();
    }

    void Start()
    {
    }
}

using UnityEngine;
using TMPro;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Display : MonoBehaviour
{
    [Header("Display Settings")]
    public GameObject ViewPanel;
    public TMP_Text DisplayText; // e.g., 57 characters wide x 17 rows high
    [SerializeField]
    public const int rows = 12;
    [SerializeField]
    public const int columns = 57;

    [Header("World Settings")]
    protected int worldDataWidth;
    public int WorldDataWidth { get { return worldDataWidth; } }
    protected int worldDataHeight;
    public int WorldDataHeight { get { return worldDataHeight; } }
    protected WorldData worldData;
    public Vector2Int currentViewPos;

    // Temporary tile display overrides.
    public Dictionary<WorldTilePos, string> tileOverrides = new Dictionary<WorldTilePos, string>();
    public Dictionary<LevelPOS, string> LeveltileOverrides = new Dictionary<LevelPOS, string>();

    // Example: using a ticker to trigger the initial display build.
    protected Ticker ticker;

    void Start()
    {
        worldData = Game_Manager.Instance.worldData;
        ticker = Ticker.Instance;
        Debug.Log($"WorldDisplay initialized: worldDataWidth={worldDataWidth}, worldDataHeight={worldDataHeight}, columns={columns}, rows={rows}");
        ticker.RegisterTask(0, () =>
        {
            BuildWorldDisplay();
            return true;
        });
    }

    /// <summary>
    /// Rebuilds the entire ASCII display from the current view.
    /// </summary>
    public virtual void BuildWorldDisplay()
    {
        Debug.Log($"Building display with view position: {currentViewPos.x}, {currentViewPos.y}");
        StringBuilder worldBuilder = new StringBuilder();

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                int worldX = currentViewPos.x + col;
                int worldY = currentViewPos.y + row;
                WorldTilePos posKey = new WorldTilePos(worldX, worldY);

                string displayChar = " "; // default

                if (worldData.WorldTileData.TryGetValue(posKey, out WorldTile tile))
                {
                    // Use an override if available.
                    if (tileOverrides.TryGetValue(posKey, out string overrideDisplay))
                    {
                        displayChar = overrideDisplay;
                    }
                    else
                    {
                        // If an entity is on the tile, show its symbol.
                        if (tile.HasEntityOnTile && tile.EntityOnTile != null)
                        {
                            displayChar = $"<color={tile.EntityOnTile.entitySymbolColor}>{tile.EntityOnTile.EntitySymbol}</color>";
                        }
                        else
                        {
                            displayChar = GetTileDisplay(tile);
                        }
                    }
                }

                worldBuilder.Append(displayChar);
            }
            if (row < rows - 1)
            {
                worldBuilder.AppendLine();
            }
        }

        DisplayText.text = worldBuilder.ToString();
    }

    /// <summary>
    /// Returns a formatted string (with rich text color tags) for a given world tile.
    /// </summary>
    protected string GetTileDisplay(WorldTile tile)
    {
        if (tile.IsPOI || tile.IsRoad)
        {
            switch (tile.POI)
            {
                case WorldTile.POIType.Mine:
                    Debug.Log("Mine BB1");
                    return $"<color={TextAtlas.Mine}>{TextAtlas.mineChar}</color>";
                case WorldTile.POIType.AbandonedMine:
                    return $"<color={TextAtlas.AbandonMine}>{TextAtlas.abandonedMineChar}</color>";
                case WorldTile.POIType.Farm:
                    return $"<color={TextAtlas.Farm}>{TextAtlas.farmChar}</color>";
                case WorldTile.POIType.Town:
                    return $"<color={TextAtlas.Town}>{TextAtlas.townChar}</color>";
                case WorldTile.POIType.Road:
                    return $"<color={TextAtlas.Road}>{TextAtlas.roadChar}</color>";
                case WorldTile.POIType.Border:
                    return $"<color={TextAtlas.Border}>{TextAtlas.borderChar}</color>";
                default:
                    return TextAtlas.forest.ToString();
            }
        }
        switch (tile.TileType)
        {
            case WorldTile.WorldTileType.Water:
                return $"<color={TextAtlas.water}>{TextAtlas.waterChar}</color>";
            case WorldTile.WorldTileType.Desert:
                return $"<color={TextAtlas.desert}>{TextAtlas.desertChar}</color>";
            case WorldTile.WorldTileType.Forest:
                return $"<color={TextAtlas.forest}>{TextAtlas.forestChar}</color>";
            case WorldTile.WorldTileType.Mountain:
                return $"<color={TextAtlas.mountain}>{TextAtlas.mountainChar}</color>";
            case WorldTile.WorldTileType.Snow:
                return $"<color={TextAtlas.snow}>{TextAtlas.snowChar}</color>";
            default:
                return $"<color={TextAtlas.forest}>{TextAtlas.forest}</color>";
        }
    }

    /// <summary>
    /// Returns a formatted string for a given level tile.
    /// </summary>
    protected string GetTileDisplay(LevelTile tile)
    {
        if (tile.Biome == TileEnums.LevelTileBiome.Forest)
        {
            switch (tile.ForestTileType)
            {
                case TileEnums.ForestTiles.LushFloor:
                    return $"<color={TextAtlas.ForestFloorLush}>{TextAtlas.ForestFloorLushChar}</color>";
                case TileEnums.ForestTiles.DirtFloor:
                    return $"<color={TextAtlas.ForestFloorDirt}>{TextAtlas.ForestFloorDirtChar}</color>";
                case TileEnums.ForestTiles.GrassFloor:
                    return $"<color={TextAtlas.ForestFloorGrass}>{TextAtlas.ForestFloorGrassChar}</color>";
                case TileEnums.ForestTiles.MudFloor:
                    return $"<color={TextAtlas.ForestFloorMud}>{TextAtlas.ForestFloorMudChar}</color>";
                case TileEnums.ForestTiles.LeavesFloor:
                    return $"<color={TextAtlas.ForestFloorLeaves}>{TextAtlas.ForestFloorLeavesChar}</color>";
                case TileEnums.ForestTiles.RockyGroundFloor:
                    return $"<color={TextAtlas.ForestFloorRockyGround}>{TextAtlas.ForestFloorRockyGroundChar}</color>";
                default:
                    return $"<color={TextAtlas.ForestFloorLush}>{TextAtlas.ForestFloorLushChar}</color>";
            }
        }
        return $"<color={TextAtlas.forest}>{TextAtlas.forestChar}</color>";
    }

    public void SetTileOverride(WorldTilePos pos, string symbol)
    {
        tileOverrides[pos] = symbol;
    }

    public void SetTileOverride(LevelPOS pos, string symbol)
    {
        LeveltileOverrides[pos] = symbol;
    }

    public void RemoveTileOverride(WorldTilePos pos)
    {
        if (tileOverrides.ContainsKey(pos))
        {
            tileOverrides.Remove(pos);
        }
    }

    public void RemoveTileOverride(LevelPOS pos)
    {
        if (LeveltileOverrides.ContainsKey(pos))
        {
            LeveltileOverrides.Remove(pos);
        }
    }
}

public static class DisplayUtilities
{
    /// <summary>
    /// Removes rich text tags from a string.
    /// </summary>
    public static string StripRichText(string input)
    {
        return Regex.Replace(input, "<.*?>", "");
    }

    /// <summary>
    /// Counts the number of rows and the maximum number of columns (i.e., characters) per row.
    /// </summary>
    public static void CountRowsAndColumns(string text, out int rowCount, out int maxColumns)
    {
        // Remove rich text tags so that we count only visible characters.
        string strippedText = StripRichText(text);
        string[] rows = strippedText.Split('\n');
        rowCount = rows.Length;
        maxColumns = 0;
        foreach (string row in rows)
        {
            if (row.Length > maxColumns)
            {
                maxColumns = row.Length;
            }
        }
    }
}
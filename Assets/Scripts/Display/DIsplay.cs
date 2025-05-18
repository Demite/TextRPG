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

    protected void BuildDisplay<TPos>(System.Func<int, int, TPos> createPos, System.Func<TPos, string> getDisplay)
    {
        int estimatedCapacity = rows * (columns + 1);
        StringBuilder builder = new StringBuilder(estimatedCapacity);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                int worldX = currentViewPos.x + col;
                int worldY = currentViewPos.y + row;
                TPos pos = createPos(worldX, worldY);
                builder.Append(getDisplay(pos));
            }
            if (row < rows - 1)
            {
                builder.AppendLine();
            }
        }

        DisplayText.text = builder.ToString();
    }

    /// <summary>
    /// Rebuilds the entire ASCII display from the current view.
    /// </summary>
    public virtual void BuildWorldDisplay()
    {
        Debug.Log($"Building display with view position: {currentViewPos.x}, {currentViewPos.y}");

        BuildDisplay<WorldTilePos>(
            (x, y) => new WorldTilePos(x, y),
            pos =>
            {
                if (tileOverrides.TryGetValue(pos, out string ov))
                    return ov;
                if (worldData.WorldTileData.TryGetValue(pos, out WorldTile tile))
                {
                    if (tile.HasEntityOnTile && tile.EntityOnTile != null)
                    {
                        return $"<color={tile.EntityOnTile.entitySymbolColor}>{tile.EntityOnTile.EntitySymbol}</color>";
                    }
                    return GetTileDisplay(tile);
                }
                return " ";
            });
    }

    /// <summary>
    /// Returns a formatted string (with rich text color tags) for a given world tile.
    /// </summary>
    protected string GetTileDisplay(WorldTile tile)
    {
        if (string.IsNullOrEmpty(tile.BaseDisplayString))
        {
            tile.UpdateBaseDisplayString();
        }
        return tile.BaseDisplayString;
    }

    /// <summary>
    /// Returns a formatted string for a given level tile.
    /// </summary>
    protected string GetTileDisplay(LevelTile tile)
    {
        if (string.IsNullOrEmpty(tile.BaseDisplayString))
        {
            tile.UpdateBaseDisplayString();
        }
        return tile.BaseDisplayString;
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
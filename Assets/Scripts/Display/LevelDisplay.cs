using System.Text;
using UnityEngine;

public class LevelDisplay : Display
{
    public int Rows => rows;
    public int Columns => columns;
    private int worldDataZ;
    public int WorldDataZ { get { return worldDataZ; } }


    void Start()
    {
        worldData = Game_Manager.Instance.worldData;
    }
    public void SetLevelSize(int width, int height)
    {
        this.worldDataWidth = width;
        this.worldDataHeight = height;
    }
    /// <summary>
    /// Rebuilds the entire ASCII display from the current view.
    /// </summary>
    public override void BuildWorldDisplay()
    {
        if (Game_Manager.Instance.player.IsInLevel)
        {
            StringBuilder worldBuilder = new StringBuilder();

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    int worldX = currentViewPos.x + col;
                    int worldY = currentViewPos.y + row;
                    LevelPOS posKey = new LevelPOS(worldX, worldY);

                    string displayChar = " "; // default

                    if (worldData.ActiveLevelData.TryGetValue(posKey, out LevelTile tile))
                    {
                        // Use an override if available.
                        if (LeveltileOverrides.TryGetValue(posKey, out string overrideDisplay))
                        {
                            displayChar = overrideDisplay;
                        }
                        else
                        {
                            // If an entity is on the tile, show its symbol.
                            if (tile.IsOccupiedByEnitiy)
                            {
                                displayChar = $"<color={tile.entity.entitySymbolColor}>{tile.entity.EntitySymbol}</color>";
                            }
                            else if (tile.IsOccupiedByFoliage)
                            {
                                displayChar = $"<color={tile.foliage.SymbolColor}>{tile.foliage.Symbol}</color>";
                            }
                            else if (tile.IsOccupiedByAttribute)
                            {
                                displayChar = $"<color={tile.attribute.SymbolColor}>{tile.attribute.Symbol}</color>";
                            }
                            else if (tile.IsOccupiedByBuilding)
                            {
                                switch (tile.BuildingPart)
                                {
                                    case BuildingPart.Wall:
                                        displayChar = $"<color={tile.Building.WallColor}>{tile.Building.BuildWallSymbol}</color>";
                                        break;
                                    case BuildingPart.Door:
                                        displayChar = $"<color={tile.Building.DoorColorClosedColor}>{tile.Building.BuildDoorSymbol}</color>";
                                        break;
                                    case BuildingPart.Floor:
                                        displayChar = $"<color={tile.Building.BuildingFloorColor}>{tile.Building.BuildFloorSymbol}</color>";
                                        break;
                                    case BuildingPart.Stairs:
                                        displayChar = $"<color={tile.Building.StairsColor}>{tile.Building.BuildingStairsSymbol}</color>";
                                        break;
                                    default:
                                        displayChar = $"<color={tile.Building.BuildingFloorColor}>{tile.Building.BuildFloorSymbol}</color>";
                                        break;
                                }
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

            // For debugging: count rows and columns.
            int rowCount, maxColumns;
            DisplayUtilities.CountRowsAndColumns(DisplayText.text, out rowCount, out maxColumns);
            Debug.Log($"Display built with {rowCount} rows and a maximum of {maxColumns} columns.");

        }
    }
}

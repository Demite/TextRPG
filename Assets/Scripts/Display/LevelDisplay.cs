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
            BuildDisplay<LevelPOS>(
                (x, y) => new LevelPOS(x, y),
                pos =>
                {
                    if (LeveltileOverrides.TryGetValue(pos, out string ov))
                        return ov;
                    if (worldData.ActiveLevelData.TryGetValue(pos, out LevelTile tile))
                    {
                        if (tile.IsOccupiedByEnitiy)
                        {
                            return $"<color={tile.entity.entitySymbolColor}>{tile.entity.EntitySymbol}</color>";
                        }
                        else if (tile.IsOccupiedByFoliage)
                        {
                            return $"<color={tile.foliage.SymbolColor}>{tile.foliage.Symbol}</color>";
                        }
                        else if (tile.IsOccupiedByAttribute)
                        {
                            return $"<color={tile.attribute.SymbolColor}>{tile.attribute.Symbol}</color>";
                        }
                        else if (tile.IsOccupiedByBuilding)
                        {
                            switch (tile.BuildingPart)
                            {
                                case BuildingPart.Wall:
                                    return $"<color={tile.Building.WallColor}>{tile.Building.BuildWallSymbol}</color>";
                                case BuildingPart.Door:
                                    return $"<color={tile.Building.DoorColorClosedColor}>{tile.Building.BuildDoorSymbol}</color>";
                                case BuildingPart.Floor:
                                    return $"<color={tile.Building.BuildingFloorColor}>{tile.Building.BuildFloorSymbol}</color>";
                                case BuildingPart.Stairs:
                                    return $"<color={tile.Building.StairsColor}>{tile.Building.BuildingStairsSymbol}</color>";
                                default:
                                    return $"<color={tile.Building.BuildingFloorColor}>{tile.Building.BuildFloorSymbol}</color>";
                            }
                        }
                        else
                        {
                            return GetTileDisplay(tile);
                        }
                    }
                    return " ";
                });

            // For debugging: count rows and columns.
            int rowCount, maxColumns;
            DisplayUtilities.CountRowsAndColumns(DisplayText.text, out rowCount, out maxColumns);
            Debug.Log($"Display built with {rowCount} rows and a maximum of {maxColumns} columns.");
        }
    }
}

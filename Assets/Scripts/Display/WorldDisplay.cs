using UnityEngine;
using TMPro;
using System.Text;
using System.Collections.Generic;

public class WorldDisplay : Display
{
    public int Rows => rows;
    public int Columns => columns;

    void Start()
    {
        worldData = Game_Manager.Instance.worldData;
        ticker = Ticker.Instance;
        GetWorldSize();
        ticker.RegisterTask(0, () =>
        {
            BuildWorldDisplay();
            return true;
        });
    }

    private void GetWorldSize()
    {
        worldDataHeight = Game_Manager.Instance.worldGen.worldHeight;
        worldDataWidth = Game_Manager.Instance.worldGen.worldWidth;
    }

    public override void BuildWorldDisplay()
    {
        if (Game_Manager.Instance.player.IsInWorld)
        {
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
                        if (tileOverrides.TryGetValue(posKey, out string overrideDisplay))
                        {
                            displayChar = overrideDisplay;
                        }
                        else
                        {
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
    }
}

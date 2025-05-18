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
    }
}

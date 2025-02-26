using UnityEngine;

public abstract class KingdomTile
{
    // These Tiles are hold data of POI's for the Kingdom ( Tiles that Produce Resources  and etc )
    public enum KingdomTileType
    {
        None,
        Outpost,
        Town,
        Mine,
        Farm,
        LumberMill,
        Quarry,
        Fishery
    }
    public virtual KingdomTileType Type { get; set; }
}

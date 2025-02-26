using System.Collections.Generic;
using UnityEngine;

public class TownBase
{
    public string TownName;
    public KingdomBase myKingdom;
    public WorldTilePos TownLocation;
    public bool IsCapital;
    private List<string> TownNames = new List<string>();
    public List<KingdomTile> KingdomTilesUnderTown = new List<KingdomTile>();


    public TownBase(KingdomBase Kingdom, string name)
    {
        Kingdom = myKingdom;
        TownName = name;
    }
    public TownBase(KingdomBase kingdom)
    {
        FillTownNames();
        myKingdom = kingdom;
        TownName = GetRandomTownName();
    }
    public TownBase()
    {
        TownName = GetRandomTownName();
    }

    private void FillTownNames()
    {
        TownNames.Add("RiverShire");
        TownNames.Add("LakeShire");
        TownNames.Add("Brill");
        TownNames.Add("Farmington");
        TownNames.Add("Hilltop");
        TownNames.Add("Riverside");
        TownNames.Add("Lakeside");
        TownNames.Add("Hillside");
        TownNames.Add("Farmville");
        TownNames.Add("Brickton");
        TownNames.Add("Stonehaven");
        TownNames.Add("Light's Hope");
        TownNames.Add("Darkshire");
        TownNames.Add("Undercoat");

    }
    private string GetRandomTownName()
    {
        return TownNames[Random.Range(0, TownNames.Count)];
    }



}

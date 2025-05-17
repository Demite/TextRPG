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
    public List<Building> BuildingsInTown = new List<Building>();
    public List<EntityNPCBase> NpcsInTown = new List<EntityNPCBase>();
    public List<Building> HomesInTown = new List<Building>();
    public List<Building> ShopsInTown = new List<Building>();
    public Building Temple;
    public Building GuildHall;
    public Building Tavern;

    public bool HasGuildHall;
    public bool HasTemple;
    public bool HasTavern;
    public int AmountOfBuildings;
    public int NumberOfHomes;
    public int NumberOfShops;

    public TownBase(KingdomBase kingdom, string name)
    {
        myKingdom = kingdom;
        TownName = name;
    }
    public TownBase(KingdomBase Kingdom, string name, /*int amountofbuildings,*/ bool GuildHall, bool Temple, bool Tavern)
    {
        Kingdom = myKingdom;
        TownName = name;
        AmountOfBuildings = 3;//amountofbuildings;
        HasGuildHall = GuildHall;
        HasTemple = Temple;
        HasTavern = Tavern;
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

using UnityEngine;
public enum BuildingRace
{
    Human,
    Elf,
    Dwarf
}
public enum BuildingType
{
    Shop,
    House,
    Inn,
    Tavern,
    GuildHall,
    CityHall,
    Temple,
    Library
}
public enum BuildingPart
{
    Wall,
    Floor,
    Door,
    Stairs,
    None
}

public class Building
{
    public string Name;
    public string Description;
    public BuildingRace buildingRace;
    public BuildingType buildingType;

    public string BuildWallSymbol = "#";
    public string BuildFloorSymbol = ".";
    public string BuildDoorSymbol = "\"";
    public string BuildingStairsSymbol = "^";

    public string BuildingFloorColor = "";
    public string DoorColorOpenColor = "";
    public string DoorColorClosedColor = "";
    public string WallColor = "";
    public string StairsColor = "";

    public Building(string name, string description, BuildingRace buildingRace, BuildingType buildingType)
    {
        Name = name;
        Description = description;
        this.buildingRace = buildingRace;
        this.buildingType = buildingType;
    }


}

using UnityEngine;

public enum HouseType
{
    House1,
    House2
}
public class HumanHouse : Building
{
    public HouseType houseType;
    public string[] house;

    public HumanHouse(string name, string description, HouseType houseType) : base(name, description, BuildingRace.Human, BuildingType.House)
    {
        buildingRace = BuildingRace.Human;
        buildingType = BuildingType.House;
        this.houseType = houseType;
        SetHouseType();
        this.BuildingFloorColor = "#A67C52"; // Warm Brown (Wooden floor)
        this.DoorColorClosedColor = "#6B4F4F"; // Dark Brown (Closed door)
        this.DoorColorOpenColor = "#C9B79C"; // Soft Beige (Open door highlight)
        this.WallColor = "#8A8A8A"; // Stone Gray (Walls)
        this.StairsColor = "#B09474"; // Light Brown (Wooden stairs)

    }


    public string[] house1 = new string[]
    {
        "##########",
        "#........#",
        "#........#",
        "#........#",
        "#........#",
        "#........#",
        "####\"#####",
    };
    public string[] house2 = new string[]
    {
        "##########",
        "#........#",
        "#........#",
        "#........#",
        "\"........#",
        "#........#",
        "##########",
    };

    private void SetHouseType()
    {
        switch (houseType)
        {
            case HouseType.House1:
                house = house1;
                break;
            case HouseType.House2:
                house = house2;
                break;
        }
    }
}

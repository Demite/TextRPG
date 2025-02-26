using UnityEngine;

public interface IEntity
{
    public int Strength { get; set; }
    public int Dexterity { get; set; }
    public int Constitution { get; set; }
    public int Intelligence { get; set; }
    public int Wisdom { get; set; }
    public int Willpower { get; set; }
    public int Charisma { get; set; }
    public int HitPointsMax { get; set; }
    public int HitPointsCurrent { get; set; }
    public int staminaPointsMax { get; set; }
    public int staminaPointsCurrent { get; set; }
    public int manaPointsMax { get; set; }
    public int manaPointsCurrent { get; set; }
    string entityName { get; set; }
    WorldTilePos entityWorldPos { get; set; }
    string entitySymbolColor { get; set; }
    string EntitySymbol { get; set; }
    void DoTurn();
}

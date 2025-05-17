using UnityEngine;

public abstract class PlayerBase : MonoBehaviour, IEntity
{
    public virtual string entityName { get; set; } = "DefaultName";
    public virtual bool isMyTurn { get; set; } = false;
    public string EntitySymbol { get; set; } = "@";
    public string entitySymbolColor { get; set; }
    public bool isPlayer;
    public bool isEntityTurn;
    public WorldTilePos entityWorldPos { get; set; }
    public WorldTilePos previousPos { get; set; }
    public LevelPOS entityLevelPos { get; set; }
    public EntityRace race { get; set; }
    public StarterClass starterClass { get; set; }
    public EntityGender gender { get; set; }
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


    public virtual void DoTurn()
    {
        Debug.Log($"{entityName} is doing its turn.");
    }
}

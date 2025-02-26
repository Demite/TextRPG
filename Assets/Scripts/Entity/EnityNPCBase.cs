using UnityEngine;
public enum NPC
{
    Aggressive,
    Friendly
}
public abstract class EntityNPCBase : IEntity
{
    public virtual string entityName { get; set; } = "????";
    public virtual bool isMyTurn { get; set; } = false;
    public virtual int WanderRange { get; set; } = 5;
    public string EntitySymbol { get; set; } = "";
    public string entitySymbolColor { get; set; }
    public WorldTilePos entityWorldPos { get; set; }
    public WorldTilePos previousPos { get; set; }
    public EntityRace race { get; set; }
    public StarterClass starterClass { get; set; }
    public EntityGender gender { get; set; }
    public NPC npcType { get; set; }
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

    public abstract void DoTurn();

    public void MoveTo(WorldTilePos newPos)
    {
        WorldData data = Game_Manager.Instance.worldData;
        previousPos = entityWorldPos;
        Debug.Log($"{entityName} moving from {previousPos.x},{previousPos.y} to {newPos.x},{newPos.y}");
        entityWorldPos = newPos;
        data.RemoveEntityOnTile(this, previousPos);
        data.SetEntityOnTile(this, newPos);
    }
    //Testing method
    public void SpawnEntityNearPlayer(int amount, EntityNPCBase entity)
    {
        if (Game_Manager.Instance?.player == null)
        {
            Debug.LogError("Player instance is not set in Game_Manager.");
            return;
        }

        WorldTilePos playerPos = Game_Manager.Instance.player.entityWorldPos;
        for (int i = 0; i < amount; i++)
        {
            int x = playerPos.x + Random.Range(-WanderRange, WanderRange);
            int y = playerPos.y + Random.Range(-WanderRange, WanderRange);
            WorldTilePos newPos = new WorldTilePos(x, y);

            // For a new entity, set both previous and current positions to the new position.
            entity.previousPos = newPos;
            entity.entityWorldPos = newPos;
            Game_Manager.Instance.worldData.SetEntityOnTile(entity, newPos);
        }
    }


    public virtual void TakeTurn()
    {
        Debug.Log($"{entityName} is taking its turn.");
    }
}

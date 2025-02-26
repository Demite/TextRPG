using UnityEngine;
public class Rabbit : EntityNPCBase
{
    public override string entityName { get; set; } = "Rabbit";
    public override bool isMyTurn { get; set; }
    public override int WanderRange { get => base.WanderRange; set => base.WanderRange = value; }
    private AiBASE aiBase;
    public void Init(int x, int y)
    {
        EntitySymbol = "R";
        entitySymbolColor = "#FFA500";
        Debug.Log($"[Rabbit.Init] Symbol: {EntitySymbol}, Color: {entitySymbolColor}");

        entityWorldPos = new WorldTilePos(x, y);
        Game_Manager.Instance.worldData.SetEntityOnTile(this, entityWorldPos);

        npcType = NPC.Friendly;
        aiBase = new AiBASE();

        Game_Manager.Instance.EntitiesInGame.Add(this);
    }

    public void SpawnRabbitNearPlayer(int amount)
    {
        // Set the display properties first.
        EntitySymbol = "R";
        entitySymbolColor = "#FFA500";
        npcType = NPC.Friendly;
        aiBase = new AiBASE();

        // Then call the base spawn logic.
        base.SpawnEntityNearPlayer(amount, this);

        // Optionally, add the entity to the game.
        Game_Manager.Instance.EntitiesInGame.Add(this);
    }

    public override void DoTurn()
    {
        aiBase.MoveTo(this);
    }
}

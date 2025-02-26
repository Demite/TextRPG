using System.Collections.Generic;
using UnityEngine;

public class Turn_Manager : MonoBehaviour
{
    public int turnCount = 0;

    /// <summary>
    /// Returns a new list of IEntity instances that are occupied on the level.
    /// </summary>
    public static List<IEntity> GetEntitiesInLevel(WorldData worlddata)
    {
        List<IEntity> entities = new List<IEntity>();
        foreach (var tile in worlddata.ActiveLevelData.Values)
        {
            if (tile.IsOccupiedByEnitiy)
            {
                entities.Add(tile.entity);
            }
        }
        return entities;
    }

    /// <summary>
    /// Returns a list of IEntity instances within the given circular range from the center.
    /// (Uses squared distance for efficiency.)
    /// </summary>
    public static List<IEntity> GetEntitiesInRange(WorldTilePos center, int range, List<IEntity> entities, WorldData worldData)
    {
        var entitiesInRange = new List<IEntity>();

        if (worldData?.WorldTileData == null)
        {
            Debug.LogWarning("WorldData or its WorldTileData dictionary is null.");
            return entitiesInRange;
        }

        if (!worldData.WorldTileData.ContainsKey(center))
        {
            Debug.LogWarning($"Center tile is not present in world data: {center.x}, {center.y}");
            return entitiesInRange;
        }

        int rangeSquared = range * range;
        foreach (IEntity entity in entities)
        {
            if (entity?.entityWorldPos == null || !worldData.WorldTileData.ContainsKey(entity.entityWorldPos))
                continue;

            int dx = entity.entityWorldPos.x - center.x;
            int dy = entity.entityWorldPos.y - center.y;
            int distSquared = dx * dx + dy * dy;

            if (distSquared <= rangeSquared)
                entitiesInRange.Add(entity);
        }

        return entitiesInRange;
    }

    /// <summary>
    /// Processes NPC turns once after the player's turn has ended.
    /// </summary>
    public void ProcessNPCTurns()
    {
        Game_Manager gm = Game_Manager.Instance;
        if (gm == null)
        {
            Debug.LogError("Game_Manager instance not found.");
            return;
        }
        if (gm.player == null)
        {
            Debug.LogError("Player reference is missing.");
            return;
        }

        // Check if the player is in the world map or a level.
        if (gm.player.IsInWorld) // World map turns.
        {
            if (gm.displayPanels?.WorldData == null)
            {
                Debug.LogError("DisplayPanels or its WorldData reference is missing.");
                return;
            }

            // Gather NPCs (entities within 100 tiles excluding the player).
            List<IEntity> entitiesInRange = GetEntitiesInRange(gm.player.entityWorldPos, 100, gm.EntitiesInGame, gm.displayPanels.WorldData);
            entitiesInRange.Remove(gm.player);

            Debug.Log($"NPCs in range of player: {entitiesInRange.Count}");
            foreach (IEntity npc in entitiesInRange)
            {
                Debug.Log($"NPC Turn: {npc.entityName}");
                npc.DoTurn();
            }
        }
        else // Level turns.
        {
            if (gm.worldData?.ActiveLevelData == null)
            {
                Debug.LogError("ActiveLevelData reference is missing.");
                return;
            }

            // Gather NPCs in the level.
            List<IEntity> entitiesInLevel = GetEntitiesInLevel(gm.worldData);
            entitiesInLevel.Remove(gm.player);

            Debug.Log($"NPCs in level: {entitiesInLevel.Count}");
            foreach (IEntity npc in entitiesInLevel)
            {
                Debug.Log($"NPC Turn: {npc.entityName}");
                npc.DoTurn();
            }
        }

        // Common turn-end processing.
        turnCount++;
        gm.CurrentTurnState = TurnState.PlayerTurn;
        gm.player.ResetTurn();
    }
}

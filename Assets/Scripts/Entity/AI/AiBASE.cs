using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class AiBASE
{
    private Pathfindier pathfinder;
    private static readonly System.Random random = new System.Random();
    public AINode myAINode { get; private set; }
    public AIState myState;
    public AIType myType;
    public AIAction myAction;
    public const int TilesPerTurn = 1;

    public AiBASE()
    {
        pathfinder = new Pathfindier();
        myState = AIState.Idle;
        myType = AIType.passive;
        myAction = AIAction.Wander;
    }
    public bool ValidMoveNode(IEntity current, IEntity target)
    {
        WorldTilePos currentpos = current.entityWorldPos;
        WorldTilePos targetpos = target.entityWorldPos;
        WorldData worldData = Game_Manager.Instance.worldData;

        foreach (var tile in worldData.WorldTileData)
        {
            if (tile.Key == targetpos)
            {
                if (tile.Value.IsTileTransversable && tile.Value.EntityOnTile == null)
                {
                    // Updated: No Entity on the tile, so it's a valid move.
                    return true;
                }
            }
        }
        // Updated: return false if the target position is not in the world data.
        return false;

    }
    public bool ValidMoveNode(IEntity current, WorldTilePos targetPosition)
    {
        WorldData worldData = Game_Manager.Instance.worldData;

        // Try to get the tile directly rather than iterating over all tiles
        if (worldData.WorldTileData.TryGetValue(targetPosition, out WorldTile tile))
        {
            if (tile.IsTileTransversable && tile.EntityOnTile == null)
            {
                Debug.Log("Valid Move Node");
                return true;
            }
            else
            {
                Debug.Log("Invalid Move Node: Tile is not traversable or has an entity on it.");
                return false;
            }
        }
        else
        {
            Debug.Log("Invalid Move Node: Tile not found in world data.");
        }

        return false;
    }

    public bool ValidAttackNode(IEntity current, IEntity target)
    {
        WorldTilePos currentpos = current.entityWorldPos;
        WorldTilePos targetpos = target.entityWorldPos;
        WorldData worldData = Game_Manager.Instance.worldData;

        foreach (var tile in worldData.WorldTileData)
        {
            if (tile.Key == targetpos)
            {
                if (tile.Value.EntityOnTile != null)
                {
                    foreach (var neighbor in Pathfindier.NeighoringTiles(currentpos, worldData))
                    {
                        if (worldData.WorldTileData.TryGetValue(neighbor, out WorldTile neighborTile))
                        {
                            if (neighborTile.IsTileTransversable && neighborTile.EntityOnTile == null)
                            {
                                // Tile is Walkable and has no entity on it. ( We can move to it and attack the target)
                                return true;
                            }
                            // Tile Is not walkable or has an entity on it.
                            return false;
                        }
                        // Tile is not in the world data.
                        return false;
                    }
                    Debug.LogError("No Neighbors found");
                }
                // Updated: No entity on the tile, so it's not a valid attack node.
                return false;
            }
            // Updated: return false if the target position is not in the world data.
            return false;
        }
        // Updated: return false if the target position is not in the world data.
        return false;
    }
    public AINode GetRandomPosition(IEntity m, int distance, int maxAttempts = 10)
    {
        WorldData worldData = Game_Manager.Instance.worldData;
        WorldTilePos currentPos = m.entityWorldPos;

        for (int i = 0; i < maxAttempts; i++)
        {
            int randomx = random.Next(0, distance);
            int randomy = random.Next(0, distance);
            WorldTilePos randomPosition = new WorldTilePos(currentPos.x + randomx, currentPos.y + randomy);

            if (worldData.WorldTileData.TryGetValue(randomPosition, out WorldTile tile))
            {
                Debug.Log($"We found our new position at {randomPosition.x},{randomPosition.y}");
                return new AINode(randomPosition.x, randomPosition.y);
            }
        }

        Debug.LogWarning("No valid position found after maximum attempts.");
        return null;
    }
    public void MoveTo(IEntity m)
    {
        Debug.Log("No Node In Arguments Generating Random Node");
        if (m is EntityNPCBase npc)
        {
            Debug.Log("Entity is NPC");
            AINode newNode = GetRandomPosition(m, 10);
            if (newNode == null)
            {
                Debug.LogWarning("GetRandomPosition returned null. Aborting move.");
                return;
            }
            Debug.Log($"New Node Generated at {newNode.TargetPOS.x},{newNode.TargetPOS.y}");

            Debug.Log("New Node Generated Making sure its Valid");
            if (ValidMoveNode(npc, newNode.TargetPOS))
            {
                Debug.Log("Node is Valid Creating a Path to node");
                List<WorldTilePos> path = Pathfindier.FindPath(npc.entityWorldPos, newNode.TargetPOS, Game_Manager.Instance.worldData);

                // Log the full path
                if (path.Count > 0)
                {
                    string pathString = string.Join(" -> ", path.Select(t => $"({t.x},{t.y})"));
                    Debug.Log("Generated Path: " + pathString);
                }

                // If the path has more than one tile, skip the first tile if it's the current position.
                if (path.Count > 1)
                {
                    Debug.Log("Path Found Moving to Next Tile in path");
                    myAINode = new AINode(path[1].x, path[1].y);
                    npc.MoveTo(path[1]);
                }
                else if (path.Count == 1)
                {
                    Debug.Log("Path only contains the current position. No move made.");
                }
            }
            else
            {
                Debug.LogWarning("New Node is not a valid move node.");
            }
        }
    }
}

public class AINode
{
    public int x_Destination;
    public int y_Destination;
    public WorldTilePos TargetPOS { get; private set; }

    public AINode(int x, int y)
    {
        if(x == 0 && y == 0 )
        {
            Debug.LogError("Invalid Node Position");
        }
        x_Destination = x;
        y_Destination = y;
        TargetPOS = new WorldTilePos(x, y);
    }

    public void setDestination(int x, int y)
    {
        x_Destination = x;
        y_Destination = y;
        TargetPOS = new WorldTilePos(x, y);
    }
}
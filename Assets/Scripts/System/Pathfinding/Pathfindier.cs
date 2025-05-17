using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple A* pathfinding class that works on a grid represented by WorldTileData.
/// It allows movement in the four cardinal directions (up, down, left, right),
/// checks if tiles are traversable (including whether an entity is occupying a tile),
/// and applies a small penalty for turns (to simulate turn-by-turn biases).
/// </summary>
public class Pathfindier
{
    // Node structure to store pathfinding data.
    private struct Node
    {
        public WorldTilePos position;
        public int gCost;  // Cost from the start node.
        public int hCost;  // Heuristic cost to the goal.
        public int fCost { get { return gCost + hCost; } }
        // For path reconstruction, 'parent' is used.
        // Parent is nullable because WorldTilePos is a struct.
        public WorldTilePos? parent;
        public Vector2Int direction; // The movement direction from the parent.

        public Node(WorldTilePos pos, int g, int h, WorldTilePos? parent, Vector2Int direction)
        {
            this.position = pos;
            this.gCost = g;
            this.hCost = h;
            this.parent = parent;
            this.direction = direction;
        }
    }

    /// <summary>
    /// Finds a path from the start position to the goal using the A* algorithm.
    /// Returns a list of WorldTilePos representing the path (including start and goal).
    /// Returns an empty list if no path is found.
    /// </summary>
    public static List<WorldTilePos> FindPath(WorldTilePos start, WorldTilePos goal, WorldData worldData)
    {
        List<Node> openSet = new List<Node>();
        HashSet<WorldTilePos> closedSet = new HashSet<WorldTilePos>();

        // Create the start node. We pass null for parent to indicate there is none.
        Node startNode = new Node(start, 0, ManhattanDistance(start, goal), null, Vector2Int.zero);
        openSet.Add(startNode);

        // Dictionary to help reconstruct the path.
        Dictionary<WorldTilePos, Node> cameFrom = new Dictionary<WorldTilePos, Node>();
        cameFrom[start] = startNode;

        while (openSet.Count > 0)
        {
            // Sort openSet so the node with the lowest fCost is at the front.
            openSet.Sort((a, b) =>
            {
                int compare = a.fCost.CompareTo(b.fCost);
                if (compare == 0)
                    compare = a.hCost.CompareTo(b.hCost);
                return compare;
            });

            Node current = openSet[0];
            openSet.RemoveAt(0);

            // If we have reached the goal, reconstruct and return the path.
            if (current.position.Equals(goal))
            {
                return ReconstructPath(cameFrom, current.position);
            }

            closedSet.Add(current.position);

            foreach (WorldTilePos neighborPos in GetNeighbors(current.position, worldData))
            {
                // Skip neighbors that have already been processed.
                if (closedSet.Contains(neighborPos))
                    continue;

                // Check if the neighbor is traversable (i.e. not blocked and not occupied).
                if (!IsTraversable(neighborPos, worldData))
                    continue;

                // Calculate the cost from current to this neighbor.
                int tentativeGCost = current.gCost + 1;

                // Determine the movement direction from current to neighbor.
                Vector2Int newDirection = new Vector2Int(neighborPos.x - current.position.x, neighborPos.y - current.position.y);
                // Apply a turn penalty if the movement direction changes.
                if (current.direction != Vector2Int.zero && newDirection != current.direction)
                {
                    tentativeGCost += 1; // Turn penalty.
                }

                // Check if the neighbor is already in the open set.
                bool inOpenSet = false;
                foreach (Node n in openSet)
                {
                    if (n.position.Equals(neighborPos))
                    {
                        inOpenSet = true;
                        break;
                    }
                }

                // If not in the open set or we found a cheaper path, update the neighbor.
                if (!inOpenSet || tentativeGCost < cameFrom[neighborPos].gCost)
                {
                    Node neighborNode = new Node(
                        neighborPos,
                        tentativeGCost,
                        ManhattanDistance(neighborPos, goal),
                        current.position,
                        newDirection
                    );
                    cameFrom[neighborPos] = neighborNode;

                    if (!inOpenSet)
                    {
                        openSet.Add(neighborNode);
                    }
                }
            }
        }

        // If we exit the loop, no path was found.
        return new List<WorldTilePos>();
    }

    public static int GetDistanceBetweenLevelPositions(LevelPOS start, LevelPOS goal)
    {
        return Mathf.Abs(start.x - goal.x) + Mathf.Abs(start.y - goal.y);
    }
    public static int GetDistanceBetweenLevelPositions(WorldTilePos a, WorldTilePos b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    // Manhattan distance heuristic for grid-based movement.
    private static int ManhattanDistance(WorldTilePos a, WorldTilePos b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    // Reconstructs the path from the goal node back to the start node using the cameFrom dictionary.
    private static List<WorldTilePos> ReconstructPath(Dictionary<WorldTilePos, Node> cameFrom, WorldTilePos current)
    {
        List<WorldTilePos> path = new List<WorldTilePos>();
        path.Add(current);
        // Here we assume that a null parent indicates the start node.
        while (cameFrom[current].parent.HasValue)
        {
            current = cameFrom[current].parent.Value;
            path.Add(current);
        }
        path.Reverse();
        return path;
    }

    // Returns the neighboring tile positions (up, down, left, right) that exist in the world.
    private static List<WorldTilePos> GetNeighbors(WorldTilePos pos, WorldData worldData)
    {
        List<WorldTilePos> neighbors = new List<WorldTilePos>();

        List<WorldTilePos> potentialNeighbors = new List<WorldTilePos>()
        {
            new WorldTilePos(pos.x, pos.y - 1), // Up
            new WorldTilePos(pos.x, pos.y + 1), // Down
            new WorldTilePos(pos.x - 1, pos.y), // Left
            new WorldTilePos(pos.x + 1, pos.y)  // Right
        };

        foreach (WorldTilePos neighbor in potentialNeighbors)
        {
            if (worldData.WorldTileData.ContainsKey(neighbor))
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }
    /// <summary>
    /// This Method is used in world generation to find the distance between a town and a suggested tile.
    /// </summary>
    /// <param name="town"></param>
    /// <param name="tile"></param>
    /// <returns></returns>
    public int GetDistance(TownBase town, WorldTilePos tile)
    {
        var path = FindPath(town.TownLocation, tile, Game_Manager.Instance.worldData);
        int distance = path.Count;
        return distance;
    }

    public static List<WorldTilePos> NeighoringTiles(WorldTilePos pos, WorldData worldData)
    {
        List<WorldTilePos> neighbors = new List<WorldTilePos>();

        List<WorldTilePos> potentialNeighbors = new List<WorldTilePos>()
        {
            new WorldTilePos(pos.x, pos.y - 1), // Up
            new WorldTilePos(pos.x, pos.y + 1), // Down
            new WorldTilePos(pos.x - 1, pos.y), // Left
            new WorldTilePos(pos.x + 1, pos.y)  // Right
        };

        foreach (WorldTilePos neighbor in potentialNeighbors)
        {
            if (worldData.WorldTileData.ContainsKey(neighbor))
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }
    /// <summary>
    /// Method to return a list of tiles that are within a certain distance from a given position SQUARED.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="worldData"></param>
    /// <param name="distance"></param>"
    /// <returns></returns>
    public static List<WorldTilePos> AvaliableSquaredTiles(WorldTilePos pos, WorldData worldData, int distance)
    {
        List<WorldTilePos> Tiles = new List<WorldTilePos>();

        for (int x = -distance; x <= distance; x++)
        {
            for (int y = -distance; y <= distance; y++)
            {
                WorldTilePos tile = new WorldTilePos(pos.x + x, pos.y + y);
                if (worldData.WorldTileData.ContainsKey(tile))
                {
                    Tiles.Add(tile);
                }
            }
        }
        return Tiles;
    }
    // Checks if a given tile is traversable.
    // This includes checking whether the tile exists, is marked as traversable,
    // and is not occupied by an entity.
    private static bool IsTraversable(WorldTilePos pos, WorldData worldData)
    {
        if (!worldData.WorldTileData.TryGetValue(pos, out WorldTile tile))
            return false;

        // Updated: check that the tile is traversable and has no entity.
        return tile.IsTileTransversable && tile.EntityOnTile == null;
    }

    // Helper method to check if a tile has an entity on it.
    public static bool IsEntityOnTile(WorldTilePos pos, WorldData worldData)
    {
        if (!worldData.WorldTileData.TryGetValue(pos, out WorldTile tile))
            return false;

        // Updated: returns true if an entity exists on the tile.
        return tile.EntityOnTile != null;
    }
}

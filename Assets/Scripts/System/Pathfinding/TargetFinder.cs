using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// TargetFinder provides methods to locate and filter potential targets
/// based on pathing, range, and area-of-effect shapes (e.g., cones).
/// It leverages Pathfindier for path-based queries.
/// </summary>
public class TargetFinder
{
    private WorldData _worldData;

    public TargetFinder(WorldData worldData)
    {
        _worldData = worldData;
    }

    /// <summary>
    /// Finds the nearest target (e.g., enemy) from start within maxRange (Manhattan).
    /// </summary>
    public WorldTilePos? FindNearestTarget(WorldTilePos start, IEnumerable<WorldTilePos> potentialTargets, int maxRange)
    {
        WorldTilePos? best = null;
        int bestDistance = int.MaxValue;

        foreach (var target in potentialTargets)
        {
            int distance = Pathfindier.GetDistanceBetweenLevelPositions(start, target);
            if (distance <= maxRange && distance < bestDistance)
            {
                // Validate there's a viable path
                var path = Pathfindier.FindPath(start, target, _worldData);
                if (path.Count > 0)
                {
                    bestDistance = distance;
                    best = target;
                }
            }
        }

        return best;
    }

    /// <summary>
    /// Returns all targets within a cone from start in a given direction.
    /// angleWidth in degrees, length in tiles.
    /// </summary>
    public List<WorldTilePos> GetTargetsInCone(
        WorldTilePos start,
        Vector2Int direction,
        float angleWidth,
        int length,
        IEnumerable<WorldTilePos> potentialTargets)
    {
        var results = new List<WorldTilePos>();
        Vector2 dir = new Vector2(direction.x, direction.y);
        float halfAngle = angleWidth * 0.5f;

        foreach (var tile in potentialTargets)
        {
            Vector2 offset = new Vector2(tile.x - start.x, tile.y - start.y);
            if (offset.magnitude > length || offset == Vector2.zero) continue;

            float angle = Vector2.Angle(dir, offset);
            if (angle <= halfAngle)
            {
                // Optional: check line-of-sight/path unblocked
                var path = Pathfindier.FindPath(start, tile, _worldData);
                if (path.Count > 0)
                    results.Add(tile);
            }
        }
        return results;
    }

    /// <summary>
    /// Returns all targets reachable by path within maxPathDistance (path length).
    /// </summary>
    public List<WorldTilePos> GetTargetsInPathRange(
        WorldTilePos start,
        int maxPathDistance,
        IEnumerable<WorldTilePos> potentialTargets)
    {
        var results = new List<WorldTilePos>();

        foreach (var target in potentialTargets)
        {
            var path = Pathfindier.FindPath(start, target, _worldData);
            if (path.Count > 0 && path.Count - 1 <= maxPathDistance)
                results.Add(target);
        }

        return results;
    }
}

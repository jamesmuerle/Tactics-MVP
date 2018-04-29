using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathUtils {
    public static Dictionary<Vector2Int, List<Vector2Int>> SearchFromPos(
        Vector2Int pos,
        int maxDistance,
        Func<Vector2Int, bool> isBlocked
    ) {
        Dictionary<Vector2Int, List<Vector2Int>> paths = new Dictionary<Vector2Int, List<Vector2Int>>();
        paths[pos] = new List<Vector2Int>();
        Dictionary<int, List<Vector2Int>> processQueue = new Dictionary<int, List<Vector2Int>>();
        processQueue[0] = new List<Vector2Int> { pos };

        for (int distance = 0; distance < maxDistance; distance += 1) {
            processQueue[distance + 1] = new List<Vector2Int>();
            foreach (Vector2Int posToProcess in processQueue[distance]) {
                SearchSurroundingPositions(posToProcess, paths, processQueue, isBlocked);
            }
        }

        return paths;
    }

    private static void SearchSurroundingPositions(
        Vector2Int pos,
        Dictionary<Vector2Int, List<Vector2Int>> paths,
        Dictionary<int, List<Vector2Int>> processQueue,
        Func<Vector2Int, bool> isBlocked
    ) {
        foreach (Vector2Int direction in Directions.CARDINAL_DIRECTIONS) {
            Vector2Int target = pos + direction;
            if (!paths.ContainsKey(target) && !isBlocked(target)) {
                List<Vector2Int> newPath = new List<Vector2Int>();
                newPath.AddRange(paths[pos]);
                newPath.Add(target);
                paths[target] = newPath;
                processQueue[newPath.Count].Add(target);
            }
        }
    }
}

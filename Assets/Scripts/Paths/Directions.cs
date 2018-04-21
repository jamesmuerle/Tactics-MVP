using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Directions {
    public static Vector2Int NORTH = new Vector2Int(0, 1);
    public static Vector2Int WEST = new Vector2Int(-1, 0);
    public static Vector2Int SOUTH = new Vector2Int(0, -1);
    public static Vector2Int EAST = new Vector2Int(1, 0);
    public static Vector2Int NORTHWEST = NORTH + WEST;
    public static Vector2Int NORTHEAST = NORTH + EAST;
    public static Vector2Int SOUTHWEST = SOUTH + WEST;
    public static Vector2Int SOUTHEAST = SOUTH + EAST;
    public static Vector2Int[] CARDINAL_DIRECTIONS = {
        NORTH,
        WEST,
        SOUTH,
        EAST
    };
}

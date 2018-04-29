using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MapManager : MonoBehaviour {
    public int width = 8;
    public int height = 8;
    public GameObject grunt;

    public GameObject floor;
    public GameObject tileMoveHighlight;
    public GameObject tileAttackHighlight;
    public Tile[,] tiles;
    private Unit[,] units;

    private Transform mapHolder;
    private Transform tileHighlightsHolder;
    private Transform unitsHolder;

    public void Initialize () {
        CreateTiles();
        InstantiateTileHighlightsHolder();
        CenterCamera();
        AddUnits();
    }

    private void CreateTiles () {
        mapHolder = new GameObject("Map").transform;
        tiles = new Tile[width, height];

        for (int x = 0; x < width; x += 1) {
            for (int y = 0; y < height; y += 1) {
                GameObject floorInstance =
                    Instantiate(floor, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                tiles[x, y] = floorInstance.GetComponent<Tile>();
                floorInstance.transform.SetParent(mapHolder);
            }
        }
    }

    private void InstantiateTileHighlightsHolder() {
        tileHighlightsHolder = new GameObject("MapHighlights").transform;
    }

    private void CenterCamera () {
        Camera mainCamera = Camera.main;
        mainCamera.orthographicSize = Math.Max(width, height) / 2;
        mainCamera.transform.position = new Vector3(width / 2 - 0.5f, height / 2 - 0.5f, -1);
    }

    private void AddUnits() {
        units = new Unit[width, height];
        unitsHolder = new GameObject("Units").transform;

        AddUnitAt(1, 1);
        AddUnitAt(2, 2);
        AddUnitAt(4, 2);
        AddUnitAt(2, 4);
        AddUnitAt(4, 4);
    }

    private void AddUnitAt(int x, int y) {
        GameObject gruntInstance =
            Instantiate(grunt, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
        gruntInstance.transform.SetParent(unitsHolder);
        units[x, y] = gruntInstance.GetComponent<Unit>();
    }

    public Unit GetUnitAt(Vector2Int pos) {
        return units[pos.x, pos.y];
    }

    public void UpdateUnitPosition(Unit unit, Vector2Int newPos) {
        Vector2Int oldPos = unit.GetPosition();
        units[oldPos.x, oldPos.y] = null;
        units[newPos.x, newPos.y] = unit;
    }

    public void RemoveUnit(Unit unit) {
        Vector2Int pos = unit.GetPosition();
        units[pos.x, pos.y] = null;
    }

    public void ClearHighlights() {
        foreach (Transform mapHighlight in tileHighlightsHolder) {
            Destroy(mapHighlight.gameObject);
        }
    }

    public Dictionary<Vector2Int, List<Vector2Int>> HighlightMovementRange(Unit unit) {
        Dictionary<Vector2Int, List<Vector2Int>> possibleTargets =
            PathUtils.SearchFromPos(unit.GetPosition(), unit.movementRange, pos => {
                if (pos.x < width && pos.x >= 0 && pos.y < height && pos.y >= 0) {
                    return units[pos.x, pos.y] != null;
                }
                else {
                    return false;
                }
            });

        foreach (Vector2Int target in possibleTargets.Keys) {
            AddHighlightToPos(tileMoveHighlight, target.x, target.y);
        }

        return possibleTargets;
    }

    public void HighlightAttackRange(Unit unit) {
        Vector2Int basePos = unit.GetPosition();
        int range = unit.attackRange;
        for (int x = basePos.x - range; x <= basePos.x + range; x += 1) {
            int maxYDistance = range - Math.Abs(x - basePos.x);
            for (int y = basePos.y - maxYDistance; y <= basePos.y + maxYDistance; y += 1) {
                if (x >= 0 && x < width && y >= 0 && y < height && units[x, y] != null && units[x, y] != unit) {
                    AddHighlightToPos(tileAttackHighlight, x, y);
                }
            }
        }
    }

    private void AddHighlightToPos(GameObject highlightPrefab, int x, int y) {
        GameObject highlight =
            Instantiate(highlightPrefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
        highlight.transform.SetParent(tileHighlightsHolder);
    }
}

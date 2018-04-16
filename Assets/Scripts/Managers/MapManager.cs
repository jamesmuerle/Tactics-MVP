using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MapManager : MonoBehaviour {
    public int width = 8;
    public int height = 8;

    public GameObject floor;
    public GameObject tileMoveHighlight;
    public GameObject tileAttackHighlight;
    public Tile[,] tiles;

    private Transform mapHolder;
    private Transform tileHighlightsHolder;

    public void SetupBoard () {
        CreateTiles();
        InstantiateTileHighlightsHolder();
        CenterCamera();
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

    public void ClearHighlights() {
        foreach (Transform mapHighlight in tileHighlightsHolder) {
            Destroy(mapHighlight.gameObject);
        }
    }

    public void HighlightMovementRange(Unit unit) {
        HighlightRangeWithPrefab(tileMoveHighlight, unit.GetPosition(), unit.movementRange);
    }

    public void HighlightAttackRange(Unit unit) {
        HighlightRangeWithPrefab(tileAttackHighlight, unit.GetPosition(), unit.attackRange);
    }

    private void HighlightRangeWithPrefab(GameObject highlightPrefab, Vector2Int basePos, int range) {
        for (int x = basePos.x - range; x <= basePos.x + range; x += 1) {
            int maxYDistance = range - Math.Abs(x - basePos.x);
            for (int y = basePos.y - maxYDistance; y <= basePos.y + maxYDistance; y += 1) {
                if (x >= 0 && x < width && y >= 0 && y < height) {
                    AddHighlightToPos(highlightPrefab, x, y);
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

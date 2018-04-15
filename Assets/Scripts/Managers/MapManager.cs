using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MapManager : MonoBehaviour {
    public int width = 8;
    public int height = 8;

    public GameObject floor;
    public GameObject tileHighlight;
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
        Vector2Int basePos = unit.GetPosition();
        int maxDistance = unit.movementRange;
        for (int x = basePos.x - maxDistance; x <= basePos.x + maxDistance; x += 1) {
            int maxYDistance = maxDistance - Math.Abs(x - basePos.x);
            for (int y = basePos.y - maxYDistance; y <= basePos.y + maxYDistance; y += 1) {
                if (x >= 0 && x < width && y >= 0 && y < height) {
                    AddHighlightToPos(x, y);
                }
            }
        }
    }

    private void AddHighlightToPos(int x, int y) {
        GameObject highlight =
            Instantiate(tileHighlight, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
        highlight.transform.SetParent(tileHighlightsHolder);
    }
}

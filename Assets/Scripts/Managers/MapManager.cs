using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MapManager : MonoBehaviour {
    public int width = 8;
    public int height = 8;

    public GameObject floor;
    public Tile[,] tiles;

    private Transform mapHolder;

    public void SetupBoard () {
        CreateTiles();
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

    private void CenterCamera () {
        Camera mainCamera = Camera.main;
        mainCamera.orthographicSize = Math.Max(width, height) / 2;
        mainCamera.transform.position = new Vector3(width / 2 - 0.5f, height / 2 - 0.5f, -1);
    }
}

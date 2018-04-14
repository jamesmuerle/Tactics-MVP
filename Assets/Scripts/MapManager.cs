using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {
    public int width = 8;
    public int height = 8;

    public GameObject floor;

    private Transform mapHolder;

    public void SetupBoard () {
        mapHolder = new GameObject("Map").transform;
        for (int x = 0; x < width; x += 1) {
            for (int y = 0; y < height; y += 1) {
                GameObject instance =
                    Instantiate(floor, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(mapHolder);
            }
        }
    }
}

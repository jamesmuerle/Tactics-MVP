using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public MapManager mapManager;


	// Use this for initialization
	void Awake () {
        mapManager = GetComponent<MapManager>();
        InitGame();
	}

    private void InitGame () {
        mapManager.SetupBoard();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

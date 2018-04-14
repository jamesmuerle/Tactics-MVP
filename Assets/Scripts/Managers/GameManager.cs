﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public MapManager mapManager;

	void Awake () {
        mapManager = GetComponent<MapManager>();
        InitGame();
	}

    private void InitGame () {
        mapManager.SetupBoard();
    }
}
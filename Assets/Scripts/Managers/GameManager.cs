using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public MapManager mapManager;
    public UnitsManager unitsManager;

	void Awake () {
        InitGame();
	}

    private void InitGame () {
        mapManager.SetupBoard();
        unitsManager.SetupUnits();
    }
}

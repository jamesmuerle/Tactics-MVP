using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsManager : MonoBehaviour {
    public MapManager mapManager;
    public GameObject grunt;
    public GameObject pathContainer;

    private Transform unitsHolder;
    private Unit[,] units;

    private Unit selectedUnit;
    private PathContainer path;

    public void SetupUnits () {
        AddUnits();
        InitializePath();
        BindToMapEvents();
    }

    private void AddUnits() {
        unitsHolder = new GameObject("Units").transform;
        units = new Unit[mapManager.width, mapManager.height];

        AddUnitAt(4, 4);
        AddUnitAt(3, 4);
        AddUnitAt(2, 4);
        AddUnitAt(7, 5);
        AddUnitAt(6, 3);
    }

    private void AddUnitAt(int x, int y) {
        GameObject gruntInstance =
            Instantiate(grunt, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
        gruntInstance.transform.SetParent(unitsHolder);
        units[x, y] = gruntInstance.GetComponent<Unit>();
    }

    private void InitializePath() {
        path = Instantiate(pathContainer, Vector3.zero, Quaternion.identity)
            .GetComponent<PathContainer>();
    }

    private void BindToMapEvents() {
        Tile[,] tiles = mapManager.tiles;
        for (int x = 0; x < tiles.GetLength(0); x += 1) {
            for (int y = 0; y < tiles.GetLength(1); y += 1) {
                tiles[x, y].posClicked.AddListener(HandlePosClicked);
                tiles[x, y].posEntered.AddListener(HandlePosEntered);
                tiles[x, y].posExited.AddListener(HandlePosExited);
            }
        }
    }

    private void HandlePosClicked(int x, int y) {
        Unit unitAtPos = units[x, y];
        if (unitAtPos) {
            HandleUnitClicked(unitAtPos);
        }
        else if (selectedUnit) {
            HandleTileClicked(new Vector2Int(x, y));
        }
    }

    private void HandleUnitClicked(Unit clickedUnit) {
        if (clickedUnit == selectedUnit) {
            DeselectUnit();
        }
        else {
            if (selectedUnit) {
                DeselectUnit();
            }

            SelectUnit(clickedUnit);
            path.SetSourceUnit(clickedUnit);
        }
    }

    private void HandleTileClicked(Vector2Int clickedPosition) {
        Vector2Int oldPos = selectedUnit.GetPosition();
        Vector2Int newPos = path.GetTarget();
        if (newPos == clickedPosition) {
            selectedUnit.MoveThroughPath(path.GetPath());
            units[oldPos.x, oldPos.y] = null;
            units[newPos.x, newPos.y] = selectedUnit;
        }
        DeselectUnit();
    }

    private void DeselectUnit() {
        selectedUnit.isSelected = false;
        selectedUnit = null;
        path.Empty();
        mapManager.ClearHighlights();
    }

    private void SelectUnit(Unit unitToSelect) {
        unitToSelect.isSelected = true;
        unitToSelect.isMoving = true;
        selectedUnit = unitToSelect;
        mapManager.HighlightMovementRange(unitToSelect);
    }

    private void HandlePosEntered(int x, int y) {
        Unit unitAtPos = units[x, y];
        if (unitAtPos) {
            HandleUnitEntered(unitAtPos);
        }
        if (selectedUnit) {
            path.GoToPosition(new Vector2Int(x, y));
        }
    }

    private void HandleUnitEntered(Unit enteredUnit) {
        enteredUnit.isHovered = true;
    }

    private void HandlePosExited(int x, int y) {
        Unit unitAtPos = units[x, y];
        if (unitAtPos) {
            HandleUnitExited(unitAtPos);
        }
    }

    private void HandleUnitExited(Unit exitedUnit) {
        exitedUnit.isHovered = false;
    }
}

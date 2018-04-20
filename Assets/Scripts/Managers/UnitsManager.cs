using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsManager : MonoBehaviour {
    public MapManager mapManager;
    public GameObject pathContainer;

    private Unit selectedUnit;
    private PathContainer path;

    public void Initialize () {
        InitializePath();
        BindToMapEvents();
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
                tiles[x, y].posRightClicked.AddListener(HandlePosRightClicked);
            }
        }
    }

    private void HandlePosClicked(Vector2Int pos) {
        Unit unitAtPos = mapManager.GetUnitAt(pos);
        if (unitAtPos) {
            HandleUnitClicked(unitAtPos);
        }
        else {
            HandleTileClicked(pos);
        }
    }

    private void HandleUnitClicked(Unit clickedUnit) {
        if (clickedUnit == selectedUnit) {
            if (selectedUnit.hasMoved) {
                selectedUnit.isAttacking = false;
                DeselectUnit();
            }
            else {
                selectedUnit.hasMoved = true;
                selectedUnit.isMoving = false;
                SelectUnitForAttacking(selectedUnit);
            }
        }
        else if (selectedUnit && selectedUnit.hasMoved && !selectedUnit.hasAttacked) {
            if (selectedUnit.IsUnitInAttackRange(clickedUnit)) {
                selectedUnit.ExecuteAttackOn(clickedUnit);
                selectedUnit.isDone = true;
                selectedUnit.isMoving = false;
                DeselectUnit();
            }
            else {
                selectedUnit.isMoving = false;
                selectedUnit.isAttacking = false;
                DeselectUnit();
                SelectNewUnit(clickedUnit);
            }
        }
        else {
            SelectNewUnit(clickedUnit);
        }
    }

    private void HandleTileClicked(Vector2Int clickedPosition) {
        if (selectedUnit) {
            if (!selectedUnit.hasMoved) {
                Vector2Int newPos = path.GetTarget();
                if (newPos == clickedPosition) {
                    mapManager.UpdateUnitPosition(selectedUnit, newPos);
                    selectedUnit.MoveThroughPath(path.GetPath());
                    ClearMovement();
                    SelectUnitForAttacking(selectedUnit);
                }
                else {
                    selectedUnit.isMoving = false;
                    selectedUnit.isAttacking = false;
                    DeselectUnit();
                }
            }
            else {
                selectedUnit.isMoving = false;
                DeselectUnit();
            }
        }
    }

    private void DeselectUnit() {
        selectedUnit.isSelected = false;
        selectedUnit = null;
        ClearMovement();
    }

    private void ClearMovement() {
        path.Empty();
        mapManager.ClearHighlights();
    }

    private void SelectNewUnit(Unit unitToSelect) {
        if (!unitToSelect.hasMoved) {
            SelectUnitForMoving(unitToSelect);
        }
        else if (!unitToSelect.hasAttacked) {
            SelectUnitForAttacking(unitToSelect);
        }
    }

    private void SelectUnitForMoving(Unit unitToSelect) {
        unitToSelect.isSelected = true;
        unitToSelect.isMoving = true;
        selectedUnit = unitToSelect;
        mapManager.HighlightMovementRange(unitToSelect);
        path.SetSourceUnit(unitToSelect);
    }

    private void SelectUnitForAttacking(Unit unitToSelect) {
        unitToSelect.isSelected = true;
        unitToSelect.isAttacking = true;
        selectedUnit = unitToSelect;
        mapManager.HighlightAttackRange(unitToSelect);
    }

    private void HandlePosEntered(Vector2Int pos) {
        if (selectedUnit && !selectedUnit.hasMoved) {
            path.GoToPosition(pos);
        }

        Unit unitAtPos = mapManager.GetUnitAt(pos);
        if (unitAtPos) {
            HandleUnitEntered(unitAtPos);
        }
        else {
            HandleTileEntered(pos);
        }
    }

    private void HandleUnitEntered(Unit enteredUnit) {
        path.SetInvisible();
        enteredUnit.isHovered = true;
    }

    private void HandleTileEntered(Vector2Int pos) {
        path.SetVisible();
    }

    private void HandlePosExited(Vector2Int pos) {
        Unit unitAtPos = mapManager.GetUnitAt(pos);
        if (unitAtPos) {
            HandleUnitExited(unitAtPos);
        }
    }

    private void HandleUnitExited(Unit exitedUnit) {
        exitedUnit.isHovered = false;
    }

    private void HandlePosRightClicked(Vector2Int pos) {
        if (selectedUnit) {
            selectedUnit.isMoving = false;
            selectedUnit.isAttacking = false;
            DeselectUnit();
        }
    }
}

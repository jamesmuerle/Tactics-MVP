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

    /**
     * Handle a left mouse click on a position.
     */
    private void HandlePosClicked(Vector2Int pos) {
        Unit unitAtPos = mapManager.GetUnitAt(pos);
        if (selectedUnit && unitAtPos) {
            PerformActionWithUnitOnUnit(selectedUnit, unitAtPos);
        }
        else if (selectedUnit) {
            PerformActionWithUnitOnTile(selectedUnit, pos);
        }
        else if (unitAtPos) {
            SelectUnit(unitAtPos);
        }
        else {
            DeselectUnit();
        }
    }

    private void PerformActionWithUnitOnUnit(Unit sourceUnit, Unit targetUnit) {
        if (!sourceUnit.hasAttacked && sourceUnit.IsUnitInAttackRange(targetUnit)) {
            if (!sourceUnit.isMoving) {
                sourceUnit.ExecuteAttackOn(targetUnit);
                if (targetUnit.isDead) {
                    mapManager.RemoveUnit(targetUnit);
                }
                DeselectUnit();
            }
        }
        else if (sourceUnit == targetUnit && !sourceUnit.hasMoved) {
            MoveUnitTo(sourceUnit, sourceUnit.GetPosition());
        }
        else {
            DeselectUnit();
            SelectUnit(targetUnit);
        }
    }

    private void PerformActionWithUnitOnTile(Unit sourceUnit, Vector2Int targetPos) {
        if (!sourceUnit.hasMoved && path.GetTarget() == targetPos) {
            MoveUnitTo(sourceUnit, targetPos);
        }
        else {
            DeselectUnit();
        }
    }

    private void MoveUnitTo(Unit sourceUnit, Vector2Int targetPos) {
        mapManager.UpdateUnitPosition(selectedUnit, targetPos);
        selectedUnit.MoveThroughPath(path.GetPath());
        path.Empty();
        SelectUnitForAttacking(selectedUnit);
    }

    /**
     * Helper methods for unit selection.
     */
    private void SelectUnit(Unit targetUnit) {
        if (!targetUnit.hasMoved) {
            SelectUnitForMoving(targetUnit);
        }
        else if (!targetUnit.hasAttacked) {
            SelectUnitForAttacking(targetUnit);
        }
    }

    private void SelectUnitForMoving(Unit targetUnit) {
        targetUnit.isSelected = true;
        selectedUnit = targetUnit;
        mapManager.ClearHighlights();
        mapManager.HighlightMovementRange(targetUnit);
        mapManager.HighlightAttackRange(targetUnit);
        path.SetSourceUnit(targetUnit);
    }

    private void SelectUnitForAttacking(Unit targetUnit) {
        targetUnit.isSelected = true;
        selectedUnit = targetUnit;
        mapManager.ClearHighlights();
        mapManager.HighlightAttackRange(targetUnit);
    }

    private void DeselectUnit() {
        if (selectedUnit) {
            selectedUnit.isSelected = false;
            selectedUnit = null;
            path.Empty();
            mapManager.ClearHighlights();
        }
    }

    /**
     * Handle the mouse entering a position.
     */
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

    /**
     * Handling the mouse exiting from a position.
     */
    private void HandlePosExited(Vector2Int pos) {
        Unit unitAtPos = mapManager.GetUnitAt(pos);
        if (unitAtPos) {
            HandleUnitExited(unitAtPos);
        }
    }

    private void HandleUnitExited(Unit exitedUnit) {
        exitedUnit.isHovered = false;
    }

    /**
     * Handle a right mouse click on a position.
     */
    private void HandlePosRightClicked(Vector2Int pos) {
        DeselectUnit();
    }
}

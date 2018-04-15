using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsManager : MonoBehaviour {
    public MapManager mapManager;
    public GameObject grunt;

    private Transform unitsHolder;
    private Unit selectedUnit;

    public void SetupUnits () {
        AddUnits();
    }

    private void AddUnits() {
        unitsHolder = new GameObject("Units").transform;

        for (int x = 0; x < mapManager.width; x += 1) {
            for (int y = 0; y < mapManager.height; y += 1) {
                GameObject gruntInstance =
                    Instantiate(grunt, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                gruntInstance.transform.SetParent(unitsHolder);
                gruntInstance.GetComponent<Unit>().unitClicked.AddListener(HandleUnitClicked);
            }
        }
    }

    private void HandleUnitClicked(Unit clickedUnit) {
        if (clickedUnit == selectedUnit) {
            clickedUnit.isSelected = false;
            selectedUnit = null;
        }
        else {
            if (selectedUnit) {
                selectedUnit.isSelected = false;
            }

            clickedUnit.isSelected = true;
            selectedUnit = clickedUnit;
        }
    }
}

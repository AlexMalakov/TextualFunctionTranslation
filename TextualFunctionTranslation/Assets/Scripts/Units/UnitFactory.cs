using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class UnitBlueprint {
    public string unitName { get; private set; }
    public int unitCost { get; private set; }
    public Sprite unitIcon { get; private set; }
    public GameObject unitPrefab { get; private set; } //object with unit property
}

public class UnitFactory : MonoBehaviour
{
    [SerializeField] private List<UnitBlueprint> curUnits;

    static List<UnitBlueprint> units;
    public void Awake() {
        units = this.curUnits;
    }


    public static Unit createUnit(string unitName) {
        foreach(UnitBlueprint up in units) {
            if(up.unitName == unitName) {
                return Instantiate(up.unitPrefab).GetComponent<Unit>().setBlueprint(up);
            }
        }

        throw new Exception("UNIT WITH NAME " + unitName + " DOES NOT EXIST!");
    }

    public static UnitBlueprint lookupBlueprint(string unitName) {
        foreach(UnitBlueprint up in units) {
            if(up.unitName == unitName) {
                return up;
            }
        }

        throw new Exception("UNIT WITH NAME " + unitName + " DOES NOT EXIST!");
    }
}

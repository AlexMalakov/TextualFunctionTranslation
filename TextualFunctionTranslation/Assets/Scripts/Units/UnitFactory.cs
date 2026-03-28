using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class UnitBlueprint {
    public string unitName;
    public int unitCost;
    public Sprite unitIcon;
    public GameObject unitPrefab; //object with unit property
}

public class UnitFactory : MonoBehaviour
{
    [SerializeField] private List<UnitBlueprint> curUnits;
    [SerializeField] private CombatManager cm;
    private static CombatManager com;

    static List<UnitBlueprint> units;
    public void Awake() {
        units = this.curUnits;
        com = cm;
    }


    public static Unit createUnit(string unitName) {
        foreach(UnitBlueprint up in units) {
            if(up.unitName == unitName) {
                return Instantiate(up.unitPrefab).GetComponent<Unit>().setBlueprint(up, com);
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

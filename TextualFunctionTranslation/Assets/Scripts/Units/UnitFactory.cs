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
    [SerializeField] private CombatLog log;
    [SerializeField] private ParticleManager pm;
    private static CombatManager com;
    private static CombatLog logg;
    private static ParticleManager pmm;

    static List<UnitBlueprint> units;

    public void Awake() { //Awake because this needs to run first
        units = this.curUnits;
        com = cm;
        logg = log;
        pmm = pm;
    }

    public static List<UnitBlueprint> getAllBlueprints() {
        return units;
    }


    public static Unit createUnit(string unitName) {
        foreach(UnitBlueprint up in units) {
            if(up.unitName == unitName) {
                return Instantiate(up.unitPrefab).GetComponent<Unit>().setBlueprint(up, com, logg, pmm);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class UnitPrefabs {
    public string unitName;
    public GameObject unitPrefab;
}

public class UnitFactory : MonoBehaviour
{
    [SerializeField] private List<UnitPrefabs> units;
    


    public Unit createUnit(string unitName) {
        foreach(UnitPrefabs up in this.units) {
            if(up.unitName == unitName) {
                return Instantiate(up.unitPrefab).GetComponent<Unit>();
            }
        }

        throw new Exception("UNIT WITH NAME " + unitName + " DOES NOT EXIST!");
    }
}

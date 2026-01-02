using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitPrefabs {
    public string unitName;
    public GameObject unitPrefab;

    public Unit getUnit() {
        return Instantiate(unitPrefab).GetComponent<Unit>();
    }
}

public class UnitFactory : MonoBehaviour
{
    [SerializeField] private List<UnitPrefabs> units;
    


    public Unit createUnit(string unitName) {
        foreach(UnitPrefabs up in this.units) {
            if(up.unitName == unitName) {
                return up.getUnit();
            }
        }

        throw new Exception("UNIT WITH NAME " + unitName + " DOES NOT EXIST!");
    }
}

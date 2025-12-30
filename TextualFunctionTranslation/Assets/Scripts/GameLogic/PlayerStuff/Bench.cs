using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bench : MonoBehaviour
{
    Unit[] benchedUnits;
    [SerializeField] private int benchSize;

    void Awake() {
        benchedUnits = new Unit[this.benchSize];
    }


    public bool canAddUnitToBench() {
        foreach(Unit u in this.benchedUnits) {
            if(u == null) {
                return true;
            }
        }
        return false;
    }

    public void addUnitToBench(Unit u) {
        if(!canAddUnitToBench) {
            throw new Exception("BENCH IS FULL!!!");
        }

        for(int i = 0; i < this.benchedUnits.Length; i++) {
            if(this.benchedUnits[i] == null) {
                this.benchedUnits[i] = u;
                break;
            }
        }
    }

    public void removeUnitFromBench(Unit u) {
        for(int i = 0; i < this.benchedUnits.Length; i++) {
            if(this.benchedUnits[i] == u) {
                this.benchedUnits[i] = null;
                return;
            }
        }

        throw new Exception("Unit not in bench!!!");
    }


}

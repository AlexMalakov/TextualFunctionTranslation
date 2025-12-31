using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BenchSlot : MonoBehaviour
{
    private Unit unit;
    private int slotNum;

    public void init(int slotNum) {
        this.slotNum = slotNum;
    }

    public bool isOccupied() {
        return this.unit != null;
    }

    public Unit getUnit() {
        return this.unit;
    }

    public void removeUnit() {
        this.unit = null;
    }

    public int insertUnit(Unit u) {
        this.unit = u;
        return this.slotNum;
    }

    public int getSlotNum() {
        return this.slotNum;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BenchSlot : MonoBehaviour
{
    [SerializeField] private GameObject blinkMe;

    private Unit unit;
    private int slotNum;
    private bool flashing;

    private Unit unitBackup;

    public void saveState() {
        this.unitBackup = this.unit;
    }

    public void backupState() {
        this.unit = this.unitBackup;
        if(this.unit != null) {
            this.unit.placeUnitOnBench();
            this.unit.transform.position = this.transform.position;
        }
    }

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

    public BenchSlot insertUnit(Unit u) {
        this.unit = u;
        u.placeUnitOnBench();
        return this;
    }

    public int getSlotNum() {
        return this.slotNum;
    }

    public void tutorialFlash(bool flashOn) {
        this.flashing = flashOn;
        if(flashOn) {
            this.flashOn();
        } else {
            this.flashOff();
        }
    }

    private void flashOn() {
        if(!this.flashing) {
            return;
        }
        this.blinkMe.SetActive(true);
        Invoke(nameof(flashOff), .5f);
    }

    private void flashOff() {
        this.blinkMe.SetActive(false);

        if(this.flashing) {
            Invoke(nameof(flashOn), .5f);
        }
    }

}

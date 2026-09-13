using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BenchSlot : MonoBehaviour
{
    [SerializeField] private Renderer render;
    private Unit unit;
    private int slotNum;
    private bool flashing;
    private Color defaultColor;

    public void init(int slotNum) {
        this.slotNum = slotNum;
        this.defaultColor = this.render.material.color;
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
        this.render.material.color = new Color(255f, 0f, 0f);
        Invoke(nameof(flashOff), .5f);
    }

    private void flashOff() {
        this.GetComponent<Renderer>().material.color = this.defaultColor;

        if(this.flashing) {
            Invoke(nameof(flashOn), .5f);
        }
    }

}

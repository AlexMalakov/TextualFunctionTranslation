using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Bench : MonoBehaviour
{
    [SerializeField] private List<BenchSlot> benchSlotPositions;

    private List<Unit> draggedUnits = new List<Unit>();

    void Awake() {
        for(int i = 0; i < this.benchSlotPositions.Count; i++) {
            this.benchSlotPositions[i].init(i);
        }
    }

    public bool canAddUnitToBench() {
        foreach(BenchSlot b in this.benchSlotPositions) {
            if(!b.isOccupied()) {
                return true;
            }
        }
        return false;
    }

    public int addUnitToBench(Unit u) { //TODO either spawn unit in bench or move unit into bench
        foreach(BenchSlot b in this.benchSlotPositions) {
            if(!b.isOccupied()) {
                return b.insertUnit(u);
            }
        }

        throw new Exception("BENCH IS FULL!!!");
    }

    public void removeUnitFromBench(Unit u) {
        foreach(BenchSlot b in this.benchSlotPositions) {
            if(b.getUnit() == u) {
                b.removeUnit();
                return;
            }
        }

        throw new Exception("Unit not in bench!!!");
    }

    public IEnumerator dragUnitToBench(Unit u, int dest) {
        if(this.draggedUnits.Contains(u)) { //is this possible? {
            yield break;
        }
        this.draggedUnits.Add(u);

        Vector3 startPos = u.transform.position;
        Vector3 endPos = this.benchSlotPositions[dest].transform.position;

        float elapsed = 0f;
        float totalDur = (endPos - startPos).magnitude / this.player.getUnitDragSpeed();

        while(elapsed < totalDur) {

            if (!this.draggedUnits.Contains(u)) {
                yield break;
            }

            u.transform.position = Vector3.Lerp(startPos, endPos, elapsed/totalDur);
            elapsed += Time.time;
            yield return null;
        }

        this.draggedUnits.Remove(u);
    }

    public void unitClickedOn(Unit u) {
        this.draggedUnits.Remove(u);
    }

    //TODO: might want to be able to organize bench although it doesn't really matter...
    //public void updateBenchLayout(){}.....
}

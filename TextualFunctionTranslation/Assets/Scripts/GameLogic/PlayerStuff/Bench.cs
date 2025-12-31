using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Bench : MonoBehaviour
{
    Unit[] benchedUnits;
    [SerializeField] private int benchSize;
    [SerializeField] private Transform benchStart;
    [SerializeField] private float benchSlotOffset;

    private List<Unit> draggedUnits = new List<Unit>();

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

    public int addUnitToBench(Unit u) { //TODO either spawn unit in bench or move unit into bench
        if(!canAddUnitToBench()) {
            throw new Exception("BENCH IS FULL!!!");
        }

        for(int i = 0; i < this.benchedUnits.Length; i++) {
            if(this.benchedUnits[i] == null) {
                this.benchedUnits[i] = u;
                return i;
            }
        }
        return -1;
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

    public IEnumerator dragUnitToBench(Unit u, int dest) {
        if(this.draggedUnits.Contains(u)) { //is this possible? {
            yield break;
        }
        this.draggedUnits.Add(u);

        Vector3 startPos = u.transform.position;
        Vector3 endPos = new Vector3(this.benchStart.x, this.benchStart.y, this.benchStart.z + dest * this.benchSlotOffset);

        float elapsed = 0f;
        float totalDur = (endPos - startPos).magnitude / this.player.getUnitDragSpeed();

        while(elapsed < totalDur) {

            if (!this.draggedUnits.Contains(u)) {
                yield break;
            }

            u1.transform.position = Vector3.Lerp(startPos, endPos, elapsed/totalDur);
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

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Bench : MonoBehaviour
{
    Unit[] benchedUnits;
    [SerializeField] private int benchSize;

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

    public void addUnitToBench(Unit u) { //TODO either spawn unit in bench or move unit into bench
        if(!canAddUnitToBench()) {
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

    public IEnumerator dragUnitToBench(Unit u) {
        if(this.draggedUnits.Contains(u1)) { //is this possible? {
            yield break;
        }
        this.draggedUnits.Add(u1);

        Vector3 startPos = u1.transform.position;
        Vector3 endPos = Vector3.zero; //TODO: find the spot in the bench im going into

        float elapsed = 0f;
        float totalDur = (endPos - startPos).magnitude / this.player.getUnitDragSpeed();

        while(elapsed < totalDur) {

            if (!this.draggedUnits.Contains(u1)) {
                yield break;
            }

            u1.transform.position = Vector3.Lerp(startPos, endPos, elapsed/totalDur);
            elapsed += Time.time;
            yield return null;
        }

        this.draggedUnits.Remove(u1);
    }

    public void unitClickedOn(Unit u) {
        this.draggedUnits.Remove(u)
    }

    //TODO: might want to be able to organize bench although it doesn't really matter...

    // //return false if fails
    // //this is ugly as shit but womp womp
    // public bool attemptUpdateBench(Unit u, HexSquare square) {
    //     //TODO: check if unit and square can be used for layout
    //     //TODO: move units to new positions
    //     if(this.layout.ContainsKey(u)) {
    //         if(square.getUnit() != null) {
    //             Unit swapper = square.getUnit();
    //             this.layout[swapper] = this.layout[unit];
    //             swapper.placeUnitOnSquare(this.layout[unit]);
    //         } 
    //         this.layout[u] = square;
    //     } else {
    //         if(this.player.getMaxCombatUnits() <= this.layout.Count && (square.getUnit() == null || !this.player.getBench().canAddUnitToBench())) {
    //             return false;//TODO: move unit back to the bench
    //         }

    //         if(square.getUnit() != null) {
    //             Unit swapper = square.getUnit();
    //             this.layout.Remove(swapper);
    //             this.player.getBench.addUnitToBench(swapper);
    //         }
    //         this.layout.Add(u, square);
    //     }
    //     u.placeUnitOnSquare(square);
    //     return true;
    // }


}

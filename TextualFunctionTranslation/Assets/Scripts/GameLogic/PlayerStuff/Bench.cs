using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Bench : MonoBehaviour
{
    [SerializeField] private List<BenchSlot> benchSlotPositions;
    [SerializeField] private HumanPlayer player;

    void Awake() { //Awake because this doesn't affect other systems
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

    public BenchSlot addUnitToBench(Unit u) { //TO CHECK: unit always gets moved in to bench (as of writing this comment this is true)
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
    }

    public BenchSlot unitInBench(Unit u) {
        foreach(BenchSlot b in this.benchSlotPositions) {
            if(b.getUnit() == u) {
                return b;
            }
        }

        return null;
    }

    public bool updateBenchLayout(Unit u, int slotNum) {
        if(!this.player.ownsUnit(u)) {
            return false;
        }
        
        BenchSlot benchPos = this.unitInBench(u);
        if(benchPos) {
            if(this.benchSlotPositions[slotNum].isOccupied()) { //swap to units on the bench
                Debug.Log("BENCH LAYOUT UPDATE CASE INTERNAL SWAP");
                Unit swapper = this.benchSlotPositions[slotNum].getUnit();
                benchPos.insertUnit(swapper);
                StartCoroutine(this.player.getUnitMoveManager().dragUnitTo(swapper, benchPos.transform, false));
            } else {
                Debug.Log("BENCH LAYOUT UPDATE CASE SHIFT");
                benchPos.removeUnit();
            }
        } else {
            if(!this.canAddUnitToBench() && u.getPosition() == null) { //double check if position == null or positions == NULLSQUARE !!!!!!
                //only fail condition: bench is full and the unit didn't come from the board or bench (so it came from the shop)
                return false;
            }

            if(this.benchSlotPositions[slotNum].isOccupied()) {
                Debug.Log("BENCH LAYOUT UPDATE CASE EXTERNAL SWAP");
                Unit swapper = this.benchSlotPositions[slotNum].getUnit();
                HexSquare uPos = u.getPosition();
                u.unclaimSquare();
                this.player.getUnitLayout().addUnitFromBench(swapper, uPos);
                // swapper.placeUnitOnSquare(uPos, false);
                // StartCoroutine(this.player.getUnitMoveManager().dragUnitTo(swapper, uPos.transform, false));
            } else {
                Debug.Log("BENCH LAYOUT UPDATE CASE EXTERNAL");
            }

            this.player.getUnitLayout().removeUnitFromLayout(u);
            
        }
        this.benchSlotPositions[slotNum].insertUnit(u);
        StartCoroutine(this.player.getUnitMoveManager().dragUnitTo(u, this.benchSlotPositions[slotNum].transform, true));
        return true;
    }

    public bool hasAnotherStarLevel(Unit notMe, string unitName, int starLevel) {
        return this.getAnotherStarLevel(notMe, unitName, starLevel) != null;
    }

    public Unit getAnotherStarLevel(Unit notMe, string unitName, int starLevel) {
        foreach(BenchSlot b in this.benchSlotPositions) {
            if(b.getUnit() && b.getUnit() != notMe && b.getUnit().getName() == unitName  && b.getUnit().getMergeLevel() == starLevel) {
                return b.getUnit();
            }
        }
        return null;
    }


    public string jsonify() {
        return "";
    }

}

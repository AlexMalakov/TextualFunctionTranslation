using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Bench : MonoBehaviour
{
    [SerializeField] private List<BenchSlot> benchSlotPositions;
    [SerializeField] private Player player;

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

    public BenchSlot addUnitToBench(Unit u) { //TODO either spawn unit in bench or move unit into bench
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
                Unit swapper = this.benchSlotPositions[slotNum].getUnit();
                benchPos.insertUnit(swapper);
                StartCoroutine(this.player.getUnitMoveManager().dragUnitTo(swapper, benchPos.transform, false));
            } else {
                benchPos.removeUnit();
            }
        } else {
            if(!this.canAddUnitToBench() && u.getPosition() == null) { //double check if position == null or positions == NULLSQUARE !!!!!!
                //only fail condition: bench is full and the unit didn't come from the board or bench (so it came from the shop)
                return false;
            }

            if(this.benchSlotPositions[slotNum].isOccupied()) {
                Unit swapper = this.benchSlotPositions[slotNum].getUnit();
                HexSquare uPos = u.getPosition();
                u.unclaimSquare();
                swapper.placeUnitOnSquare(uPos);
                StartCoroutine(this.player.getUnitMoveManager().dragUnitTo(swapper, uPos.transform, false));
            }
            
        }
        this.benchSlotPositions[slotNum].insertUnit(u);
        this.player.getUnitLayout().removeUnitFromLayout(u);
        StartCoroutine(this.player.getUnitMoveManager().dragUnitTo(u, this.benchSlotPositions[slotNum].transform, true));
        return true;
    }


    public string jsonify() {
        return "";
    }

}

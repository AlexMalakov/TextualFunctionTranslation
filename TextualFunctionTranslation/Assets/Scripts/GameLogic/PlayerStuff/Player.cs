using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Player : MonoBehaviour
{
    [SerializeField] private List<Unit> playersUnits;
    [SerializeField] private PlayerUnitLayout unitLayout;
    [SerializeField] private Bench bench;
    [SerializeField] private UnitMovementManager umm;

    [SerializeField] private int unitCap; //TODO: upadte this based on the current round
    [SerializeField] private int money = 0;

    public void init() {
        this.unitLayout.initLayout();
    }

    public bool canPurchaseUnit(string unitName, int unitCost) {
        return this.money >= unitCost && (this.bench.canAddUnitToBench() || this.unitLayout.getActiveUnits().Count < this.unitCap || this.canMergeUnit(unitName));
    }

    public void purchaseUnit(Unit u) { //TODO: Make sure unit actually arrives where it needs to
        this.money -= u.getCost();
        this.playersUnits.Add(u);

        if(this.canMergeUnit(u.getName())) {
            //don't need to move unit because it will get exploded by the merge
            this.mergeUnit(u, 1);
        }
        else if(this.bench.canAddUnitToBench()) {
            BenchSlot unitSlot = this.bench.addUnitToBench(u); //throws exception if it fails but shouldnt since im checking for room first
            StartCoroutine(this.getUnitMoveManager().dragUnitTo(u, unitSlot.transform, false));
        } else {
            //don't need to? it gets auto dragged in
            this.unitLayout.addUnitFromShop(u);
        }
    }

    public bool canMergeUnit(string unitName) {
        return this.bench.hasStarLevel(unitName, 1) || this.unitLayout.hasStarLevel(unitName, 1);
    }

    //recursively merge until can no longer merge any longer
    private void mergeUnit(Unit u, int starLevel) {
        Unit mergeTo;
        if(this.bench.hasStarLevel(u.getName(), starLevel)) {
            mergeTo = this.bench.getStarLevel(u.getName(), starLevel);
        } else if(this.unitLayout.hasStarLevel(u.getName(), starLevel)){
            mergeTo = this.unitLayout.getStarLevel(u.getName(), starLevel);
            
        } else {
            return;
        }
        this.killUnitInMerge(u, mergeTo);
        mergeTo.mergeUnit();
        this.mergeUnit(mergeTo, ++starLevel);
    }

    private IEnumerator killUnitInMerge(Unit preMerge, Unit postMerge) {
        yield return this.umm.dragUnitTo(preMerge, postMerge.transform.position, false);
        Destroy(preMerge);
    }

    public void sellUnit(Unit u) {
        if(!this.playersUnits.Contains(u)) {
            throw new Exception("cannot sell unit you do not own");
        }

        this.money += u.getCost() - 1;
        this.playersUnits.Remove(u);
        this.bench.removeUnitFromBench(u);
        this.unitLayout.removeUnitFromLayout(u);
        Destroy(u.gameObject);
    }

    public List<Unit> getActiveUnits() {
        return this.unitLayout.getActiveUnits();
    }

    public PlayerUnitLayout getUnitLayout() {
        return this.unitLayout;
    }

    public Bench getBench() {
        return this.bench;
    }

    public UnitMovementManager getUnitMoveManager() {
        return this.umm;
    }

    public bool ownsUnit(Unit u) {
        return this.playersUnits.Contains(u);
    }

    //returns true if a unit is in the combat layout
    public bool unitIsActive(Unit u) {
        return this.unitLayout.getActiveUnits().Contains(u);
    }

    public int getMaxCombatUnits() {
        return this.unitCap;
    }
}

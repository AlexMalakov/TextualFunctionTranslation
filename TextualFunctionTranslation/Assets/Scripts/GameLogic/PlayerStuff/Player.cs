using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private List<Unit> playersUnits;
    [SerializeField] private PlayerUnitLayout unitLayout;
    [SerializeField] private Bench bench;
    [SerializeField] private UnitMovementManager umm;

    [SerializeField] private int unitCap; //TODO: upadte this based on the current round
    private int money = 0;

    public void init() {
        this.unitLayout.initLayout();
    }

    public bool canPurchaseUnit(int unitCost) {
        return this.money >= unitCost && (this.bench.canAddUnitToBench() || this.unitLayout.getActiveUnits() < this.unitCap);
    }

    public void purchaseUnit(Unit u) {
        this.money -= u.getCost();
        this.playersUnits.Add(u);

        if(this.bench.canAddUnitToBench()) {
            this.bench.addUnitToBench(); //throws exception if it fails but shouldnt since im checking for room first
        } else {
            this.layout.addUnitFromShop();
        }
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

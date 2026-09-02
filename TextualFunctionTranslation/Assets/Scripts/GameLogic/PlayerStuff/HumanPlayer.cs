using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HumanPlayer : Player //player but has money and shit
{
    [SerializeField] private Bench bench;
    [SerializeField] private UnitMovementManager umm;
    [SerializeField] private GameManager gm;

    [SerializeField] private int unitCap;
    [SerializeField] private int money = 0;
    [SerializeField] private PlayerInfo pi;
    [SerializeField] private int lenseCount;

    [SerializeField] private Shop shop;

    private List<Unit> unitsCopy;

    private int prevMoney = 0;
    private int prevShopIndex = 0;


    public virtual void init() {
        base.init();
        this.unitsCopy = new List<Unit>(this.playersUnits);
        this.pi.setDollars(this.money);
        this.prevMoney = this.money;
        this.prevShopIndex = this.shop.getShopIndex();
        this.lenseCount = 0;
    }

    public void setUnitCap(int cap) {
        this.unitCap = cap; //TODO: is it possible to exceed unit cap?
    }

    public int getMoney() {
        return this.money;
    }

    public void gainMoney(int payday) {
        this.money += payday;
        this.pi.setDollars(this.money);
    }

    public bool canPurchaseUnit(string unitName, int unitCost) {
        return this.money >= unitCost && (
            (this.gm.canModifyBoard() 
                && (this.bench.canAddUnitToBench() 
                    || this.unitLayout.getActiveUnits().Count < this.unitCap 
                    || this.canMergeUnit(unitName)
                )
            ) || (!this.gm.canModifyBoard()
                &&  this.hasBenchSpace(unitName)
            )
        );
    }

    public bool hasBenchSpace(string unitName) {
        return this.bench.canAddUnitToBench() || this.bench.hasStarLevel(unitName, 1);
    }

    public void purchaseUnit(Unit u) { //TODO: Make sure unit actually arrives where it needs to
        this.money -= u.getCost();
        this.pi.setDollars(this.money);
        this.gainUnit(u);
    }

    public void gainUnit(Unit u) {
        this.playersUnits.Add(u);

        if(this.canMergeUnit(u.getName())) {
            //don't need to move unit because it will get exploded by the merge
            this.mergeUnit(u, 1);
        }
        else if(this.bench.canAddUnitToBench()) {
            BenchSlot unitSlot = this.bench.addUnitToBench(u); //throws exception if it fails but shouldnt since im checking for room first
            u.rotateTo(this.getUnitRotation(true));
            StartCoroutine(this.getUnitMoveManager().dragUnitTo(u, unitSlot.transform, false));
        } else {
            //don't need to? it gets auto dragged in
            u.rotateTo(this.getUnitRotation(false));
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
        this.pi.setDollars(this.money);
        Destroy(u.gameObject);
    }

    public Bench getBench() {
        return this.bench;
    }

    public UnitMovementManager getUnitMoveManager() {
        return this.umm;
    }

    public int getMaxCombatUnits() {
        return this.unitCap;
    }

    public void loseCombat() {
        for(int i = this.playersUnits.Count - 1; i >= 0; i--) {
            if(!this.unitsCopy.Contains(this.playersUnits[i])) {
                Unit pointer = this.playersUnits[i];
                this.playersUnits.Remove(pointer);
                pointer.destroyObject();
            }
        }
        this.money = this.prevMoney;
        this.pi.setDollars(this.money);
        this.shop.setShopIndex(this.prevShopIndex);
    }

    public void gainLenses(int deltaL) {
        if(deltaL > 0) {
            //Little particle to show it went up
        } else if(deltaL < 0) {
            //Little particle to show it went down
        }

        this.lenseCount += deltaL;
    }

    public int getLenses() {
        return this.lenseCount;
    }
}

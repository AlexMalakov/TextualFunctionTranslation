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
    [SerializeField] private ShopPool pool;

    private Dictionary<Unit, int> unitsCopy;
    private List<Unit> destroyBackup = new List<Unit>();

    private int prevMoney = 0;


    public override void init() {
        base.init();
        this.unitsCopy = new Dictionary<Unit, int>();
        foreach(Unit u in this.playersUnits) {
            this.unitsCopy.Add(u, u.getMergeLevel());
        }

        foreach(Unit u in this.destroyBackup) {
            u.destroyObject();
        }
        this.destroyBackup = new List<Unit>();

        this.bench.saveBenchState();
        this.pool.backupThePool();
        this.pi.setDollars(this.money);
        this.prevMoney = this.money;
        this.lenseCount = 0;
    }

    public void setUnitCap(int cap) {
        this.unitCap = cap; //TODO: is it possible to exceed unit cap?
    }

    public int getUnitCap() {
        return this.unitCap;
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
                    || this.canMergeUnit(null, unitName)
                )
            ) || (!this.gm.canModifyBoard()
                &&  this.hasBenchSpace(unitName)
            )
        );
    }

    public bool hasBenchSpace(string unitName) {
        return this.bench.canAddUnitToBench() || this.bench.hasAnotherStarLevel(null, unitName, 1);
    }

    public void purchaseUnit(Unit u) { //TODO: Make sure unit actually arrives where it needs to
        this.money -= u.getCost();
        this.pi.setDollars(this.money);
        this.gainUnit(u);
    }

    public void gainUnit(Unit u) {
        this.playersUnits.Add(u);

        if(this.canMergeUnit(u, u.getName())) {
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

    public bool canMergeUnit(Unit mergeMe, string unitName) {
        return this.bench.hasAnotherStarLevel(mergeMe, unitName, 1) || this.unitLayout.hasAnotherStarLevel(mergeMe, unitName, 1);
    }

    //recursively merge until can no longer merge any longer
    private void mergeUnit(Unit u, int starLevel) {
        Unit mergeTo;
        if(this.bench.hasAnotherStarLevel(u, u.getName(), starLevel)) {
            mergeTo = this.bench.getAnotherStarLevel(u, u.getName(), starLevel);
        } else if(this.unitLayout.hasAnotherStarLevel(u, u.getName(), starLevel)){
            mergeTo = this.unitLayout.getAnotherStarLevel(u, u.getName(), starLevel);
        } else {
            return;
        }

        Debug.Log("MERGE CHECK!!!!!!");
        if(mergeTo == u) {
            Debug.Log("DODGED IT");
            return;
        }

        StartCoroutine(this.killUnitInMerge(u, mergeTo));
        mergeTo.mergeUnit();
        this.mergeUnit(mergeTo, ++starLevel);
    }

    private IEnumerator killUnitInMerge(Unit preMerge, Unit postMerge) {
        yield return this.umm.dragUnitTo(preMerge, postMerge.transform.position, false);
        this.bench.removeUnitFromBench(preMerge);
        this.unitLayout.removeUnitFromLayout(preMerge);
        this.playersUnits.Remove(preMerge);
        preMerge.fakeDestroyObject();
        this.destroyBackup.Add(preMerge);
    }

    public void sellUnit(Unit u) {
        if(!this.playersUnits.Contains(u)) {
            throw new Exception("cannot sell unit you do not own");
        }

        this.money += u.getCost() - 1;
        this.pool.returnToPool(u);
        this.playersUnits.Remove(u);
        this.bench.removeUnitFromBench(u);
        this.unitLayout.removeUnitFromLayout(u);
        this.pi.setDollars(this.money);

        u.fakeDestroyObject();
        this.destroyBackup.Add(u);
        // u.destroyObject();
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
        for(int i = this.playersUnits.Count - 1; i >= 0; i--) { //remove any units that we say we have but we wouldn't from the restart
            if(!this.unitsCopy.ContainsKey(this.playersUnits[i])) {
                Unit pointer = this.playersUnits[i];
                this.playersUnits.Remove(pointer);
                pointer.destroyObject();
            }
        }

        foreach(Unit u in this.unitsCopy.Keys) { //return any units we should have but got rid of (be careful of merge)
            if(this.destroyBackup.Contains(u)) {
                this.destroyBackup.Remove(u);
                u.gameObject.SetActive(true);
                this.playersUnits.Add(u);
            }
            u.setMergeLevel(this.unitsCopy[u]);
        }

        this.bench.resetBenchState();
        this.pool.rewindPool();
        this.unitLayout.resetToCombatStart();
        this.money = this.prevMoney;
        this.pi.setDollars(this.money);
    }

    public void gainLenses(int deltaL) {
        if(deltaL > 0) {
            //TODO: Little particle to show it went up
        } else if(deltaL < 0) {
            //Little particle to show it went down
        }

        this.lenseCount += deltaL;
        this.lenseCount = Mathf.Max(0, this.lenseCount);
        this.pi.setLenseCount(this.lenseCount);
    }

    public int getLenses() {
        return this.lenseCount;
    }

    public Unit getUnitWithName(string name) {
        foreach(Unit u in this.unitLayout.getActiveUnits()) {
            if(u.getName() == name) {
                return u;
            }
        }

        return null;
    }
}

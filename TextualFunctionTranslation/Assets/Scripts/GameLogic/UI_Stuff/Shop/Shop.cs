using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Shop : MonoBehaviour
{
    [SerializeField] private HumanPlayer player;
    [SerializeField] private StatusMessage brokeNotifier;
    [SerializeField] private List<ShopButton> shopButtons;

    [Header ("special")]
    [SerializeField] private TutorialStage tut;
    [SerializeField] private List<string> tutorialShop;
    [SerializeField] private List<string> heroShop;
    [SerializeField] private ShopPool pool;

    private List<UnitBlueprint> currentEntries = new List<UnitBlueprint>();

    private bool tutorialLockOn = false;
    private bool tutorialActive = false;

    void Start() { //sets initial entries, runs after factories have finished
        if(!this.tutorialActive) {
            this.updateEntries();
        } else {
            this.updateEntries(this.tutorialShop);
        }
        
    }

    public void onUnitPurchased(UnitBlueprint ub) {
        if(this.tutorialLockOn) {
            return;
        }

        if(!this.player.canPurchaseUnit(ub.unitName, ub.unitCost)) //TODO SWAP COST 
        {
            this.brokeNotifier.gameObject.SetActive(true);
            this.brokeNotifier.displayMessage("YOU ARE TOO BROKE TO BUY THIS!");
            return;
        }
        
        
        Unit u = UnitFactory.createUnit(ub.unitName);
        this.player.purchaseUnit(u);
        this.pool.removeFromPool(ub);
        
        if(this.tutorialActive) {
            this.tut.onUnitBought();
        }

        this.updateEntries();
    }

    public void updateEntries(List<string> hardcodedEntries) {
        for(int i = 0; i < this.shopButtons.Count; i++) {
            this.shopButtons[i].setOffering(UnitFactory.lookupBlueprint(hardcodedEntries[i]));
        }
    }

    public void updateEntries() {
        this.currentEntries = pool.sampleFromPool();
        for(int i = 0; i < this.shopButtons.Count; i++) {
            this.shopButtons[i].setOffering(this.currentEntries[i]);
        }
    }

    public string getVigUnitName() {
        return this.currentEntries[0].unitName;;
    }

    public void giftVigUnit() {
        if(this.player.hasBenchSpace(this.currentEntries[0].unitName)) {
            Unit u = UnitFactory.createUnit(this.currentEntries[0].unitName);
            this.player.gainUnit(u);

            this.updateEntries();
        } else {
            this.trashVigUnit();
        }
    }

    public void trashVigUnit() {
        this.updateEntries();
    }

    public void tutorialLock(bool lockOn) {
        this.tutorialLockOn = lockOn;

        foreach(ShopButton be in this.shopButtons) {
            be.tutorialLockStatus(lockOn);
        }
    }

    public void tutorialFlash(bool flashOn) {
        foreach(ShopButton be in this.shopButtons) {
            be.tutorialFlash(flashOn);
        }
    }

    public void tutorialStarted(bool started) {
        this.tutorialActive = started;
        if(started) 
            this.updateEntries(this.tutorialShop);
    }

    public void revealHero() {
        this.updateEntries(this.heroShop);
    }
}
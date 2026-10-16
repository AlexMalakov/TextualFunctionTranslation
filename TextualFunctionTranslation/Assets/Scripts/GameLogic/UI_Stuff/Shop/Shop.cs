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
    private bool shopLock = false;

    void Start() { //sets initial entries, runs after factories have finished
        if(!this.tutorialActive) {
            this.updateEntries();
        } else {
            this.updateEntries(this.tutorialShop);
        }
        
    }

    public void onUnitPurchased(UnitBlueprint ub) {
        if(this.tutorialLockOn || this.shopLock) {
            return;
        }

        if(!this.player.canPurchaseUnit(ub.unitName, ub.unitCost)) //TODO SWAP COST 
        {
            this.brokeNotifier.gameObject.SetActive(true);
            if(ub.unitCost > this.player.getMoney()) {
                this.brokeNotifier.displayMessage("CANNOT BUY THIS UNIT (YOU ARE BROKE)!");
            } else {
                this.brokeNotifier.displayMessage("CANNOT BUY THIS UNIT (NO SPACE)!");
            }
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
            this.player.receiveGiftedUnit(u);
            this.brokeNotifier.displayMessage("Your vigeneries have recruited " + u.getName() + " to your army.");
            this.pool.removeFromPool(u.getBlueprint());

            this.updateEntries();
        } else {
            this.trashVigUnit();
        }
    }

    public void trashVigUnit() {
        this.updateEntries();
    }

    public List<UnitBlueprint> getEntries() {
        return this.currentEntries;
    }

    public void tutorialLock(bool lockOn) {
        this.tutorialLockOn = lockOn;

        foreach(ShopButton be in this.shopButtons) {
            be.tutorialLockStatus(this.shopLock || this.tutorialLockOn);
        }
    }

    //parallel lock to tutorial lokc
    public void lockShop(bool lockOn) {
        this.shopLock = lockOn;

        foreach(ShopButton be in this.shopButtons) {
            be.tutorialLockStatus(this.shopLock || this.tutorialLockOn);
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
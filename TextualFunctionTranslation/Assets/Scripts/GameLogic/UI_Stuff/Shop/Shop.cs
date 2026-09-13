using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ShopEntry {
    public List<string> units;
}

public class Shop : MonoBehaviour
{
    [SerializeField] private HumanPlayer player;
    [SerializeField] private List<ShopEntry> shopEntries;
    [SerializeField] private List<ShopButton> shopButtons;
    [SerializeField] private StatusMessage brokeNotifier;

    [Header ("special")]
    [SerializeField] private TutorialStage tut;
    [SerializeField] private ShopEntry tutorialShop;
    [SerializeField] private ShopEntry heroShop;

    private bool tutorialLockOn = false;
    private bool tutorialActive = false;

    private int shopIndex = 0;

    public int getShopIndex() {
        return this.shopIndex;
    }

    public void setShopIndex(int index) {
        this.shopIndex = index;
        this.shopIndex = this.shopIndex >= this.shopEntries.Count ? 0 : this.shopIndex;
    }

    private void nextShop() {
        this.shopIndex++;
        this.shopIndex = this.shopIndex >= this.shopEntries.Count ? 0 : this.shopIndex;
    }

    void Start() { //sets initial entries, runs after factories have finished
        if(!this.tutorialActive) {
            this.updateEntries(this.shopEntries[shopIndex]);
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
        Debug.Log("UNIT CREATED AT Y" + u.transform.position.y);
        this.player.purchaseUnit(u);
        
        if(this.tutorialActive) {
            this.tut.onUnitBought();
        } else {
            this.shopIndex++;
        }

        this.updateEntries(this.shopEntries[shopIndex]);
    }

    private void updateEntries(ShopEntry curEntry) {
        for(int i = 0; i < this.shopButtons.Count; i++) {
            this.shopButtons[i].setOffering(UnitFactory.lookupBlueprint(this.shopEntries[shopIndex].units[i]));
        }
    }

    public string getVigUnitName() {
        return this.shopEntries[shopIndex].units[0];
    }

    public void giftVigUnit() {
        if(this.player.hasBenchSpace(this.shopEntries[shopIndex].units[0])) {
            Unit u = UnitFactory.createUnit(this.shopEntries[shopIndex].units[0]);
            this.player.gainUnit(u);
            this.shopIndex++;
            this.updateEntries(this.shopEntries[shopIndex]);
        } else {
            this.trashVigUnit();
        }
    }

    public void trashVigUnit() {
        this.shopIndex++;
        this.updateEntries(this.shopEntries[shopIndex]);
    }

    public void tutorialLock(bool lockOn) {
        this.tutorialLockOn = lockOn;
    }

    public void tutorialFlash(bool flashOn) {
        foreach(ShopButton be in this.shopButtons) {
            be.tutorialFlash(flashOn);
        }
    }

    public void tutorialStarted(bool started) {
        this.tutorialActive = started;
        this.updateEntries(this.tutorialShop);
    }

    public void revealHero() {
        this.updateEntries(this.heroShop);
    }
}
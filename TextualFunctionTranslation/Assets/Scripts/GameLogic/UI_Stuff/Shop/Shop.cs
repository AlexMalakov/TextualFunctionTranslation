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

    private int shopIndex = 0; //TODO: make sure on restarts this gets saved?

    void Start() { //sets initial entries, runs after factories have finished
        this.updateEntries();
    }

    public void onUnitPurchased(UnitBlueprint ub) {
        if(!this.player.canPurchaseUnit(ub.unitName, ub.unitCost)) //TODO SWAP COST 
        {
            this.brokeNotifier.displayMessage("YOU ARE TOO BROKE TO BUY THIS!");
            return;
        }
        
        Unit u = UnitFactory.createUnit(ub.unitName);
        this.player.purchaseUnit(u);
        shopIndex++;
        this.updateEntries();
    }

    private void updateEntries() {
        for(int i = 0; i < this.shopButtons.Count; i++) {
            this.shopButtons[i].setOffering(UnitFactory.lookupBlueprint(shopEntries[shopIndex].units[i]));
        }
    }

    public string getVigUnitName() {
        return this.shopEntries[shopIndex].units[0];
    }

    public void giftVigUnit() {
        if(this.player.hasBenchSpace()) {
            Unit u = UnitFactory.createUnit(this.shopEntries[shopIndex].units[0]);
            this.player.gainUnit(u);
            shopIndex++;
            this.updateEntries();
        } else {
            this.trashVigUnit();
        }
    }

    public void trashVigUnit() {
        this.shopIndex++;
        this.updateEntries();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ShopEntry {
    public List<string> units;
}

public class Shop : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private List<ShopEntry> shopEntries;
    [SerializeField] private List<ShopButton> shopButtons;
    private int shopIndex = 0; //TODO: make sure on restarts this gets saved?

    void Awake() {
        this.updateEntries();
    }

    public void onUnitPurchased(UnitBlueprint ub) {
        if(!this.player.canPurchaseUnit(ub.unitCost)) //TODO SWAP COST 
        {
            //TODO: NOTIFY PLAYER THAT THEY ARE TOO BUM ASS POOR
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
}

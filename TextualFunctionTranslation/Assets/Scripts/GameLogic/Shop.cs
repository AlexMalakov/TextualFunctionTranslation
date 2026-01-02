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
    [SerializeField] private ShopEntry shopEntries;
    [SerializeField] private List<ShopButton> shopButtons;
    private int shopIndex = 0; //TODO: make sure on restarts this gets saved?

    void Awake() {
        this.updateEntries();
    }

    public void onUnitPurchased(Unit u) {
        //TODO: check if can buy the unit
        //TODO: check if can put the unit somehwere

        //if yes, place unit first on bench then on board
        //then show next shop
    }

    private void updateEntries() {
        for(int i = 0; i < this.shopButtons.Count; i++) {
            Unit u = null;
            this.shopButtons[i].setOffering(null);
        }
    }
}

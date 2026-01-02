using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopButton : MonoBehaviour
{
    //duplicated code cuz im a dork
    [SerializeField] private Button button;
    [SerializeField] private Shop shop;

    [SerializeField] private Unit unitBeingSold;

    [SerializeField] private TMP_Text unitName;
    [SerializeField] private TMP_Text unitCost;
    [SerializeField] private Image unitSprite;

    public void Start() {
        UnityEngine.UI.Button btn = button.GetComponent<UnityEngine.UI.Button>();
		btn.onClick.AddListener(onButtonClick);
    }

    public void setOffering(Unit unitBeingSold) {
        //TODO: set cost TMP
        //TODO: set unit name
        //TODO: set unit sprite
        this.unitBeingSold = unitBeingSold;
    }

    public void onButtonClick() {
        this.shop.onUnitPurchased(this.unitBeingSold);
    }

}

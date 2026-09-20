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

    [SerializeField] private UnitBlueprint unitBeingSold;

    [SerializeField] private TMP_Text unitName;
    [SerializeField] private TMP_Text unitCost;
    [SerializeField] private Image unitSprite;

    [SerializeField] private ButtonCover cover;

    private bool tutFlashOn;
    private Color color;

    public void Awake() { //Awake because this doesn't affect other systems
        UnityEngine.UI.Button btn = button.GetComponent<UnityEngine.UI.Button>();
		btn.onClick.AddListener(onButtonClick);
    }

    public void setOffering(UnitBlueprint unitBeingSold) {
        this.unitBeingSold = unitBeingSold;
        unitCost.text = "" + unitBeingSold.unitCost;
        unitName.text = unitBeingSold.unitName;
        this.unitSprite.sprite = unitBeingSold.unitIcon;
    }

    public void onButtonClick() {
        this.shop.onUnitPurchased(this.unitBeingSold);
    }

    public void tutorialFlash(bool flashOn) {
        this.cover.flashButton(flashOn);
    }

    public void tutorialLockStatus(bool locked) {
        this.cover.coverButton(!locked);
    }


}

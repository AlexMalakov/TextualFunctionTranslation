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

    [SerializeField] private Image image;

    private bool tutFlashOn;
    private Color color;

    public void Awake() { //Awake because this doesn't affect other systems
        UnityEngine.UI.Button btn = button.GetComponent<UnityEngine.UI.Button>();
		btn.onClick.AddListener(onButtonClick);
        this.color = image.color;
    }

    public void setOffering(UnitBlueprint unitBeingSold) {
        this.unitBeingSold = unitBeingSold;
        unitCost.text = "" + unitBeingSold.unitCost;
        unitName.text = unitBeingSold.unitName;
        this.unitSprite.sprite = unitBeingSold.unitIcon;
    }

    public void onButtonClick() {
        Debug.Log("CLICKED!");
        this.shop.onUnitPurchased(this.unitBeingSold);
    }

    public void tutorialFlash(bool flashOn) {
        this.tutFlashOn = flashOn;
        if(flashOn) {
            this.flash();
        } else {
            this.unflash();
        }
    }

    private void flash() {
        if(!this.tutFlashOn) {
            return;
        }
        this.image.color = new Color(255, 0, 0);
        Invoke(nameof(unflash), .5f);
    }

    private void unflash() {
        this.image.color = this.color;

        if(this.tutFlashOn) {
            Invoke(nameof(flash), .5f);
        }
    }

}

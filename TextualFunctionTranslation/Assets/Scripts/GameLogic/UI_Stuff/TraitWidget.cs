using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TraitWidget : MonoBehaviour
{
    [SerializeField] private Trait trait;
    [SerializeField] private TMP_Text traitCount;
    [SerializeField] private TMP_Text traitName;
    [SerializeField] private TMP_Text traitTieringInfo;
    [SerializeField] private Image img;


    public void init(Trait t) {
        this.trait = t;
        this.traitCount.text = "0";
        this.traitName.text = t.getTraitName();

        string text = "";
        foreach(int tier in t.getTraitTiering()) {
            text = text + tier + " / ";
        }
        if(text.Length > 0) {
            text = text.Substring(0, text.Length-2);
        }

        this.traitTieringInfo.text = text;
    }

    public void update() {
        int numA = this.trait.getNumberActive();
        this.traitCount.text = "" + numA;

        string text = "";

        int prevTier = 0;
        foreach(int tier in this.trait.getTraitTiering()) {
            if(prevTier <= numA && tier > numA && prevTier != 0) {
                text = text.Substring(0, text.Length - 4);
                text = text + "<color=#FF0000>" + tier + "</color> / ";
            }

            prevTier = tier;
            text = text + tier + " / ";
        }
        if(text.Length > 0) {
            text = text.Substring(0, text.Length-2);
        }

        this.traitTieringInfo.text = text;
    }

    public void flashTrait() {
        img.color = new Color(255,0,0);
        Invoke(nameof(unflashTrait), .5f);
    }

    private void unflashTrait() {
        img.color = new Color(0,255,0);
    }

    //this being a number is bad but consider that its ui code so idc about good design
    public void makeActive(bool isActive, int activeNum) { //-1 it's not even active, 0 it's player's and active 1 it's players and not active 2 it's enemies
        switch(activeNum) {
            case -1:
                img.color = new Color(125,125,125);
                break;
            case 0:
                img.color = new Color(125,160,125);
                break;
            case 1:
                img.color = new Color(125,125,125);
                break;
            case 2:
                img.color = new Color(160,125,125);
                break;
        }

        this.gameObject.SetActive(isActive);
    }
}

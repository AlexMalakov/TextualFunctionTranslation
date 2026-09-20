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
    [SerializeField] private ButtonCover cover;
    [SerializeField] private GameObject activeBorder;


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
                text = text + "<color=#FF0000>" + prevTier + "</color> / ";
            }

            prevTier = tier;
            text = text + tier + " / ";
        }
        if(text.Length > 0) {
            text = text.Substring(0, text.Length-2);
        }

        if(this.trait.getTraitTiering().Count == 1 && numA >= prevTier) {
            text = "<color=#FF0000>" + prevTier + "</color>";
        }

        this.traitTieringInfo.text = text;
    }


    public void flashTrait() {
        this.cover.flashTrait();
    }

    //this being a number is bad but consider that its ui code so idc about good design
    public void makeActive(bool isActive, bool enoughForProc) {
        this.gameObject.SetActive(isActive);
        this.activeBorder.SetActive(enoughForProc);
    }
}

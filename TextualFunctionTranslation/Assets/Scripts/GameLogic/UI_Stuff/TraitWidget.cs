using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TraitWidget : MonoBehaviour
{
    [SerializeField] private Trait trait;
    [SerializeField] private TMP_Text traitCount;
    [SerializeField] private TMP_Text traitName;
    [SerializeField] private TMP_Text traitTieringInfo;


    public void init(Trait t) {
        this.trait = t;
        this.traitCount.text = "0";
        this.traitName.text = t.getTraitName();

        string text = "";
        foreach(int tier in t.getTraitTiering()) {
            text = text + tier + " / ";
        }
        if(text.Length > 0) {
            //TODO: remember how to do a substring in this language
        }

        this.traitTieringInfo.text = text;
    }

    public void update() {
        this.traitCount.text = this.trait.getNumberActive();

        int number = 0;
        string text = "";
        foreach(int tier in t.getTraitTiering()) {
            if(tier <= this.trait.getNumberActive()) {
                number = tier;
            } 
            text = text + tier + " / ";
        }
        if(text.Length > 0) {
            //TODO: remember how to do a substring in this language
        }

        //TODO: color based on trait tiering

        this.traitTieringInfo.text = text;
    }
}

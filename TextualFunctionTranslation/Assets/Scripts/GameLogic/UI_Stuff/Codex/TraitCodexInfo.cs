using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TraitCodexInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text traitName;
    [SerializeField] private TMP_Text traitBreakpoints;
    [SerializeField] private TMP_Text traitDescription;
    [SerializeField] private Image image;

    
    public void showTrait(Trait t) {
        this.traitName.text = t.getTraitName();

        string text = "";
        foreach(int teir in t.getTraitTiering()) {
            text += teir + "/";
        } 
        if(text.Length > 0) {
            text = text.Substring(0, text.Length-2);
        }

        this.traitBreakpoints.text = text;
        this.traitDescription.text = t.getTraitDescription();

        //TODO: trait image
        //TODO: units that have a trait
    }
}

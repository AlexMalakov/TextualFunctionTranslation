using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitCodexInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text unitName;
    [SerializeField] private TMP_Text unitCost;
    [SerializeField] private TMP_Text unitStats;
    [SerializeField] private Image image;

    [SerializeField] private TraitCodexButton roleTraitButton;
    [SerializeField] private TraitCodexButton classTraitButton;

    
    public void showUnit(UnitBlueprint ub) {
        // this.unitName.text = ub.unitName;
        // this.unitCost.text = ub.unitCost + "";

        // Unit actual = ub.unitPrefab.GetComponent<Unit>();

        // this.unitStats.text = "Health: " + actual.getMaxHealth(1) + "/" + actual.getMaxHealth(2) + "/" + actual.getMaxHealth(3)
        //             + "\nDamage: " + actual.getDamage(1) + "/" + actual.getDamage(2) + "/" + actual.getDamage(3)
        //             + "\nAttack Speed: " + actual.getAttackSpeed()
        //             + "\nRange: " + actual.getRange()
        //             + "\nDecision Order: " + actual.getDecisionPriority()
        //             + "\nTaunt: " + actual.getTaunt();
        // roleTraitButton.init(actual.getRoleTrait()); //unlikely but possible danger
        // classTraitButton.init(actual.getClassTrait());

    }
}


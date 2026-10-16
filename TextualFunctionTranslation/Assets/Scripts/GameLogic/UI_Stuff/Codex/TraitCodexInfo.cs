using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TraitCodexInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text traitName;
    [SerializeField] private TMP_Text traitNarrative;
    [SerializeField] private TMP_Text traitEffect;
    [SerializeField] private Image icon;
    [SerializeField] private Image imageExplenation;

    [SerializeField] private List<Image> unitIconImages;
    [SerializeField] private Sprite capsSprite;
    [SerializeField] private List<Sprite> demonSprites;



    public void Awake() {
        foreach(Image img in this.unitIconImages) {
            img.gameObject.SetActive(false);
        }
    }

    public void showTrait(Trait t) {
        this.traitName.text = t.getTraitName();

        string text = "";
        foreach(int teir in t.getTraitTiering()) {
            text += teir + "/";
        } 
        if(text.Length > 0) {
            text = text.Substring(0, text.Length-2);
        }

        this.traitNarrative.text = t.getTraitNarrative();
        this.traitEffect.text = t.getTraitEffect();

        this.icon.sprite = t.getTraitIcon();
        this.imageExplenation.sprite = t.getTraitEffectImg();
        
        foreach(Image img in this.unitIconImages) {
            img.gameObject.SetActive(false);
        }

        if(t.getTraitName() == "hero") {
            this.unitIconImages[0].gameObject.SetActive(true);
            this.unitIconImages[0].sprite = this.capsSprite;
        } else if(t.getTraitName() == "demon") {
            this.unitIconImages[0].gameObject.SetActive(true);
            this.unitIconImages[0].sprite = this.demonSprites[0];
            this.unitIconImages[1].gameObject.SetActive(true);
            this.unitIconImages[1].sprite = this.demonSprites[1];
            this.unitIconImages[2].gameObject.SetActive(true);
            this.unitIconImages[2].sprite = this.demonSprites[2];
        }else {
            int ptr = 0;
            foreach(UnitBlueprint ub in UnitFactory.getAllBlueprints()) {
                if(ub.unitPrefab.GetComponent<Unit>().getRoleTraitName() == t.getTraitName() || ub.unitPrefab.GetComponent<Unit>().getClassTraitName() == t.getTraitName()) {
                    this.unitIconImages[ptr].gameObject.SetActive(true);
                    this.unitIconImages[ptr].sprite = ub.unitIcon;
                    ptr+=1;
                }
            }
        }
        
    }
}

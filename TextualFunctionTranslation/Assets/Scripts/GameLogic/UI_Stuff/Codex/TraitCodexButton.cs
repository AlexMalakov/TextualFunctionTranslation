using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//lets say hypothetically we had one script for buttosn jk loser
public class TraitCodexButton : MonoBehaviour
{
    //ok everyone say it with me "abstraction is for dorks!!!!"
    [SerializeField] private Button button;
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text trait_name;
    [SerializeField] private TMP_Text trait_count;
    [SerializeField] private TMP_Text trait_breakpoints;
    private TraitCodexInfo tci;
    private GameObject traitCodex;

    private Trait trait;

    public void Awake() { //Awake because this doesn't affect other systems
        UnityEngine.UI.Button btn = button.GetComponent<UnityEngine.UI.Button>();
		btn.onClick.AddListener(onButtonClick);
    }

    public void init(Trait trait, TraitCodexInfo tci, GameObject traitCodex) {
        this.tci = tci;
        this.traitCodex = traitCodex;
        if(this.trait_name != null) {
            trait_name.text = trait.getTraitName(); 
            string text = "";
            foreach(int tier in trait.getTraitTiering()) {
                text = text + tier + " / ";
            }
            if(text.Length > 0) {
                text = text.Substring(0, text.Length-2);
            }
            this.trait_breakpoints.text = text;
            this.trait_count.text = trait.getNumberActive() + "";
        }        
        
        this.trait = trait;
        this.image.sprite = this.trait.getTraitIcon();
    }

    //mr president a fifth button script has hit the tower
    public void onButtonClick() {
        this.traitCodex.SetActive(true);
        this.tci.showTrait(this.trait);
    }
}

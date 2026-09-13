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
    [SerializeField] private GameObject listTab;
    [SerializeField] private TraitCodexInfo tci;
    [SerializeField] private TMP_Text text;
    [SerializeField] private Image image;

    private Trait trait;

    public void Awake() { //Awake because this doesn't affect other systems
        UnityEngine.UI.Button btn = button.GetComponent<UnityEngine.UI.Button>();
		btn.onClick.AddListener(onButtonClick);
    }

    public void init(Trait trait) {
        this.trait = trait;
        this.text.text = this.trait.getTraitName();
        // this.image.sprite = this.trait.getTraitIcon(); //TODO: uncomment me once traits have icons
    }

    //mr president a fifth button script has hit the tower
    public void onButtonClick() {
        this.listTab.SetActive(false);
        this.tci.showTrait(this.trait);
    }
}

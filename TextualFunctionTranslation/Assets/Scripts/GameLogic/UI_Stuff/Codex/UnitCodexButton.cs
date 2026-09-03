using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//lets say hypothetically we had one script for buttosn jk loser
public class UnitCodexButton : MonoBehaviour
{
    //ok everyone say it with me "abstraction is for dorks!!!!"
    [SerializeField] private Button button;
    [SerializeField] private GameObject gridTab;
    [SerializeField] private UnitCodexInfo uci;
    [SerializeField] private TMP_Text text;
    [SerializeField] private Image image;

    private UnitBlueprint ub;

    public void Awake() { //Awake because this doesn't affect other systems
        UnityEngine.UI.Button btn = button.GetComponent<UnityEngine.UI.Button>();
		btn.onClick.AddListener(onButtonClick); //abstraction remaines for chumps
    }

    public void init(UnitBlueprint ub) {
        this.ub = ub;
        this.text.text = this.ub.unitName;
        this.image.sprite = this.ub.unitIcon;
    }

    //mr president a sixth button script has hit the tower
    public void onButtonClick() {
        this.gridTab.SetActive(false);
        this.uci.showUnit(this.ub);
    }
}

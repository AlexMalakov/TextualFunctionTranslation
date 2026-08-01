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
    [SerializeField] private UnitCodeInfo uci;
    [SerializeField] private TMP_Text text;
    [SerializeField] private Image image;

    private Unit unit;

    public void Awake() { //Awake because this doesn't affect other systems
        UnityEngine.UI.Button btn = button.GetComponent<UnityEngine.UI.Button>();
		btn.onClick.AddListener(onButtonClick); //abstraction remaines for chumps
    }

    public void init(Unit unit) {
        this.unit = unit;
        this.text.text = this.unit.getName();
        //TODO: assign trait icon
    }

    //mr president a sixth button script has hit the tower
    public void onButtonClick() {
        this.gridTab.SetActive(false);
        this.uci.showUnit(this.unit);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CypherPopUpButton : MonoBehaviour
{
    //ok everyone say it with me "abstraction is for dorks!!!!"
    [SerializeField] private Button button;
    [SerializeField] private CypherTextDisplay disp;

    [SerializeField] private TMP_Text buttonText;

    public void Awake() { //Awake because this doesn't affect other systems
        UnityEngine.UI.Button btn = button.GetComponent<UnityEngine.UI.Button>();
		btn.onClick.AddListener(onButtonClick);
    }

    public void onButtonClick() {
        bool status = this.disp.toggleDisplay();

        buttonText.text = status ? "hide scroll" : "show scroll";
    }

    public void setDisplay(bool setMe) {
        this.gameObject.SetActive(setMe);
    }
}

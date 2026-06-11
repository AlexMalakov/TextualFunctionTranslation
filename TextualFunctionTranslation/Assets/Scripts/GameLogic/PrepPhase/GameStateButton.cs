using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameStateButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private GameManager game;
    [SerializeField] private TMP_Text button_text;
    private bool isPrecombat = false;
    private bool isResults = false;

    public void Awake() { //Awake because this doesn't affect other systems
        UnityEngine.UI.Button btn = button.GetComponent<UnityEngine.UI.Button>();
		btn.onClick.AddListener(onButtonClick);
    }

    public void onButtonClick() {
        if(isPrecombat) {
            this.game.onPreperationEnd();
        } else if(isResults){
            this.game.onResultsEnd();
        }
    }

    public void runPrecombat(bool isEnabled) {
        this.gameObject.SetActive(isEnabled);
        this.button_text = "start combat";
        this.isPrecombat = isEnabled;
    }

    public void runResults(bool isEnabled) {
        this.gameObject.SetActive(isEnabled);
        this.button_text = "next round";
        this.isResults = isEnabled;
    }
}


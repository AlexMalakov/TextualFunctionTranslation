using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStartButton : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] GameManager game;

    private bool bEnabled = false;

    public void Awake() { //Awake because this doesn't affect other systems
        UnityEngine.UI.Button btn = button.GetComponent<UnityEngine.UI.Button>();
		btn.onClick.AddListener(onButtonClick);
    }

    public void onButtonClick() {
        this.game.onCombatStart();
    }

    public void setEnabled(bool isEnabled) {
        this.bEnabled = isEnabled;
    }
}


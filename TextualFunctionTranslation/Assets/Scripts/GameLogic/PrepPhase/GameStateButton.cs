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
    [SerializeField] private ButtonCover cover;
    private bool isPrecombat = false;
    private bool isResults = false;
    private bool isEnabled = true;

    private bool tutorialLockOn = false;


    public void Awake() { //Awake because this doesn't affect other systems
        UnityEngine.UI.Button btn = button.GetComponent<UnityEngine.UI.Button>();
		btn.onClick.AddListener(onButtonClick);
        this.isEnabled = true;
    }

    public void onButtonClick() {
        if(this.tutorialLockOn)
            return;
        
        if(isPrecombat) {
            this.game.onPreperationEnd();
        } else if(isResults){
            this.game.onResultsEnd(true);
        }
    }

    public void runPrecombat(bool isEnabled) {
        this.isEnabled = isEnabled;
        this.cover.coverButton(isEnabled && !this.tutorialLockOn);
        this.button_text.text = "start combat";
        this.isPrecombat = isEnabled;
    }

    public void runResults(bool isEnabled, bool wonCombat) {
        this.isEnabled = isEnabled;
        this.cover.coverButton(isEnabled && !this.tutorialLockOn);
        this.button_text.text = wonCombat ? "next round" : "restart round";
        this.isResults = isEnabled;
    }

    public void tutorialLock(bool lockOn) {
        this.tutorialLockOn = lockOn;
        this.cover.coverButton(!lockOn);
    }

    public void tutorialFlash(bool startFlash) {
        this.cover.flashButton(startFlash);
    }

}


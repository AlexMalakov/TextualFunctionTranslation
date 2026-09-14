using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameStateButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image image;
    [SerializeField] private GameManager game;
    [SerializeField] private TMP_Text button_text;
    private bool isPrecombat = false;
    private bool isResults = false;

    private bool tutorialLockOn = false;
    private bool tutorialFlashOn = false;

    private Color color;

    public void Awake() { //Awake because this doesn't affect other systems
        UnityEngine.UI.Button btn = button.GetComponent<UnityEngine.UI.Button>();
		btn.onClick.AddListener(onButtonClick);
        this.color = image.color;
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
        this.gameObject.SetActive(isEnabled);
        this.button_text.text = "start combat";
        this.isPrecombat = isEnabled;
    }

    public void runResults(bool isEnabled, bool wonCombat) {
        this.gameObject.SetActive(isEnabled);
        this.button_text.text = wonCombat ? "next round" : "restart round";
        this.isResults = isEnabled;
    }

    public void tutorialLock(bool lockOn) {
        this.tutorialLockOn = lockOn;
    }

    public void tutorialFlash(bool startFlash) {
        this.tutorialFlashOn = startFlash;
        if(startFlash)
            this.flash();
        else
            this.unflash();
    }

    private void flash() {
        if(!this.tutorialFlashOn)
            return;
        this.image.color = new Color(255, 0 ,0);

        Invoke(nameof(unflash), .5f);
    }   

    private void unflash() {
        this.image.color = this.color;
        if(this.tutorialFlashOn) {
            Invoke(nameof(flash), .5f);
        }
    }
}


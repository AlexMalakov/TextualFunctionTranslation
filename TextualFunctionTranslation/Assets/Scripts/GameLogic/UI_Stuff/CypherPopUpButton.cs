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
    [SerializeField] private Image image;
    [SerializeField] private TutorialStage tut;

    private bool tutorialLockOn = false;
    private bool tutorialFlashOn = false;

    private Color color;

    public void Awake() { //Awake because this doesn't affect other systems
        UnityEngine.UI.Button btn = button.GetComponent<UnityEngine.UI.Button>();
		btn.onClick.AddListener(onButtonClick);
        this.color = this.image.color;
    }

    public void onButtonClick() {
        if(this.tutorialLockOn) {
            return;
        }
        this.tut.onScrollClosed();
        bool status = this.disp.toggleDisplay();

        buttonText.text = status ? "hide scroll" : "show scroll";
    }

    public void setDisplay(bool setMe) {
        this.gameObject.SetActive(setMe);
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

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

    [SerializeField] private ButtonCover buttonCover;

    private bool tutorialLockOn = false;

    private bool canBePressed;

    private Color color;

    public void Awake() { //Awake because this doesn't affect other systems
        UnityEngine.UI.Button btn = button.GetComponent<UnityEngine.UI.Button>();
		btn.onClick.AddListener(onButtonClick);
        this.color = this.image.color;
    }

    public void onButtonClick() {
        if(this.tutorialLockOn && !this.canBePressed) {
            return;
        }
        this.tut.onScrollClosed();
        bool status = this.disp.toggleDisplay();

        buttonText.text = status ? "hide scroll" : "show scroll";
    }

    public void setDisplay(bool setMe) {
        this.canBePressed = setMe;
        this.buttonCover.coverButton(!setMe); //DO MROE
    }

    public void tutorialLock(bool lockOn) {
        this.tutorialLockOn = lockOn;
        this.buttonCover.coverButton(!lockOn);
    }

    public void tutorialFlash(bool startFlash) {
        this.buttonCover.flash(startFlash);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//lets say hypothetically we had one script for buttosn jk loser
public class LenseButton : MonoBehaviour // +1
{
    //ok everyone say it with me "abstraction is for dorks!!!!"
    [SerializeField] private Button button;
    [SerializeField] private LenseItem li;
    [SerializeField] private ButtonCover buttonCover;

    private bool lenseStatus = false;

    private UnitBlueprint ub;
    private bool lockOn;

    public void Awake() { //Awake because this doesn't affect other systems
        UnityEngine.UI.Button btn = button.GetComponent<UnityEngine.UI.Button>();
		btn.onClick.AddListener(onButtonClick); //abstraction remaines for chumps
    }

    public void showButton(bool status) { //TODO use the cover
        if(lenseStatus) {
            this.onButtonClick();
        }

        this.buttonCover.coverButton(status);
    }

    //mr president a seventh button script has hit the tower
    public void onButtonClick() {
        if(lockOn) {
            return;
        }
        this.lenseStatus = !this.lenseStatus;
        this.li.toggleLenses(lenseStatus);
    }

    public void flash(bool startFlash) {
        this.buttonCover.flashButton(startFlash);
    }

    public void tutorialLock(bool lockOn) {
        this.lockOn = lockOn;
        this.buttonCover.coverButton(!lockOn);
    }
}
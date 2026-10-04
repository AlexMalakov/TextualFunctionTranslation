using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//lets say hypothetically we had one script for buttosn jk loser
public class ButtonObjectToggler : MonoBehaviour
{
    //ok everyone say it with me "abstraction is for dorks!!!!"
    [SerializeField] private Button button;
    [SerializeField] private GameObject toggleMe;

    [SerializeField] private ButtonCover cover;

    private bool lockOn;
    private bool showing = false;

    public void Awake() { //Awake because this doesn't affect other systems
        UnityEngine.UI.Button btn = button.GetComponent<UnityEngine.UI.Button>();
		btn.onClick.AddListener(onButtonClick);
    }

    public void onButtonClick() {
        if(lockOn) {
            return;
        }
        this.toggle();
    }

    private void toggle() {
        this.showing = !this.showing;
        this.toggleMe.SetActive(this.showing);
    }

    public void tutorialLock(bool lockOn) {
        this.lockOn = lockOn;
        this.cover.coverButton(!lockOn);

        if(showing && this.lockOn) {
            this.toggle();
        }
    }
}

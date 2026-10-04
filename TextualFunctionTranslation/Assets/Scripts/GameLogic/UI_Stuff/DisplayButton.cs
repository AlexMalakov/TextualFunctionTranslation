using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayButton : MonoBehaviour
{
    //ok everyone say it with me "abstraction is for dorks!!!!"
    [SerializeField] private Button button;
    [SerializeField] private TraitDisplay td;
    [SerializeField] private string obj1Text;
    [SerializeField] private string obj2Text;
    [SerializeField] private TMP_Text text;

    [SerializeField] private ButtonCover cover;

    private bool lockOn;
    private bool obj1Visible = true;

    public void Awake() { //cant even keep count anymore too many i would say
        UnityEngine.UI.Button btn = button.GetComponent<UnityEngine.UI.Button>();
		btn.onClick.AddListener(onButtonClick);
        this.text.text = obj1Visible ? obj1Text : obj2Text;
    }

    public void onButtonClick() {
        if(lockOn) {
            return;
        }

        obj1Visible = !obj1Visible;
        td.toggleShowCypher();
        this.text.text = obj1Visible ? obj1Text : obj2Text;
    }

    public void toggleFromDisplay() {
        obj1Visible = !obj1Visible;
        this.text.text = obj1Visible ? obj1Text : obj2Text;
    }

    public void tutorialLock(bool lockOn) {
        this.lockOn = lockOn;
        this.cover.coverButton(!lockOn);
    }

    public void showButton(bool status) {
        this.cover.coverButton(status);
    }
}




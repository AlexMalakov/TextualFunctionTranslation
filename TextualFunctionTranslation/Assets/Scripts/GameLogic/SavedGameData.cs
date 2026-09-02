using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
public class SavedGameData : MonoBehaviour
{
    public bool showTutorial;
    public bool showDialogue;

    //TODO: combat speed slider?

    public void Awake() {
        this.showTutorial = PlayerPrefs.GetInt("tutorial", 1) > 0;
        this.showDialogue = PlayerPrefs.GetInt("dialogue", 1) > 0;
    }

    public void setTutorialStatus(bool status) {
        this.showTutorial = status;
        PlayerPrefs.SetInt("tutorial", status ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void setDialogueStatus(bool status) {
        this.showDialogue = status;
        PlayerPrefs.SetInt("dialogue", status ? 1 : 0);
        PlayerPrefs.Save();
    }
}

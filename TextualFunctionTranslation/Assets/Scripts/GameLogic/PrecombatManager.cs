using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrecombatManager : MonoBehaviour
{
    [SerializeField] private CameraRayCast userClicks;
    [SerializeField] private GameStateButton gameButton;
    [SerializeField] private CombatManager combatManager;

    private bool inPrep;
    private bool dialogueLocked;

    public void handlePrep() {
        if(!this.inPrep) {
            this.startPrep();
        }

        this.userClicks.checkUserInteraction();
    }

    private void startPrep() {
        this.inPrep = true;
        this.gameButton.runPrecombat(!this.dialogueLocked);
        this.combatManager.placeUnitsFromLayouts();
    }

    public void lockForDialogue(bool locked) {
        this.dialogueLocked = locked;
        if(this.inPrep) {
            this.gameButton.runPrecombat(!locked);
        }
    }

    public void endPrep() {
        this.inPrep = false;
        this.userClicks.releaseUnit();
        this.gameButton.runPrecombat(false);
    }
}

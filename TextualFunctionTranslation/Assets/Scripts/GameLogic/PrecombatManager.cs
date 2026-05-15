using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrecombatManager : MonoBehaviour
{
    [SerializeField] private CameraRayCast userClicks;
    [SerializeField] private GameStartButton gameStart;
    [SerializeField] private CombatManager combatManager;

    private bool inPrep;

    public void handlePrep() {
        if(!this.inPrep) {
            this.startPrep();
        }

        this.userClicks.checkUserInteraction();
    }

    private void startPrep(PlayerUnitLayout currentEnemy) {
        this.inPrep = true;
        this.gameStart.setEnabled(true);
        this.combatManager.placeUnitsFromLayouts(currentEnemy);
    }

    public void endPrep() {
        this.inPrep = false;
        this.userClicks.releaseUnit();
        this.gameStart.setEnabled(false);
    }
}

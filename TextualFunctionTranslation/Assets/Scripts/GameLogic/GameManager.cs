using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool inCombat = false;    

    [SerializeField] private CombatScheduler combatSched;
    [SerializeField] private CombatManager combatManager;



    void Update() {
        if(this.inCombat) {
            this.handleCombat();
        } else {
            this.handleNonCombat();
        }
    }

    private void handleNonCombat() {
        //TODO: NON COMBAT PHASE
        this.onCombatStart();
    }

    //TODO HOOK THIS AND END TO SOMETHING
    private void onCombatStart() {
        this.combatManager.initializeCombat();
        this.inCombat = true;
    }

    private void onCombatEnd() {
        this.inCombat = false;
    }

    private void handleCombat() {
        this.combatSched.runNUpdates();
    }
}

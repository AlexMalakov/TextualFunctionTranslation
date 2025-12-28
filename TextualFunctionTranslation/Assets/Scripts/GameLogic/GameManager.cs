using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStage {
    preperation, combat, result
}

public class GameManager : MonoBehaviour
{
    private GameStage gameState = GameStage.preperation;    

    [SerializeField] private CombatScheduler combatSched;
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private HexBoard board;

    void Awake() {
        this.board.initBoard();
        this.combatManager.initPlayers();
    }


    void Update() {
        if(this.gameState == GameStage.combat) {
            this.handleCombat();
        } else if(this.gameState == GameStage.preperation){
            this.handlePreperation();
        } else { //results
            this.gameState = GameStage.preperation;
        }
    }

    private void handlePreperation() {
        //TODO: NON COMBAT PHASE
        this.onCombatStart();
    }

    //TODO HOOK THIS END TO SOMETHING
    private void onCombatStart() {
        this.combatManager.initializeCombat();
        this.gameState = GameStage.combat;
    }

    private void onCombatEnd() {
        this.gameState = GameStage.result;
    }

    private void handleCombat() {
        this.combatSched.runNUpdates();
    }
}

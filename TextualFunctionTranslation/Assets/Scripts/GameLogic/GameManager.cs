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

    [SerializeField] private PrecombatManager pcm;

    void Awake() {
        this.board.initBoard();
        this.combatManager.initPlayers();
    }


    void Update() {
        if(this.gameState == GameStage.combat) {
            this.handleCombat();
        } else if(this.gameState == GameStage.preperation) {
            this.handlePreperation();
        } else { //results TODO make it do stuff el em ef ay oh
            this.gameState = GameStage.preperation;
        }
    }

    private void handlePreperation() {
        this.pcm.handlePrep();
    }

    public void onCombatStart() {
        this.pcm.endPrep();
        this.combatManager.initializeCombat();
        this.gameState = GameStage.combat;
    }

    private void onCombatEnd() {
        //TODO tell game manager to end comabt
        this.gameState = GameStage.result;
    }

    private void handleCombat() {
        this.combatSched.runNUpdates();
    }
}

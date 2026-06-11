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
    [SerializeField] private List<CombatStage> currentLevel; 

    private int currentStage = 0; //TODO: skip tutorial?

    [SerializeField] private PrecombatManager pcm;
    [SerializeField] private ReviewManager rvm;

    void Start() { //starts the game, runs after factories have finished
        this.onPreperationStart();
    }


    void Update() {
        if(this.gameState == GameStage.combat) { 
            this.handleCombat();
        } else if(this.gameState == GameStage.preperation) {
            this.handlePreperation();
        } else { //results TODO make it do stuff el em ef ay oh
            this.handleResults();
        }
    }

    private void onPreperationStart() {
        this.board.initBoard();
        this.combatManager.initPlayers(this.currentLevel[this.currentStage].getCombatLayout());
    }

    private void handlePreperation() {
        this.pcm.handlePrep();
    }

    private void onPreperationEnd() {
        this.gameState = GameStage.combat;
        this.pcm.endPrep();

        this.onCombatStart();
    }

    public void onCombatStart() {
        this.combatManager.initializeCombat();
    }

    private void handleCombat() {
        this.combatSched.runNUpdates();
    }

    public void onCombatEnd() {
        //TODO tell combat manager to end comabt
        this.gameState = GameStage.result;
        this.onResultsStart();
    }

    private void onResultsStart() {
        this.combatManager.runSquaresReveal();
        string ciphertext = this.combatManager.encrypt(this.currentLevel[this.currentStage].getLevelCipher());
        this.combatManager.runCypherAnimation();

        this.rvm.onResultsStart();
        //TODO: display encrypted text
    }

    private void handleResults() {
        this.rvm.handleResults();
    }

    private void onResultsEnd() {
        this.rvm.onResultsEnd();
        this.currentStage++;
        this.gameState = GameStage.preperation;
        this.onPreperationStart();
    }
}

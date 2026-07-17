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
        } else {
            this.handleResults();
        }
    }

    public void onPreperationStart() {
        this.board.initBoard();
        this.combatManager.initPlayers(this.currentLevel[this.currentStage].getCombatLayout(), this.currentLevel[currentStage].getAlphabet());
    }

    public void handlePreperation() {
        this.pcm.handlePrep();
    }

    public void onPreperationEnd() {
        this.gameState = GameStage.combat;
        this.pcm.endPrep();

        this.onCombatStart();
    }

    public void onCombatStart() {
        this.combatManager.initializeCombat();
        this.combatSched.onCombatStart();
    }

    public void handleCombat() {
        this.combatSched.runNUpdates();
    }

    public void onCombatEnd() {
        this.gameState = GameStage.result;
        this.onResultsStart();
    }

    public void onResultsStart() {
        string ciphertext = this.combatManager.encrypt(this.currentLevel[this.currentStage].getLevelCipher());
        StartCoroutine(this.runAnimations(ciphertext));

        this.rvm.onResultsStart();
    }

    private IEnumerator runAnimations(string cypherText) {
        yield return this.combatManager.runCypherAnimation();
        yield return this.combatManager.runSquaresReveal();
        //TODO: display encrypted text
    }

    public void handleResults() {
        this.rvm.handleResults();
    }

    public void onResultsEnd() {
        this.rvm.onResultsEnd();
        this.currentStage++;
        this.gameState = GameStage.preperation;
        this.onPreperationStart();
    }
}

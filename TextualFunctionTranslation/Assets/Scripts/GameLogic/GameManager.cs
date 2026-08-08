using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStage {
    preperation, combat, result_loss, result_win
}

public class GameManager : MonoBehaviour
{
    private GameStage gameState = GameStage.preperation;    

    [SerializeField] private CombatScheduler combatSched;
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private HexBoard board;
    [SerializeField] private List<CombatStage> currentLevel; 
    [SerializeField] private PlayerInfo pi;

    [SerializeField] private HumanPlayer player;

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
        this.pi.setLevel(this.currentLevel[this.currentStage].getStageName());
        this.combatManager.initPlayers(
            this.currentLevel[this.currentStage].getCombatLayout(),
            this.currentLevel[this.currentStage].getAlphabet(),
            this.currentLevel[this.currentStage].getMaxUnits()
        );
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

    public void restartLevel() {
        //TODO: play you lose message
        this.gameState = GameStage.result_loss;
        this.onCombatEnd();
    }

    public void continueToNextLevel() {
        this.gameState = GameStage.result_win;
        this.onCombatEnd();
    }


    public void onCombatEnd() {
        this.onResultsStart(); //TODO button to reflect combat outcome
    }

    public void onResultsStart() {
        string ciphertext = this.combatManager.encrypt(this.currentLevel[this.currentStage].getLevelCipher());
        StartCoroutine(this.runAnimationsAndSwap(ciphertext));

        this.rvm.onResultsStart(ciphertext);
    }

    private IEnumerator runAnimationsAndSwap(string cypherText) {
        yield return this.combatManager.runCypherAnimation();
        yield return new WaitForSeconds(GlobalData.BetweenRevealDuration);
        yield return this.combatManager.runSquaresReveal();
        this.rvm.displayReview();
        this.combatManager.resetToPostCombat();
    }

    public void handleResults() {
        this.rvm.handleResults();
    }

    public void onResultsEnd() {
        if(this.gameState == GameStage.result_win) {
            this.rvm.onResultsEnd();
            this.player.gainMoney(this.currentLevel[this.currentStage].getRewardMoney());
            this.currentStage++;
            this.gameState = GameStage.preperation;
            this.onPreperationStart();
        } else {
            this.rvm.onResultsEnd();
            this.player.loseCombat();
            this.gameState = GameStage.preperation;
            this.onPreperationStart();
        }

        
    }
}

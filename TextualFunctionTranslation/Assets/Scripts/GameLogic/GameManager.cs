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
    [SerializeField] private TutorialStage tut;
    [SerializeField] private PlayerInfo pi;
    [SerializeField] private StatusMessage statusMessage;

    [SerializeField] private HumanPlayer player;
    [SerializeField] private TextAsset dialogueJson;

    private int currentStage = 0;
    private bool tutorialActive = true;
    private bool blockPlusPlus = false; //sometimes you just write stupid shit...

    [SerializeField] private PrecombatManager pcm;
    [SerializeField] private ReviewManager rvm;
    [SerializeField] private SavedGameData data;

    private bool boardLocked = true;

    public void Awake() {
        List<SectionDialogue> dialoguePerSection = JsonUtility.FromJson<LevelDialogue>(dialogueJson.text).levels;
        foreach(SectionDialogue sd in dialoguePerSection) {
            foreach(CombatStage s in this.currentLevel) {
                if(s.getStageName() == sd.level_name) {
                    s.setDialogue(sd.dialogue);
                }
            }
        }
    }

    void Start() { //starts the game, runs after factories have finished
        this.tutorialActive = this.data.showTutorial; //once tutorial starts you can't instantly skip it i think :)
        if(!this.tutorialActive) {
            this.player.gainMoney(GlobalData.StartingMoney + this.tut.getRewardMoney());
            //TODO: make sure player is equiped as if they did the tutorial
        } else {
            this.player.gainMoney(GlobalData.StartingMoney);
            this.tut.startTutorial();
        }
        this.onPreperationStart();
    }

    private CombatStage getCurrentLevel() {
        if(this.tutorialActive) {
            return this.tut;
        }
        return this.currentLevel[this.currentStage];
    }


    public void onTutorialOver() {
        this.tutorialActive = false;
        this.blockPlusPlus = true;
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

    public bool canModifyBoard() {
        return this.boardLocked;
    }

    public void onPreperationStart() {

        this.board.initBoard();
        this.pi.setLevel(this.getCurrentLevel().getStageName());
        this.combatManager.initPlayers(
            this.getCurrentLevel().getCombatLayout(),
            this.getCurrentLevel().getAlphabet(),
            this.getCurrentLevel().getMaxUnits()
        );

        if(this.data.showDialogue && this.getCurrentLevel().hasDialogue(false)) {
            this.getCurrentLevel().getDialogue(false).startDialogue(null);
        } else {
            this.postPrepDialogue();
        }
    }

    private void postPrepDialogue() {
        this.boardLocked = false;
    }

    public void handlePreperation() {
        this.pcm.handlePrep();
    }

    public void onPreperationEnd() {
        this.gameState = GameStage.combat;
        this.pcm.endPrep();
        this.boardLocked = true;

        StartCoroutine(this.onCombatStart());
    }

    public IEnumerator onCombatStart() {
        if(this.tutorialActive) {
            this.tut.onTutorialCombatStart();
        }
        this.combatManager.initializeCombat();
        yield return this.combatManager.precombatAnims();
        this.combatSched.onCombatStart();
    }

    public void handleCombat() {
        this.combatSched.runNUpdates();
    }

    public void restartLevel(bool randomizerSurvived) {
        this.statusMessage.displayMessage("YOU LOSE");
        this.gameState = GameStage.result_loss;
        this.onCombatEnd(false, randomizerSurvived);
    }

    public void continueToNextLevel(bool randomizerSurvived) {
        this.gameState = GameStage.result_win;
        this.onCombatEnd(true, randomizerSurvived);
    }


    public void onCombatEnd(bool wonCombat, bool randomizerSuvived) {
        this.combatSched.combatOver();
        ReviewData rd = new ReviewData(wonCombat, randomizerSuvived);
        this.onResultsStart(wonCombat, rd);
    }

    public void onResultsStart(bool wonCombat, ReviewData rd) {
        if(this.data.showDialogue && this.getCurrentLevel().hasDialogue(true)) {
            this.getCurrentLevel().getDialogue(true).startDialogue(rd);
        } else {
            this.postResultsDialogue(rd);
        }
    }

    private void postResultsDialogue(ReviewData rd) {
        string ciphertext = this.combatManager.encrypt(this.getCurrentLevel().getLevelCipher());
        StartCoroutine(this.runAnimationsAndSwap(ciphertext));

        this.rvm.onResultsStart(ciphertext, rd);
    }

    private IEnumerator runAnimationsAndSwap(string cypherText) {
        yield return this.combatManager.runCypherAnimation();
        yield return new WaitForSeconds(GlobalData.BetweenRevealDuration);
        yield return this.combatManager.runSquaresReveal();
        if(this.tutorialActive) {
            this.tut.onTutorialResultsStart();
        }
        this.rvm.displayReview();
        this.combatManager.resetToPostCombat();
    }

    public void handleResults() {
        this.rvm.handleResults();
    }

    public void onResultsEnd(bool nextStage) {
        if(this.gameState == GameStage.result_win && nextStage) {
            this.rvm.onResultsEnd();
            this.player.gainMoney(this.getCurrentLevel().getRewardMoney());
            if(this.blockPlusPlus) {//"sometimes you just write stupid shit..." shut up dork this is brilliant
                this.blockPlusPlus = false;
            } else {
                this.currentStage++;
            }
            this.gameState = GameStage.preperation;
            this.combatManager.wipeEnemyLayout();
            this.onPreperationStart();
        } else {
            this.rvm.onResultsEnd();
            this.player.loseCombat();
            this.gameState = GameStage.preperation;
            this.onPreperationStart();
        }
    }

    public void doneYapping(ReviewData? rd) {
        if(this.gameState == GameStage.preperation) {
            this.postPrepDialogue();
        } else {
            this.postResultsDialogue(rd.Value);
        }
    }
}

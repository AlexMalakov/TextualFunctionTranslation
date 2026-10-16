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
    [SerializeField] private Player enemy;
    [SerializeField] private TextAsset dialogueJson;
    [SerializeField] private LevelViewer levelView;
    [SerializeField] private Shop shop;

    [SerializeField] private RoyalGuardsTrait enemyRG;

    [SerializeField] private List<RandTMP> scramblableItems;
    private int livingRandomizers = 0;
    private int randomizerFrameCount = 0;
    private int FRAMES_TILL_RAND = 15;

    private int currentStage = 0;
    private bool tutorialActive = true;

    [SerializeField] private PrecombatManager pcm;
    [SerializeField] private ReviewManager rvm;
    [SerializeField] private SavedGameData data;

    [Header ("testing")]
    [SerializeField] private DroolingPlayer drooler; 
    [SerializeField] private bool runningTest;

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

    public CombatStage getCurrentLevel() {
        if(this.tutorialActive) {
            return this.tut;
        }
        return this.currentLevel[this.currentStage];
    }

    void Update() {
        if(this.gameState == GameStage.combat) {
            this.handleCombat();
        } else if(this.gameState == GameStage.preperation) {
            this.handlePreperation();
        } else {
            this.handleResults();
        }

        if(this.livingRandomizers > 0) {
            if(this.randomizerFrameCount % this.FRAMES_TILL_RAND == 0) {
                this.scrambleAllTMP();
            }

            this.randomizerFrameCount = (this.randomizerFrameCount + 1) % this.FRAMES_TILL_RAND;
        } else {
            this.resetAllTMP();
        }
    }

    public bool canModifyBoard() {
        return this.boardLocked;
    }

    public void onPreperationStart() {
        this.board.initBoard();
        this.pi.setLevel(this.getCurrentLevel().getStageName());
        this.pi.setEnemyGloryStacks(this.getCurrentLevel().getEnemyGloryStacks());
        this.enemyRG.loadLevelStacks(this.getCurrentLevel().getEnemyGloryStacks());

        this.combatManager.initPlayers(
            this.getCurrentLevel().getCombatLayout(),
            this.getCurrentLevel().getAlphabet(),
            this.getCurrentLevel().getMaxUnits()
        );

        if(this.data.showDialogue && this.getCurrentLevel().hasDialogue(false)) {
            this.pcm.lockForDialogue(true);
            this.getCurrentLevel().getDialogue(false).startDialogue(null);
        } else {
            this.postPrepDialogue();
        }

        ShopRarity curOdds = this.getCurrentLevel().setShopOdds();
        if(!this.tut.isActive()) {
            this.shop.updateEntries();
        }
        this.shop.lockShop(false);
        this.levelView.onLevelStart(this.getCurrentLevel().getRewardMoney(), this.player.getActiveUnits().Count, this.getCurrentLevel().getMaxUnits(), curOdds);

        
        this.livingRandomizers = this.enemy.countRandomizers();
        Debug.Log("RAND COUNT: " + livingRandomizers);
        if(this.livingRandomizers > 0) {
            this.randomizerFrameCount = 0;
            this.scrambleAllTMP();
        }

        if(runningTest) {
            this.drooler.prepStart();
        }
    }

    private void postPrepDialogue() {
        this.boardLocked = false;
        this.pcm.lockForDialogue(false);
    }

    public void handlePreperation() {
        this.pcm.handlePrep();

        if(runningTest) {
            this.drooler.prepHappening();
        }
    }

    public void onPreperationEnd() {
        if(this.gameState != GameStage.preperation || this.boardLocked) {
            return;
        }
        this.gameState = GameStage.combat;
        this.pcm.endPrep();
        this.boardLocked = true;
        this.shop.lockShop(true);

        StartCoroutine(this.onCombatStart());
    }

    public IEnumerator onCombatStart() {
        if(this.tutorialActive) {
            this.tut.onTutorialCombatStart();
        }
        this.combatManager.initializeCombat();
        if(runningTest) {
            this.drooler.combatStart();
        }
        yield return this.combatManager.precombatAnims();
        this.combatSched.onCombatStart();
    }

    public void handleCombat() {
        this.combatSched.runNUpdates();
    }

    public void restartLevel() {
        this.statusMessage.displayMessage("YOU LOSE");
        this.gameState = GameStage.result_loss;
        this.onCombatEnd(false);
    }

    public void continueToNextLevel() {
        this.gameState = GameStage.result_win;
        this.onCombatEnd(true);
    }


    public void onCombatEnd(bool wonCombat) {
        this.combatSched.combatOver();
        ReviewData rd = new ReviewData(wonCombat);
        this.onResultsStart(wonCombat, rd);
    }

    public void onResultsStart(bool wonCombat, ReviewData rd) {
        if(this.data.showDialogue && this.getCurrentLevel().hasDialogue(true)) {
            this.getCurrentLevel().getDialogue(true).startDialogue(rd);
        } else {
            StartCoroutine(this.postResultsDialogue(rd));
        }
    }



    private IEnumerator postResultsDialogue(ReviewData rd) {
        yield return new WaitForSeconds(4f); //break before the cyphers start
        yield return StartCoroutine(this.runAnimationsAndSwap());

        string ciphertext = this.combatManager.encrypt(this.getCurrentLevel().getLevelCipher());
        this.rvm.onResultsStart(ciphertext, rd);
        this.player.clearMergeQueue();

        if(runningTest) {
            this.drooler.resultsStart(rd.playerWon);
        }
    }

    private IEnumerator runAnimationsAndSwap() {
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
        this.rvm.onResultsEnd();

        if(this.tutorialActive && nextStage) {
            this.player.gainMoney(this.getCurrentLevel().getRewardMoney());
            this.combatManager.wipeEnemyLayout();
            this.gameState = GameStage.preperation;
            this.tutorialActive = false;
        } else if(this.gameState == GameStage.result_win && nextStage) {
            this.player.gainMoney(this.getCurrentLevel().getRewardMoney());
            this.currentStage++;
            this.gameState = GameStage.preperation;
            this.combatManager.wipeEnemyLayout();
        } else {
            this.player.loseCombat();
            this.gameState = GameStage.preperation;

            if(runningTest) {
                this.drooler.resultsEnd();
            }
        }
        this.board.hideAllSquares();
        this.onPreperationStart();
    }

    public void doneYapping(ReviewData? rd) {
        if(this.gameState == GameStage.preperation) {
            this.postPrepDialogue();
        } else {
            StartCoroutine(this.postResultsDialogue(rd.Value));
        }
    }
    

    private void scrambleAllTMP() {
        foreach(RandTMP t in this.scramblableItems) {
            t.scramble();
        }
    }

    private void resetAllTMP() {
        foreach(RandTMP t in this.scramblableItems) {
            t.reset();
        }
    }

    public static string scrambleString(string input) {
        char[] chars = input.ToCharArray();

        for (int i = 0; i < chars.Length; i++) {
            if (chars[i] >= 'a' && chars[i] <= 'z') {
                chars[i] = (char)Random.Range('a', 'z' + 1);
            } else if (chars[i] >= 'A' && chars[i] <= 'Z') {
                chars[i] = (char)Random.Range('A', 'Z' + 1);
            }
        }

        return new string(chars);
    }

    public void onRandomizerDeath() {
        this.livingRandomizers--;
        if(this.livingRandomizers <= 0) {
            this.resetAllTMP();
        }
    }
}

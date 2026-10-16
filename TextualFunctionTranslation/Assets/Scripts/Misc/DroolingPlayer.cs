using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DroolingPlayer : MonoBehaviour
{
    //the smartest game ai ever concevied
    [Header ("level config")]
    [SerializeField] private bool canDrool;
    [SerializeField] private bool isSimulationMode;
    [SerializeField] private bool isCuratedMode;
    [SerializeField] private bool isGauntletMode;

    [Header ("drooler config")]
    [SerializeField] private bool cheatShop; //allows drooler to merge and buy specific units without them being in the shop
    [SerializeField] private int buttonMashCeil; //max amount of clicky clicky till next combat
    [SerializeField] private float sellOdds; //odds of selling a unit randomly (will grief the round)
    [SerializeField] private int minCap;
    [SerializeField] private int maxCap;

    [Header ("gauntlet config")]
    [SerializeField] private int maxFailedAttempts = 1;
    [SerializeField] private int maxBossAttempts = 1;
    [SerializeField] private int maxFramesBetweenActions = 10;
    [SerializeField] private int framesBetweenDialogueLines = 10;
    [SerializeField] [Range(1f, 10f)] private float gauntletTimeScale = 1f;
    [SerializeField] private string bossLogFileName = "drooling_gauntlet.log";

    [Header ("gauntlet reveal speed")]
    [SerializeField] private bool overrideRevealDurations = true;
    [SerializeField] private float squareRevealDuration = 0.05f;
    [SerializeField] private float cypherRevealDuration = 0.05f;
    [SerializeField] private float betweenRevealDuration = 0.05f;
    [SerializeField] private float movieFrameDuration = 0.1f;

    [Header ("plugins to the game")]
    [SerializeField] private List<Button> buttons;
    [SerializeField] private List<Button> shopButtons;
    [SerializeField] private HexBoard board;
    [SerializeField] private GameStateButton gameState;
    [SerializeField] private RetryLevelButton retryButton;
    [SerializeField] private Shop shop;
    [SerializeField] private int totalBenchSlots;
    [SerializeField] private GameManager gm;
    [SerializeField] private SavedGameData data;
    [SerializeField] private ResultsRaycast moviePlayer;
    [SerializeField] private LenseItem lenses;
    [SerializeField] private LevelViewer levelView;
    [SerializeField] private UnitExpander expander;

    [Header ("puppets")]
    [SerializeField] private HumanPlayer player;
    [SerializeField] private Player enemy;
    [SerializeField] private PlayerUnitLayout fakePLayout;
    [SerializeField] private PlayerUnitLayout fakeELayout;

    private List<UnitBlueprint> allUseableUnits;
    private List<Trait> allUseableTraits;

    private static int gauntletRun = 0;

    private Coroutine routine;
    private int sellBudget;
    private int failedAttempts = 0;
    private int bossAttempts = 0;
    private int dialogueFrameCounter = 0;

    private System.Random rng = new System.Random();

    private bool gauntletActive() {
        return this.canDrool && this.isGauntletMode && !this.isSimulationMode && !this.isCuratedMode;
    }

    private void Awake() {
        if(!this.gauntletActive()) {
            return;
        }

        gauntletRun++;
        this.data.showTutorial = false;
        Time.timeScale = this.gauntletTimeScale;
    }

    private void OnDestroy() {
        if(this.gauntletActive()) {
            Time.timeScale = 1f;
        }
    }

    private void Start() {
        if(!canDrool) {
            return;
        }

        if(this.allUseableUnits == null) {
            initUsableUnits();
        }

        if(this.allUseableTraits == null) {
            initUseableTraits();
        }

        if(this.gauntletActive() && this.overrideRevealDurations) {
            GlobalData.SquareRevealDuration = this.squareRevealDuration;
            GlobalData.CypherRevealDuration = this.cypherRevealDuration;
            GlobalData.BetweenRevealDuration = this.betweenRevealDuration;
            GlobalData.MovieFrameDuration = this.movieFrameDuration;
        }
    }

    private void Update() {
        if(!this.gauntletActive()) {
            return;
        }

        this.dialogueFrameCounter = (this.dialogueFrameCounter + 1) % Mathf.Max(1, this.framesBetweenDialogueLines);
        if(this.dialogueFrameCounter != 0) {
            return;
        }

        CombatStage level = this.gm.getCurrentLevel();
        if(level.hasDialogue(false)) {
            level.getDialogue(false).nextLine();
        }
        if(level.hasDialogue(true)) {
            level.getDialogue(true).nextLine();
        }
    }

    private void initUsableUnits() {
        this.allUseableUnits = new List<UnitBlueprint>(UnitFactory.getAllBlueprints());
        for(int i = allUseableUnits.Count - 1; i >= 0; i--) {
            if(allUseableUnits[i].unitName == "rasmusborregaard") {
                allUseableUnits.RemoveAt(i);
            }
        }
    }

    private void initUseableTraits() {
        this.allUseableTraits = new List<Trait>(player.getAllTraits());

        for(int i = allUseableTraits.Count - 1; i >= 0; i--) {
            if(allUseableTraits[i].getTraitName() == "demon" || allUseableTraits[i].getTraitName() == "hero") {
                allUseableTraits.RemoveAt(i);
            }
        }
    }

    public void prepStart() {
        if(isSimulationMode) {
            int cap = nextInt(minCap, maxCap);
            this.givePlayerRandomComp(player, fakePLayout, cap);
            this.givePlayerRandomComp(enemy, fakeELayout, cap);
        } else if(isCuratedMode) {
            int cap = 0;//get cap from level
            //dont need to give enemy a comp, it will have it from the level
            this.givePlayerRandomComp(player, fakePLayout, cap);
        } else if(this.gauntletActive()) {
            this.restartRoutine(this.gauntletPrep());
        }
    }

    public void prepHappening() {
        if(this.isGauntletMode && !this.isSimulationMode && !this.isCuratedMode) {
            return;
        }
        gameState.GetComponent<Button>().onClick.Invoke();
    }

    public void combatStart() {
        if(isSimulationMode) {
            //do nothing
        } else if(isCuratedMode) {
            //do nothing
        } else if(this.gauntletActive()) {
            this.restartRoutine(this.gauntletCombat());
        }
    }

    public void resultsStart(bool playerWon) {
        if(isSimulationMode) {
            retryButton.GetComponent<Button>().onClick.Invoke();
        } else if(isCuratedMode) {
            //force next level
        } else if(this.gauntletActive()) {
            this.restartRoutine(this.gauntletResults(playerWon));
        }
    }

    public void resultsEnd() {
        if(isSimulationMode) {
            enemy.wipeLayoutUnits();
        }
    }

    private void restartRoutine(IEnumerator next) {
        if(this.routine != null) {
            StopCoroutine(this.routine);
        }
        this.routine = StartCoroutine(next);
    }

    private IEnumerator gauntletPrep() {
        yield return null;

        while(this.gm.canModifyBoard()) {
            yield return null;
        }

        this.sellBudget = this.player.getUnitCap() / 4;
        int actions = nextInt(0, this.buttonMashCeil);
        for(int i = 0; i < actions; i++) {
            this.randomPrepAction();
            yield return this.waitFrames();
        }

        yield return this.fixComp();

        this.routine = null;
        this.click(this.gameState.GetComponent<Button>());
    }

    private IEnumerator gauntletCombat() {
        yield return null;

        int actions = nextInt(0, this.buttonMashCeil);
        for(int i = 0; i < actions; i++) {
            this.clickRandom(this.buttonsExcept());
            yield return this.waitFrames();
        }

        this.routine = null;
    }

    private IEnumerator gauntletResults(bool playerWon) {
        yield return null;

        int actions = nextInt(0, this.buttonMashCeil);
        for(int i = 0; i < actions; i++) {
            this.randomResultsAction();
            yield return this.waitFrames();
        }

        if(playerWon) {
            this.failedAttempts = 0;
            this.routine = null;
            this.click(this.gameState.GetComponent<Button>());
            yield break;
        }

        if(this.enemy.getActiveUnits().Any(u => u is DemonKing)) {
            this.bossAttempts++;
            if(this.bossAttempts >= this.maxBossAttempts) {
                this.logBossRun();
                this.restartGame();
                yield break;
            }
        } else {
            this.failedAttempts++;
            if(this.failedAttempts >= this.maxFailedAttempts) {
                this.restartGame();
                yield break;
            }
        }

        this.routine = null;
        this.click(this.retryButton.GetComponent<Button>());
    }

    private IEnumerator waitFrames() {
        int frames = nextInt(1, Mathf.Max(1, this.maxFramesBetweenActions));
        for(int i = 0; i < frames; i++) {
            yield return null;
        }
    }

    private void randomPrepAction() {
        switch(nextInt(0, 5)) {
            case 0:
                this.clickRandom(this.buttonsExcept(this.gameState.GetComponent<Button>()));
                break;
            case 1:
                this.buyRandom(false);
                break;
            case 2:
                this.buyRandom(true);
                break;
            case 3:
                this.dragRandom(this.player.getBench().getUnitsOnBench());
                break;
            case 4:
                this.dragRandom(this.player.getActiveUnits());
                break;
            case 5:
                this.sellRandom();
                break;
        }
    }

    private void randomResultsAction() {
        switch(nextInt(0, 3)) {
            case 0:
                this.clickRandom(this.buttonsExcept(this.gameState.GetComponent<Button>(), this.retryButton.GetComponent<Button>()));
                break;
            case 1:
                this.playRandomMovie();
                break;
            case 2:
                this.moviePlayer.endMovie();
                break;
            case 3:
                this.useRandomLense();
                break;
        }
    }

    private IEnumerator fixComp() {
        yield return this.fillBoard();

        while(this.buyRandom(false)) {
            yield return null;
            yield return this.fillBoard();
        }
    }

    private IEnumerator fillBoard() {
        while(this.player.getActiveUnits().Count < this.player.getUnitCap() && this.player.getBench().getUnitsOnBench().Count > 0) {
            List<HexSquare> empty = this.board.getPlayerSquares().Where(s => s.getUnit() == null).ToList();
            if(empty.Count == 0) {
                yield break;
            }

            List<Unit> benched = this.player.getBench().getUnitsOnBench();
            this.dragTo(benched[nextInt(0, benched.Count - 1)], empty[nextInt(0, empty.Count - 1)]);
            yield return null;
        }
    }

    private bool clickable(Button b) {
        return b.isActiveAndEnabled && b.IsInteractable();
    }

    private void click(Button b) {
        if(this.clickable(b)) {
            b.onClick.Invoke();
        }
    }

    private List<Button> buttonsExcept(params Button[] excluded) {
        return this.buttons.Where(b => !excluded.Contains(b) && this.clickable(b)).ToList();
    }

    private void clickRandom(List<Button> options) {
        if(options.Count == 0) {
            return;
        }
        this.click(options[nextInt(0, options.Count - 1)]);
    }

    private List<int> purchasableShopSlots(bool mergesOnly) {
        List<int> slots = new List<int>();
        List<UnitBlueprint> entries = this.shop.getEntries();

        for(int i = 0; i < Mathf.Min(entries.Count, this.shopButtons.Count); i++) {
            UnitBlueprint ub = entries[i];
            if(!this.clickable(this.shopButtons[i]) || !this.player.canPurchaseUnit(ub.unitName, ub.unitCost)) {
                continue;
            }
            if(mergesOnly && !this.player.canMergeUnit(null, ub.unitName)) {
                continue;
            }
            slots.Add(i);
        }

        return slots;
    }

    private bool buyRandom(bool mergesOnly) {
        List<int> slots = this.purchasableShopSlots(mergesOnly);
        if(slots.Count == 0) {
            return false;
        }

        this.click(this.shopButtons[slots[nextInt(0, slots.Count - 1)]]);
        return true;
    }

    private void dragRandom(List<Unit> pool) {
        if(pool.Count == 0) {
            return;
        }

        Unit u = pool[nextInt(0, pool.Count - 1)];
        if(nextInt(0, 1) == 0) {
            List<HexSquare> allSquares = this.board.filterOperationOnSquares(s => true);
            this.dragTo(u, allSquares[nextInt(0, allSquares.Count - 1)]);
        } else {
            this.dragTo(u, nextInt(0, this.totalBenchSlots - 1));
        }
    }

    private bool pickUp(Unit u) {
        if(!this.player.ownsUnit(u)) {
            return false;
        }

        this.expander.fakeSwapTargets(u);
        this.player.getUnitMoveManager().overrideMovement(u);
        return true;
    }

    private void dragTo(Unit u, HexSquare square) {
        if(this.pickUp(u) && this.player.getUnitLayout().attemptUpdateLayout(u, square)) {
            this.onLayoutChanged();
        }
    }

    private void dragTo(Unit u, int benchSlot) {
        if(this.pickUp(u) && this.player.getBench().updateBenchLayout(u, benchSlot)) {
            this.onLayoutChanged();
        }
    }

    private void sellRandom() {
        if(this.sellBudget <= 0 || this.rng.NextDouble() >= this.sellOdds) {
            return;
        }

        List<Unit> owned = new List<Unit>(this.player.getActiveUnits());
        owned.AddRange(this.player.getBench().getUnitsOnBench());
        if(owned.Count == 0) {
            return;
        }

        Unit u = owned[nextInt(0, owned.Count - 1)];
        if(!this.pickUp(u)) {
            return;
        }

        this.sellBudget--;
        this.player.sellUnit(u);
        this.onLayoutChanged();
    }

    private void onLayoutChanged() {
        this.levelView.onUnitLayoutUpdate(this.player.getActiveUnits().Count, this.player.getUnitCap());
    }

    private void playRandomMovie() {
        List<Unit> visible = new List<Unit>(this.player.getActiveUnits());
        visible.AddRange(this.player.getBench().getUnitsOnBench());
        visible.AddRange(this.enemy.getActiveUnits());
        visible = visible.Where(u => u.gameObject.activeInHierarchy).ToList();

        if(visible.Count == 0) {
            this.moviePlayer.endMovie();
            return;
        }

        Unit chosen = visible[nextInt(0, visible.Count - 1)];
        this.expander.fakeSwapTargets(chosen);
        this.moviePlayer.endMovie();
        this.moviePlayer.onUnitPressed(chosen);
    }

    private void useRandomLense() {
        if(!this.lenses.isToggled() || this.player.getLenses() <= 0) {
            return;
        }

        List<HexSquare> exposed = this.board.filterOperationOnSquares(s => s.getUnit() == null || !s.getUnit().gameObject.activeInHierarchy);
        if(exposed.Count == 0) {
            return;
        }

        this.lenses.useLense(exposed[nextInt(0, exposed.Count - 1)]);
    }

    private void restartGame() {
        this.routine = null;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void logBossRun() {
        string line = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            + " | run " + gauntletRun
            + " | faced the boss on " + this.gm.getCurrentLevel().getStageName()
            + " | boss attempts " + this.bossAttempts
            + System.Environment.NewLine;
        File.AppendAllText(Path.Combine(Application.persistentDataPath, this.bossLogFileName), line);
    }

    private void givePlayerRandomComp(Player p, PlayerUnitLayout fakeL, int cap) {
        if(p.getUnitLayout() != null) {
            p.wipeLayoutUnits();
        }

        foreach(Trait t in p.getAllTraits()) {
            t.wipeCount();
        }

        p.assignLayout(fakeL);

        Dictionary<Unit, HexSquare> comp = arrangeComp(buildComposition(cap, generateConfig(cap)), p is HumanPlayer);
        if(p is HumanPlayer) {
            foreach(Unit u in comp.Keys) {
                ((HumanPlayer)p).fakeDroolingGainUnit(u);
            }
        }

        p.getUnitLayout().fakeInitLayout(comp);
        p.claimUnitsFromLayout();
    }

    private Dictionary<Trait, int> generateConfig(int cap) {
        if(this.allUseableTraits == null) {
            initUseableTraits();
        }

        int numTraits = nextInt(0, Mathf.CeilToInt(cap/4));
        List<Trait> options = new List<Trait>(allUseableTraits);
        Dictionary<Trait, int> traitsChosen = new Dictionary<Trait, int>();

        for(int i = 0; i < numTraits && options.Count > 0; i++) {
            int chosen = nextInt(0, options.Count - 1);
            Trait t = options[chosen];
            options.RemoveAt(chosen);
            List<int> teirs = t.getTraitTiering();
            traitsChosen.Add(t, nextInt(t.getMinimumForActive(), teirs[teirs.Count - 1]));
        }

        return traitsChosen;
    }

    private Dictionary<Unit, HexSquare> arrangeComp(List<Unit> currentComp, bool isPlayer) {
        List<HexSquare> workingCopy = new List<HexSquare>(isPlayer ? board.getPlayerSquares() : board.getEnemySquares());

        Dictionary<Unit, HexSquare> placement = new Dictionary<Unit,HexSquare>();

        foreach(Unit u in currentComp) {
            int chosen = nextInt(0, workingCopy.Count-1);
            placement.Add(u, workingCopy[chosen]);
            workingCopy.RemoveAt(chosen);
        }

        return placement;
    }

    private List<Unit> addDemonsToComp(List<Unit> currentComp, bool mimic, bool randomizer, bool demonKing) {
        if(mimic) {
            //make a new mimic, add them to the game
        }

        if(randomizer) {
            //make a new randomizer, add them to the game
        }

        if(demonKing) {
            //make a new demon king, add them to the game
        }

        return currentComp;
    }

    private List<Unit> buildComposition(int cap, Dictionary<Trait, int> tConfig) {
        if(this.allUseableUnits == null) {
            initUsableUnits();
        }

        List<Unit> comp = new List<Unit>();
        foreach(Trait t in tConfig.Keys) {
            comp.AddRange(sampleFromTrait(t, tConfig[t], comp));
        }

        cap = cap - comp.Count;
        List<UnitBlueprint> options = new List<UnitBlueprint>(allUseableUnits);
        comp.AddRange(sample(cap, options, comp));
        return comp;
    }

    private List<Unit> sampleFromTrait(Trait t, int c, List<Unit> alreadyBringing) {
        foreach(Unit u in alreadyBringing) {
            if(u.getRoleTraitName() == t.getTraitName() || u.getClassTraitName() == t.getTraitName()) {
                c--;
            }
        }

        List<UnitBlueprint> options = new List<UnitBlueprint>();
        foreach(UnitBlueprint ub in allUseableUnits) {
            if(ub.unitPrefab.GetComponent<Unit>().getRoleTraitName() == t.getTraitName()) {
                options.Add(ub);
            } else if(ub.unitPrefab.GetComponent<Unit>().getClassTraitName() == t.getTraitName()) {
                options.Add(ub);
            }
        }

        return sample(c, options, alreadyBringing);
    }

    private List<Unit> sample(int n, List<UnitBlueprint> options, List<Unit> alreadyBringing) {
        foreach(Unit u in alreadyBringing) {
            if(options.Contains(u.getBlueprint())) {
                options.Remove(u.getBlueprint());
            }
        }

        List<Unit> samples = new List<Unit>();
        for(int i = 0; i < n && options.Count > 0; i++) {
            int chosen = nextInt(0, options.Count-1);
            samples.Add(UnitFactory.createUnit(options[chosen].unitName));
            options.RemoveAt(chosen);
        }

        return samples;
    }

    private int nextInt(int min, int max) {
        return rng.Next(min, max + 1);
    }


    /*
    actions this dickhead can take:
        - drag units around
            - drag units to the bench
            - drag units to the layout
            - drag units within the bench
            - drag units within the layout
            - drag units to narnia from bench
            - drag units to narnia from layout

        - sell units
        - buy units

        - start combat
        - mash buttons in combat

        - review unit actions
        - spam lenses
        - spam movies

        - go to the next level
        - retry the current level


    flow:
        - simulation mode:
            - 2 random comps facing eachother

        - level select mode:
            - random drooler comp vs level's comp

        - campaign mode:
            - drooler attempts to beat the game



        - will need to take over game (droolingGameClass to not actually touch game maybe?)
            - Override game methods to go to drooler and not to anything that involves a human player

        game hook audit:
            - Dialogue: calls doneYapping(this.reviewDataSave) -- need to intercept
            - CombatManager: calls continueToNextLevel() restartLevel() onRandomizerDeath() -- more flow logic
            - TutorialStage: calls onTutorialOver() --- need to intercept
            - HumanPlayer: calls canModifyBoard() which is only false between prep start and prep end -- we are fine with this existing if game still controls part of the game
            - GameStateButton: calls onPreperationEnd() and onResultsEnd(true) -- need to intercept both
            - RetryLevelButton: calls onResultsEnd(false) -- need to intercept
            - DialogueUIController: uses it to scramble a string, LEAVE AS IS
            - RandTMP: uses it to scramble a string, LEAVE AS IS

        - audit assesment: leave as is, would rather attack player input sources rather than game itself


        - in addition to game, need to capture every instance where player input is used and replace with drooler
            - need to find all sources of Input. ___
            - currently:
                - Dialogue: call nextLine()
                - tutorialStage: call nextDialogue()
                - CameraRayCast: call the unit layout and bench methods themselves
                - MouseTransform: call the unit layout and bench methods themselves
                - ResultsRaycast: onUnitPressed(Unit u) and endMovie() <- make sure to clal since its called a ton by movie viewer
                - UnitExpander: just call UnitExpander.fakeSwapTargets(Unit u)
                - UnitSellDock: just call HumanPlayer.SellUnit(Unit u)
                - LenseItem: just call LenseItem.useLense(HexSquare s)
            - then need to capture every button
            - currently: GameStateButton, RetryLevelButton, CypherPopUpButton, LenseButton, TraitWidget, ButtonObjectHider, ButtonObjectToggler, CloseButton, TraitCodexButton, UnitCodexButton, ShopButton, CopyButton
    */
}

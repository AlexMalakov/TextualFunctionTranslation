using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DroolingPlayer : MonoBehaviour
{
    //the smartest game ai ever concevied
    [Header ("level config")]
    [SerializeField] private bool canDrool;
    [SerializeField] private bool isSimulationMode;
    [SerializeField] private bool isLevelSelectMode;

    [Header ("drooler config")]
    [SerializeField] private bool cheatShop; //allows drooler to merge and buy specific units without them being in the shop
    [SerializeField] private int buttonMashCeil; //max amount of clicky clicky till next combat
    [SerializeField] private float sellOdds; //odds of selling a unit randomly (will grief the round)
    [SerializeField] private int minCap;
    [SerializeField] private int maxCap;

    [Header ("plugins to the game")]
    [SerializeField] private List<Button> buttons;
    [SerializeField] private HexBoard board;
    [SerializeField] private GameStateButton gameState;
    [SerializeField] private RetryLevelButton retryButton;

    [Header ("puppets")]
    [SerializeField] private Player player;
    [SerializeField] private Player enemy;
    [SerializeField] private PlayerUnitLayout fakePLayout;
    [SerializeField] private PlayerUnitLayout fakeELayout;

    private List<UnitBlueprint> allUseableUnits;
    private List<Trait> allUseableTraits;



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

        //SELECT MODE -- for now lets just latch on
    }

    private void initUsableUnits() {
        this.allUseableUnits = UnitFactory.getAllBlueprints();
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


    public void resultsStart() {
        retryButton.GetComponent<Button>().onClick.Invoke();
    }

    public void resultsEnd() {
        enemy.wipeLayoutUnits();
    }

    public void prepStart() {
        //we want to replace the comps with our comps idealy
        player.wipeLayoutUnits();

        if(enemy.getUnitLayout() != null) {
            enemy.wipeLayoutUnits();
        }

        foreach(Trait t in player.getAllTraits()) {
            t.wipeCount();
        }

        foreach(Trait t in enemy.getAllTraits()) {
            t.wipeCount();
        }

        player.assignLayout(fakePLayout);
        enemy.assignLayout(fakeELayout);

        int cap = nextInt(minCap, maxCap);
        Dictionary<Unit, HexSquare> playerComp = arrangeComp(buildComposition(cap, generateConfig(cap)), true);
        Dictionary<Unit, HexSquare> enemyComp = arrangeComp(buildComposition(cap, generateConfig(cap)), false);

        foreach(Unit u in playerComp.Keys) {
            player.fakeDroolingGainUnit(u);
        }

        player.getUnitLayout().fakeInitLayout(playerComp);
        enemy.getUnitLayout().fakeInitLayout(enemyComp);


        player.claimUnitsFromLayout();
        enemy.claimUnitsFromLayout();
    }

    public void combatStart() {
        //do nothing
    }

    public void prepHappening() {
        gameState.GetComponent<Button>().onClick.Invoke();
    }

    //randomly makes a config
    private Dictionary<Trait, int> generateConfig(int cap) {
        if(this.allUseableTraits == null) {
            initUseableTraits();
        }

        int numTraits = nextInt(0, Mathf.CeilToInt(cap/4));
        List<Trait> options = new List<Trait>(allUseableTraits);
        Dictionary<Trait, int> traitsChosen = new Dictionary<Trait, int>();

        for(int i = 0; i < numTraits; i++) {
            int chosen = nextInt(0, options.Count - 1);
            Trait t = options[chosen];
            List<int> teirs = t.getTraitTiering();
            traitsChosen.Add(t, nextInt(t.getMinimumForActive(), teirs[teirs.Count - 1]));
        }

        return traitsChosen;
    }

    //public void fakeInitLayout(Dictionary<Unit, HexSquare> fakeLayout) {
    private Dictionary<Unit, HexSquare> arrangeComp(List<Unit> currentComp, bool isPlayer) {
        List<HexSquare> ssssss = isPlayer ? board.getPlayerSquares() : board.getEnemySquares();
        List<HexSquare> workingCopy = new List<HexSquare>(ssssss);

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
        Debug.Log("SAMPLING FROM TRAIT: " + t.getTraitName() + " WANT " + c);
        //TODO: returns c units with trait t
        foreach(Unit u in alreadyBringing) {
            if(u.getRoleTraitName() == t.getTraitName() || u.getClassTraitName() == t.getTraitName()) {
                c--;
                Debug.Log("C MINUS MINUS");
            }
        }
        Debug.Log("DOWN TO: " + c);

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

    //without replacement
    private List<Unit> sample(int n, List<UnitBlueprint> options, List<Unit> alreadyBringing) {
        Debug.Log("FRESH SAMPLE ===== FOR " + n + " ALREADY BRINGING " + alreadyBringing.Count);
        foreach(Unit u in alreadyBringing) {
            if(options.Contains(u.getBlueprint())) {
                Debug.Log("Removing from options: " + u.getName());
                options.Remove(u.getBlueprint());
            }
        }

        List<Unit> samples = new List<Unit>();
        for(int i = 0; i < n; i++) {
            int chosen = nextInt(0, options.Count-1);
            Debug.Log("INDEX OUT OF RANGE?" + chosen + ", OUT OF " + options.Count);
            samples.Add(UnitFactory.createUnit(options[chosen].unitName));
            options.RemoveAt(chosen);
        }

        return samples;
    }

    //presses buttons 
    private void clickyclicky() {
        for(int i = 0; i < this.buttonMashCeil; i++) {
            buttons[nextInt(0, buttons.Count-1)].onClick.Invoke();
            //if combat is started, need to end combat oops
        }
    }

    System.Random rng = new System.Random();

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

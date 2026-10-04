using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public enum TutorialAction {
    BuyUnit = 0,
    PlaceUnit = 1,
    DoCombat = 2,
    InCombat = 3,
    ReadScroll = 4,
    LenseUsed = 5,
    ViewMovie = 6,
    DialogueOver = 7
}

[System.Serializable]
public struct TutorialJsonLoad {
    public List<TutorialSection> sections;
}

[System.Serializable]
public struct TutorialSection
{
    public List<DialogueLine> dialogue;
    public TutorialAction section;
}

public class TutorialStage : CombatStage //this is poorly abstracted they have like not much in common tho :/
{
    [SerializeField] private TextAsset tutorialDialogueLoad;

    [SerializeField] private Shop shop;
    [SerializeField] private AxialCoord cordsToPlace;
    [SerializeField] private string unitToWatch;
    [SerializeField] private CypherPopUpButton scrollButton;
    [SerializeField] private GameStateButton combatButton;
    [SerializeField] private CameraRayCast raycast;
    [SerializeField] private ResultsRaycast movie;
    [SerializeField] private GameManager game;
    [SerializeField] private LenseItem lense;
    [SerializeField] private AxialCoord cordsToLense;
    [SerializeField] private HexBoard board;
    [SerializeField] private HumanPlayer player;
    [SerializeField] private BenchSlot leftMostSlot;
    [SerializeField] private RetryLevelButton retry;
    [SerializeField] private ButtonObjectToggler codexButtonTrait;
    [SerializeField] private UnitSellDock sellDock;
    [SerializeField] private DisplayButton swapCypherTrait;
    [SerializeField] private LenseButton lb;
    [SerializeField] private string unitToMove;


    [SerializeField] private DialogueUiController dia;

    private List<TutorialSection> sections;
    private int currentSection;
    private int pointerInSection;
    
    private bool started = false;

    //bad design ik but oh well this is a difficult thing to design smartly i feel
    //as in I don't want to redesign everything for what is literally the last feature I will write
    private bool purchasedUnit;
    private bool placedUnit;
    private bool combatStarted;
    private bool combatOver;
    private bool scrollClosed;
    private bool lenseUsed;
    private bool movieViewed;

    public override bool hasDialogue(bool combatHappened) {
        return false; //we control it not game manager
    }

    public override Dialogue getDialogue(bool combatHappened) {
        return null;
    }

    protected override void onAwake() { //can be an awake because it just loads sections
        this.sections = JsonUtility.FromJson<TutorialJsonLoad>(tutorialDialogueLoad.text).sections;
    }

    public void startTutorial() { //wired
        this.started = true;
        this.currentSection = 0;
        this.pointerInSection = 0;
        this.disableControls(true);
        this.startDialogue();
    }

    private void disableControls(bool toggle) {
        this.combatButton.tutorialLock(toggle);
        this.shop.tutorialLock(toggle);
        this.raycast.tutorialLock(toggle);
        this.scrollButton.tutorialLock(toggle);
        this.movie.tutorialLock(toggle);
        this.lb.tutorialLock(toggle);
        this.lense.tutorialLock(toggle);
        this.retry.tutorialLock(toggle); //society if i abstracted all this garbage
        this.swapCypherTrait.tutorialLock(toggle);
        this.codexButtonTrait.tutorialLock(toggle);
        this.sellDock.tutorialLock(toggle);

        this.shop.tutorialStarted(toggle);
        this.raycast.tutorialStarted(toggle);
        this.movie.tutorialStarted(toggle);
        this.lense.tutorialStarted(toggle);
    }

     //TODO: tie to game manager

    public void Update() {
        if(!this.started) {
            return;
        }

        if(Input.GetKeyDown(KeyCode.Space)) {
            this.nextDialogue();
        }

        if(this.pointerInSection >= this.sections[this.currentSection].dialogue.Count) {
            this.checkSectionCondition();
        }
    }

    private void startDialogue() {
        if(this.sections[this.currentSection].dialogue.Count > 0) {
            DialogueLine nextLine = this.sections[this.currentSection].dialogue[this.pointerInSection];

            this.dia.displayLine(nextLine.on_the_left, nextLine.speaker, nextLine.text, false);
        }
    }

    public void nextDialogue() {
        if(this.pointerInSection < this.sections[this.currentSection].dialogue.Count) {
            this.pointerInSection++;

            if(this.pointerInSection < this.sections[this.currentSection].dialogue.Count) {
                DialogueLine nextLine = this.sections[this.currentSection].dialogue[this.pointerInSection];
                dia.displayLine(nextLine.on_the_left, nextLine.speaker, nextLine.text, false);
            } else {
                this.dia.hideDialogue();
                this.onDialogueOver();
            }
        }
    }

    private void onDialogueOver() {
        switch(this.sections[this.currentSection].section) {
            case TutorialAction.BuyUnit:
                this.shop.tutorialLock(false);
                this.shop.tutorialFlash(true);
                break;
            case TutorialAction.PlaceUnit:
                this.raycast.tutorialLock(false);
                this.board.getHexAt(this.cordsToPlace).tutorialFlash(true);
                this.leftMostSlot.tutorialFlash(true);
                break;
            case TutorialAction.DoCombat:
                this.combatButton.tutorialLock(false);
                this.combatButton.tutorialFlash(true);
                break;
            case TutorialAction.InCombat:
                break;
            case TutorialAction.LenseUsed:
                this.board.getHexAt(this.cordsToLense).tutorialFlash(true);
                this.lense.tutorialLock(false);
                this.lb.tutorialLock(false);
                this.lb.flash(true);
                break;
            case TutorialAction.ReadScroll:
                this.scrollButton.tutorialLock(false);
                this.scrollButton.tutorialFlash(true);
                break;
            case TutorialAction.ViewMovie:
                this.movie.tutorialLock(false);
                this.player.getUnitWithName(this.unitToWatch).getPosition().tutorialFlash(true);
                break;
        }
    }

    private void checkSectionCondition() {
        switch(this.sections[this.currentSection].section) {
            case TutorialAction.BuyUnit: //A one unit shop, so no block needed
                if(this.purchasedUnit) {
                    this.currentSection++;
                    this.pointerInSection = 0;
                    this.startDialogue();
                    this.shop.tutorialFlash(false);
                    this.shop.tutorialLock(true);
                }
                break;
            case TutorialAction.PlaceUnit: //block needed
                if(this.placedUnit) {
                    this.currentSection++;
                    this.pointerInSection = 0;
                    this.startDialogue();
                    this.board.getHexAt(this.cordsToPlace).tutorialFlash(false);
                    this.raycast.tutorialLock(true);
                    this.leftMostSlot.tutorialFlash(false);
                }
                break;
            case TutorialAction.DoCombat:
                if(this.combatStarted) {
                    this.currentSection++;
                    this.pointerInSection = 0;
                    this.startDialogue();
                    this.combatButton.tutorialFlash(false);
                    this.combatButton.tutorialLock(true);
                }
                break;
            case TutorialAction.InCombat:
                if(this.combatOver) {
                    this.currentSection++;
                    this.startDialogue();
                }
                break;
            case TutorialAction.ReadScroll:
                if(this.scrollClosed) {
                    this.currentSection++;
                    this.pointerInSection = 0;
                    this.startDialogue();
                    this.scrollButton.tutorialFlash(false);
                    this.scrollButton.tutorialLock(true);
                }
                break;
            case TutorialAction.LenseUsed:
                if(this.lenseUsed) {
                    this.currentSection++;
                    this.pointerInSection = 0;
                    this.startDialogue();
                    this.board.getHexAt(this.cordsToLense).tutorialFlash(false);
                    this.lense.tutorialLock(true);
                    this.lb.tutorialLock(true);
                    this.lb.flash(false);
                    this.board.revealAllSquares(); //After lense is used dialogue gives the player all squares revealed :)
                }
                break;
            case TutorialAction.ViewMovie: //block needed
                if(this.movieViewed) {
                    this.currentSection++;
                    this.pointerInSection = 0;
                    this.startDialogue();
                    this.player.getUnitWithName(unitToWatch).getPosition().tutorialFlash(false);
                    this.movie.tutorialLock(true);
                }
                break;
            case TutorialAction.DialogueOver:
                this.endTutorial();
                break;
        }
        
        
    }

    public void onUnitBought() {
        this.purchasedUnit = true;
    }

    public bool isTutorialPlacement(HexSquare dest, Unit moved) {
        this.placedUnit = this.placedUnit || (this.cordsToPlace.equal(dest.getCoords()) && moved.getName() == this.unitToMove);
        return this.placedUnit;
    }

    //I can only happen once we've purchased and placed a unit
    public void onTutorialCombatStart() { //wired
        this.combatStarted = true;
    }

    //I can only happen during scroll closed and movie viewed
    public void onTutorialResultsStart() { //wired
        this.combatOver = true;
    }


    public void onScrollClosed() {
        this.scrollClosed = true;
    }

    public bool onLenseUse(HexSquare s) {
        this.lenseUsed = this.lenseUsed || (s.getCoords().equal(this.cordsToLense));
        return this.lenseUsed;
    }

    public bool isMovieUnit(Unit unitWatched) {
        return this.unitToWatch == unitWatched.getName();
    }

    public void onMovieEnded() {
        this.movieViewed = true;
    }

    private void endTutorial() {
        this.disableControls(false);
        this.started = false;
        this.game.onTutorialOver(); //TODO: this doesn't end the stage this just undoes all controls
    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum TutorialAction {
    BuyUnit,
    PlaceUnit,
    DoCombat,
    InCombat,
    ReadScroll,
    LenseUsed, //TODO add me
    ViewMovie,
    DialogueOver //TODO add me
}

[System.Serializable]
public struct TutorialSection
{
    public List<DialogueLine> dialogue;
    public TutorialAction action;
}

public class TutorialStage : CombatStage //this is poorly abstracted they have like not much in common tho :/
{
    [SerializeField] private List<TutorialSection> sections;

    [SerializeField] private Shop shop;
    [SerializeField] private HexSquare squareToPlace;
    [SerializeField] private Unit unitToWatch;
    [SerializeField] private CypherPopUpButton scrollButton;
    [SerializeField] private GameStateButton combatButton;
    [SerializeField] private CameraRayCast raycast;
    [SerializeField] private ResultsRaycast movie;
    [SerializeField] private GameManager game;
    [SerializeField] private LenseItem lense;
    [SerializeField] private HexSquare squareToLense;

    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private Dictionary<string, Sprite> nameToIcon;    
    [SerializeField] private Image talkerSprite;
    [SerializeField] private TMP_Text dialogueText;

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

    public void startTutorial() { //wired
        this.started = true;
        this.currentSection = 0;
        this.pointerInSection = 0;
        this.disableControls(true);
    }

    private void disableControls(bool toggle) {
        this.combatButton.tutorialLock(toggle);
        this.shop.tutorialLock(toggle);
        this.raycast.tutorialLock(toggle);
        this.scrollButton.tutorialLock(toggle);
        this.movie.tutorialLock(toggle);
        this.lense.tutorialLock(toggle);

        this.shop.tutorialStarted(toggle);
        this.raycast.tutorialStarted(toggle);
        this.movie.tutorialStarted(toggle);
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

    private void nextDialogue() {
        if(this.pointerInSection < this.sections[this.currentSection].dialogue.Count) {
            this.dialogueBox.SetActive(true);
            this.pointerInSection++;

            if(this.pointerInSection < this.sections[this.currentSection].dialogue.Count) {
                DialogueLine nextLine = this.sections[this.currentSection].dialogue[this.pointerInSection];
                this.dialogueText.text = nextLine.dialogue;
                this.talkerSprite.sprite = this.nameToIcon[nextLine.speaker];

                if(nextLine.onTheLeft) {
                    this.dialogueBox.GetComponent<RectTransform>().localScale  = new Vector3(1, 1, 1);
                } else {
                    this.dialogueBox.GetComponent<RectTransform>().localScale  = new Vector3(-1, 1, 1);
                }
            } else {
                this.dialogueBox.SetActive(false);
                this.onDialogueOver();
            }
        }
    }

    private void onDialogueOver() {
        switch(this.sections[this.currentSection].action) {
            case TutorialAction.BuyUnit:
                this.shop.tutorialLock(false);
                this.shop.tutorialFlash(true);
                break;
            case TutorialAction.PlaceUnit:
                this.raycast.tutorialLock(false);
                this.squareToPlace.tutorialFlash(true);
                break;
            case TutorialAction.DoCombat:
                this.combatButton.tutorialLock(false);
                this.combatButton.tutorialFlash(true);
                break;
            case TutorialAction.InCombat:
                break;
            case TutorialAction.LenseUsed:
                this.squareToLense.tutorialFlash(true);
                this.lense.tutorialLock(false);
                break;
            case TutorialAction.ReadScroll:
                this.scrollButton.tutorialLock(false);
                this.scrollButton.tutorialFlash(true);
                break;
            case TutorialAction.ViewMovie:
                this.movie.tutorialLock(false);
                this.unitToWatch.getPosition().tutorialFlash(true);
                break;
        }
    }

    private void checkSectionCondition() {
        switch(this.sections[this.currentSection].action) {
            case TutorialAction.BuyUnit: //A one unit shop, so no block needed
                if(this.purchasedUnit) {
                    this.currentSection++;
                    this.pointerInSection = 0;
                    this.shop.tutorialFlash(false);
                    this.shop.tutorialLock(true);
                }
                break;
            case TutorialAction.PlaceUnit: //block needed
                if(this.placedUnit) {
                    this.currentSection++;
                    this.pointerInSection = 0;
                    this.squareToPlace.tutorialFlash(false);
                    this.raycast.tutorialLock(true);
                }
                break;
            case TutorialAction.DoCombat:
                if(this.combatStarted) {
                    this.currentSection++;
                    this.pointerInSection = 0;
                    this.combatButton.tutorialFlash(false);
                    this.combatButton.tutorialLock(true);
                }
                break;
            case TutorialAction.InCombat:
                if(this.combatOver) {
                    this.currentSection++;
                }
                break;
            case TutorialAction.ReadScroll:
                if(this.scrollClosed) {
                    this.currentSection++;
                    this.pointerInSection = 0;
                    this.scrollButton.tutorialFlash(false);
                    this.scrollButton.tutorialLock(true);
                }
                break;
            case TutorialAction.LenseUsed:
                if(this.lenseUsed) {
                    this.currentSection++;
                    this.pointerInSection = 0;
                    this.squareToLense.tutorialFlash(false);
                    this.lense.tutorialLock(true);
                }
                break;
            case TutorialAction.ViewMovie: //block needed
                if(this.movieViewed) {
                    this.currentSection = 0;
                    this.pointerInSection = 0;
                    this.unitToWatch.getPosition().tutorialFlash(false);
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

    public bool isTutorialPlacement(HexSquare dest) {
        this.placedUnit = this.placedUnit || (this.squareToPlace == dest);
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
        this.lenseUsed = this.lenseUsed || (s == this.squareToLense);
        return this.lenseUsed;
    }

    public bool onMovieEnded(Unit unitWatched) {
        this.movieViewed = this.movieViewed || (this.unitToWatch == unitWatched);
        return this.movieViewed;
    }

    private void endTutorial() {
        this.disableControls(false);
        this.started = false;
        this.game.onTutorialOver(); //TODO: this doesn't end the stage this just undoes all controls
    }

    
}

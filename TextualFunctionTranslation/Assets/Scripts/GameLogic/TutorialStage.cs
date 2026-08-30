using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TutorialAction {
    BuyUnit,
    PlaceUnit,
    DoCombat,
    InCombat,
    ReadScroll,
    ViewMovie
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

    private int currentSection;
    private int pointerInSection;
    
    private bool started = false;

    //bad design ik but oh well this is a difficult thing to design smartly i feel
    private bool purchasedUnit;
    private bool placedUnit;
    private bool combatStarted;
    private bool combatOver;
    private bool scrollClosed;
    private bool movieViewed;

    public override bool hasDialogue(bool combatHappened) {
        return false; //we control it not game manager
    }

    public override Dialogue getDialogue(bool combatHappened) {
        return null;
    }

    //TODO: On Prep start will call me :), inform prep manager that the tutorial is active?
    public void startTutorial() {
        this.started = true;
        this.currentSection = 0;
        this.pointerInSection = 0;

        //TODO: disable everything
        //TO DISABLE: shop, unit mouse moving, all real buttons
    }

    private void disableControls() {
        if(this.currentSection >= this.sections.Count) {
            this.combatButton.tutorialLock(false);
            this.shop.tutorialLock(false);
            this.raycast.tutorialLock(false);
            this.scrollButton.tutorialLock(false);
            this.movie.tutorialLock(false);
            return;
        }

        this.combatButton.tutorialLock(true);
        this.shop.tutorialLock(true);
        this.raycast.tutorialLock(true);
        this.scrollButton.tutorialLock(true);
        this.movie.tutorialLock(true);

        switch(this.sections[this.currentSection].action) {
            case TutorialAction.BuyUnit:
                this.shop.tutorialLock(false);
                break;
            case TutorialAction.PlaceUnit:
                this.raycast.tutorialLock(false);
                break;
            case TutorialAction.DoCombat:
                this.combatButton.tutorialLock(false);
                break;
            case TutorialAction.InCombat:
                break;
            case TutorialAction.ReadScroll:
                this.scrollButton.tutorialLock(false);
                break;
            case TutorialAction.ViewMovie:
                this.movie.tutorialLock(false);
                break;
        }
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
            this.pointerInSection++;
            //TODO: hide current dialogue

            if(this.pointerInSection < this.sections[this.currentSection].dialogue.Count) {
                //TODO: show next dialogue
            }
        }
    }

    private void checkSectionCondition() {
        switch(this.sections[this.currentSection].action) {
            case TutorialAction.BuyUnit: //A one unit shop
                if(this.purchasedUnit) {
                    this.currentSection++;
                    this.pointerInSection = 0;
                    this.shop.tutorialFlash(false);
                    this.squaredToPlace.tutorialFlash(true);
                }
                break;
            case TutorialAction.PlaceUnit:
                if(this.placedUnit) {
                    this.currentSection++;
                    this.pointerInSection = 0;
                    this.squareToPlace.tutorialFlash(false);
                    this.combatButton.tutorialFlash(true);
                }
                break;
            case TutorialAction.DoCombat:
                if(this.combatStarted) {
                    this.currentSection++;
                    this.pointerInSection = 0;
                    this.combatButton.tutorialFlash(false);
                }
                break;
            case TutorialAction.InCombat:
                if(this.combatOver) {
                    this.currentSection++;
                    this.scrollButton.tutorialFlash(true);
                }
                break;
            case TutorialAction.ReadScroll:
                if(this.scrollClosed) {
                    this.currentSection++;
                    this.pointerInSection = 0;
                    this.scrollButton.tutorialFlash(false);
                    this.unitToWatch.getPosition().tutorialFlash(true);
                }
                break;
            case TutorialAction.ViewMovie:
                if(this.movieViewed) {
                    this.currentSection++;
                    this.pointerInSection = 0;
                    this.unitToWatch.getPosition().tutorialFlash(false);

                    this.game.onTutorialOver();
                }
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
    public void onTutorialCombatStart() {
        this.combatStarted = true;
    }

    //I can only happen during scroll closed and movie viewed
    public void onTutorialResultsStart() {
        this.combatOver = true;
    }


    public void onScrollClosed() {
        this.scrollClosed;
    }

    public bool onMovieEnded(Unit unitWatched) {
        this.movieViewed = this.movieViewed || (this.unitToWatch == unitWatched);
        return this.movieViewed;
    }

    private void endTutorial() {
        this.disableControls();
        this.currentSection = 0;
        this.started = false;
        this.game.onTutorialOver(); //TODO: this doesn't end the stage this just undoes all controls
    }

    
}

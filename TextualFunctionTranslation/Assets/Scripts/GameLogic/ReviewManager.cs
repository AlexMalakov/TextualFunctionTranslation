using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public struct ReviewData {
    public bool playerWon;

    public ReviewData(bool playerWon) {
        this.playerWon = playerWon;
    }
}

public class ReviewManager : MonoBehaviour
{

    [SerializeField] private GameStateButton gameStateButton;
    [SerializeField] private CypherPopUpButton cypherButton;
    [SerializeField] private CypherTextDisplay display;
    [SerializeField] private ResultsRaycast moviePlayer;
    [SerializeField] private LenseButton lb;
    [SerializeField] private LenseItem li;
    [SerializeField] private RetryLevelButton retry;

    public void Awake() {
        this.retry.enableButton(false);
        this.lb.showButton(false);
        this.cypherButton.setDisplay(false);
    }

    public void onResultsStart(string txt, ReviewData data) {
        this.gameStateButton.runResults(true, data.playerWon);
        this.display.setCypherText(txt);
        this.lb.showButton(true);
        this.retry.enableButton(true);
    }


    public void handleResults() {
        this.moviePlayer.checkUserInteraction();
        this.li.checkUserInteraction();

        
    }

    public void onResultsEnd() {
        this.gameStateButton.runResults(false, true);
        this.cypherButton.setDisplay(false);
        this.display.setDisplay(false);
        this.moviePlayer.runResults(false);
        this.lb.showButton(false);
        this.retry.enableButton(false);
    }

    public void displayReview() {
        this.cypherButton.setDisplay(true);
        this.display.setDisplay(true);
        this.moviePlayer.runResults(true);
    }


}

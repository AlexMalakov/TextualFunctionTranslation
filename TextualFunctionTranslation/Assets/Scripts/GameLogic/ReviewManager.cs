using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviewManager : MonoBehaviour
{

    [SerializeField] private GameStateButton gameStateButton;
    [SerializeField] private CypherPopUpButton cypherButton;
    [SerializeField] private CypherTextDisplay display;
    [SerializeField] private ResultsRaycast moviePlayer;
    [SerializeField] private LenseButton lb;

    public void onResultsStart(string txt) {
        this.gameStateButton.runResults(true);
        this.display.setCypherText(txt);
        this.lb.showButton(true;)
    }

    public void handleResults() {
        moviePlayer.checkUserInteraction();
    }

    public void onResultsEnd() {
        this.gameStateButton.runResults(false);
        this.cypherButton.setDisplay(false);
        this.display.setDisplay(false);
        this.moviePlayer.runResults(false);
        this.lb.showButton(false);
    }

    public void displayReview() {
        this.cypherButton.setDisplay(true);
        this.display.setDisplay(true);
        this.moviePlayer.runResults(true);
    }
}

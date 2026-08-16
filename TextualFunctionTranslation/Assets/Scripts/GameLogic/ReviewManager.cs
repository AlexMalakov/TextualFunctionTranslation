using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReviewManager : MonoBehaviour
{

    [SerializeField] private GameStateButton gameStateButton;
    [SerializeField] private CypherPopUpButton cypherButton;
    [SerializeField] private CypherTextDisplay display;
    [SerializeField] private ResultsRaycast moviePlayer;
    [SerializeField] private LenseButton lb;
    [SerializeField] private List<TMP_Text> scramblableItems;

    public void onResultsStart(string txt, bool wonCombat) {
        this.display.setCypherText(txt);
        this.lb.showButton(true);
    }

    public void updateButton(bool wonCombat) {
        this.gameStateButton.runResults(true, wonCombat);
    }

    public void handleResults() {
        moviePlayer.checkUserInteraction();
    }

    public void onResultsEnd() {
        this.gameStateButton.runResults(false, true);
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

    public List<TMP_Text> getRandomizerText() {
        returnm this.scramblableItems;
    }
}

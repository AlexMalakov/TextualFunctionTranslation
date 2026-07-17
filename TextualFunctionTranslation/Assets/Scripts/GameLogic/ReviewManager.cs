using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviewManager : MonoBehaviour
{

    [SerializeField] private GameStateButton gameStateButton;
    [SerializeField] private CypherPopUpButton cypherButton;
    [SerializeField] private CypherTextDisplay display;

    public void onResultsStart() {
        this.gameStateButton.runResults(true);
        this.cypherButton.setDisplay(true);
        this.display.setDisplay(true);
    }

    public void handleResults() {

    }

    public void onResultsEnd() {
        this.gameStateButton.runResults(false);
        this.cypherButton.setDisplay(false);
        this.display.setDisplay(false);
    }
}

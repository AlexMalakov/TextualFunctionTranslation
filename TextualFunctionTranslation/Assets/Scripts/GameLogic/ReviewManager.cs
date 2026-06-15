using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviewManager : MonoBehaviour
{


    [SerializeField] private GameStateButton gameStateButton;

    public void onResultsStart() {
        this.gameStateButton.runResults(true);
    }

    public void handleResults() {

    }

    public void onResultsEnd() {
        this.gameStateButton.runResults(false);
    }
}

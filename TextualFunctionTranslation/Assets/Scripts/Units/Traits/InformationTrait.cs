using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InformationTrait : Trait
{
    private List<HexSquare> squaresToReveal = new List<HexSquare>();

    

    protected void logSquare(HexSquare square) {
        this.squaresToReveal.Add(square);
    }


    public override void runSquareReveal() {
        foreach(HexSquare sq in this.squaresToReveal) {
            sq.displayValue(true);
            sq.highlightSquare(new Color(255, 180, 125), .5f);      
        }
    }

    public override void resetRevealedSquares() {
        foreach(HexSquare sq in this.squaresToReveal) {
            sq.displayValue(false);
        }

        this.squaresToReveal = new List<HexSquare>();
    }

}

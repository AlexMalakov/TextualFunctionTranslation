using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MartyrTrait : CypherTrait
{
    [SerializeField] private CombatManager cm;
    
    public override void onUnitDeath(Unit traitHaver) {
        
    }

    //TODO: some kind of visual element?
    private void applyInDirection(Unit originUnit, int qOffset, int rOffset, int sOffset) {
        List<HexSquare> squares = this.getSquaresInDirection(originUnit, qOffset, rOffset, sOffset);
        if(s.Count == 1) {
            if(squares.getUnit() != null) {
                //TODO: debuff unit
            }
            return;
        } 

        List<CharacterTransformation> cypher = new List<CharacterTransformation>();
        foreach(HexSquare s in ) {
            if(s.getUnit() != null) {
                //TODO: debuff unit
            }


        }
    }

    private List<HexSquare> getSquaresInDirection(Unit originUnit, int qOffset, int rOffset, int sOffset) {
        List<HexSquare> squares = new List<HexSquare>();
        HexSquare iter = originUnit.getPosition();
        int distance = getNumberActive() + 1;

        for(int i = 1; i < distance; i++) {
            iterC = iter.getCoords();
            HexSquare check = this.cm.getBoard().getHexAt(new AxialCoord(iterC.q + qOffset, iterC.r + rOffset, iterC.s + sOffset));
            if(!check.isSquare()) {
                break;
            }

            iter = check;
            squares.Add(iter);
        }

        return squares;
    }
}

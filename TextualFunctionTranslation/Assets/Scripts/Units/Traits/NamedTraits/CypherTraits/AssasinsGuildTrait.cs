using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AssasinsGuildTrait : CypherTrait
{
    [SerializeField] private CombatManager cm;
    //on combat start, and on unit defeat?

    public override void onPrecombatStart() {
        List<CharacterTransformation> cypher = new List<CharacterTransformation>();

        Func<HexSquare, bool> fakeFilter = (HexSquare s) => {
            return true;
        };

        List<HexSquare> possibleTargets = this.cm.getBoard().filterOperationOnSquares(fakeFilter);
        foreach(HexSquare s in possibleTargets) {
            cypher.Add(new CharacterTransformation(s, this.cm.getBoard().getHexAt(s.getCoords().getMirroredCoord())));
        }

        this.addToCypher(cypher);
    }

    public override void preCombatFinal(Unit traitHaver) {
        AxialCoord mirrored = traitHaver.getPosition().getCoords().getMirroredCoord();

        if(this.cm.getBoard().getHexAt(mirrored).getUnit() == null) {
            //TODO: send cm to this square
            return;
        }

        List<HexSquare> suspects = new List<HexSquare>();
        for(int radius = 1; radius < 8; radius++) { //8 is overkill lets be real here
            suspects.AddRange(this.getSquaresInRadius(this.cm.getBoard().getHexAt(mirrored), radius, this.cm.getBoard()));
            foreach(HexSquare s in suspects) {
                if(s.getUnit() != null) {
                    //TODO: send unit to this square
                    return;
                }
            }
        }
    }

    public override string getTraitName() {
        return "assasins";
    }

    protected override int getMinimumForActive() {
        return 2;
    }
}

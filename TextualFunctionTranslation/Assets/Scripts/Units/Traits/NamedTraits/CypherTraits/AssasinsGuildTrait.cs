using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AssasinsGuildTrait : CypherTrait
{
    [SerializeField] private CombatManager cm;
    [SerializeField] private UnitMovementManager umm;
    //todo: untargetable for the first 3 seconds

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
        OffsetCoord readable = mirrored.toOffset();
        if(this.cm.getBoard().getHexAt(mirrored).getUnit() == null) {
            traitHaver.placeUnitOnSquare(this.cm.getBoard().getHexAt(mirrored), false);
            StartCoroutine(this.umm.boingTo(traitHaver, this.cm.getBoard().getHexAt(mirrored).transform.position));
            return;
        }

        List<HexSquare> suspects = new List<HexSquare>();
        for(int radius = 1; radius < 8; radius++) { //8 is overkill lets be real here
            suspects.AddRange(this.getSquaresInRadius(this.cm.getBoard().getHexAt(mirrored), radius, this.cm.getBoard()));
            foreach(HexSquare s in suspects) {
                if(s.getUnit() != null) {
                    traitHaver.placeUnitOnSquare(s, false);
                    StartCoroutine(this.umm.boingTo(traitHaver, s.transform.position));
                    return;
                }
            }
        }
    }



    public override string getTraitName() {
        return "assassin";
    }

    public override string getTraitDescription() {
        return ("A secretive assassins guild involved in all downfalls of all nobels and aristocrats in the kingdom. "
                    + "At the start of combat jump to an unocupied square on the opponents side of the board.");
    }

    protected override int getMinimumForActive() {
        return 2;
    }

    public override List<int> getTraitTiering() {
        return new List<int>{2, 4};
    }
}

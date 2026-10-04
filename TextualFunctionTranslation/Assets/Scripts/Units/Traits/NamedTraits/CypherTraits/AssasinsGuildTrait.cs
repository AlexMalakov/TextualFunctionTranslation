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
        this.log.logEvent(new CombatEvent(traitHaver, traitHaver, CombatEventType.Start));

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
                if(s.getUnit() == null) {
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

    public override string getTraitNarrative() {
        return "A secrative guild of cut-throats and assassins.";
    }

    public override string getTraitEffect() {
        return "At the start of combat, leaps to the other size. Gains 3/5 seconds of untargetability on leap.";
    }


    public override int getMinimumForActive() {
        return 2;
    }

    public override List<int> getTraitTiering() {
        return new List<int>{2, 4};
    }
}

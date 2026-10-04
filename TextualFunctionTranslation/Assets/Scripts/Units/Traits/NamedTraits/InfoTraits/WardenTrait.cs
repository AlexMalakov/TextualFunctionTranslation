using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WardenTrait : InformationTrait
{
    [SerializeField] private CombatManager cm;
    [SerializeField] private float smallBoostAmount;
    [SerializeField] private float largeBoostAmount;

    public override void onCombatStart(Unit traitHaver) {

        this.log.logEvent(new CombatEvent(traitHaver, traitHaver, CombatEventType.Start));

        List<HexSquare> close2 = this.getBackTwoSquares(traitHaver.getPosition(), traitHaver);
        foreach(HexSquare s in this.getBackTwoSquares(traitHaver.getPosition(), traitHaver)) {
            foreach(HexSquare s2 in this.getBackTwoSquares(s, traitHaver)) {
                if(!close2.Contains(s2)) {
                    close2.Add(s2);
                }
            }
        }

        float buffAmt = this.getNumberActive() > 2 ? this.largeBoostAmount : this.smallBoostAmount;
        Operation buffOpp = new Operation(OperationType.PostAdd, buffAmt, "Warden Health Buff " + traitHaver.getName(), false);

        foreach(HexSquare s in close2) {
            if(s.isSquare()) {
                if(s.getUnit() != null) {
                    s.getUnit().buffStat(BuffableStats.Health, buffOpp, traitHaver);
                }
                this.logSquare(s);
            }
        }
    }

    private List<HexSquare> getBackTwoSquares(HexSquare origin, Unit traitHaver) { //
        if(origin == null || !origin.isSquare()) {
            return new List<HexSquare>();
        }

        AxialCoord pos = traitHaver.getPosition().getCoords();
        HexSquare leftSquare;
        HexSquare rightSquare;
        if(cm.isPlayerUnit(traitHaver)) { //just dont use r
            leftSquare = cm.getBoard().getHexAt(new AxialCoord(pos.q - 1, pos.r + 1, pos.s));
            rightSquare = cm.getBoard().getHexAt(new AxialCoord(pos.q, pos.r + 1, pos.s - 1));
        } else {
            leftSquare = cm.getBoard().getHexAt(new AxialCoord(pos.q + 1, pos.r - 1, pos.s));
            rightSquare = cm.getBoard().getHexAt(new AxialCoord(pos.q, pos.r - 1, pos.s + 1));
        }

        return new List<HexSquare>(){leftSquare, rightSquare};
    }

    public override string getTraitName() {
        return "warden";
    }

    public override string getTraitNarrative() {
        return "Burly knights who shield the kingdom's weakest from danger.";
    }

    public override string getTraitEffect() {
        return (
            "Grants the units behind it in a triangle of size 2 " + smallBoostAmount + "/" + largeBoostAmount + " bonus health. Stacks with bonuses from other wardens."
        );
    }


    public override int getMinimumForActive() {
        return 2;
    }

    public override List<int> getTraitTiering() {
        return new List<int>{2, 4};
    }

}

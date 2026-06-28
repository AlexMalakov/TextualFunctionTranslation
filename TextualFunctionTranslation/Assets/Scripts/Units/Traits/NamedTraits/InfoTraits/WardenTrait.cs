using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WardenTrait : InformationTrait
{
    [SerializeField] private CombatManager cm;
    [SerializeField] private float smallBoostAmount;
    [SerializeField] private float largeBoostAmount;

    public override void onCombatStart(Unit traitHaver) {
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

        float buffAmt = this.getNumberActive() > 2 ? this.largeBoostAmount : this.smallBoostAmount;
        Operation buffOpp = new Operation(OperationType.PreMultiply, buffAmt, "Warden Health Buff ");

        if(leftSquare.isSquare()) {
            if(leftSquare.getUnit() != null) {
                leftSquare.getUnit().buffStat(BuffableStats.Health, buffOpp, traitHaver);
            }
            this.logSquare(leftSquare);
        }

        if(rightSquare.isSquare()) {
            if(rightSquare.getUnit() != null) {
                rightSquare.getUnit().buffStat(BuffableStats.Health, buffOpp, traitHaver);
            }
            this.logSquare(rightSquare);
        }
    }

    public override string getTraitName() {
        return "warden";
    }

    protected override int getMinimumForActive() {
        return 2;
    }

    public override List<int> getTraitTiering() {
        return new List<int>{2, 4};
    }

}

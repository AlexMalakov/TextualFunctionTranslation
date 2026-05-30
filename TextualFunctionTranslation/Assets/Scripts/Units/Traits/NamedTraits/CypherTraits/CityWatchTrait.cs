using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityWatchTrait : CypherTrait
{
    [SerializeField] private CombatManager cm;
    [SerializeField] private float healthBuff;
    [SerializeField] private float damageBuff;
    [SerializeField] private float fullTraitMultiplier;

    //TODO: should this trait's buffs stack
    public override void onCombatStart(Unit traitHaver){
        HexSquare start = traitHaver.getPosition();
        List<Unit> leftBuff = new List<Unit>();
        List<Unit> rightBuff = new List<Unit>();
        
        List<CharacterTransformation> loopdeloop = new List<CharacterTransformation>();
        HexSquare rightEdge;

        while(true) {
            AxialCoord sCords = start.getCoords();
            HexSquare next = this.cm.getBoard().getHexAt(new AxialCoord(sCords.q + 1, sCords.r, sCords.s - 1));

            if(!next.isSquare()) {
                rightEdge = start;
                break;
            }

            if(next.getUnit() != null) {
                rightBuff.Add(next.getUnit());
            }

            loopdeloop.Add(new CharacterTransformation(next, start));
            start = next;
        }

        HexSquare leftEdge;
        while(true) {
            AxialCoord sCords = start.getCoords();
            HexSquare next = this.cm.getBoard().getHexAt(new AxialCoord(sCords.q - 1, sCords.r, sCords.s + 1));

            if(!next.isSquare()) {
                leftEdge = start;
                break;
            }

            if(next.getUnit() != null) {
                leftBuff.Add(next.getUnit());
            }

            loopdeloop.Add(new CharacterTransformation(start, next));
            start = next;
        }

        float amt = this.getNumberActive() > 2 ? this.healthBuff * this.fullTraitMultiplier : this.healthBuff;
        Operation buff = new Operation(OperationType.PreMultiply, amt, "city watch health buff");
        foreach(Unit u in leftBuff) {
            u.buffStat(BuffableStats.Health, buff);
        }

        amt = this.getNumberActive() > 2 ? this.damageBuff * this.fullTraitMultiplier : this.damageBuff;
        buff = new Operation(OperationType.PreMultiply, amt, "city watch damage buff");
        foreach(Unit u in rightBuff) {
            u.buffStat(BuffableStats.Damage, buff);
        }


        loopdeloop.Add(new CharacterTransformation(leftEdge, rightEdge));
        this.addToCypher(loopdeloop);
    }

    private List<Unit> getAllUnitsInDirection(HexSquare start, int direction) {

        List<Unit> allUnits = new List<Unit>();
        while(true) {
            AxialCoord sCords = start.getCoords();
            start = this.cm.getBoard().getHexAt(new AxialCoord(sCords.q + direction, sCords.r, sCords.s - direction));

            if(!start.isSquare())
                return allUnits;

            if(start.getUnit() != null)
                allUnits.Add(start.getUnit());
        }
    }

    public override string getTraitName() {
        return "city watch";
    }

    protected override int getMinimumForActive() {
        return 2;
    }
}

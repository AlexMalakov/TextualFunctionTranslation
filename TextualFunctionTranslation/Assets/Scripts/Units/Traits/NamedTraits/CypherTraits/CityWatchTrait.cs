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
            u.buffStat(BuffableStats.Health, buff, traitHaver);
        }

        amt = this.getNumberActive() > 2 ? this.damageBuff * this.fullTraitMultiplier : this.damageBuff;
        buff = new Operation(OperationType.PreMultiply, amt, "city watch damage buff");
        foreach(Unit u in rightBuff) {
            u.buffStat(BuffableStats.Damage, buff, traitHaver);
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

    public override string getTraitDescription() {
        return ("The reliable town guard that has been protecting the kingdom for hundreds of years. " 
            + " Grants " + this.healthBuff + "/" + (this.healthBuff * this.fullTraitMultiplier)
            + " health to the units in it's row on the left, and " 
            + this.damageBuff +"/" + (this.damageBuff * this.fullTraitMultiplier) 
            + " damage to the units on it's row on the right. The damage and health buffs are seperate but do not stack");
    }

    protected override int getMinimumForActive() {
        return 2;
    }

    public override List<int> getTraitTiering() {
        return new List<int>{2, 4};
    }
}

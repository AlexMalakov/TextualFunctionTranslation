using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MartyrTrait : CypherTrait
{
    [SerializeField] private CombatManager cm;
    [SerializeField] private float debuffAmountSmall;
    [SerializeField] private float debuffAmount;
    [SerializeField] private float debuffAmountLarge;

    [SerializeField] private float damageAmountSmall;
    [SerializeField] private float damageAmount;
    [SerializeField] private float damageAmountLarge;

    [SerializeField] private float healPercentSmall;
    [SerializeField] private float healPercent;
    [SerializeField] private float healPercentLarge;
    
    public override void onUnitDeath(Unit traitHaver, Unit killer) {
        this.applyInDirection(traitHaver, 1, -1, 0);
        this.applyInDirection(traitHaver, -1, 1, 0);
        this.applyInDirection(traitHaver, 1, 0, -1);
        this.applyInDirection(traitHaver, -1, 0, 1);
        this.applyInDirection(traitHaver, 0, -1, 1);
        this.applyInDirection(traitHaver, 0, 1, -1);
    }

    //TODO: some kind of visual element?
    private void applyInDirection(Unit originUnit, int qOffset, int rOffset, int sOffset) {
        List<HexSquare> squares = this.getSquaresInDirection(originUnit, qOffset, rOffset, sOffset);

        if(squares.Count == 0)
            return;

        float debuff = 1;
        float damage = 0;
        float heal = 0;

        if(this.getNumberActive() == 1) {
            float debuff = this.debuffAmountSmall;
            float damage = this.damageAmountSmall;
            float heal = this.healPercentSmall;
        } else if(this.getNumberActive() == 2) {
            float debuff = this.debuffAmount;
            float damage = this.damageAmount;
            float heal = this.healPercent;
        } else if(this.getNumberActive() == 3) {
            float debuff = this.debuffAmountLarge;
            float damage = this.damageAmountLarge;
            float heal = this.healPercentLarge;
        }

        Operation buff = new Operation(OperationType.PostMultiply, debuff, "Martyr Debuff");

        if(squares.Count == 1) {
            Unit u = squares[0].getUnit();
            if(u != null && this.cm.sameOwner(originUnit, u)) {
                //TODO: heal unit
            } else if(u != null) {
                u.buffStat(UnitStats.Damage, buff);
                u.takeDamage(damage);
            }
            return;
        } 

        List<CharacterTransformation> cypher = new List<CharacterTransformation>();
        for(int i = 1; i < squares.Count; i++) {
            Unit u = squares[0].getUnit();
            if(u != null && this.cm.sameOwner(originUnit, u)) {
                //TODO: heal unit
            } else if(u != null) {
                u.buffStat(UnitStats.Damage, buff);
                u.takeDamage(damage);
            }

            cypher.Add(new CharacterTransformation(squares[i], squares[i-1]));
        }
        
        cypher.Add(new CharacterTransformation(squares[0], squares[squares.Count-1]));
        this.addToCypher(cypher);
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

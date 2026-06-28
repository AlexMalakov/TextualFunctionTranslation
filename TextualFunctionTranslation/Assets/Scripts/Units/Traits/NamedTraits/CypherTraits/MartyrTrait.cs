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
            debuff = this.debuffAmountSmall;
            damage = this.damageAmountSmall;
            heal = this.healPercentSmall;
        } else if(this.getNumberActive() == 2) {
            debuff = this.debuffAmount;
            damage = this.damageAmount;
            heal = this.healPercent;
        } else if(this.getNumberActive() == 3) {
            debuff = this.debuffAmountLarge;
            damage = this.damageAmountLarge;
            heal = this.healPercentLarge;
        }

        Operation buff = new Operation(OperationType.PostMultiply, debuff, "Martyr Debuff");

        if(squares.Count == 1) {
            Unit u = squares[0].getUnit();
            if(u != null && this.cm.sameOwner(originUnit, u)) {
                u.heal(heal);
            } else if(u != null) {
                u.buffStat(BuffableStats.Damage, buff, originUnit);
                u.takeDamage(damage, originUnit);
            }
            return;
        } 

        List<CharacterTransformation> cypher = new List<CharacterTransformation>();
        for(int i = 1; i < squares.Count; i++) {
            Unit u = squares[0].getUnit();
            if(u != null && this.cm.sameOwner(originUnit, u)) {
                u.heal(heal);
            } else if(u != null) {
                u.buffStat(BuffableStats.Damage, buff, originUnit);
                u.takeDamage(damage, originUnit);
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
            AxialCoord iterC = iter.getCoords();
            HexSquare check = this.cm.getBoard().getHexAt(new AxialCoord(iterC.q + qOffset, iterC.r + rOffset, iterC.s + sOffset));
            if(!check.isSquare()) {
                break;
            }

            iter = check;
            squares.Add(iter);
        }

        return squares;
    }

    public override string getTraitName() {
        return "martyr";
    }

    protected override int getMinimumForActive() {
        return 1;
    }

    public override List<int> getTraitTiering() {
        return new List<int>{1,2,3};
    }
}

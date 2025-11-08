using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//idea: each pla
public class CombatManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Player enemy;
    [SerializeField] private HexBoard board;

    //not very nice but it will make this unit division work better
    private int livingPlayerUnits = -1;
    private int livingEnemyUnits = -1;
    private List<Unit> units;

    public void Start() {

        //todo this doesn't update with the shop, or across different combats
        this.units = new List<Unit>();
        this.units.AddRange(this.player.getUnits());
        this.units.AddRange(this.enemy.getUnits());
        livingPlayerUnits = this.player.getUnits().Count;
        livingEnemyUnits = this.enemy.getUnits().Count;
        this.units.Sort((a,b) => a.getDecisionPriority().CompareTo(b.getDecisionPriority())); //TODO: swap with inumerable.OrderBy to make this stable so its deterministic
        //TODO: knob to tune: tiebreak behavior. Does player get to win ties?6
    }

    //check each unit every frame
    public void runAttackFrame() {
        foreach(Unit u in this.units) {
            u.checkUnitHasTask(this);
        }
    }

    private bool sameOwner(Unit unit1, Unit unit2) {
        return (this.player.getUnits().Contains(unit1) && this.player.getUnits().Contains(unit2))
            || (this.enemy.getUnits().Contains(unit1) && this.enemy.getUnits().Contains(unit2));
    }

    public Unit requestAttackTarget(Unit attacker, Unit previousTarget) {
        var getAttackableSquares = (HexSquare s) => {
            return HexSquare.hexDist(attacker.getPosition(), s) <= attacker.getRange()
                && s.containsUnit()
                //TODO: && can reach that unit?
                && !sameOwner(attacker, s.getUnit());
        }

        List<Square> possibleTargets = this.board.filterOperationOnSquares(getAttackableSquares);

        int bestTaunt = int.MinValue;
        Unit target = null;
        foreach(Square s in possibleTargets) {
            if(s.getUnit() == previousTarget) {
                return previousTarget;
            }

            if(s.getUnit().getTaunt() > bestTaunt) {
                target = s.getUnit();
                bestTaunt = target.getTaunt();
            }
        }

        //might return null?
        return target;
    }

    public void onUnitDeath(Unit u) {
        if(player.getUnit().Contains(u)) {
            this.livingPlayerUnits--;
            if(livingPlayerUnits <= 0) {
                this.onCombatOver();
            }
        } else if(enemy.getUnit().Contains(u)) {
            this.livingEnemyUnits--;
            if(livingEnemyUnits <= 0) {
                this.onCombatOver();
            }
        } else {
            throw new RuntimeException("im dead but not in any list something is wrong");
        }
        units.Remove(u);
    }

    //ill figure it out later lol
    public void onCombatOver() {
        //TODO: implement
    }
}

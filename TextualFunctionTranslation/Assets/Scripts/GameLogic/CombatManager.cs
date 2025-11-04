using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    List<Unit> units;
    HexBoard board;

    public void Start() {
        this.units.Sort((a,b) => a.getDecisionPriority().CompareTo(b.getDecisionPriority()));
    }

    public void requestCombatAction(Unit u){}



    public void runAttackFrame() {
        foreach(Unit u in this.units) {
            u.checkUnitHasTask(this);
        }
    }

    public Unit requestAttackTarget(Unit attacker, Unit previousTarget) {
        var getAttackableSquares = (HexSquare s) => {
            return HexSquare.hexDist(attacker.getPosition(), s) <= attacker.getRange()
                && s.containsUnit()
                //TODO: && can reach that unit?
                && s.getUnit().getOwner() != attacker.getOwner();
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    List<Unit> units;

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
        return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//idea: each pla
public class CombatManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Player enemy;
    [SerializeField] private HexBoard board;
    [SerializeField] private GameObject healthbarPrefab;

    //not very nice but it will make this unit division work better
    private int livingPlayerUnits = -1;
    private int livingEnemyUnits = -1;
    private List<Unit> units;

    public void initPlayers() {
        player.init();
        enemy.init();
    }

    public void initializeCombat() {
        this.units = new List<Unit>();
        this.units.AddRange(this.player.getActiveUnits());
        this.units.AddRange(this.enemy.getActiveUnits());
        livingPlayerUnits = this.player.getActiveUnits().Count;
        livingEnemyUnits = this.enemy.getActiveUnits().Count;
        this.units.Sort((a,b) => a.getDecisionPriority().CompareTo(b.getDecisionPriority())); //TODO: swap with inumerable.OrderBy to make this stable so its deterministic
        //TODO: knob to tune: tiebreak behavior. Does player get to win ties?6

        this.player.getUnitLayout().placeUnits(this.healthbarPrefab);
        this.enemy.getUnitLayout().placeUnits(this.healthbarPrefab);
    }

    //check each unit every frame
    public void runAttackFrame() {
        foreach(Unit u in this.units) {
            u.checkUnitHasTask(this);
        }
    }

    public HexBoard getBoard() {
        return this.board;
    }

    private bool sameOwner(Unit unit1, Unit unit2) {
        return (this.player.getActiveUnits().Contains(unit1) && this.player.getActiveUnits().Contains(unit2))
            || (this.enemy.getActiveUnits().Contains(unit1) && this.enemy.getActiveUnits().Contains(unit2));
    }

    public Unit requestAttackTarget(Unit attacker, Unit previousTarget) { //I IS ALWAYS RETURNING NULL PROBABLY NOT CORRECT !!!!!!!!!!
        Func<HexSquare, bool> getAttackableSquares = (HexSquare s) => {
            return s.getUnit() != null
                //TODO: && can reach that unit?
                && !sameOwner(attacker, s.getUnit());
        };

        List<HexSquare> possibleTargets = this.board.filterOperationOnSquares(getAttackableSquares);

        int bestTaunt = int.MinValue;
        Unit target = null;
        foreach(HexSquare s in possibleTargets) {
            if(s.getUnit() == previousTarget) {
                return previousTarget;
            }

            if(s.getUnit().getTaunt() > bestTaunt) {
                target = s.getUnit();
                bestTaunt = target.getTaunt();
            }
        }

        //TODO: handle might return null?
        return target;
    }

    public HexSquare requestRoute(Unit attacker, Unit target) {
        return this.board.pathfindToSquare(attacker.getPosition(), target.getPosition());
    }

    public void onUnitDeath(Unit u) {
        if(player.getActiveUnits().Contains(u)) {
            this.livingPlayerUnits--;
            if(livingPlayerUnits <= 0) {
                this.onCombatOver();
            }
        } else if(enemy.getActiveUnits().Contains(u)) {
            this.livingEnemyUnits--;
            if(livingEnemyUnits <= 0) {
                this.onCombatOver();
            }
        } else {
            throw new Exception("im dead but not in any list something is wrong");
        }
        units.Remove(u);
    }

    //ill figure it out later lol
    public void onCombatOver() {
        //TODO: implement
    }
}

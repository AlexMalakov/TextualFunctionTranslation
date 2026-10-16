using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroCheck : MonoBehaviour
{
    [SerializeField] private Shop shop;
    [SerializeField] private HumanPlayer player;
    [SerializeField] private Player enemy;

    private bool failed = false;

    public void precombatCheck() {
        Debug.Log("PRECOMBAT CHECK!!!");
        this.failed = false; //reset

        if(!this.checkKing()) {
            Debug.Log("PRECOMBAT CHECK!!! FAILED KING");
            this.failed = true;
            return;
        }

        if(!this.outcastsAlone()) {
            Debug.Log("PRECOMBAT CHECK!!! FAILED OUTCASTS");
            this.failed = true;
            return;
        }

        if(!this.noDups()) {
            Debug.Log("PRECOMBAT CHECK!!! FAILED DUPS");
            this.failed = true;
            return;
        }

        if(!this.circleJerk()) {
            Debug.Log("PRECOMBAT CHECK!!! FAILED JERK");
            this.failed = true;
            return;
        }

        if(!this.isBroke()) {
            Debug.Log("PRECOMBAT CHECK!!! NOT BROKE");
            this.failed = true;
            return;
        }

        Debug.Log("PASSED HERO CHECK!");
        //haven't failed yet, just need death unit to die now
    }

    private bool checkKing() {
        bool hasKing = false;
        foreach(Unit u in this.enemy.getActiveUnits()) {
            hasKing = hasKing || (u is DemonKing);
        }

        return hasKing;
    }

    private bool outcastsAlone() {
        List<Unit> outcasts = new List<Unit>();
        foreach(Unit u in this.player.getActiveUnits()) {
            if(u.getRoleTrait().getTraitName() == "outcast") {
                outcasts.Add(u);
            }
        }

        if(outcasts.Count < 2) {
            return false;
        }

        bool selfBuffing = false;
        foreach(Unit o in outcasts) {
            if(o.isBuffedBySelf()) {
                return false;
            }
        }

        return !selfBuffing;
    }

    private bool noDups() {
        List<string> unitNames = new List<string>();
        foreach(Unit u in this.player.getActiveUnits()) {
            if(unitNames.Contains(u.getName())) {
                return false;
            }
            unitNames.Add(u.getName());
        }
        return true;
    }

    private bool circleJerk() {
        foreach(Unit u in this.player.getActiveUnits()) {
            if(!u.isBuffedByOther()) {
                return false;
            }
        }
        return true;
    }

    private bool isBroke() {
        return this.player.getMoney() == 0;
    }

    public void onDeathCheck(Unit dead) {
        if(!player.ownsUnit(dead))
            return;
        if(!this.failed) {
            if(this.isSacrifice(dead)) {
                this.shop.revealHero();
            }
            
            this.failed = true; //even when hero is revealed so that we don't waste effort once we're done
        }
    }

    private bool isSacrifice(Unit dead) {
        return dead.getClassTrait().getTraitName() == "troll" || dead.getRoleTrait().getTraitName() == "plague doctor";
    }

}

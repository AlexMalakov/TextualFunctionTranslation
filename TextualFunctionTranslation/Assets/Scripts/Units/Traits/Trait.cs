using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
Needed trait behavior
Vegenary - on combat end
Rotator - on combat start 
royal knights - on unit defeat, on combat survive
plug - on unit defeat
assasin - on combat end
flank guard - on combat start

scout - on first attack
druid - when layout phase starts
warden - on combat start
marksman - on unit attack
outsider - on combat start
tracker - on combat end
*/

public abstract class Trait : MonoBehaviour
{
    public string name;
    public int minimumForActivate;

    public Trait(string name, int minimumForActivate) {
        this.name = name;
        this.minimumForActivate = minimumForActivate;
    }

    public int getMinimumForActive() {
        return this.minimumForActivate;
    }

    public bool attemptActivate(int numUnits) {
        if(numUnits < this.minimumForActivate) {
            return false;
        }

        return true;

        //TODO: ELSE DO STUFF !!!!!
    }

    //this should do whatever the trait does
    public abstract void onPrecombatStart(Unit traitHaver);

    public abstract void onCombatStart(Unit traitHaver);

    public abstract void onFirstAttackAgainstUnit(Unit traitHaver, Unit target);

    public abstract void onUnitAttack(Unit traitHaver, Unit target); //target not needed but correct to have 

    public abstract void onEnemyDefeat(Unit traitHaver, Unit target);

    public abstract void onCombatSurvival(Unit traitHaver);

    public abstract void onCombatEnd(Unit traitHaver);


}



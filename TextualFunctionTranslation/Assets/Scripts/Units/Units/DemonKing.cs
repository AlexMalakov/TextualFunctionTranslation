using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonKing : Unit
{
    [Header ("king")]
    [SerializeField] private float wakeTimer = 5f;
    private bool isAsleep = true;
    private bool isWaking = false;

    public override void triggerUpdate(CombatManager manager) {
        if(this.currentState == null) {
            this.currentState = this.Idle;
        }
        if(this.isAsleep) {
            return;
        }

        base.triggerUpdate(manager);
    }

    public override void initUnit(HexSquare square) {
        this.isAsleep = true;
        this.isWaking = false;

        base.initUnit(square);
    }

    public override void onCombatEnd() {
        CancelInvoke(nameof(wakeUp));
        base.onCombatEnd();
    }


    public override void takeDamage(float damage, Unit from) {
        if(this.isAsleep && !this.isWaking) {
            this.isWaking = true;
            Invoke(nameof(wakeUp), this.wakeTimer);
            this.triggerAnimation("king_rise");
        }

        base.takeDamage(damage, from);
    }

    private void wakeUp() {
        this.isAsleep = false;
        //TODO say some aura shit
    }

    public override void onDeath() {
        //TODO; say some even more aura shit
        this.currentHealth = this.calcMaxHealth();
        //TODO: reset health bar back up to full >:)

    }
}

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
        if(this.isAsleep) {
            return;
        }

        base.triggerUpdate();
    }

    public override void initUnit(HexSquare square, HealthBarManager hpBar, Transform hpCanvas) {
        this.isAsleep = true;
        this.isWaking = false;

        base.initUnit(square, hpBar, hpCanvas);
        //TODO: trigger zzz anim
    }


    public override void takeDamage(float damage, Unit from) {
        if(this.isAsleep && !this.isWaking) {
            this.isWaking = true;
            Invoke(nameof(wakeUp), this.wakeTimer);
            //TODO: trigger waking anim
        }

        base.takeDamage(damage, from);
    }

    private void wakeUp() {
        this.isAsleep = false;
        //TODO say some aura shit
    }

    public override void onDeath() {
        //TODO; say some even more aura shit
        this.currentHealth = this.healthCalculator.calculate();
        //TODO: reset health bar back up to full >:)

    }
}

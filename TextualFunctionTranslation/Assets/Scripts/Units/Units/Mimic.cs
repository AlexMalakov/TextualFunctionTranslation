using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mimic : Unit
{
    public override float calcDamage() {
        return this.currentTarget != null ? this.currentTarget.calcDamage() : base.calcDamage();
    }

    public override int getRange() {
        int newRange = this.currentTarget != null ? this.currentTarget.getRange() : base.getRange();
        this.ac.setAttackType(newRange - 1);
        return newRange;
    }

    public override float calcAttackSpeed() {
        return this.currentTarget != null ? this.currentTarget.calcAttackSpeed() : base.calcAttackSpeed();
    }
}

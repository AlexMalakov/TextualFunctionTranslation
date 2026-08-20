using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mimic : Unit
{
    public override float calcDamage() {
        return this.currentTarget != null ? this.currentTarget.calcDamage() : this.damage;
    }

    public override int getRange() {
        return this.currentTarget != null ? this.currentTarget.getRange() : 1;
    }

    public override int calcAttackSpeed() {
        return this.currentTarget != null ? this.currentTarget.getAttackSpeed();
    }
}

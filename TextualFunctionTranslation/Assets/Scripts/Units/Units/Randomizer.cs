using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randomizer : Unit
{
    [Header ("randomizer dets")]
    [SerializeField] private int minRange;
    [SerializeField] private int maxRange;
    [SerializeField] private float minDamage;
    [SerializeField] private float maxDamage;

    [SerializeField] private int randomSeed;

    private System.Random rng;
    
    public override int getRange() {
        if(this.rng = null) {
            this.rng = new System.Random(randomSeed);
        }
        
        return this.rng.Next(this.minRange, this.maxRange);
    }

    public override float calcDamage() {
        if(this.rng = null) {
            this.rng = new System.Random(randomSeed);
        }

        return this.damageCalculator.calculateFromN((float)this.rng.NextDouble(this.minDamage, this.maxDamage));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

//out of 100
[System.Serializable]
public struct ShopRarity {
    public int chance2;
    public int chance3;
    public int chance4;
    public int chance5;

    public ShopRarity(int c2, int c3, int c4, int c5) {
        if(c2 + c3 + c4 + c5 != 100) {
            throw new Exception("shop rarity must be ints that add to 100");
        }
        this.chance2 = c2;
        this.chance3 = c3;
        this.chance4 = c4;
        this.chance5 = c5;
    }
}

public class ShopPool : MonoBehaviour
{
    private Dictionary<int, Dictionary<UnitBlueprint, int>> poolAndCount;
    private Dictionary<int, Dictionary<UnitBlueprint, int>> backupPool;
    private ShopRarity sr = new ShopRarity(100, 0, 0, 0);

    [SerializeField] private List<string> startingUnits;
    [SerializeField] private List<string> skipUnits;

    private System.Random myRng;


    public void initFromLevel(ShopRarity rare, int seed) {
        this.sr = rare;
        this.myRng = new System.Random(seed);
    }

    public void initPool(List<UnitBlueprint> blups) {
        poolAndCount = new Dictionary<int, Dictionary<UnitBlueprint, int>>();
        backupPool = new Dictionary<int, Dictionary<UnitBlueprint, int>>();
        for(int i = 2; i <= 5; i++) {
            poolAndCount.Add(i, new Dictionary<UnitBlueprint, int>());
        }

        this.myRng = new System.Random(System.DateTime.Now.Millisecond);

        foreach(UnitBlueprint ub in blups) {
            if(this.skipUnits.Contains(ub.unitName)) {
                continue;
            }

            this.poolAndCount[ub.unitCost].Add(ub, this.startingUnits.Contains(ub.unitName) ? 3 : 4);
        }
    }

    public void backupThePool() {
        backupPool = new Dictionary<int, Dictionary<UnitBlueprint, int>>();

        foreach(int cost in this.poolAndCount.Keys) {
            Dictionary<UnitBlueprint, int> countsPerUnit = new Dictionary<UnitBlueprint, int>();
            foreach(UnitBlueprint ub in this.poolAndCount[cost].Keys) {
                countsPerUnit.Add(ub, this.poolAndCount[cost][ub]);
            }
            backupPool.Add(cost, countsPerUnit);
        }
    }

    public void rewindPool() {
        poolAndCount = new Dictionary<int, Dictionary<UnitBlueprint, int>>();
        foreach(int cost in this.backupPool.Keys) {
            Dictionary<UnitBlueprint, int> countsPerUnit = new Dictionary<UnitBlueprint, int>();
            foreach(UnitBlueprint ub in this.backupPool[cost].Keys) {
                countsPerUnit.Add(ub, this.backupPool[cost][ub]);
            }
            poolAndCount.Add(cost, countsPerUnit);
        }
    }

    public void removeFromPool(UnitBlueprint unit) {
        this.poolAndCount[unit.unitCost][unit] = this.poolAndCount[unit.unitCost][unit] - 1;
    }

    public void returnToPool(Unit u) {
        int realCost = u.getBlueprint().unitCost;
        if(this.poolAndCount[realCost].ContainsKey(u.getBlueprint())) {
            this.poolAndCount[realCost][u.getBlueprint()] = this.poolAndCount[realCost][u.getBlueprint()] + (1 << (u.getMergeLevel()-1));
        } else {
            for(int i = 0; i < this.poolAndCount[realCost].Keys.Count; i++) {
                UnitBlueprint ub = this.poolAndCount[realCost].Keys.ToList()[i];
                if(ub.unitName == u.getName()) {
                    this.poolAndCount[realCost][ub] = this.poolAndCount[realCost][ub] + (1 << (u.getMergeLevel()-1));
                }
            }
        }
    }

    public List<UnitBlueprint> sampleFromPool() {
        List<UnitBlueprint> sample = new List<UnitBlueprint>();
        for(int i = 0; i < 5; i++) {
            int rng = this.myRng.Next(0, 100);
            if (rng < this.sr.chance2)
                sample.Add(this.sampleFromCost(2, sample));
            else if (rng < this.sr.chance2 + this.sr.chance3)
                sample.Add(this.sampleFromCost(3, sample));
            else if (rng < this.sr.chance2 + this.sr.chance3 + this.sr.chance4)
                sample.Add(this.sampleFromCost(4, sample));
            else
                sample.Add(this.sampleFromCost(5, sample));
        }

        return sample;
    }

    private UnitBlueprint sampleFromCost(int cost, List<UnitBlueprint> alreadyTaking) {
        for(int i = 2; i < 6; i++) {
            int rotateCost = 2 + ((cost + i)%4);
            UnitBlueprint nullcheck = this.sampleFromCostInternals(rotateCost, alreadyTaking);
            if(nullcheck != null) {
                return nullcheck;
            }
        }

        Debug.Log("WE HAVE MIRACULOUSLY COMPLETED THE POOL");
        //backup we just say fuck the pool and pick randomly
        int rCost = this.myRng.Next(2, 6);
        Dictionary<UnitBlueprint, int> chosen = this.poolAndCount[rCost];
        return chosen.Keys.ToList()[this.myRng.Next(0, chosen.Count)];
    }

    private UnitBlueprint sampleFromCostInternals(int cost, List<UnitBlueprint> alreadyTaking) {
        List<UnitBlueprint> options = new List<UnitBlueprint>();
        foreach(UnitBlueprint ub in this.poolAndCount[cost].Keys) {
            int count = alreadyTaking.Count(bl => bl == ub);
            for(int i = 0; i < this.poolAndCount[cost][ub] - count; i++) {
                options.Add(ub);
            }
        }

        if(options.Count == 0) {
            return null;
        }

        return options[this.myRng.Next(0, options.Count)];
    }
}

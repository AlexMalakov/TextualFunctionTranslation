using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TraitFactory : MonoBehaviour
{
    [SerializeField] private CombatManager cm;

    [SerializeField] private Player player;
    [SerializeField] private Player enemy;
    private static List<Trait> staticPlayerTraits;
    private static List<Trait> staticEnemyTraits;
    private static CombatManager staticCM;

    public void Awake() { //Awake because this needs to run first
        staticPlayerTraits = this.player.getAllTraits();
        staticEnemyTraits = this.enemy.getAllTraits();
        staticCM = cm;
    }

    public static Trait getTrait(string t_name, Unit traitGetter) {
        if(staticCM.isPlayerUnit(traitGetter)) {
            foreach(Trait t in staticPlayerTraits) {
                if(t.getTraitName() == t_name) {
                    return t;
                }
            }
        }
        foreach(Trait t in staticEnemyTraits) {
            if(t.getTraitName() == t_name) {
                return t;
            }
        }

        throw new Exception("TRAIT DOES NOT EXIST IN FACTORY!" + t_name);
    }
}

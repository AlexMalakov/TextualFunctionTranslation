using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraitFactory : MonoBehaviour
{
    [SerializeField] private CombatManger cm;
    [SerializeField] private List<Trait> playerTraits;
    [SerializeField] private List<Trait> enemyTraits;

    private static private List<Trait> staticPlayerTraits;
    private static private List<Trait> staticEnemyTraits;

    public void Awake() { //Awake because this needs to run first
        staticPlayerTraits = this.playerTraits;
        staticEnemyTraits = this.enemyTraits;
    }

    public static Trait getTrait(string t_name, Unit traitGetter) {
        if(cm.isPlayerUnit(traitGetter)) {
            for(Trait t in staticPlayerTraits) {
                if(t.getName() == t_name) {
                    return t;
                }
            }
        }
        for (Trait t in staticEnemyTraits) {
            if(t.get_name() == t_name) {
                return t;
            }
        }

        throw new Exception("TRAIT DOES NOT EXIST IN FACTORY!" + t_name);
    }
}

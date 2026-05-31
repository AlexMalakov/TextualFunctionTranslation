using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraitDisplay : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Player enemy;

    [SerializeField] private GameObject traitDisplayWidgetPrefab;
    private List<TraitWidget> traitWidgets;
    //TODO: this is a bad design -- i think this should be moved into trait, also i think i need to think about splitting trait objects between player's and enemies :)

    public void Awake() {
        this.traitWidgets = new List<TraitWidget>();
        for(int i = 0; i < 26; i++) {
            //this.traitWidgets.Add(Instantiate(traitDisplayWidgetPrefab).GetComponent<TraitWidget>());
            //this.traitWidgets[i].init()
        }
    }

    public void onUnitLayoutUpdate() {
        //TODO: display all traits for player, enemy

        List<Trait> playerTraits = this.player.getUnitLayout().getActiveTraits();
        List<Trait> enemyTraits = this.enemy.getUnitLayout().getActiveTraits();

        //TODO: sort alphabetically?

        foreach(Unit u in this.player.getActiveUnits()) {
            if(!playerTraits.Contains(u.getRoleTrait())) {
                playerTraits.Add(u.getRoleTrait());
            }

            if(!playerTraits.Contains(u.getClassTrait())) {
                playerTraits.Add(u.getClassTrait());
            }
        }

        foreach(Unit u in this.enemy.getActiveUnits()) {
            if(!enemyTraits.Contains(u.getRoleTrait())) {
                enemyTraits.Add(u.getRoleTrait());
            }

            if(!enemyTraits.Contains(u.getClassTrait())) {
                enemyTraits.Add(u.getClassTrait());
            }
        }

        //TODO: create little thingies for each trait, which shows the name and the number, and when hovered over displays more info about it?
        //use TRAIT.getNumberActive()
    }
}

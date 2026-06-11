using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TraitDisplay : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Player enemy;
    [SerializeField] private Panel panel;

    [SerializeField] private GameObject traitDisplayWidgetPrefab;
    private List<Trait> playerTraits;
    private List<Trait> enemyTraits;

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

        this.playerTraits = playerTraits;
        this.enemyTraits = enemyTraits;
    }

    public void flashTrait() {
        panel.color = new Color(255,0,0);
        Invoke(nameof("unflashTrait"), .5f);
    }

    private void unflashTrait() {
        panel.color = new Color(0,255,0);
    }
}

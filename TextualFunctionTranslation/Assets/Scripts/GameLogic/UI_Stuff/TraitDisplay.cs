using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TraitDisplay : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Player enemy;
    [SerializeField] private GameObject twPrefab;
    [SerializeField] private Transform twParent;

    [SerializeField] private float spacing = 60f;
    [SerializeField] private float sectionSpacing = 30f;

    private List<TraitWidget> allWidgets;

    private void clearFromBar() {
        foreach(TraitWidget tw in this.allWidgets) {
            tw.makeActive(false, -1);
        }
    }

    public void onUnitLayoutUpdate() {
        clearFromBar();

        List<Trait> playerInActiveTraits = new List<Trait>();
        List<Trait> playerActiveTraits = this.player.getUnitLayout().getActiveTraits();
        List<Trait> enemyActiveTraits = this.enemy.getUnitLayout().getActiveTraits();

        float y = 0;

        foreach(Unit u in this.player.getActiveUnits()) {
            if(!playerActiveTraits.Contains(u.getRoleTrait()) && !playerInActiveTraits.Contains(u.getRoleTrait())) {
                playerInActiveTraits.Add(u.getRoleTrait());
            }

            if(!playerActiveTraits.Contains(u.getClassTrait()) && !playerInActiveTraits.Contains(u.getClassTrait())) {
                playerInActiveTraits.Add(u.getClassTrait());
            }
        }

        foreach(Trait t in enemyActiveTraits.OrderByDescending(t => t.getTraitName())) {
            this.setupWidget(t.getTraitWidget(), y, 2);
            y -= spacing;
        }

        foreach(Trait t in playerActiveTraits.OrderBy(t => t.getTraitName())) {
            this.setupWidget(t.getTraitWidget(), y, 0);
            y -= spacing;
        }

        foreach(Trait t in playerInActiveTraits.OrderBy(t => t.getTraitName())) {
            this.setupWidget(t.getTraitWidget(), y, 1);
            y -= spacing;
        }
    }

    public TraitWidget createWidget() {
        TraitWidget tw = Instantiate(twPrefab, this.twParent).GetComponent<TraitWidget>();
        this.allWidgets.Add(tw);
        return tw;
    }

    private void setupWidget(TraitWidget tw, float yOffset, int activeNum) {
        tw.makeActive(true, activeNum);

        RectTransform rect = tw.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, yOffset);
    }

    public IEnumerator playTraitAnim() {

        List<Trait> playerActiveTraits = this.player.getUnitLayout().getActiveTraits();
        List<Trait> enemyActiveTraits = this.enemy.getUnitLayout().getActiveTraits();

        foreach(Trait t in playerActiveTraits.OrderByDescending(t => t.getTraitName())) {
            t.getTraitWidget().flashTrait();
            yield return new WaitForSeconds(.5f);
        }

        foreach(Trait t in enemyActiveTraits.OrderBy(t => t.getTraitName())) {
            t.getTraitWidget().flashTrait();
            yield return new WaitForSeconds(.5f);
        }
    }
}

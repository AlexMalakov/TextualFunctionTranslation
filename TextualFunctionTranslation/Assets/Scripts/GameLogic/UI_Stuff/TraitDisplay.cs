using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TraitDisplay : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Player enemy;
    [SerializeField] private GameObject twPrefab;
    [SerializeField] private Transform playerParent;
    [SerializeField] private Transform enemyParent;

    [SerializeField] private float spacing = 60f;
    [SerializeField] private float sectionSpacing = 30f;

    private List<TraitWidget> playerWidgets = new List<TraitWidget>();
    private List<TraitWidget> enemyWidgets = new List<TraitWidget>();

    private void clearFromBar() {
        foreach(TraitWidget tw in this.playerWidgets) {
            tw.makeActive(false, false);
        }

        foreach(TraitWidget tw in this.enemyWidgets) {
            tw.makeActive(false, false);
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
            this.setupWidget(t.getTraitWidget(), y, true);
            y -= spacing;
        }

        foreach(Trait t in playerActiveTraits.OrderBy(t => t.getTraitName())) {
            this.setupWidget(t.getTraitWidget(), y, true);
            y -= spacing;
        }

        foreach(Trait t in playerInActiveTraits.OrderBy(t => t.getTraitName())) {
            this.setupWidget(t.getTraitWidget(), y, false);
            y -= spacing;
        }

        int counter = 0;
        foreach(Trait t in playerActiveTraits.OrderBy(t => t.getTraitName())) {
            t.getTraitWidget().transform.SetSiblingIndex(counter);
            counter++;
        }

        foreach(Trait t in playerInActiveTraits.OrderBy(t => t.getTraitName())) {
            t.getTraitWidget().transform.SetSiblingIndex(counter);
            counter++;
        }

        counter = 0;
        foreach(Trait t in playerInActiveTraits.OrderByDescending(t => t.getTraitName())) {
            t.getTraitWidget().transform.SetSiblingIndex(counter);
            counter++;
        }
    }


    public TraitWidget createWidget(bool playersTrait) {
        if(playersTrait) {
            TraitWidget tw = Instantiate(twPrefab, this.playerParent).GetComponent<TraitWidget>();
            this.playerWidgets.Add(tw);
            return tw;
        } else {
            TraitWidget tw = Instantiate(twPrefab, this.enemyParent).GetComponent<TraitWidget>();
            this.enemyWidgets.Add(tw);
            return tw;
        }
    }

    private void setupWidget(TraitWidget tw, float yOffset, bool isEnoughForActive) {
        tw.makeActive(true, isEnoughForActive);

        RectTransform rect = tw.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, yOffset);
    }

    public IEnumerator playCypherTraitAnim() {
        List<Trait> playerActiveCypherTraits = this.player.getUnitLayout().getActiveTraits().Where(t => t is CypherTrait).ToList();
        List<Trait> enemyActiveCypherTraits = this.enemy.getUnitLayout().getActiveTraits().Where(t => t is CypherTrait).ToList();

        foreach(Trait t in playerActiveCypherTraits.OrderByDescending(t => t.getTraitName())) {
            if(t is VegeneryTrait) { //I'm sorry but this has to happen with the visual hurk (thats me vomiting i am vomiting)
                ((VegeneryTrait)t).buyTopUnit();
            }
            t.getTraitWidget().flashTrait();
            yield return new WaitForSeconds(.5f);
        }

        foreach(Trait t in enemyActiveCypherTraits.OrderBy(t => t.getTraitName())) {
            if(t is VegeneryTrait) {
                ((VegeneryTrait)t).trashTopUnit();
            }
            t.getTraitWidget().flashTrait();
            yield return new WaitForSeconds(.5f);
        }
    }

    public IEnumerator playTraitSquareReveal() {
        List<Trait> playerActiveInfoTraits = this.player.getUnitLayout().getActiveTraits().Where(t => t is InformationTrait).ToList();
        List<Trait> enemyActiveInfoTraits = this.enemy.getUnitLayout().getActiveTraits().Where(t => t is InformationTrait).ToList();

        foreach(Trait t in playerActiveInfoTraits.OrderByDescending(t => t.getTraitName())) {
            if(t is TrackerTrait) {
                ((TrackerTrait)t).giveLenses(true);
            }

            t.getTraitWidget().flashTrait();
            t.runSquareReveal();
            yield return new WaitForSeconds(.5f);
        }

        foreach(Trait t in enemyActiveInfoTraits.OrderBy(t => t.getTraitName())) {
            if(t is TrackerTrait) {
                ((TrackerTrait)t).giveLenses(false);
            }

            t.getTraitWidget().flashTrait();
            t.runSquareReveal();
            yield return new WaitForSeconds(.5f);
        }
    }
}

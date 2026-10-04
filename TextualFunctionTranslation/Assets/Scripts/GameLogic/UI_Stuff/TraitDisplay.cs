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

    [SerializeField] private DisplayButton displayButton;

    private bool showingCypher = true;

    private List<TraitWidget> playerInfoWidgets = new List<TraitWidget>();
    private List<TraitWidget> playerCypherWidgets = new List<TraitWidget>();
    private List<TraitWidget> enemyInfoWidgets = new List<TraitWidget>();
    private List<TraitWidget> enemyCypherWidgets = new List<TraitWidget>();

    private void clearFromBar() {
        foreach(TraitWidget tw in this.playerInfoWidgets) {
            tw.makeActive(false, false);
        }

        foreach(TraitWidget tw in this.playerCypherWidgets) {
            tw.makeActive(false, false);
        }

        foreach(TraitWidget tw in this.enemyInfoWidgets) {
            tw.makeActive(false, false);
        }

        foreach(TraitWidget tw in this.enemyCypherWidgets) {
            tw.makeActive(false, false);
        }
    }

    public void toggleShowCypher() {
        this.showingCypher = !this.showingCypher;
        this.onUnitLayoutUpdate();
    }

    public void onUnitLayoutUpdate() {
        clearFromBar();

        List<Trait> playerInActiveTraits = new List<Trait>();
        List<Trait> playerActiveTraits = this.player.getUnitLayout().getActiveTraits();
        List<Trait> enemyActiveTraits = this.enemy.getUnitLayout().getActiveTraits();

        foreach(Unit u in this.player.getActiveUnits()) {
            if(!playerActiveTraits.Contains(u.getRoleTrait()) && !playerInActiveTraits.Contains(u.getRoleTrait())) {
                playerInActiveTraits.Add(u.getRoleTrait());
            }

            if(!playerActiveTraits.Contains(u.getClassTrait()) && !playerInActiveTraits.Contains(u.getClassTrait())) {
                playerInActiveTraits.Add(u.getClassTrait());
            }
        }

        int counter = 0;
        foreach(Trait t in playerActiveTraits.OrderBy(t => t.getTraitName())) {
            if(this.showingCypher == (t is CypherTrait)) {
                this.setupWidget(t.getTraitWidget(), true);
                t.getTraitWidget().transform.SetSiblingIndex(counter);
                counter++;
            }
        }

        foreach(Trait t in playerInActiveTraits.OrderBy(t => t.getTraitName())) {
            if(this.showingCypher == (t is CypherTrait)) {
                this.setupWidget(t.getTraitWidget(), false);
                t.getTraitWidget().transform.SetSiblingIndex(counter);
                counter++;
            }
        }

        counter = 0;
        foreach(Trait t in enemyActiveTraits.OrderByDescending(t => t.getTraitName())) {
            if(this.showingCypher == (t is CypherTrait)) {
                this.setupWidget(t.getTraitWidget(), true);
                t.getTraitWidget().transform.SetSiblingIndex(counter);
                counter++;
            }
        }
    }


    public TraitWidget createWidget(bool playersTrait, bool isCypher) {
        if(playersTrait) {
            TraitWidget tw = Instantiate(twPrefab, this.playerParent).GetComponent<TraitWidget>();
            (isCypher ? this.playerCypherWidgets : this.playerInfoWidgets).Add(tw);
            return tw;
        } else {
            TraitWidget tw = Instantiate(twPrefab, this.enemyParent).GetComponent<TraitWidget>();
            (isCypher ? this.enemyCypherWidgets : this.enemyInfoWidgets).Add(tw);
            return tw;
        }
    }

    private void setupWidget(TraitWidget tw, bool isEnoughForActive) {
        tw.makeActive(true, isEnoughForActive);
    }

    public IEnumerator playCypherTraitAnim() {
        this.displayButton.showButton(false);
        if(!this.showingCypher) {
            this.toggleShowCypher();
            this.displayButton.toggleFromDisplay();
        }

        List<Trait> playerActiveCypherTraits = this.player.getUnitLayout().getActiveTraits().Where(t => t is CypherTrait).ToList();
        List<Trait> enemyActiveCypherTraits = this.enemy.getUnitLayout().getActiveTraits().Where(t => t is CypherTrait).ToList();

        foreach(Trait t in playerActiveCypherTraits.OrderByDescending(t => t.getTraitName())) {
            if(t is VegeneryTrait) { //I'm sorry but this has to happen with the visual hurk (thats me vomiting i am vomiting)
                ((VegeneryTrait)t).buyTopUnit();
            }
            t.getTraitWidget().flashTrait();
            yield return new WaitForSeconds(GlobalData.CypherRevealDuration);
        }

        foreach(Trait t in enemyActiveCypherTraits.OrderBy(t => t.getTraitName())) {
            if(t is VegeneryTrait) {
                ((VegeneryTrait)t).trashTopUnit();
            }
            t.getTraitWidget().flashTrait();
            yield return new WaitForSeconds(GlobalData.CypherRevealDuration);
        }
    }

    public IEnumerator playTraitSquareReveal() {
        if(this.showingCypher) {
            this.toggleShowCypher();
            this.displayButton.toggleFromDisplay();
        }

        List<Trait> playerActiveInfoTraits = this.player.getUnitLayout().getActiveTraits().Where(t => t is InformationTrait).ToList();
        List<Trait> enemyActiveInfoTraits = this.enemy.getUnitLayout().getActiveTraits().Where(t => t is InformationTrait).ToList();

        foreach(Trait t in playerActiveInfoTraits.OrderByDescending(t => t.getTraitName())) {
            if(t is TrackerTrait) {
                ((TrackerTrait)t).giveLenses(true);
            }

            t.getTraitWidget().flashTrait();
            t.runSquareReveal();
            yield return new WaitForSeconds(GlobalData.SquareRevealDuration);
        }

        foreach(Trait t in enemyActiveInfoTraits.OrderBy(t => t.getTraitName())) {
            if(t is TrackerTrait) {
                ((TrackerTrait)t).giveLenses(false);
            }

            t.getTraitWidget().flashTrait();
            t.runSquareReveal();
            yield return new WaitForSeconds(GlobalData.SquareRevealDuration);
        }
        
        this.displayButton.showButton(true);
    }
}

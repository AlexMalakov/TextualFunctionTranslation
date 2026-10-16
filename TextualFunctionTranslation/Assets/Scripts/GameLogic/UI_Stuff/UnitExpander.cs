using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UnitExpander : MonoBehaviour
{
    [SerializeField] private Image unitIcon;
    [SerializeField] private TMP_Text health_ratio;
    [SerializeField] private TMP_Text unit_name;

    [SerializeField] private TraitCodexButton roleTrait;
    [SerializeField] private TraitCodexButton classTrait;
    [SerializeField] private TraitCodexInfo tci;
    [SerializeField] private GameObject traitCodex;
    [SerializeField] private Unit startingUnit;
    [SerializeField] private HealthBarManager targetHPBar;

    [Header ("stats")]
    [SerializeField] private TMP_Text unit_damage;
    [SerializeField] private TMP_Text unit_attackspeed;
    [SerializeField] private TMP_Text unit_range;
    [SerializeField] private TMP_Text unit_initiative;
    [SerializeField] private TMP_Text unit_cost;
    [SerializeField] private TMP_Text unit_taunt;

    private Unit tracking;
    private bool hpBarinited = false;

    public void Update() {
        if(tracking == null) {
            this.tracking = startingUnit;
            if(tracking != null && !hpBarinited) {
                this.targetHPBar.initUniversal(this.tracking.calcMaxHealth(), this.tracking.getMergeLevel());
                hpBarinited = true;
            }
        }

        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider.gameObject.GetComponentInParent<Unit>() != null) {
                    this.tracking = hit.collider.gameObject.GetComponentInParent<Unit>();
                    this.refreshUnit();
                }
            }
        } else if(this.tracking != null) {
            this.refreshUnit();
        }
    }

    private void refreshUnit() {
        this.unitIcon.sprite = tracking.getUnitIcon();
        this.unit_name.text = tracking.getName();
        
        this.targetHPBar.updateMaxHealth(this.tracking.calcMaxHealth(), this.tracking.getMergeLevel());
        this.targetHPBar.noChipDamage(this.tracking.getCurHealth());
        this.health_ratio.text = this.tracking.getCurHealth() + "/" + this.tracking.calcMaxHealth();


        this.unit_damage.text = this.tracking.calcDamageReadOnly() + "";
        this.unit_attackspeed.text = this.tracking.calcUsableAttackSpeed().ToString("F1");
        this.unit_range.text = this.tracking.getRange() + "";
        this.unit_initiative.text = this.tracking.getDecisionPriority() + "";
        this.unit_cost.text = this.tracking.getCost() + "";
        this.unit_taunt.text = this.tracking.getTaunt() + "";

        this.roleTrait.init(this.tracking.getRoleTrait(), this.tci, this.traitCodex);
        this.classTrait.init(this.tracking.getClassTrait(), this.tci, this.traitCodex);
    }

    public void fakeSwapTargets(Unit u) {
        tracking = u;
    }
}
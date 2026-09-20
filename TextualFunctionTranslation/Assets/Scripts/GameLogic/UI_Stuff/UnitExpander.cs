using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UnitExpander : MonoBehaviour
{
    [SerializeField] private Image unitIcon;
    [SerializeField] private TMP_Text health_ratio;
    [SerializeField] private TMP_Text unit_stats;
    [SerializeField] private TMP_Text unit_name;

    [SerializeField] private TraitCodexButton roleTrait;
    [SerializeField] private TraitCodexButton classTrait;

    private Unit tracking;


    public void Start() {
        //unitIcon.sprite = defaultSprite;
        this.health_ratio.text = "";
        this.unit_stats.text = "";

        // this.roleTrait.gameObject.SetActive(false); //TODO: enable me :)
        // this.classTrait.gameObject.SetActive(false);
    }

    public void Update() {
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
        
        //TODO: healthbar
        this.health_ratio.text = this.tracking.getCurHealth() + "/" + this.tracking.calcMaxHealth();

        this.unit_stats.text = "\nDamage: " + this.tracking.calcDamage()
                    + "\nAttack Speed: " + this.tracking.calcAttackSpeed()
                    + "\nRange: " + this.tracking.getRange()
                    + "\nDecision Order: " + this.tracking.getDecisionPriority()
                    + "\nTaunt: " + this.tracking.getTaunt();

        // this.roleTrait.gameObject.SetActive(true);
        // this.classTrait.gameObject.SetActive(true);
        // this.roleTrait.init(this.tracking.getRoleTrait());
        // this.classTrait.init(this.tracking.getClassTrait());
    }



}
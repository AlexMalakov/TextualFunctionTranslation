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

    [SerializeField] private TraitCodexButton roleTrait;
    [SerializeField] private TraitCodexButton classTrait;


    public void Start() {
        //unitIcon.sprite = defaultSprite;
        this.health_ratio.text = "";
        this.unit_stats.text = "";

        this.roleTrait.gameObject.SetActive(false);
        this.classTrait.gameObject.SetActive(false);
    }

    public void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider.gameObject.GetComponentInParent<Unit>() != null) {
                    this.assignUnit(hit.collider.gameObject.GetComponentInParent<Unit>());
                }
            }
        }
    }

    private void assignUnit(Unit u) {
        //unitIcon.sprite = unitSprite;
        //TODO: healthbar
        this.health_ratio.text = u.getCurHealth() + "/" + u.calcMaxHealth();

        this.unit_stats.text = "\nDamage: " + u.calcDamage()
                    + "\nAttack Speed: " + u.calcAttackSpeed()
                    + "\nRange: " + u.getRange()
                    + "\nDecision Order: " + u.getDecisionPriority()
                    + "\nTaunt: " + u.getTaunt();

        this.roleTrait.gameObject.SetActive(true);
        this.classTrait.gameObject.SetActive(true);
        this.roleTrait.init(u.getRoleTrait());
        this.classTrait.init(u.getClassTrait());
    }



}
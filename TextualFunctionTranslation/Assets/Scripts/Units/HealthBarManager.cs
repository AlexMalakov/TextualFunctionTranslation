using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HealthBarManager : MonoBehaviour
{
    [SerializeField] private Image healthFill;
    [SerializeField] private Image damageFill;

    [SerializeField] private Vector3 worldOffset = new Vector3(0, 2f, 0);
    [SerializeField] private float chipDelay = 0.15f;
    [SerializeField] private float chipSpeed = 2f;

    private float currentHealth;
    private float maxHealth;
    private Transform target;
    private Coroutine chipRoutine;

    public void init(float maxHealth, Transform target, Transform canvasParent) {
        this.transform.parent = canvasParent;
        this.currentHealth = maxHealth;
        this.maxHealth = maxHealth;
        this.healthFill.fillAmount = 1f;
        this.damageFill.fillAmount = 1f;
        this.target = target;
        
        // transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }

    public void updateDamage(float damage) {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);

        float targetFill = currentHealth / maxHealth;
        healthFill.fillAmount = targetFill;

        if (chipRoutine != null)
            StopCoroutine(chipRoutine);

        chipRoutine = StartCoroutine(ChipDamage(targetFill));
    }

    private IEnumerator ChipDamage(float targetFill) {
        yield return new WaitForSeconds(chipDelay);

        while (damageFill.fillAmount > targetFill)
        {
            damageFill.fillAmount = Mathf.MoveTowards(
                damageFill.fillAmount,
                targetFill,
                chipSpeed * Time.deltaTime
            );
            yield return null;
        }

        Debug.Log("POST DAMAGE!" + healthFill.fillAmount + " | " + damageFill.fillAmount);
    }

    public void LateUpdate() {
        if (this.target)
            transform.position = this.target.position + this.worldOffset;
    }

    public void onDeath() {
        Destroy(this.gameObject);
    }
}

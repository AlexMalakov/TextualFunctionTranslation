using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HealthBarManager : MonoBehaviour
{
    [SerializeField] private Sprite[] mergeLevelFrames;
    [SerializeField] private Image tierFrame;

    [SerializeField] private Image healthFill;
    [SerializeField] private Image damageFill;

    [SerializeField] private Vector3 worldOffset = new Vector3(0, 3f, 0);
    [SerializeField] private float chipDelay = 0.15f;
    [SerializeField] private float chipSpeed = 2f;

    [SerializeField] private int dividerStart = 15;
    [SerializeField] private int healthPerChunk = 10;
    [SerializeField] private float dividerWidth = 0.03f;
    [SerializeField] private float minDividerWidth = 0.003f;
    [SerializeField] private float dividerWidthReduction = 0.0005f;
    [SerializeField] private GameObject hpDividerPrefab; //deadass it's just a squished rectangle
    [SerializeField] private Transform hpDividerParent;

    [SerializeField] private Color standardHealthColor;
    [SerializeField] private Color enemyHealthColor;
    [SerializeField] private Color maxHPColor;
    [SerializeField] private Image hpBarColorful;

    [SerializeField] private int maxDividerChunks = 36;

    [Header ("unit expander setting")]
    [SerializeField] private bool isCopyCat = false;

    private List<RectTransform> dividers;
    private bool isPlayerUnit = true;

    private float currentHealth;
    private float maxHealth;
    private Transform target;
    private Coroutine chipRoutine;

    public void init(float maxHealth, int mergeLevel, Transform target, Transform canvasParent) {
        this.transform.SetParent(canvasParent, false);
        this.target = target;

        this.initUniversal(maxHealth, mergeLevel);

        this.gameObject.name = "healthbar_of_" + target.gameObject.name;
        
    }

    public void setPlayerOwner(bool playerOwns) {
        if(this.isPlayerUnit == playerOwns) {
            return;
        }
        this.isPlayerUnit = playerOwns;
        this.repaintDividers();
    }

    public void initUniversal(float maxHealth, int mergeLevel) {
        this.currentHealth = maxHealth;
        this.maxHealth = maxHealth;
        this.healthFill.fillAmount = 1f;
        this.damageFill.fillAmount = 1f;
        
        this.setFrame(mergeLevel);


        this.dividers = new List<RectTransform>();
        this.buildNewDividers(this.dividerStart);
        this.repaintDividers();
    }

    public void reset() {
        this.currentHealth = maxHealth;
        this.healthFill.fillAmount = 1f;
        this.damageFill.fillAmount = 1f;
    }

    public void updateDamage(float damage) {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);

        float targetFill = currentHealth / maxHealth;
        healthFill.fillAmount = targetFill;

        if (chipRoutine != null)
            StopCoroutine(chipRoutine);

        chipRoutine = StartCoroutine(ChipDamage(targetFill));
    }

    public void noChipDamage(float curHP) {
        currentHealth = Mathf.Clamp(curHP, 0, maxHealth);
        float targetFill = currentHealth / maxHealth;
        healthFill.fillAmount = targetFill;
        damageFill.fillAmount = targetFill;
    }

    public void updateMaxHealth(float newMaxHealth, int mergeLevel) {
        float delta = newMaxHealth - this.maxHealth;
        this.maxHealth = newMaxHealth;
        if(delta > 0) {
            this.currentHealth += delta;
        }
        this.currentHealth = Mathf.Min(this.currentHealth, this.maxHealth);

        damageFill.fillAmount = this.currentHealth / this.maxHealth;
        healthFill.fillAmount = this.currentHealth / this.maxHealth;

        this.setFrame(mergeLevel);
        this.repaintDividers();
    }

    private IEnumerator ChipDamage(float targetFill) { //i am likely the culprit of some bullshit 
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

    private void buildNewDividers(int dCount) {
        for(int i = 0; i < dCount; i++) {
            this.dividers.Add(Instantiate(this.hpDividerPrefab, this.hpDividerParent).GetComponent<RectTransform>());
            this.dividers[this.dividers.Count - 1].gameObject.SetActive(false);
        }
    }
    
    private void repaintDividers() {
        foreach (RectTransform d in this.dividers) {
            d.gameObject.SetActive(false);
        }

        int chunks = Mathf.CeilToInt(this.maxHealth / this.healthPerChunk);

        if (chunks > this.maxDividerChunks) {
            this.hpBarColorful.color = this.maxHPColor;
            return;
        }
        this.hpBarColorful.color = this.isPlayerUnit ? this.standardHealthColor : this.enemyHealthColor;

        if(chunks - 1 > this.dividers.Count) {
            this.buildNewDividers(chunks - this.dividers.Count - 1);
        }

        for (int i = 1; i < chunks; i++)
        {
            this.dividers[i-1].gameObject.SetActive(true);
            float position = (float)(i * this.healthPerChunk) / this.maxHealth;

            this.dividers[i-1].anchorMin = new Vector2(position, 0);
            this.dividers[i-1].anchorMax = new Vector2(position, 1);

            float dWidth = Mathf.Max(
                this.minDividerWidth,
                this.dividerWidth - (chunks - 1) * this.dividerWidthReduction
            );

            this.dividers[i-1].offsetMin = new Vector2(-dWidth / 2f, 0);
            this.dividers[i-1].offsetMax = new Vector2(dWidth / 2f, 0);

            this.dividers[i-1].localRotation = Quaternion.identity;
            this.dividers[i-1].localScale = Vector3.one;
        }
    }

    private void setFrame(int mergeLevel) {
        this.tierFrame.sprite = this.mergeLevelFrames[mergeLevel - 1]; //indexing from 0 so -1
    }

    public void LateUpdate() {
        if(isCopyCat) {
            return;
        } else if (this.target)
            transform.position = this.target.position + this.worldOffset;
    }

    public void hideHPBar() {
        this.gameObject.SetActive(false);
    }

    public void show() {
        this.gameObject.SetActive(true);
    }

    public void onDeath() {
        //only called when the unit itself is getting destroyed
        Destroy(this.gameObject);
    }
}

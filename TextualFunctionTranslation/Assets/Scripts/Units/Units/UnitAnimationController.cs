using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject physicalModel;
    [SerializeField] private int attackType;
    [SerializeField] private bool isDemon;
    [SerializeField] private bool isDemonKing;

    private int randomIdle = 0;

    private void Awake() {
        animator.SetInteger("ATTACK_TYPE", this.attackType);
        animator.SetTrigger("idle");
    }

    public int getAttackType() {
        return this.attackType;
    }

    public void setAttackType(int newAttackType) {
        this.attackType = newAttackType;
        animator.SetInteger("ATTACK_TYPE", this.attackType);
    }

    private void OnEnable() {
        animator.SetInteger("ATTACK_TYPE", this.attackType);
        animator.SetBool("isKing", this.isDemonKing);
        // animator.SetTrigger("idle");
    }

    public void Update() {
        this.randomIdle = Random.Range(0, 4);
        animator.SetInteger("idle_random", this.randomIdle);
    }

    public void triggerAnimationWithSpeed(string trigger, float desiredDuration) {
        float speed = hardcodeCheckAnimationTimes(trigger) > 0 ? hardcodeCheckAnimationTimes(trigger)/desiredDuration: 1f;
        animator.speed = speed;
        animator.SetTrigger(trigger);
    }

    //wanted to do this with a serialized struct type but untiy didn't seem to wouldn't that
    //so instead this will do the job
    private float hardcodeCheckAnimationTimes(string trigger) {
        if(isDemon) {
            if(trigger == "windup_attack") {
                if(this.attackType == 0) {
                    return .93f;
                } else {
                    return 1.7f;
                }
            } else if(trigger == "attack") {
                if(this.attackType == 0) {
                    return 1.7f;
                } else {
                    return 1.8f;
                }
            }
        }
        if(trigger == "windup_attack") {
            if(this.attackType == 0) {
                return 1f;
            } else if(this.attackType == 1) {
                return 2.03f;
            } else if(this.attackType == 2) {
                return .75f;
            }
        } else if(trigger == "attack") {
            if(this.attackType == 0) {
                return .7f;
            } else if(this.attackType == 1) {
                return 1.07f;
            } else if(this.attackType == 2) {
                return .95f;
            }
        }
        return -1f;
    }

    public void triggerAnimation(string trigger) {
        animator.speed = 1f;
        animator.SetTrigger(trigger);
    }


}

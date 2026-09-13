using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private int attackType;

    private int randomIdle = 0;

    private void Awake() {
        animator.SetInteger("ATTACK_TYPE", this.attackType);
        animator.SetTrigger("idle");
    }

    public void Update() {
        this.randomIdle = Random.Range(0, 4);
        animator.SetInteger("idle_random", this.randomIdle);
    }

    public void triggerAnimation(string trigger) {
        this.animator.SetTrigger(trigger);
    }
}

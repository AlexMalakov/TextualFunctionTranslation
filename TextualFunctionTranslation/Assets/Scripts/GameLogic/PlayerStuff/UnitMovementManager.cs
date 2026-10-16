using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovementManager : MonoBehaviour
{
    private Dictionary<Unit, int> moveTokens = new Dictionary<Unit, int>();
    [SerializeField] private float unitDragSnapSpeed;
    [SerializeField] private float unitJumpSpeed;
    [SerializeField] private float boingMultiplicitiveHeight;
    [SerializeField] private float unitFastTime = .1f;

    public void overrideMovement(Unit u) {
        this.claimMovement(u);
    }

    private int claimMovement(Unit u) {
        int token = this.moveTokens.TryGetValue(u, out int cur) ? cur + 1 : 1;
        this.moveTokens[u] = token;
        return token;
    }

    private bool ownsMovement(Unit u, int token) {
        return this.moveTokens.TryGetValue(u, out int cur) && cur == token;
    }

    private void releaseMovement(Unit u, int token) {
        if(this.ownsMovement(u, token)) {
            this.moveTokens.Remove(u);
        }
    }

    public IEnumerator dragUnitTo(Unit u, Transform dest, bool snap) {
        return this.dragUnitTo(u, dest.position, snap);
    }

    public IEnumerator dragUnitTo(Unit u, Vector3 dest, bool snap) {
        int token = this.claimMovement(u);

        Vector3 startPos = u.transform.position;

        dest.y = startPos.y;

        float elapsed = 0f;
        float totalDur = !snap ? unitFastTime : (dest - startPos).magnitude / this.unitDragSnapSpeed;

        while(elapsed < totalDur) {
            if(!this.ownsMovement(u, token)) {
                yield break;
            }

            u.transform.position = Vector3.Lerp(startPos, dest, elapsed/totalDur);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if(this.ownsMovement(u, token)) {
            u.transform.position = dest;
        }
        this.releaseMovement(u, token);
    }

    public IEnumerator boingTo(Unit u, Vector3 dest) {
        int token = this.claimMovement(u);

        Vector3 startPos = u.transform.position;
        dest.y = startPos.y;
        u.triggerAnimation("leap");

        float elapsed = 0f;
        float totalDur = (dest - startPos).magnitude / this.unitJumpSpeed;
        float arcHeight = (dest - startPos).magnitude * this.boingMultiplicitiveHeight;

        while(elapsed < totalDur) {
            if(!this.ownsMovement(u, token)) {
                yield break;
            }

            Vector3 uPos = Vector3.Lerp(startPos, dest, elapsed/totalDur);
            uPos.y = Mathf.Sin((elapsed/totalDur) * Mathf.PI) * arcHeight;
            u.transform.position = uPos;
            elapsed += Time.deltaTime;
            yield return null;
        }

        if(this.ownsMovement(u, token)) {
            u.triggerAnimation("landing");
            u.transform.position = dest;
        }
        this.releaseMovement(u, token);
    }
}

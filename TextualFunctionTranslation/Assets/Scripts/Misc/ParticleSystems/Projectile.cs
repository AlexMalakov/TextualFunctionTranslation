using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Projectile : MonoBehaviour
{
    public IEnumerator launchProjectile(Unit sourceUnit, Unit destUnit) {

        Vector3 startPos; Vector3 endPos;
        try {
            startPos = sourceUnit.transform.position;
            endPos = destUnit.transform.position;
        } catch (Exception ex) {
            this.gameObject.SetActive(false);
            yield break;
        }
        
        float duration = .3f;
        float elapsed = 0f;

        this.transform.position = startPos;
        this.transform.LookAt(destUnit.transform);

        while(elapsed < duration) {
            try {
                endPos = destUnit.transform.position;

            } catch (Exception ex) {
                this.gameObject.SetActive(false);
                yield break;
            }

            this.transform.position = Vector3.Lerp(startPos, endPos, elapsed/duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        this.gameObject.SetActive(false);
    }
}

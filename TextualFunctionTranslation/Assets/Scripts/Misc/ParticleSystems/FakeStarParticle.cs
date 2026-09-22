using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FakeStarParticle : MonoBehaviour {

    [SerializeField] private List<GameObject> stars;
    [SerializeField] private float rotationSpeed = 90f;

    private Coroutine orbitCo;
    private float duration;

    public void doOrbit(float duration, GameObject cam, Unit u) {
        this.duration = duration;
        this.orbitCo = StartCoroutine(this.orbit(cam, u));
    }

    public IEnumerator orbit(GameObject cam, Unit u) {
        while (this.duration > 0) {
            this.transform.Rotate(0f, this.rotationSpeed * Time.deltaTime, 0f);

            foreach(GameObject star in this.stars) {
                star.transform.LookAt(cam.transform);
            }

            this.duration -= Time.deltaTime;
            yield return null;
        }

        u.stunOver();
        this.gameObject.SetActive(false);
    }

    public void increaseDurationTo(float newDur) {
        this.duration = newDur;
    }

    public void cancelAnim() {
        if(this.orbitCo != null) {
            StopCoroutine(this.orbitCo);
            this.orbitCo = null;
        }
        this.gameObject.SetActive(false);
    }

}

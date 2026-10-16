using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusMessage : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text text;

    [Header("Timing")]
    [SerializeField] private float fadeInTime = 0.25f;
    [SerializeField] private float visibleTime = 2f;
    [SerializeField] private float fadeOutTime = 0.5f;


    private Coroutine coroutine;

    public void displayMessage(string msg) {
        if(this.coroutine != null) {
            StopCoroutine(this.coroutine);
        }

        this.text.text = msg;
        this.gameObject.SetActive(true);
        this.coroutine = StartCoroutine(runFade());
    }

    private IEnumerator runFade() {
        this.gameObject.SetActive(true);

        float elapsed = 0f;
        while(elapsed < this.fadeInTime) {
            this.canvasGroup.alpha = elapsed / this.fadeInTime;
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        this.canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(visibleTime);

        elapsed = 0f;
        while(elapsed < this.fadeOutTime) {
            this.canvasGroup.alpha = 1 - (elapsed / this.fadeOutTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        this.canvasGroup.alpha = 0f;
        this.gameObject.SetActive(false);
        this.coroutine = null;
    }

}
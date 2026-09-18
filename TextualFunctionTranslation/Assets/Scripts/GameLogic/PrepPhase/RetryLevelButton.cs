using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//lets say hypothetically we had one script for buttosn jk loser
public class RetryLevelButton : MonoBehaviour //number 6 lets fucking goooooo
{
    //ok everyone say it with me "abstraction is for dorks!!!!"
    [SerializeField] private Button button;
    [SerializeField] private GameObject screenCover;
    [SerializeField] private GameManager gm;

    private bool works = true;

    private Trait trait;

    public void Awake() { //Awake because this doesn't affect other systems
        UnityEngine.UI.Button btn = button.GetComponent<UnityEngine.UI.Button>();
		btn.onClick.AddListener(onButtonClick);
    }

    //mr president a fifth button script has hit the tower
    public void onButtonClick() {
        if(!this.works) {
            return;
        }
        this.gm.onResultsEnd(false);
    }

    public void enableButton(bool enabled) {
        this.works = enabled;
        this.screenCover.SetActive(!enabled);
    }
}

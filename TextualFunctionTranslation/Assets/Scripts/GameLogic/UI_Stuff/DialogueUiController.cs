using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class DialogueUiController : MonoBehaviour
{
    [SerializeField] private List<DialogueIconMap> nameToIcon;
    [SerializeField] private GameObject leftObject;
    [SerializeField] private GameObject rightObject;
    [SerializeField] private GameObject belowBar;

    [SerializeField] private Image rightImage;
    [SerializeField] private Image leftImage;

    [SerializeField] private TMP_Text rightText;
    [SerializeField] private TMP_Text leftText;

    public void displayLine(bool left, string iconToShow, string text) {
        if(left) {
            this.leftObject.SetActive(true);
            this.rightObject.SetActive(false);
            this.belowBar.SetActive(true);

            this.leftImage.sprite = this.nameToIcon.First(x => x.name == iconToShow).icon;
            this.leftText.text = text;
        } else {
            this.leftObject.SetActive(false);
            this.rightObject.SetActive(true);
            this.belowBar.SetActive(true);

            Debug.Log("MATCH FOR icon :" + iconToShow);
            this.rightImage.sprite = this.nameToIcon.First(x => x.name == iconToShow).icon;
            this.rightText.text = text;
        }
    }

    public void hideDialogue() {
        this.leftObject.SetActive(false);
        this.rightObject.SetActive(false);
        this.belowBar.SetActive(false);
    }
}

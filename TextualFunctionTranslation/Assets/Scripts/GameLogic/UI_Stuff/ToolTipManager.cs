using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ToolTipManager : MonoBehaviour
{
    [SerializeField] private GameObject toolTipPopUp;
    [SerializeField] private TMP_Text text;

    public void Update() {
        if(EventSystem.current == null) {
            return;
        }

        PointerEventData ptr = new PointerEventData(EventSystem.current){position = Input.mousePosition};
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(ptr, results);

        foreach(RaycastResult r in results) {
            ToolTip t = r.gameObject.GetComponent<ToolTip>();

            if(t != null) {
                this.showTip(t);
                return;
            }
        }

        this.hideTip();
    }


    private void showTip(ToolTip tip) {
        toolTipPopUp.SetActive(true);
        text.text = tip.getToolTip();

        RectTransform target = tip.GetComponent<RectTransform>();
        Vector3[] corners = new Vector3[4];
        target.GetWorldCorners(corners);
        Vector3 bottomCenter = (corners[0] + corners[3]) / 2f;
        toolTipPopUp.GetComponent<RectTransform>().position = bottomCenter;
    }

    private void hideTip() {
        toolTipPopUp.SetActive(false);
    }
}

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
    [SerializeField] private float gap = 10f;

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

        RectTransform popup = toolTipPopUp.GetComponent<RectTransform>();
        Rect target = this.getScreenRect(tip.GetComponent<RectTransform>());
        Rect pop = this.getScreenRect(popup);

        float w = pop.width;
        float h = pop.height;

        //vertical: below if it fits, else above, else whichever side has more room
        float roomBelow = target.yMin - this.gap;
        float roomAbove = Screen.height - target.yMax - this.gap;
        float y;
        if(roomBelow >= h) {
            y = target.yMin - this.gap - h / 2f;
        } else if(roomAbove >= h) {
            y = target.yMax + this.gap + h / 2f;
        } else {
            y = roomBelow >= roomAbove ? h / 2f : Screen.height - h / 2f;
        }

        //horizontal: centered on the target, slid left/right to stay on screen
        float x = Mathf.Clamp(target.center.x, w / 2f, Screen.width - w / 2f);

        //move by the difference so pivot/anchors don't matter
        Vector3 delta = new Vector3(x, y, 0f) - (Vector3)pop.center;
        popup.position += delta;
    }

    private void hideTip() {
        toolTipPopUp.SetActive(false);
    }

    private Rect getScreenRect(RectTransform rt) {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        return new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);
  }
}

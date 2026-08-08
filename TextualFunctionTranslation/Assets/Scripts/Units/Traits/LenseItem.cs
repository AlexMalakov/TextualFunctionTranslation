using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LenseItem : MonoBehaviour
{
    
    [SerializeField] private HumanPlayer player;
    private bool lenseToggle = false;



    private void toggleLenses() {
        this.lenseToggle = !this.lenseToggle;
    }

    public void checkUserInteraction() {
        if(this.lenseToggle && player.getLenses() > 0 && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider.gameObject.GetComponentInParent<HexSquare>() != null) {
                    this.useLense(hit.collider.gameObject.GetComponentInParent<HexSquare>());
                }
            }
        }
    }



    private void useLense(HexSquare s) {
        if(s.isShowingChar()) {
            return;
        }

        this.player.gainLenses(-1);
        s.displayValue(true);
    }
}

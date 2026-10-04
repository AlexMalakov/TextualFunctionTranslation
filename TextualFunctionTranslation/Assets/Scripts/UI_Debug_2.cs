using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Debug_2 : MonoBehaviour
{
    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        PointerEventData data = new PointerEventData(EventSystem.current);
        data.position = Input.mousePosition;

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);

        Debug.Log("=== UI RAYCAST ===");

        foreach (RaycastResult result in results)
        {
            Debug.Log(result.gameObject.name);
        }
    }
}


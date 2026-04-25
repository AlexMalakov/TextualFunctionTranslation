using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Debug : MonoBehaviour
{
    [SerializeField] private TMP_Text debug_box;

    public void publish_to_debug(string query) {
        debug_box.text = query;
    }
}

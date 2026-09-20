using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//yep its another button but this one actually makes sense to be different
public class CopyButton : MonoBehaviour
{
    [SerializeField] private TMP_Text tmpText;
    
    private void Awake()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(CopyText);
    }

    private void CopyText()
    {
        GUIUtility.systemCopyBuffer = tmpText.text;
    }
}


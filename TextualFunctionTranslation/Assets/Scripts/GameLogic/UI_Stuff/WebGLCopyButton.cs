using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//claude also made this one i dont know how javascript in c# works
public class WebGLCopyButton : MonoBehaviour
{
    [SerializeField] private TMP_Text source;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void WebGLCopyToClipboard(string text);
#endif

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
#if UNITY_WEBGL && !UNITY_EDITOR
        button.onClick.AddListener(CopyForWebGL);
#endif
    }

    private void OnDestroy()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (button != null) button.onClick.RemoveListener(CopyForWebGL);
#endif
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    private void CopyForWebGL()
    {
        if (source == null) return;
        WebGLCopyToClipboard(source.text);
    }
#endif
}
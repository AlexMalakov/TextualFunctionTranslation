using UnityEngine;

[RequireComponent(typeof(Shell))]
public class DelayedShellDestroy : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 2f;
    private void Start()
    {
        Invoke(nameof(DestroyShell), _lifeTime);
    }

    private void DestroyShell()
    {
        GetComponent<Shell>().Destroy();
    }
}
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Shell : MonoBehaviour
{
    public UnityEvent onDestroy;
    public UnityEvent<Collision> onCollided;
    private void OnCollisionEnter(Collision collision)
    {
        onCollided.Invoke(collision);
    }

    public void Destroy()
    {
        onDestroy.Invoke();
        Destroy(gameObject);
    }
}

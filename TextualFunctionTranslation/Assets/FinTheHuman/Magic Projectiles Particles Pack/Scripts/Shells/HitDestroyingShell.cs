
public class HitDestroyingShell : Shell
{
    private void Start()
    {
        onCollided.AddListener((c) => Destroy());
    }
}

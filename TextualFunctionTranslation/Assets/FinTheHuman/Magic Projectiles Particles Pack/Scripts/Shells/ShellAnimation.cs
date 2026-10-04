using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Shell))]
public class ShellAnimation : MonoBehaviour
{
    [SerializeField] private float _duration;
    [SerializeField] private AnimationCurve _xScaleAnimation;
    [SerializeField] private AnimationCurve _yScaleAnimation;
    [SerializeField] private AnimationCurve _zScaleAnimation;

    public void Animate()
    {
        StartCoroutine(AnimationByTime());
    } 

    public IEnumerator AnimationByTime()
    {
        float expiredSeconds = 0;
        float progress = 0;
        Vector3 scale = transform.localScale;

        while (progress < 1)
        {
            expiredSeconds += Time.deltaTime;
            progress = expiredSeconds / _duration;
            transform.localScale = new Vector3(
                scale.x * _xScaleAnimation.Evaluate(progress), 
                scale.y * _yScaleAnimation.Evaluate(progress), 
                scale.z * _zScaleAnimation.Evaluate(progress));

            yield return null;
        }
    }
}

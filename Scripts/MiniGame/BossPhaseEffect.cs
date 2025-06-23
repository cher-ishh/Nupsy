using System.Collections;
using UnityEngine;

public class BossPhaseEffect : MonoBehaviour
{
    public Transform bossTransform;

    public IEnumerator PlayPhaseEffect(Vector3 targetScale, float targetY, float bounceDuration, int bounceCount)
    {
        Vector3 originalScale = bossTransform.localScale;
        Vector3 shrinkScale = originalScale * 1.5f;

        for (int i = 0; i < bounceCount; i++)
        {
            yield return ScaleOverTime(originalScale, shrinkScale, bounceDuration / 2f);
            yield return ScaleOverTime(shrinkScale, originalScale, bounceDuration / 2f);
        }

        yield return ScaleAndMove(targetScale, targetY, 0.5f);
    }

    private IEnumerator ScaleOverTime(Vector3 from, Vector3 to, float duration)
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            bossTransform.localScale = Vector3.Lerp(from, to, t / duration);
            yield return null;
        }

        bossTransform.localScale = to;
    }

    private IEnumerator ScaleAndMove(Vector3 targetScale, float targetY, float duration)
    {
        float t = 0f;
        Vector3 startScale = bossTransform.localScale;
        Vector3 startPos = bossTransform.position;
        Vector3 endPos = new Vector3(startPos.x, targetY, startPos.z);

        while (t < duration)
        {
            t += Time.deltaTime;
            float rate = t / duration;
            bossTransform.localScale = Vector3.Lerp(startScale, targetScale, rate);
            bossTransform.position = Vector3.Lerp(startPos, endPos, rate);
            yield return null;
        }

        bossTransform.localScale = targetScale;
        bossTransform.position = endPos;
    }
}

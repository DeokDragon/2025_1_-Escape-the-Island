using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeController : MonoBehaviour
{
    public Image fadePanel;
    public float fadeSpeed = 1.5f;

    public void FadeOut()
    {
        StartCoroutine(Fade(0f, 1f));
    }

    public void FadeIn()
    {
        StartCoroutine(Fade(1f, 0f));
    }

    private IEnumerator Fade(float from, float to)
    {
        float t = 0;
        Color color = fadePanel.color;

        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            float alpha = Mathf.Lerp(from, to, t);
            fadePanel.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
    }
}

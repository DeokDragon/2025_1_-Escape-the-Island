using UnityEngine;
using TMPro;

public class TutorialHint : MonoBehaviour
{
    public TextMeshProUGUI hintText;
    public float blinkSpeed = 2f;
    public float displayTime = 8f;

    private float timer;

    void Start()
    {
        timer = displayTime;
    }

    void Update()
    {
        float alpha = Mathf.Abs(Mathf.Sin(Time.time * blinkSpeed));
        Color color = hintText.color;
        color.a = alpha;
        hintText.color = color;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            // ±ò²ûÈ÷ ¾ËÆÄ°ª 0À¸·Î ÃÊ±âÈ­ ÈÄ ²¨ÁÖ±â
            color.a = 0f;
            hintText.color = color;

            gameObject.SetActive(false);
        }
    }
}

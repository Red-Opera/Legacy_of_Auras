using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TouchLabelEffect : MonoBehaviour
{
    private Text text;

    public float loopTime = 4.0f;

    public void Start()
    {
        text = GetComponent<Text>();
        StartCoroutine(BlinkText());
    }

    private IEnumerator BlinkText()
    {
        float elapsedTime = 0f;

        Color color = text.color;
        Color toColor = new Color(color.r, color.g, color.b, 0);

        while (!StartButton.isNextScene)
        {
            // 시간에 따라 투명도를 조절
            float t = (elapsedTime % loopTime) * 2 / loopTime;

            if (t < 1f)
                text.color = Color.Lerp(color, toColor, t);

            else
                text.color = Color.Lerp(toColor, color, -(1f - t));

            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }
}

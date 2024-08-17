using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeInOutEffect : MonoBehaviour
{
    [SerializeField] private float fadeTime = 1.0f;

    private Image image;
    private SpriteRenderer sprite;
    private Text text;

    private Color color = Color.black;
    private Color toColor = Color.black;

    private bool callFirst = true;

    private void Awake()
    {
        image = GetComponent<Image>();    // 만약 페이트 효과를 넣을 객체가 일반 색깔인 경우
        sprite = GetComponent<SpriteRenderer>();                // 만약 페이트 효과를 넣을 객체가 텍스쳐인 경우
        text = GetComponent<Text>();                            // 만약 페이트 효과를 넣을 객체가 텍스트인 경우

        // 일반 컬러 또는 텍스쳐 또는 텍스쳐가 아닌 경우 에러 발생
        bool isCheck = image == null && sprite == null && text == null;
        Debug.Assert(!isCheck, "Error (Null Reference) : 해당 객체에 MeshRenderer, sprite, text 중 하나가 존재하지 않습니다.");
    }

    // 페이드 아웃과 페이드 인하기 전에 색깔 지정
    private void Initialization()
    {
        // 목적지 색깔 지정 (불투명 -> 투명)
        toColor = new Color(color.r, color.g, color.b, 0);

        if (image != null)
            color = image.color;

        else if (sprite != null)
            color = sprite.color;

        else
            color = text.color;
    }

    public void SceneLoadedFadeIn(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeIn(fadeTime, true));
    }

    public void SceneUnLoadedFadeOut(Scene scene)
    {
        gameObject.SetActive(true);
        StartCoroutine(FadeOut(fadeTime));
    }

    // 페이드 인
    public IEnumerator FadeIn(float fadeTime, bool isRemove = false)
    {
        float elapsedTime = 0f;
        
        // 첫번째로 실행하는지 확인함
        if (callFirst)
        {
            Initialization();
            callFirst = false;
        }

        while (elapsedTime < fadeTime)
        {
            // 시간에 따라 투명도를 조절
            float t = elapsedTime / fadeTime;

            if (image != null)
                image.color = Color.Lerp(color, toColor, t);

            else if (sprite != null)
                sprite.color = Color.Lerp(color, toColor, t);

            else
                text.color = Color.Lerp(color, toColor, t);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        if (isRemove)
            gameObject.SetActive(false);
    }

    // 페이드 아웃
    public IEnumerator FadeOut(float fadeTime)
    {
        float elapsedTime = 0f;
        gameObject.SetActive(true);

        if (callFirst)
        {
            Initialization();
            callFirst = false;
        }

        while (elapsedTime < fadeTime)
        {
            // 시간에 따라 투명도를 조절
            float t = elapsedTime / fadeTime;

            if (image != null)
                image.color = Color.Lerp(toColor, color, t);

            else if (sprite != null)
                sprite.color = Color.Lerp(toColor, color, t);

            else
                text.color = Color.Lerp(toColor, color, t);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }
}

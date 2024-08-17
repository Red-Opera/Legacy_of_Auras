using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    [SerializeField] private GameObject loginObj;

    public SpriteRenderer titleImg;
    private CanvasGroup canvas;
    private AudioSource backgroundSound;

    private float fadeTime = 1f;

    public static bool isNextScene = false;
    private static bool isStart = false;

    private void Start()
    {
        Debug.Assert(loginObj != null, "Error (Null Reference) : 로그인 오브젝트가 존재하지 않습니다.");

        canvas = transform.parent.GetComponent<CanvasGroup>();

        if (canvas == null)
            Debug.Assert(false, "Error (Null Reference) : 부모가 존재하지 않습니다.");

        if (titleImg == null)
            Debug.Assert(false, "Error (Null Reference) : 배경이미지가 존재하지 않습니다.");

        backgroundSound = GetComponent<AudioSource>();
        Debug.Assert(backgroundSound != null, "Error (Null Reference) : 배경음악이 존재하지 않습니다.");
    }

    private void Update()
    {
        if (Login.isStart && !isStart)
        {
            isStart = true;
            StartCoroutine(FadeOut());
        }
    }

    public void LoadScene() 
    { 
        isNextScene = true;

        loginObj.SetActive(true);
        Login.GameLogin();
    }
        
    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        float startAlpha = canvas.alpha;

        Color color = titleImg.color;
        Color toColor = new Color(color.r, color.g, color.b, 0);

        while (elapsedTime < fadeTime)
        {
            // 시간에 따라 투명도를 조절
            float t = elapsedTime / fadeTime;

            backgroundSound.volume = Mathf.Lerp(1.0f, 0.0f, t);
            canvas.alpha = Mathf.Lerp(startAlpha, 0f, t);
            titleImg.color = Color.Lerp(color, toColor, t);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // 페이드 아웃이 완료된 후 Canvas를 비활성화
        canvas.alpha = 0f;
        titleImg.gameObject.SetActive(false);
        SceneManager.LoadScene("Prologue");

        gameObject.SetActive(false);
    }
}
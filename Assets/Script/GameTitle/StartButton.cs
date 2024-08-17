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
        Debug.Assert(loginObj != null, "Error (Null Reference) : �α��� ������Ʈ�� �������� �ʽ��ϴ�.");

        canvas = transform.parent.GetComponent<CanvasGroup>();

        if (canvas == null)
            Debug.Assert(false, "Error (Null Reference) : �θ� �������� �ʽ��ϴ�.");

        if (titleImg == null)
            Debug.Assert(false, "Error (Null Reference) : ����̹����� �������� �ʽ��ϴ�.");

        backgroundSound = GetComponent<AudioSource>();
        Debug.Assert(backgroundSound != null, "Error (Null Reference) : ��������� �������� �ʽ��ϴ�.");
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
            // �ð��� ���� ������ ����
            float t = elapsedTime / fadeTime;

            backgroundSound.volume = Mathf.Lerp(1.0f, 0.0f, t);
            canvas.alpha = Mathf.Lerp(startAlpha, 0f, t);
            titleImg.color = Color.Lerp(color, toColor, t);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // ���̵� �ƿ��� �Ϸ�� �� Canvas�� ��Ȱ��ȭ
        canvas.alpha = 0f;
        titleImg.gameObject.SetActive(false);
        SceneManager.LoadScene("Prologue");

        gameObject.SetActive(false);
    }
}
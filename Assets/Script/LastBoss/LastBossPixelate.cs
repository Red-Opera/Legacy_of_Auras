using System.Collections;
using UnityEngine;

public class LastBossPixelate : MonoBehaviour
{
    [SerializeField] private RenderTexture renderTexture;   // �ȼ�ȭ�ϴ� �̹���

    [SerializeField] private float pixelateTime = 2.0f;     // �ȼ�ȭ�ϴ� �ð�

    private GameObject pixelCamera;     // �ȼ�ȭ�� ����ϴ� ī�޶�
    private LastBossAction action;      // ���� ���� �ൿ ������Ʈ
    private Animator animator;          // ���� ���� �ִϸ�����
    private bool isPixel = false;       // ���� �ȼ�ȭ�� �Ǿ����� ����

    public void Start()
    {
        pixelCamera = GameObject.Find("Pixel").transform.GetChild(0).gameObject;
        Debug.Assert(pixelCamera != null, "Error (Null Reference) : �ȼ�ȭ�ϴ� ī�޶� �������� �ʽ��ϴ�.");

        action = GetComponent<LastBossAction>();
        Debug.Assert(action != null, "Error (Null Reference) : ���� ������ �ൿ�� �����ϴ� ������Ʈ�� �������� �ʽ��ϴ�.");

        animator = GetComponent<Animator>();
        Debug.Assert(animator != null, "Error (Null Reference) : �ִϸ����Ͱ� �������� �ʽ��ϴ�.");
    }

    public IEnumerator PixelToggle()
    {
        // ���� ������ ���� �ʵ��� �����ϰ� �ȼ�ȭ �Ǵ� ���ȼ�ȭ ���� ����
        action.isAction = true;
        isPixel = !isPixel;

        // �ȼ� ī�޶� Ȱ��ȭ
        pixelCamera.SetActive(true);

        // ���� �ػ󵵸� ������
        Vector2 defualtSize = new Vector2(renderTexture.width, renderTexture.height);

        float startTime = Time.time;
        while (true)
        {
            float t = Time.time - startTime;

            if (t > pixelateTime)
                break;

            // ���� �ȼ�ȭ�� �ǵ��� ����
            renderTexture.Release();
            if (isPixel)
            {
                renderTexture.width = (int)Mathf.Lerp(960.0f, defualtSize.x, t);
                renderTexture.height = (int)Mathf.Lerp(480.0f, defualtSize.y, t);
            }

            // ���� ���ȼ�ȭ�� �ǵ��� ����
            else
            {
                renderTexture.width = (int)Mathf.Lerp(defualtSize.x, 960.0f, t);
                renderTexture.height = (int)Mathf.Lerp(defualtSize.y, 480.0f, t);
            }
            renderTexture.Create();
            
            yield return null;
        }

        // �ȼ�ȭ ���� �� �ػ󵵸� ����
        renderTexture.Release();
        renderTexture.width = (int)defualtSize.x;
        renderTexture.height = (int)defualtSize.y;
        renderTexture.Create();

        // �ȼ�ȭ�� ���� ī�޶� Ű���� ������ ����
        pixelCamera.SetActive(isPixel);

        // �ѹ��� �����ϴ� ���� ����
        while (true)
        {
            bool isIdle = animator.GetCurrentAnimatorStateInfo(0).IsName("Idle");

            if (isIdle)
                break;

            yield return null;
        }

        action.isAction = false;
    }
}

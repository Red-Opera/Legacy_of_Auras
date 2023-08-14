using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOutEffect : MonoBehaviour
{
    private MeshRenderer mesh;
    private SpriteRenderer sprite;
    private Text text;

    private Color color = Color.black;
    private Color toColor = Color.black;

    private bool callFirst = true;

    public void Awake()
    {
        mesh = GetComponent<MeshRenderer>();        // ���� ����Ʈ ȿ���� ���� ��ü�� �Ϲ� ������ ���
        sprite = GetComponent<SpriteRenderer>();    // ���� ����Ʈ ȿ���� ���� ��ü�� �ؽ����� ���
        text = GetComponent<Text>();                // ���� ����Ʈ ȿ���� ���� ��ü�� �ؽ�Ʈ�� ���

        // �Ϲ� �÷� �Ǵ� �ؽ��� �Ǵ� �ؽ��İ� �ƴ� ��� ���� �߻�
        bool isCheck = mesh == null && sprite == null && text == null;
        Debug.Assert(!isCheck, "Error (Null Reference) : �ش� ��ü�� MeshRenderer, sprite, text �� �ϳ��� �������� �ʽ��ϴ�.");
    }

    // ���̵� �ƿ��� ���̵� ���ϱ� ���� ���� ����
    public void Initialization()
    {
        // ������ ���� ���� (������ -> ����)
        toColor = new Color(color.r, color.g, color.b, 0);

        if (mesh != null)
            color = mesh.material.color;

        else if (sprite != null)
            color = sprite.material.color;

        else
            color = text.color;
    }

    // ���̵� ��
    public IEnumerator FadeIn(float fadeTime, bool isRemove = false)
    {
        float elapsedTime = 0f;
        
        // ù��°�� �����ϴ��� Ȯ����
        if (callFirst)
        {
            Initialization();
            callFirst = false;
        }

        while (elapsedTime < fadeTime)
        {
            // �ð��� ���� ������ ����
            float t = elapsedTime / fadeTime;

            if (mesh != null)
                mesh.material.color = Color.Lerp(color, toColor, t);

            else if (sprite != null)
                sprite.material.color = Color.Lerp(color, toColor, t);

            else
                text.color = Color.Lerp(color, toColor, t);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        if (isRemove)
            gameObject.SetActive(false);
    }

    // ���̵� �ƿ�
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
            // �ð��� ���� ������ ����
            float t = elapsedTime / fadeTime;

            if (mesh != null)
                mesh.material.color = Color.Lerp(toColor, color, t);

            else if (sprite != null)
                sprite.material.color = Color.Lerp(toColor, color, t);

            else
                text.color = Color.Lerp(toColor, color, t);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }
}

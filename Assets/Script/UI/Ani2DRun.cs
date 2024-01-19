using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Ani2DRun : MonoBehaviour
{
    public List<Sprite> sprites;            // ����� �̹��� ����
    public bool isLoop = false;             // �ִϸ��̼��� ���ѹݺ����� �̷�������� ����

    public int framePerSec = 20;            // �����Ӵ� ������ ����Ұ��� ����
    public bool isSign = false;             // ���� ���� ������ Ȯ��

    private Image spriteRenderer;           // �̹��� ��� ������Ʈ
    private int currentSpriteIndex = 0;     // ���� ��������Ʈ �ε���

    private float startPlayTime = 0.0f;

    void Start()
    {
        spriteRenderer = GetComponent<Image>();

        spriteRenderer.sprite = sprites[currentSpriteIndex];
        spriteRenderer.color = new Color(0, 0, 0, 0);
    }

    void Update()
    {
        
    }

    public IEnumerator Play(bool isLoop)
    {
        isSign = true;
        spriteRenderer.color = Color.white;

        startPlayTime = Time.time;
        currentSpriteIndex = 0;

        while (isLoop || (framePerSec * (Time.time - startPlayTime) < sprites.Count))
        {
            // ���� �ε����� ����
            currentSpriteIndex = (int)(framePerSec * (Time.time - startPlayTime)) % sprites.Count;

            // ���� ��������Ʈ�� ����
            spriteRenderer.sprite = sprites[currentSpriteIndex];

            // ���
            yield return new WaitForSeconds(1.0f / framePerSec);
        }

        // �ִϸ��̼��� ��� ����� õõ�� �Ⱥ��̵��� ����
        while (spriteRenderer.color.a > 0)
        {
            float alpha = spriteRenderer.color.a - Time.deltaTime / 1.0f; // ���÷� Time.deltaTime�� �̿��Ͽ� ����
            spriteRenderer.color = new Color(1f, 1f, 1f, alpha);

            yield return null; // �� �����Ӿ� ���
        }

        currentSpriteIndex = 0;
        spriteRenderer.sprite = sprites[currentSpriteIndex];

        isSign = false;
    }
}

using System;
using TMPro;
using UnityEngine;

public class PlayerHeartBounce : MonoBehaviour
{
    public float maxScale;                  // ������ �ִ� ũ��
    public float minRate;                   // �ּ� �ֱ� (���� : ��)
    public float maxRate;                   // �ִ� �ֱ� (���� : ��)

    public GameObject HeartImg;             // ��Ʈ �̹���
    public PlayerState state;               // �÷��̾� ����
    public TextMeshProUGUI currentHP;       // ���� ü���� ��Ÿ���� �ؽ�Ʈ

    private Vector3 defaultHeartimgScale;   // �̹����� �ʱ� ũ��
    private float currentTime;              // ���� �ֱ��� �ð�
    public Material imgMaterial;

    void Start()
    {
        currentTime = 0;

        defaultHeartimgScale = HeartImg.transform.localScale;
    }

    void Update()
    {
        // ü���� �ۼ�Ʈ�� ���� �� ���� �ֱ��� �ð��� ����
        float persent = (float)Int32.Parse(currentHP.text) / state.HP;
        float rate = 60 / Mathf.Lerp(maxRate, minRate, persent);

        // ũ�� Ŀ���� ȿ��
        if (currentTime < rate / 4)
            HeartImg.transform.localScale = Vector3.Lerp(defaultHeartimgScale, defaultHeartimgScale * maxScale, currentTime / (rate / 4));

        // ũ�� �۾����� ȿ��
        else if (currentTime <= rate / 2)
            HeartImg.transform.localScale = Vector3.Lerp(defaultHeartimgScale * maxScale, defaultHeartimgScale, (currentTime - rate / 4) / (rate / 4));

        // ������ �ð��� ���� ��� �ʱ�ȭ ��
        else if (currentTime >= rate)
            currentTime = 0.0f;

        currentTime += Time.deltaTime;
        imgMaterial.SetFloat("_RatePersent", persent);
    }
}

using System;
using TMPro;
using UnityEngine;

public class PlayerHeartBounce : MonoBehaviour
{
    public float maxScale;                  // 심장의 최대 크기
    public float minRate;                   // 최소 주기 (단위 : 분)
    public float maxRate;                   // 최대 주기 (단위 : 분)

    public GameObject HeartImg;             // 하트 이미지
    public PlayerState state;               // 플레이어 상태
    public TextMeshProUGUI currentHP;       // 현재 체력을 나타내는 텍스트

    private Vector3 defaultHeartimgScale;   // 이미지의 초기 크기
    private float currentTime;              // 현재 주기의 시간
    public Material imgMaterial;

    void Start()
    {
        currentTime = 0;

        defaultHeartimgScale = HeartImg.transform.localScale;
    }

    void Update()
    {
        // 체력의 퍼센트를 구한 후 다음 주기의 시간을 구함
        float persent = (float)Int32.Parse(currentHP.text) / state.HP;
        float rate = 60 / Mathf.Lerp(maxRate, minRate, persent);

        // 크기 커지는 효과
        if (currentTime < rate / 4)
            HeartImg.transform.localScale = Vector3.Lerp(defaultHeartimgScale, defaultHeartimgScale * maxScale, currentTime / (rate / 4));

        // 크기 작아지는 효과
        else if (currentTime <= rate / 2)
            HeartImg.transform.localScale = Vector3.Lerp(defaultHeartimgScale * maxScale, defaultHeartimgScale, (currentTime - rate / 4) / (rate / 4));

        // 일정한 시간이 지난 경우 초기화 함
        else if (currentTime >= rate)
            currentTime = 0.0f;

        currentTime += Time.deltaTime;
        imgMaterial.SetFloat("_RatePersent", persent);
    }
}

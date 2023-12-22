using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerGunShotUI : MonoBehaviour
{
    public PlayerState playerState;         // 플레이어의 상태

    public GameObject emptyCartridge;       // 탄피 배출 애니메이션
    public GameObject gunFire;              // 총 화염 애니메이션
    public GameObject gunImg;               // 총 이미지 오브젝트

    public TextMeshProUGUI currentText;     // 탄창에 남아있는 탄알 수
    public TextMeshProUGUI remainText;      // 현재 남아있는 탄알 수

    public Color maxCurrentColor;           // 현재 탄창이 꽉차 있을 때 색깔
    public Color zeroCurrentColor;          // 현재 탄창이 비어있을 때 색깔

    public float endFontSize;               // 사격 후 최소로 작아지는 폰트 크기
    public float resizeDuration;            // 사격 후 작아지는 속도

    private float startFontSize;            // 폰트의 기본 크기

    public void Start()
    {
        Debug.Assert(playerState != null, "Error (Null Reference) : 플레이어 상태가 존재하지 않습니다.");
        Debug.Assert(emptyCartridge != null, "Error (Null Reference) : 탄피 이펙트를 출력할 오브젝트가 존재하지 않습니다.");
        Debug.Assert(gunFire != null, "Error (Null Reference) : 화염을 뿜을 텍스트가 존재하지 않습니다.");
        Debug.Assert(currentText != null, "Error (Null Reference) : 탄창에 있는 총알을 출력할 텍스트가 존재하지 않습니다.");
        Debug.Assert(remainText != null, "Error (Null Reference) : 남아있는 총알을 출력할 텍스트가 존재하지 않습니다.");

        remainText.text = playerState.bulletCount.ToString();
        currentText.text = playerState.bulletCurrentMax.ToString();

        startFontSize = currentText.fontSize;

        SetTextColor();
    }

    // 총알 감소 이펙트
    public IEnumerator ReduceBullet()
    {
        StopCoroutine(ReduceFontSize());
        currentText.fontSize = startFontSize;
        StartCoroutine(ReduceFontSize());

        // 탄창에 있는 총알의 수를 감소시킴
        currentText.text = (Int32.Parse(currentText.text) - 1).ToString();
        SetTextColor();

        // 화구 이펙트를 활성화하고 총 이미지를 35도로 기울림
        gunFire.SetActive(true);
        gunImg.transform.localRotation = Quaternion.Euler(0.0f, 180.0f, 35.0f);

        // 탄피 생성
        GameObject newCartridge = Instantiate(emptyCartridge, gunImg.transform);
        newCartridge.layer = 6;
        newCartridge.transform.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        newCartridge.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        newCartridge.GetComponent<ParticleSystem>().gravityModifier = 2.0f;
        Destroy(newCartridge, 1.0f);

        yield return new WaitForSeconds(0.1f);

        // 총을 다시 내림
        gunImg.transform.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);

        yield return new WaitForSeconds(0.05f);
        gunFire.SetActive(false);
    }

    // 남은 총알의 크기를 조절하는 메소드
    private IEnumerator ReduceFontSize()
    {
        // 진행된 시간, 시작 글자 크기
        float elapsedTime = 0, startSize = startFontSize;

        // 글자를 줄이는 효과
        while (elapsedTime < resizeDuration / 2)
        {
            float newSize = Mathf.Lerp(startSize, endFontSize, elapsedTime / (resizeDuration / 2));
            currentText.fontSize = newSize;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 글자를 키우는 효과
        while (elapsedTime < resizeDuration)
        {
            float newSize = Mathf.Lerp(endFontSize, startSize, (resizeDuration / 2 - (elapsedTime / 2)) / (resizeDuration / 2));
            currentText.fontSize = newSize;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 다시 원래 크기로 설정
        currentText.fontSize = startFontSize; // 정확한 크기로 설정
    }

    // 남은 탄창의 개수에 따라 색깔을 지정
    public void SetTextColor()
    {
        float bulletRatio = (float)Int32.Parse(currentText.text) / playerState.bulletCurrentMax;

        Color redColor = zeroCurrentColor;
        Color greenColor = maxCurrentColor;

        // 중간 값 계산
        Color interpolatedColor = Color.Lerp(redColor, greenColor, bulletRatio);

        currentText.color = interpolatedColor;
    }
}
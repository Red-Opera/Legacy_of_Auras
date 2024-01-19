using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Ani2DRun : MonoBehaviour
{
    public List<Sprite> sprites;            // 출력할 이미지 순서
    public bool isLoop = false;             // 애니메이션이 무한반복으로 이루어지는지 여부

    public int framePerSec = 20;            // 프레임당 몇장을 출력할건지 여부
    public bool isSign = false;             // 현재 사인 중인지 확인

    private Image spriteRenderer;           // 이미지 출력 컴포넌트
    private int currentSpriteIndex = 0;     // 현재 스프라이트 인덱스

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
            // 현재 인덱스를 구함
            currentSpriteIndex = (int)(framePerSec * (Time.time - startPlayTime)) % sprites.Count;

            // 현재 스프라이트를 설정
            spriteRenderer.sprite = sprites[currentSpriteIndex];

            // 대기
            yield return new WaitForSeconds(1.0f / framePerSec);
        }

        // 애니메이션을 모두 출력후 천천히 안보이도록 설정
        while (spriteRenderer.color.a > 0)
        {
            float alpha = spriteRenderer.color.a - Time.deltaTime / 1.0f; // 예시로 Time.deltaTime을 이용하여 조절
            spriteRenderer.color = new Color(1f, 1f, 1f, alpha);

            yield return null; // 한 프레임씩 대기
        }

        currentSpriteIndex = 0;
        spriteRenderer.sprite = sprites[currentSpriteIndex];

        isSign = false;
    }
}

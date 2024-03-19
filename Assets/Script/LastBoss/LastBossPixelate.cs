using System.Collections;
using UnityEngine;

public class LastBossPixelate : MonoBehaviour
{
    [SerializeField] private RenderTexture renderTexture;   // 픽셀화하는 이미지

    [SerializeField] private float pixelateTime = 2.0f;     // 픽셀화하는 시간

    private GameObject pixelCamera;     // 픽셀화를 출력하는 카메라
    private LastBossAction action;      // 최종 보스 행동 컴포넌트
    private Animator animator;          // 최종 보스 애니메이터
    private bool isPixel = false;       // 현재 픽셀화가 되었는지 여부

    public void Start()
    {
        pixelCamera = GameObject.Find("Pixel").transform.GetChild(0).gameObject;
        Debug.Assert(pixelCamera != null, "Error (Null Reference) : 픽셀화하는 카메라가 존재하지 않습니다.");

        action = GetComponent<LastBossAction>();
        Debug.Assert(action != null, "Error (Null Reference) : 최종 보스의 행동을 관리하는 컴포넌트가 존재하지 않습니다.");

        animator = GetComponent<Animator>();
        Debug.Assert(animator != null, "Error (Null Reference) : 애니메이터가 존재하지 않습니다.");
    }

    public IEnumerator PixelToggle()
    {
        // 다음 공격을 하지 않도록 설정하고 픽셀화 또는 비픽셀화 여부 변경
        action.isAction = true;
        isPixel = !isPixel;

        // 픽셀 카메라 활성화
        pixelCamera.SetActive(true);

        // 기존 해상도를 가져옴
        Vector2 defualtSize = new Vector2(renderTexture.width, renderTexture.height);

        float startTime = Time.time;
        while (true)
        {
            float t = Time.time - startTime;

            if (t > pixelateTime)
                break;

            // 점점 픽셀화가 되도록 설정
            renderTexture.Release();
            if (isPixel)
            {
                renderTexture.width = (int)Mathf.Lerp(960.0f, defualtSize.x, t);
                renderTexture.height = (int)Mathf.Lerp(480.0f, defualtSize.y, t);
            }

            // 점점 비픽셀화가 되도록 설정
            else
            {
                renderTexture.width = (int)Mathf.Lerp(defualtSize.x, 960.0f, t);
                renderTexture.height = (int)Mathf.Lerp(defualtSize.y, 480.0f, t);
            }
            renderTexture.Create();
            
            yield return null;
        }

        // 픽셀화 했을 때 해상도를 저장
        renderTexture.Release();
        renderTexture.width = (int)defualtSize.x;
        renderTexture.height = (int)defualtSize.y;
        renderTexture.Create();

        // 픽셀화에 따라 카메라를 키는지 끄는지 설정
        pixelCamera.SetActive(isPixel);

        // 한번더 실행하는 것을 막음
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

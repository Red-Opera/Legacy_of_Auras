using System.Collections;
using UnityEngine;

public class CrossHair : MonoBehaviour
{
    [SerializeField] private float rotateSpeed;     // 회전 속도
    [SerializeField] private float startDuration;   // 조준선 크기 회복 시간

    static private GameObject player;               // 플레이어

    private Transform child;            // 조준선 오브젝트
    private Vector3 originalCrossSize;  // 초기 조준선 크기
    private bool isEnable = false;      // 현재 활성화 여부

    private void Start()
    {
        if (player == null)
            player = GameObject.Find("Model");
        Debug.Assert(player != null, "Error (Null Reference): 플레이어가 존재하지 않습니다.");

        child = transform.GetChild(0);
    }

    private void Update()
    {
        Debug.Assert(player != null, "Error (Null Reference): 플레이어가 존재하지 않습니다.");

        if (child.gameObject.activeSelf)
        {
            if (!isEnable)
            {
                isEnable = true;
                StartCoroutine(StartCrossHair());
            }

            // 플레이어를 항상 바라보고 천천히 회전함
            transform.LookAt(player.transform.position);
            child.localRotation *= Quaternion.Euler(new Vector3(0, rotateSpeed * Time.deltaTime, 0));
        }

        else
            isEnable = false;
    }

    // 조준선 시작할 때 실행하는 메소드
    private IEnumerator StartCrossHair()
    {
        // 플레이 시간을 초기화하고 현재 크기를 가져옴
        float elapsedTime = 0f;
        originalCrossSize = transform.localScale;

        // 지정된 시간까지 크기를 늘림
        while (elapsedTime < startDuration)
        {
            child.localScale = Vector3.Lerp(Vector3.zero, originalCrossSize, elapsedTime / startDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        child.localScale = originalCrossSize; // 최종 크기 설정
    }
}

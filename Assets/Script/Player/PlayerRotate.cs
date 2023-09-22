using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    public GameObject headLookAt;       // 머리가 바라봐야 하는 오브젝트
    public GameObject spineLookAt;      // 척추가 바라봐야 하는 오브젝트

    public GameObject head;             // 플레이어 머리 오브젝트
    public GameObject spine;            // 플레이어 척추 오브젝튼
    public GameObject camera;           // 카메라 오브젝트

    public static float sensitivityX = 2.0f;    // 마우스 감도(X)
    public static float sensitivityY = 2.0f;    // 마우스 감도(Y)
    public float maxYAngle = 80.0f;             // 위아래 각도 제한

    public Vector2 currentRotation = Vector2.zero;

    public void Start()
    {
        sensitivityX = GameManager.info.mouseSensitivityX;
        sensitivityY = GameManager.info.mouseSensitivityY;

        Debug.Assert(headLookAt != null, "Error (Null Reference) : 목표지점이 존재하지 않습니다.");
        Debug.Assert(spineLookAt != null, "Error (Null Reference) : 목표지점이 존재하지 않습니다.");
        Debug.Assert(head != null, "Error (Null Reference) : 머리 오브젝트가 존재하지 않습니다.");
        Debug.Assert(spine != null, "Error (Null Reference) : 척추 오브젝트가 존재하지 않습니다.");
        Debug.Assert(camera != null, "Error (Null Reference) : 카메라 오브젝트가 존재하지 않습니다.");
    }

    public void Update()
    {
        // 마우스 감도 가져오기
        sensitivityX = GameManager.info.mouseSensitivityX;
        sensitivityY = GameManager.info.mouseSensitivityY;

        // 마우스 입력 받기
        float mouseX = Input.GetAxis("Mouse X") * sensitivityX;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY;

        // 현재 회전 값 갱신
        currentRotation.x += mouseX;
        currentRotation.y -= mouseY;
        currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYAngle, maxYAngle);

        // 회전 적용
        camera.transform.eulerAngles = new Vector3(currentRotation.y, currentRotation.x, 0);
        transform.rotation = Quaternion.Euler(new Vector3(0, currentRotation.x, 0));

        // 머리와 척추 위치 업데이트
        headLookAt.transform.position = head.transform.position + camera.transform.forward * 50.0f + new Vector3(0.5f, 0, 0);
        spineLookAt.transform.position = spine.transform.position + camera.transform.forward * 50.0f + new Vector3(0.5f, 0, 0);
    }
}
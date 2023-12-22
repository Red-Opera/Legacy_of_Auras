using UnityEngine;
using UnityEngine.UIElements;

public class PlayerRotate : MonoBehaviour
{
    public GameObject headLookAt;       // 머리가 바라봐야 하는 오브젝트
    public GameObject spineLookAt;      // 척추가 바라봐야 하는 오브젝트
    public GameObject armLookAt;        // 활 쏠때 팔이 바라봐야 하는 오브젝트

    public GameObject camera;           // 카메라 오브젝트
    public GameObject cameraLookAt;     // 화살 모드시 카메라의 위치

    public PlayerWeaponChanger weaponChanger;

    public static float sensitivityX = 2.0f;    // 마우스 감도(X)
    public static float sensitivityY = 2.0f;    // 마우스 감도(Y)
    public float maxYAngle = 80.0f;             // 위아래 각도 제한

    public Vector2 currentRotation = Vector2.zero;

    private Transform headDefultPos;     // 플레이어 머리 오브젝트
    private Transform spineDefultPos;    // 플레이어 척추 오브젝트
    private Transform armDefultPos;      // 플레이어 팔 오브젝트

    public void Start()
    {
        sensitivityX = GameManager.info.mouseSensitivityX;
        sensitivityY = GameManager.info.mouseSensitivityY;

        weaponChanger = GetComponent<PlayerWeaponChanger>();

        headDefultPos = new GameObject().transform;
        headDefultPos.localPosition = headLookAt.transform.localPosition;
        headDefultPos.localRotation = headLookAt.transform.localRotation;
        headDefultPos.localScale = headLookAt.transform.localScale;

        spineDefultPos = new GameObject().transform;
        spineDefultPos.localPosition = spineLookAt.transform.localPosition;
        spineDefultPos.localRotation = spineLookAt.transform.localRotation;
        spineDefultPos.localScale = spineLookAt.transform.localScale;

        armDefultPos = new GameObject().transform;
        armDefultPos.localPosition = armLookAt.transform.localPosition;
        armDefultPos.localRotation = armLookAt.transform.localRotation;
        armDefultPos.localScale = armLookAt.transform.localScale;

        Debug.Assert(headLookAt != null, "Error (Null Reference) : 목표지점이 존재하지 않습니다.");
        Debug.Assert(spineLookAt != null, "Error (Null Reference) : 목표지점이 존재하지 않습니다.");
        Debug.Assert(camera != null, "Error (Null Reference) : 카메라 오브젝트가 존재하지 않습니다.");
        Debug.Assert(weaponChanger != null, "Error (Null Reference) : 무기 변경 스크립트가 존재하지 않습니다.");
    }

    public void Update()
    {
        if (TypeStory.hasActivatedCanvas)
            return;

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
        if (headDefultPos == null)
        {
            headDefultPos = new GameObject().transform;
            headDefultPos.localPosition = headLookAt.transform.localPosition;
            headDefultPos.localRotation = headLookAt.transform.localRotation;
            headDefultPos.localScale = headLookAt.transform.localScale;

            spineDefultPos = new GameObject().transform;
            spineDefultPos.localPosition = spineLookAt.transform.localPosition;
            spineDefultPos.localRotation = spineLookAt.transform.localRotation;
            spineDefultPos.localScale = spineLookAt.transform.localScale;

            armDefultPos = new GameObject().transform;
            armDefultPos.localPosition = armLookAt.transform.localPosition;
            armDefultPos.localRotation = armLookAt.transform.localRotation;
            armDefultPos.localScale = armLookAt.transform.localScale;
        }

        headLookAt.transform.localPosition = headDefultPos.transform.localPosition + camera.transform.localRotation * Vector3.forward / 10;
        spineLookAt.transform.localPosition = spineDefultPos.transform.localPosition + camera.transform.localRotation * Vector3.forward / 10;

        armLookAt.transform.localPosition = armDefultPos.transform.localPosition + camera.transform.localRotation * Vector3.forward / 10;
    }
}
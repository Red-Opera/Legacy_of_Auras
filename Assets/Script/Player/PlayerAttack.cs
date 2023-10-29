using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Camera camera;               // 플레이어 카메라
    public GameObject bow;              // 플레이어 활
    public GameObject arrow;            // 화살 오브젝트

    private Animator animator;          // 플레이어의 애니메이터
    private WeaponType weaponType;      // 현재 소유하고 있는 무기

    public Transform camaraToMove;              // 카메라가 이동할 위치
    public Transform defCameraTrans;            // 기본 카메라 위치

    public float cmrMoveTime;                   // 카메라가 목표지점까지 이동하는데 걸리는 시간
    private float cameraMoveStartTime;          // 카메라 이동 시작 시간
    private bool isCameraMoving = false;        // 카메라 이동 중인지 여부
    private bool isReady = false;               // 현재 화살을 준비하는지 여부

    public void Start()
    {
        animator = GetComponent<Animator>();

        Debug.Assert(GetComponent<PlayerWeaponChanger>() != null, "Error (Null Reference) : 플레이어 무기 변화 컴포넌트가 존재하지 않습니다.");
        Debug.Assert(animator != null, "Error (Null Reference) : 애니메이션 컴포넌트가 존재하지 않습니다.");
        Debug.Assert(bow != null, "Error (Null Reference) : 활이 존재하지 않습니다.");
        Debug.Assert(arrow != null, "Error (Null Reference) : 화살이 존재하지 않습니다.");
    }

    public void Update()
    {
        // 지속적으로 어떠한 무기를 갖고 있는지 확인
        weaponType = GetComponent<PlayerWeaponChanger>().weaponType;

        // 활을 장착하고 있고 마우스를 누르고 있을동안 화살을 쏠 준비를 함 (단, 플레이어가 아직 NPC와 대화 중인 경우 공격할 수 없음)
        if (ChatNPC.isEnd && Input.GetMouseButton(0) && weaponType == WeaponType.BOW)
        {
            if (!(isCameraMoving || animator.GetBool("ArrowReady")))     // 카메라가 이동 중이거나 화살이 준비되었을 때 중지
                StartCoroutine(BowAttack());
        }

        else if (weaponType == WeaponType.BOW)
            AttackCancel();

        // 카메라 이동할 수 있는지 확인하여 이동할 수 있을 경우 이동
        if (isCameraMoving)
            CameraHasMove(isReady);
    }

    // 화살 공격 메소드
    private IEnumerator BowAttack()
    {
        // 화살을 쏠 준비하는 애니메이션 실행
        animator.SetBool("ArrowReady", true);
        isReady = true;

        // 화살쏘는데 잘보이도록 카메라 이동
        CameraMove();

        yield return new WaitForSeconds(1.0f);

        GameObject newBow = Instantiate(arrow, bow.transform.GetChild(0).transform);
        newBow.transform.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
        newBow.transform.localPosition = new Vector3(-6.5f, 0.5f, -0.5f);

        // 소리 재생
        AudioClip clip = bow.GetComponent<AudioSource>().clip;
        bow.GetComponent<AudioSource>().PlayOneShot(clip);
    }

    private void AttackCancel()
    {
        if (isCameraMoving || !animator.GetBool("ArrowReady"))
            return;

        animator.SetBool("ArrowReady", false);
        isReady = false;

        CameraMove();
    }

    // 카메라 이동 설정 메소드
    private void CameraMove()
    {
        // 카메라 이동 시작 설정
        cameraMoveStartTime = Time.time;
        isCameraMoving = true;
    }

    // 카메라 이동 메소드
    private void CameraHasMove(bool ready)
    {
        // 카메라가 이동한 시간, 이동완료한 퍼센트
        float elapsedTime = Time.time - cameraMoveStartTime;
        float t = elapsedTime / cmrMoveTime;

        // 화살을 준비하는 경우
        if (ready)
        {
            // 시간에 따라서 자연스럽게 카메라 이동
            camera.transform.localPosition = Vector3.Lerp(defCameraTrans.localPosition, camaraToMove.localPosition, t * 2);
            camera.transform.localRotation = Quaternion.Lerp(defCameraTrans.localRotation, camaraToMove.localRotation, t * 2);
        }
        
        else
        {
            camera.transform.localPosition = Vector3.Lerp(camaraToMove.localPosition, defCameraTrans.localPosition, t * 2);
            camera.transform.localRotation = Quaternion.Lerp(camaraToMove.localRotation, defCameraTrans.localRotation, t * 2);
        }

        // 카메라가 이동완료한 경우 카메라 이동 중지
        if (t >= cmrMoveTime)
            isCameraMoving = false;
    }
}
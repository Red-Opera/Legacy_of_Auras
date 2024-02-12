using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Camera camera;               // 플레이어 카메라
    public GameObject bow;              // 플레이어 활
    public GameObject arrow;            // 화살 오브젝트
    public GameObject gun;              // 플레이어 총
    public GameObject bullet;           // 총알 오브젝트
    public GameObject emptyCartridge;   // 탄피 배출 이펙트
    public PlayerGunShotUI gunUI;       // 총 UI을 관리하는 오브젝트

    public TextMeshProUGUI currentGunText;      // 탄창에 남은 총알의 개수

    public AudioClip gunSoundClip;              // 총 소리 클립
    public AudioClip noRemainBullet;            // 총알이 없을때 나는 소리

    private Animator animator;                  // 플레이어의 애니메이터
    private WeaponType weaponType;              // 현재 소유하고 있는 무기

    public Transform camaraToMove;              // 카메라가 이동할 위치
    public Transform defCameraTrans;            // 기본 카메라 위치

    public float cmrMoveTime;                   // 카메라가 목표지점까지 이동하는데 걸리는 시간
    private float cameraMoveStartTime;          // 카메라 이동 시작 시간
    private bool isCameraMoving = false;        // 카메라 이동 중인지 여부
    private bool isReady = false;               // 현재 화살을 준비하는지 여부

    private ParticleSystem gunFlash;            // 총 쏠때 번쩍임
    private PlayerGunReLoad reLoad;             // 총 로드 스크립트

    public void Start()
    {
        animator = GetComponent<Animator>();

        Debug.Assert(GetComponent<PlayerWeaponChanger>() != null, "Error (Null Reference) : 플레이어 무기 변화 컴포넌트가 존재하지 않습니다.");
        Debug.Assert(animator != null, "Error (Null Reference) : 애니메이션 컴포넌트가 존재하지 않습니다.");
        Debug.Assert(bow != null, "Error (Null Reference) : 활이 존재하지 않습니다.");
        Debug.Assert(gun != null, "Error (Null Reference) : 총이 존재하지 않습니다.");
        Debug.Assert(arrow != null, "Error (Null Reference) : 화살이 존재하지 않습니다.");
        Debug.Assert(gunUI != null, "Error (Null Reference) : 총알 수를 관리하는 컴포넌트가 존재하지 않습니다.");

        gunFlash = gun.transform.GetChild(0).GetComponent<ParticleSystem>();

        Debug.Assert(gunFlash != null, "Error (Null Reference) : 총 번쩍 이펙트가 존재하지 않습니다.");
        Debug.Assert(emptyCartridge != null, "Error (Null Reference) : 탄피 이펙트가 존재하지 않습니다.");

        reLoad = GetComponent<PlayerGunReLoad>();
        Debug.Assert(reLoad != null, "Error (Null Reference) : 플레이어의 총 로드 컴포넌트 존재하지 않습니다.");
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

        else if (Input.GetMouseButtonDown(0) && weaponType == WeaponType.GUN)
            GunAttack();

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

        if (!Input.GetMouseButton(0))
            yield break;

        GameObject newBow = Instantiate(arrow, bow.transform.GetChild(0).transform);
        newBow.transform.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
        newBow.transform.localPosition = new Vector3(-6.5f, 0.5f, -0.5f);

        // 소리 재생
        AudioClip clip = bow.GetComponent<AudioSource>().clip;
        bow.GetComponent<AudioSource>().PlayOneShot(clip);
    }

    // 총 공격 메소드
    private void GunAttack()
    {
        if (reLoad.isReLoad)
            return;

        if (Int32.Parse(currentGunText.text.ToString()) <= 0)
        {
            gun.GetComponent<AudioSource>().PlayOneShot(noRemainBullet);
            return;
        }

        StartCoroutine(gunUI.ReduceBullet());
        animator.SetTrigger("FireGun");

        GameObject newBullet = Instantiate(bullet, gun.transform.GetChild(0).transform);
        newBullet.transform.localRotation = Quaternion.Euler(90.0f, 90.0f, 0.0f);
        newBullet.transform.parent = null;
        newBullet.transform.localScale = new Vector3(20.0f, 20.0f, 20.0f);

        gun.GetComponent<AudioSource>().PlayOneShot(gunSoundClip);

        if (gunFlash == null)
            gunFlash = gun.transform.GetChild(0).GetComponent<ParticleSystem>();

        gunFlash.Play();

        GameObject newCartridge = Instantiate(emptyCartridge, gun.transform);
        newCartridge.transform.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
        Destroy(newCartridge, 1.5f);
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
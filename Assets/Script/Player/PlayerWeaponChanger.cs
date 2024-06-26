using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public enum WeaponType { NULL, BOW, GUN, AURAS, ITEM1, ITEM2, ITEM3, ITEM4, ITEM5 }

public class PlayerWeaponChanger : MonoBehaviour
{
    [HideInInspector] public bool bowEquid = false; // 활 준비 모습으로 변경하는 값

    public GameObject bow;                  // 활 오브젝트
    public GameObject gun;                  // 총 오브젝트
    public GameObject spin;                 // 활을 집어놓을 위치
    public GameObject hand;                 // 플레이어가 잡을 손 위치
    public MultiAimConstraint arrowRig;     // 활 리깅    
    public GameObject gunUI;                // 총 UI
   
    private Animator animator;      // 플레이어 애니메이터

    public WeaponType weaponType;  // 현재 플레이어 공격타입

    private Vector3 bowPosition;    // 활의 위치
    private Vector3 bowRotation;    // 활의 회전

    private Vector3 gunPosition;    // 총의 위치
    private Vector3 gunRotation;    // 총의 회전

    private bool isChange = false;  // 현재 무기가 변경되는지 여부

    public void Start()
    {
        weaponType = WeaponType.NULL;

        animator = GetComponent<Animator>();

        Debug.Assert(bow != null, "Error (Null Reference) : 활오브젝트가 존재하지 않습니다.");
        Debug.Assert(gun != null, "Error (Null Reference) : 총오브젝트가 존재하지 않습니다.");
        Debug.Assert(animator != null, "Error (Null Reference) : 애니메이션 컴포넌트가 존재하지 않습니다.");

        bowPosition = bow.transform.localPosition;
        bowRotation = bow.transform.localRotation.eulerAngles;

        gunPosition = gun.transform.localPosition;
        gunRotation = gun.transform.localRotation.eulerAngles;

        arrowRig.weight = 0.0f;
    }

    public void Update()
    {
        if (!ChatNPC.isEnd || isChange || TypeStory.hasActivatedCanvas)
            return;

        // 화살 상태가 아니고 숫자 1를 눌렸을 때 활로 바꿈 (단, 만약 플레이어가 아직 대화 중인 경우 무기를 바꿀 수 없음)
        if ((Input.GetKeyDown(KeyCode.Alpha1) && 
            !animator.GetBool("ArrowReady") && 
            (weaponType == WeaponType.NULL || weaponType == WeaponType.BOW)) || bowEquid)
            StartCoroutine(EquidUnEquidBow());

        else if (Input.GetKeyDown(KeyCode.Alpha2) && 
                 (weaponType == WeaponType.NULL || weaponType == WeaponType.GUN) && 
                 gun.activeSelf && !animator.GetCurrentAnimatorStateInfo(1).IsName("ReloadGun"))
            EquidUnEquidGun();

        else if (Input.GetKeyDown(KeyCode.Alpha3) &&
            (weaponType == WeaponType.NULL || weaponType == WeaponType.AURAS) && 
            PlayerGetAurasArrow.isGetArrow)
            StartCoroutine(ChangeAurasArrow());
    }

    // 활 무기 착용 메소드
    private IEnumerator EquidUnEquidBow()
    {
        // 현재 무기가 변함을 표시
        isChange = true;
        bowEquid = false;

        // 활 착용/해제 애니메이션 실행
        animator.SetTrigger("BowReady");

        // 무기를 작용하지 않는 상태라면
        if (weaponType == WeaponType.NULL)
            weaponType = WeaponType.BOW;    // 현재 무기 타입을 활로 변경

        // 무기를 착용하고 있는 상태이고 사격 준비 중이 아닌 경우
        else if (!animator.GetBool("ArrowReady"))
            weaponType = WeaponType.NULL;   // 무기 상태를 없음으로 표시

        // 해당 타입에 맞는 Idle로 변경
        animator.SetFloat("IdleMode", (float)weaponType);

        yield return new WaitForSeconds(0.4f);

        if (weaponType != WeaponType.BOW)
        {
            // 등에 활 위치
            bow.transform.SetParent(spin.transform);
            bow.transform.localPosition = bowPosition;
            bow.transform.localRotation = Quaternion.Euler(bowRotation);
            arrowRig.weight = 0.0f;
        }

        else
        {
            // 손에 활 위치
            bow.transform.SetParent(hand.transform);
            bow.transform.localPosition = Vector3.zero;
            bow.transform.localRotation = Quaternion.Euler(new Vector3(-110, 10, 0));
            arrowRig.weight = 1f;
        }

        // 무기 변하는 상태를 해제
        isChange = false;
    }

    // 총 착용 메소드
    private void EquidUnEquidGun()
    {
        isChange = true;

        if (weaponType == WeaponType.NULL)
        {
            animator.SetBool("GunReady", true);
            weaponType = WeaponType.GUN;
        }

        else
        {
            animator.SetBool("GunReady", false);
            weaponType = WeaponType.NULL;
        }

        animator.SetFloat("IdleMode", (float)weaponType);

        if (weaponType != WeaponType.GUN)
        {
            gun.transform.SetParent(spin.transform);
            gun.transform.localPosition = gunPosition;
            gun.transform.localRotation = Quaternion.Euler(gunRotation);

            gunUI.SetActive(false);
        }

        else
        {
            gun.transform.SetParent(hand.transform);
            gun.transform.localPosition = new Vector3(0.00285f, -0.0026f, -0.00111f);
            gun.transform.localRotation = Quaternion.Euler(new Vector3(-20, 180, -37.5f));

            gunUI.SetActive(true);
        }

        isChange = false;
    }

    private IEnumerator ChangeAurasArrow()
    {
        isChange = true;

        // 오라의 화살에서 바꿀 때는 공격 타입을 2부터 시작
        if (weaponType == WeaponType.NULL)
        {
            weaponType = WeaponType.AURAS;
            animator.SetFloat("IdleMode", 2.1f);
        }

        else
            weaponType = WeaponType.NULL;

        // 오라의 화살로 바꾸는 경우 : 3으로 변경, Idle로 바꾸는 경우 2.1로 변경
        float targetMode = (float)weaponType;
        if (weaponType == WeaponType.NULL)
            targetMode = 2.1f;

        while (true)
        {
            // 천천히 해당 애니메이션이 바뀌도록 설정
            animator.SetFloat("IdleMode", Mathf.Lerp(animator.GetFloat("IdleMode"), targetMode, 5.0f * Time.deltaTime));

            // 오라의 화살일 때는 0으로 가면 중지, Idle로 변할 때는 2로간 경우 중지
            if (Mathf.Abs(animator.GetFloat("IdleMode") - targetMode) < 0.1f)
                break;

            yield return null;
        }

        animator.SetFloat("IdleMode", (float)weaponType);

        isChange = false;
    }
}
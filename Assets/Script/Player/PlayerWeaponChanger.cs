using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public enum WeaponType { NULL, BOW, GUN }

public class PlayerWeaponChanger : MonoBehaviour
{
    public GameObject bow;          // 활 오브젝트
    public GameObject spin;         // 활을 집어놓을 위치
    public GameObject hand;         // 플레이어가 잡을 손 위치
   
    private Animator aniamtor;      // 플레이어 애니메이터

    public WeaponType weaponType { get; private set; }  // 현재 플레이어 공격타입

    private Vector3 bowPosition;    // 활의 위치
    private Vector3 bowRotation;    // 활의 회전

    private bool isChange = false;  // 현재 무기가 변경되는지 여부

    public void Start()
    {
        weaponType = WeaponType.NULL;

        aniamtor = GetComponent<Animator>();

        Debug.Assert(bow != null, "Error (Null Reference) : 활오브젝트가 존재하지 않습니다.");
        Debug.Assert(aniamtor != null, "Error (Null Reference) : 애니메이션 컴포넌트가 존재하지 않습니다.");

        bowPosition = bow.transform.localPosition;
        bowRotation = bow.transform.localRotation.eulerAngles;
    }

    public void Update()
    {
        // 화살 상태가 아니고 숫자 1를 눌렸을 때 활로 바꿈 (단, 만약 플레이어가 아직 대화 중인 경우 무기를 바꿀 수 없음)
        if (ChatNPC.isEnd && Input.GetKeyDown(KeyCode.Alpha1) && !aniamtor.GetBool("ArrowReady") && !isChange)
            StartCoroutine(EquidUnEquidBow());
    }

    // 활 무기 착용 메소드
    private IEnumerator EquidUnEquidBow()
    {
        // 현재 무기가 변함을 표시
        isChange = true;

        // 활 착용/해제 애니메이션 실행
        aniamtor.SetTrigger("BowReady");

        // 무기를 작용하지 않는 상태라면
        if (weaponType == WeaponType.NULL)
            weaponType = WeaponType.BOW;    // 현재 무기 타입을 활로 변경

        // 무기를 착용하고 있는 상태이고 사격 준비 중이 아닌 경우
        else if (!aniamtor.GetBool("ArrowReady"))
            weaponType = WeaponType.NULL;   // 무기 상태를 없음으로 표시

        // 해당 타입에 맞는 Idle로 변경
        aniamtor.SetFloat("IdleMode", (float)weaponType);

        yield return new WaitForSeconds(0.4f);

        if (weaponType == WeaponType.NULL)
        {
            // 등에 활 위치
            bow.transform.SetParent(spin.transform);
            bow.transform.localPosition = bowPosition;
            bow.transform.localRotation = Quaternion.Euler(bowRotation);
        }

        else
        {
            // 손에 활 위치
            bow.transform.SetParent(hand.transform);
            bow.transform.localPosition = Vector3.zero;
            bow.transform.localRotation = Quaternion.Euler(new Vector3(-100, 0, 0));
        }

        // 무기 변하는 상태를 해제
        isChange = false;
    }
}